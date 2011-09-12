#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System;

namespace Reflection_Specification
{
    //[TestFixture]
    //public class when_selecting_methods
    //{
    //    [Test]
    //    public void selecting_a_method_amongst_two_detects_optional_parameters()
    //    {
    //        var nv = new NameValueCollection();
    //        nv.Add("key", "value");
    //        var result = ReflectionHelper.FindMethod(new MemberInfo[] { 
    //            this.FromClr().GetMethod("testmethod"),
    //            this.FromClr().GetMethod("testmethod2")
    //        },
    //            new List<NameValueCollection> { nv }, null, null);
    //        result.ShouldNotBeNull();
    //    }
    //    [Test]
    //    public void selecting_a_method_matches_name_value_collection()
    //    {
    //        var nv = new NameValueCollection {
    //            { "key", "keyvalue" },
    //            { "testvalue", "testvaluevalue" }
    //        };
    //        var result = ReflectionHelper.FindMethod(
    //            GetMethods("testmethod", "testmethod2")
    //            , new List<NameValueCollection>(), null, nv);
    //        result.GetParameterByName("testvalue").Value.ShouldBe("testvaluevalue");
    //        result.GetParameterByName("key").Value.ShouldBe("keyvalue");
    //    }
    //    [Test]
    //    public void selecting_a_method_matches_name_value_before_simple_properties()
    //    {
    //        var uriParams = new NameValueCollection { { "key", "keyvalue" } };
    //        var entityBody = new NameValueCollection { { "testvaue", "testvalueresult" } };
    //        var result = ReflectionHelper.FindMethod(
    //            GetMethods("testmethod", "MethodTakingNameValueCollection"),
    //            new List<NameValueCollection>() { uriParams }, null, entityBody);
    //        result.GetParameterByName("entity").ShouldNotBeNull();
    //    }
    //    [Test]
    //    public void selecting_a_method_matches_the_method_with_the_most_parameters_filled()
    //    {
    //        var entityBody = new NameValueCollection {
    //            { "key", "keyvalue" },
    //            { "testvalue", "testvalueresult" } ,
    //            { "optionalString", "optionalResult" }
    //        };
    //        var result = ReflectionHelper.FindMethod(
    //            GetMethods("testmethod", "testmethodoptional"),
    //            new List<NameValueCollection>(),
    //            null,
    //            entityBody);
    //        result.ShouldNotBeNull();
    //        result.Method.ShouldNotBeNull();
    //        result.Method.Name.ShouldBe("testmethodoptional");
    //    }
    //    [Test]
    //    public void an_entity_body_of_type_NameValueCollection_is_turned_into_a_changeset_for_the_type()
    //    {
    //        var entityBody = new NameValueCollection { { "TestType1.Property1", "3" }, { "TestType1.Property2", "true" }};

    //        var theSelectedMethod = ReflectionHelper.FindMethod(GetMethods("TestMethodWithChangeSet", "testmethod"), new List<NameValueCollection>(), null, entityBody);

    //        theSelectedMethod.ShouldNotBeNull();

    //        theSelectedMethod.Method.ShouldNotBeNull();
    //        theSelectedMethod.Method.Name.ShouldBe("TestMethodWithChangeSet");

    //        var theParameter = theSelectedMethod.GetParameterByName("changeset");

    //        theParameter.ShouldNotBeNull();
    //        theParameter.Value.ShouldBeOfType<ChangeSet<TestType1>>();

    //        var theValue = theParameter.Value as ChangeSet<TestType1>;
    //        theValue.Changes.Count.ShouldBe(2);
    //        theValue.Changes["Property1"].ShouldBe(3);
    //        theValue.Changes["Property2"].ShouldBe(true);
    //    }
    //    [Test]
    //    public void an_entity_body_of_type_NameValueCollection_is_turned_into_the_complex_types_required_by_the_method()
    //    {
    //        var entityBody = new NameValueCollection { { "TestType1.Property1", "3" } };
    //        var theSelectedMethod = ReflectionHelper.FindMethod(GetMethods("TestMethodWithType", "testmethod"), new List<NameValueCollection>(), null, entityBody);

    //        theSelectedMethod.ShouldNotBeNull();
    //        theSelectedMethod.Method.Name.ShouldBe("TestMethodWithType");

    //        var theParameter = theSelectedMethod.GetParameterByName("type");

    //        theParameter.ShouldNotBeNull();
    //        theParameter.HasBeenSet.ShouldBeTrue();

    //        var theParameterValue = theParameter.Value as TestType1;
    //        theParameterValue.ShouldNotBeNull();
    //        theParameterValue.Property1.ShouldBe(3);
    //        theParameterValue.Property2.ShouldBeFalse();
    //    }
    //    [Test]
    //    public void constructing_an_object_from_a_NameValueCollection_returns_the_correct_properties_set()
    //    {
    //        var testObject = ReflectionHelper.ConstructComposedType(typeof(TestType1), new NameValueCollection { { "TestType1.Property1", "3" }, { "TestType1.Property2", "true" } }) as TestType1;
    //        testObject.ShouldNotBeNull();
    //        testObject.Property1.ShouldBe(3);
    //        testObject.Property2.ShouldBe(true);

    //    }
    //    [Test]
    //    public void parsing_complex_type_names_returns_no_duplicates()
    //    {
    //        var list = ReflectionHelper.ParseNameValueCollectionForComposedTypeNames(new NameValueCollection { { "Test.Test", "bla" }, { "Test.Test2", "bla" }, { "TestSecond.Test3", "bla" } });
    //        list.Count.ShouldBe(2);
    //        list[0].ShouldBe("Test");
    //        list[1].ShouldBe("TestSecond");
    //    }
    //    private static MemberInfo[] GetMethods(params string[] names)
    //    {
    //        List<MemberInfo> mis = new List<MemberInfo>();
    //        foreach (var name in names)
    //            mis.Add(typeof(when_selecting_methods).GetMethod(name));
    //        return mis.ToArray();
    //    }
    //    public void testmethod(string key, [Optional]int testvalue) { }
    //    public void testmethodoptional(string key, [Optional] int testvalue, [Optional] string optionalString) { }
    //    public void testmethod2(string key) { }

    //    public void TestMethodWithType(TestType1 type) {}
    //    public void TestMethodWithChangeSet(ChangeSet<TestType1> changeset) { }
    //    public void MethodTakingNameValueCollection(string key, NameValueCollection entity) { }

    //    public class TestType1
    //    {
    //        public int Property1 { get; set; }
    //        public bool Property2 { get; set; }
    //    }
    //    public class TestType2 : TestType1
    //    {
    //    }
    //}
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