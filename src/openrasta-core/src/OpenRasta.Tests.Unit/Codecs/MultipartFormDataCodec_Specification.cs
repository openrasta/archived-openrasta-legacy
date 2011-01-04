#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRasta.Binding;
using OpenRasta.Diagnostics;
using OpenRasta.IO.Diagnostics;
using OpenRasta.Testing;
using NUnit.Framework;
using OpenRasta.Codecs;
using OpenRasta.TypeSystem;
using OpenRasta.Web;
using OpenRasta.IO;
using OpenRasta;
using System.IO;
using System.Net.Mime;
using System;
using Moq;
using Moq.Language;
using OpenRasta.DI;
using OpenRasta.Tests.Unit.Fakes;

namespace MultipartFormDataCodec_Specification
{

    public class when_the_requested_type_is_enumerating_IHttpEntity : multipart_codec
    {
        [Test]
        public void string_parameters_are_passed_correctly()
        {
            given_context();

            GivenAMultipartRequestStream(Scenarios.ONE_FIELD_ONE_FILE);

            when_decoding<IEnumerable<IMultipartHttpEntity>>();

            ThenTheRawResultShouldContain(
                entity => {
                    entity.WithEntity("johndoe").Headers
                        .ContentDisposition = new ContentDispositionHeader("form-data;name=\"username\"");
                },
                entity => {
                    entity.WithEntity("Content of a document")
                        .Headers.ContentDisposition = new ContentDispositionHeader("form-data; name=\"document\"; filename=\"document.txt\"");
                    entity.ContentType = new MediaType("text/plain;charset=utf-8");
                });
        }
    }

    public class when_parsing_parts_for_base_types : multipart_codec
    {
        [Test]
        public void a_string_is_assigned()
        {
            given_context();
            GivenAMultipartRequestStream(Scenarios.TWO_FIELDS);

            when_decoding<string>("username");

            then_decoding_result<string>()
                .ShouldBe("johndoe");
        }
        [Test]
        public void a_datetime_is_assigned()
        {
            given_context();
            GivenAMultipartRequestStream(Scenarios.TWO_FIELDS);

            when_decoding<DateTime>("dateofbirth");

            then_decoding_result<DateTime>()
                .ShouldBe(DateTime.Parse("10 Dec 2001"));
        }
        [Test]
        public void a_large_field_value_is_stored_on_disk()
        {
            given_context();
            GivenAMultipartRequestStream(Scenarios.LARGE_FIELD);

            when_decoding<IDictionary<string, IList<IMultipartHttpEntity>>>("field");

            then_decoding_result<IDictionary<string, IList<IMultipartHttpEntity>>>()["field"].First().Stream.
                ShouldBeOfType<FileStream>()
                .Length
                    .ShouldBe(85003);
            
        }
    }
    public class when_parsing_parts_with_unicode_names : multipart_codec
    {
        [Test]
        public void the_field_name_encoded_in_quoted_printable_for_utf_8_is_recognized()
        {
            given_context();
            GivenAMultipartRequestStream(Scenarios.TELEPHONE_FIELD_UTF8_QUOTED);

            when_decoding<string>("Téléphone");
            then_decoding_result<string>().ShouldBe("077 777 7777");
        }
        [Test]
        public void a_field_name_encoded_in_base64_for_utf_8_is_recognized()
        {
            given_context();
            GivenAMultipartRequestStream(Scenarios.TELEPHONE_FIELD_UTF8_BASE64);

            when_decoding<string>("Téléphone");
            then_decoding_result<string>().ShouldBe("077 777 7777");
        }
        [Test]
        public void a_field_name_encoded_in_base64_for_iso_is_recognized()
        {
            given_context();
            GivenAMultipartRequestStream(Scenarios.TELEPHONE_FIELD_ISO88591_QUOTED);

            when_decoding<string>("Téléphone");
            then_decoding_result<string>().ShouldBe("077 777 7777");
        }
        [Test]
        public void a_sub_codec_is_used_to_resolve_a_parameter_name()
        {
            
            given_context();
            GivenAMultipartRequestStream(Scenarios.FILE_FIELD);

            when_decoding<IFile>("file");
            then_decoding_result<IFile>().FileName.ShouldBe("temp.txt");
            then_decoding_result<IFile>().Length.ShouldBe(85000);
        }
    }
    public class when_parsing_parts_with_names_representing_types : multipart_codec
    {
        [Test]
        public void a_string_property_is_assigned()
        {
            
            given_context();
            GivenAMultipartRequestStream(Scenarios.TWO_FIELDS_COMPOSED_NAMES);

            when_decoding<Customer>("customer");

            then_decoding_result<Customer>().Username
                .ShouldBe("johndoe");
        }
        [Test]
        public void a_datetime_property_is_assigned()
        {
            given_context();
            GivenAMultipartRequestStream(Scenarios.TWO_FIELDS_COMPOSED_NAMES);

            when_decoding<Customer>("customer");

            then_decoding_result<Customer>().DateOfBirth.Year
                .ShouldBe(2001);

        }
        [Test]
        public void another_mime_type_for_key_values_is_used_to_parse_the_result_correctly()
        {
            given_context();
            GivenAMultipartRequestStream(Scenarios.TWO_FIELDS_COMPOSED_NAMES);

            when_decoding<IDictionary<string,string[]>>("additions");

            then_decoding_result<IDictionary<string, string[]>>()["oneplusone"][0].ShouldBe("two");
            then_decoding_result<IDictionary<string, string[]>>()["oneplustwo"][0].ShouldBe("three");
        }
        [Test]
        public void construction_of_objects_from_other_media_types_returns_the_correct_values()
        {
            given_context();
            GivenAMultipartRequestStream(Scenarios.NESTED_CONTENT_TYPES);

            when_decoding<Customer>("customer");

            then_decoding_result<Customer>().DateOfBirth.Year.ShouldBe(2001);
            then_decoding_result<Customer>().DateOfBirth.Month.ShouldBe(12);
            then_decoding_result<Customer>().DateOfBirth.Day.ShouldBe(10);            
        }
    }
    public class when_writing_multipart_formdata : context
    {
        [Test]
        public void a_form_input_value_is_encoded_correctly()
        {
            
        }
    }
    public static class MultipartExtensions
    {
        public static MultipartHttpEntity WithEntity(this MultipartHttpEntity entity, string content)
        {
            entity.Stream.Position = 0;
            entity.Stream.Write(Encoding.Default.GetBytes(content));
            entity.Stream.Position = 0;
            return entity;
        }
    }    public static class Scenarios
    {        
        public static string ONE_FIELD_ONE_FILE = @"
preamble



--boundary42
Content-Disposition: form-data; name=""username""

johndoe
--boundary42
Content-Type: text/plain;charset=utf-8
Content-Disposition: form-data; name=""document"";filename=""document.txt""

Content of a document
--boundary42--
";
        public static string TWO_FIELDS =
@"
--boundary42
Content-Disposition: form-data; name=""username""

johndoe
--boundary42
Content-Disposition: form-data; name=""dateofbirth""

12/10/2001
--boundary42--
";
        public static string TWO_FIELDS_COMPOSED_NAMES =
@"
preamble
--boundary42
Content-Disposition: form-data;name=""Customer.Username""

johndoe
--boundary42
Content-Disposition: form-data; name=""Customer.DateOfBirth.Year""

2001
--boundary42
Content-Type: application/x-www-form-urlencoded
Content-Disposition: form-data; name=""additions""

oneplusone=two&oneplustwo=three
--boundary42--
";
        public static string NESTED_CONTENT_TYPES = 
@"
--boundary42
COntent-Disposition: form-data; name=""Customer.DateOfBirth""
Content-Type: application/x-www-form-urlencoded

year=2001&month=12&day=10
--boundary42--
";
        public static string TELEPHONE_FIELD_UTF8_QUOTED = @"
--boundary42
Content-Disposition: form-data; name=""=?UTF-8?Q?T=C3=A9l=C3=A9phone?=""

077 777 7777
--boundary42--";
        public static string TELEPHONE_FIELD_UTF8_BASE64 = @"
--boundary42
Content-Disposition: form-data; name=""=?UTF-8?B?VMOpbMOpcGhvbmU=?=""

077 777 7777
--boundary42--";
        public static string TELEPHONE_FIELD_ISO88591_QUOTED = @"
--boundary42
Content-Disposition: form-data; name=""=?ISO-8859-1?Q?T=E9l=E9phone?=""

077 777 7777
--boundary42--";
        public static string LARGE_FIELD = @"
--boundary42
Content-Disposition: form-data; name=""field""

{0}END
--boundary42--".With(new string('-',85000));

        public static string FILE_FIELD = @"
--boundary42
Content-Disposition: form-data; name=""file"";filename=""temp.txt""
Content-Type: application/octet-stream

{0}
--boundary42--".With(new string('-', 85000));
        

    }
    public class multipart_codec : media_type_reader_context<MultipartFormDataObjectCodec>
    {
        protected override MultipartFormDataObjectCodec CreateCodec(ICommunicationContext context)
        {
            return new MultipartFormDataObjectCodec(Context,
                                                    DependencyManager.Codecs,
                                                    DependencyManager.GetService<IDependencyResolver>(),
                                                    TypeSystems.Default,
                                                    new DefaultObjectBinderLocator());
        }
        protected void ThenTheRawResultShouldContain(params Action<MultipartHttpEntity>[] builders)
        {
            var result =  then_decoding_result<IEnumerable<IMultipartHttpEntity>>();
            var entities = new List<IMultipartHttpEntity>();
            foreach (var builder in builders)
            {
                var expected = new MultipartHttpEntity { Stream = new MemoryStream() };

                builder(expected);
                entities.Add(expected);
                
            }
            foreach (var entity in result)
            {
                var entityData = entity.Stream.ReadToEnd();
                entities.ForEach(e => e.Stream.Position = 0);
                var foundEntity = entities.Find(fake => {
                    bool headersMatch = (fake.ContentType != null ? fake.ContentType.Equals(entity.ContentType) : entity.ContentType == null)
                        && fake.Headers.ContentDisposition.Equals(entity.Headers.ContentDisposition);
                    bool contentMatches = fake.Stream.ReadToEnd().SequenceEqual(entityData);
                    return headersMatch && contentMatches;
                });
                foundEntity.ShouldNotBeNull();
            }
        }
        protected void GivenAMultipartRequestStream(string content)
        {
            given_request_content_type("multipart/form-data;boundary=boundary42");
            given_request_stream(content);
        }
    }
}

#region Full license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
