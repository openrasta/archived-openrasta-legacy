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
using NUnit.Framework;
using OpenRasta.DI;
using OpenRasta.Pipeline.Contributors;
using OpenRasta.Security;
using OpenRasta.Testing;
using OpenRasta.Tests;
using OpenRasta.Web;
using OpenRasta.Pipeline;

namespace DigestCredentialsReader_Specification
{
    public class when_no_header_is_present : openrasta_context
    {
        [Test]
        public void the_pipeline_continues_executing()
        {
            given_pipeline_contributor<DigestAuthorizerContributor>();

            when_sending_notification<KnownStages.IBegin>();

            Result
                .ShouldBe(PipelineContinuation.Continue);
        }
    }

    public class when_a_header_is_present : openrasta_context
    {
        DigestHeader TheRequestAuthorizationHeader;

        void GivenAClientAuthzHeader(string httpMethod, string username, string password, string realm, string nonce,
                                     string uri)
        {
            TheRequestAuthorizationHeader = new DigestHeader
            {
                Username = username,
                Password = password,
                Realm = realm,
                Nonce = nonce,
                Uri = uri,
                QualityOfProtection = "auth",
                ClientNonce = "clientNonce",
                Opaque = "opaque"
            };
            TheRequestAuthorizationHeader.Response = TheRequestAuthorizationHeader.GetCalculatedResponse(httpMethod);
            Context.Request.Headers["Authorization"] = TheRequestAuthorizationHeader.ClientRequestHeader;
        }

        [Test]
        public void the_content_of_the_header_is_parsed()
        {
            string authenticationHeader =
                @"Digest username=""Mufasa"",
                 realm=""testrealm@host.com"",
                 nonce=""dcd98b7102dd2f0e8b11d0f600bfb0c093"",
                 uri=""/dir/index.html"",
                 qop=auth,
                 nc=00000001,
                 cnonce=""0a4f113b"",
                 response=""6629fae49393a05397450978507c4ef1"",
                 opaque=""5ccc069c403ebaf9f0171e9517f40e41""";
            var credentials = DigestHeader.Parse(authenticationHeader);

            credentials.Username.ShouldBe("Mufasa");
            credentials.Uri.ShouldBe("/dir/index.html");
        }

        [Test]
        public void the_parsing_returns_null_if_the_authentication_is_not_digest()
        {
            string authenticationHeader = "Basic bla";
            DigestHeader.Parse(authenticationHeader)
                .ShouldBeNull();
        }
        /// <summary>
        /// See #31 on trac.
        /// </summary>
        [Test]
        public void the_result_is_authorized_for_incoming_uri_containing_invalid_characters()
        {
            given_request_uri("http://localhost/é");
            given_request_httpmethod("GET");
            GivenAClientAuthzHeader("GET", "username", "password", "realm", "nonce", @"http://localhost/é");
            GivenAUser("username", "password");
            var authorizer = DependencyManager.GetService<DigestAuthorizerContributor>();

            authorizer.ReadCredentials(Context)
                .ShouldBe(PipelineContinuation.Continue);
            Context.OperationResult.ShouldBeNull();
        }

        [Test]
        public void the_result_is_authorized_if_everything_matches()
        {
            given_request_uri("http://localhost/uri");

            GivenAClientAuthzHeader("GET", "username", "password", "realm", "nonce", "/uri");
            Context.Request.HttpMethod = "GET";
            GivenAUser("username", "password");
            var authorizer = DependencyManager.GetService<DigestAuthorizerContributor>();

            authorizer.ReadCredentials(Context)
                .ShouldBe(PipelineContinuation.Continue);
            Context.OperationResult.ShouldBeNull();
        }

        [Test]
        public void the_result_is_authorized_if_sent_with_absolute_uri()
        {
            given_request_uri("http://localhost/uri");
            given_request_httpmethod("GET");
            GivenAClientAuthzHeader("GET", "username", "password", "realm", "nonce", @"http://localhost/uri");
            GivenAUser("username", "password");
            var authorizer = DependencyManager.GetService<DigestAuthorizerContributor>();

            authorizer.ReadCredentials(Context)
                .ShouldBe(PipelineContinuation.Continue);
            Context.OperationResult.ShouldBeNull();
        }
        [Test,Ignore]
        public void a_non_absolute_digest_uri_for_an_absolute_request_should_fail()
        {
            // See 3.2.2.5. of rfc2617 - No possibility of knowing the exact request-line yet
        }
        [Test]
        public void the_result_is_not_authorized_if_the_request_uri_and_digest_header_uri_do_not_natch()
        {

            given_request_uri("http://localhost/uri-one");

            GivenAClientAuthzHeader("GET", "username", "password", "realm", "nonce", "/uri-two");
            Context.Request.HttpMethod = "GET";
            GivenAUser("username", "password");
            var authorizer = DependencyManager.GetService<DigestAuthorizerContributor>();

            authorizer.ReadCredentials(Context)
                .ShouldBe(PipelineContinuation.RenderNow);
            Context.OperationResult.ShouldBeOfType<OperationResult.BadRequest>();
        }

        [Test]
        public void the_result_is_not_authorized_if_the_passwords_dont_match()
        {
            given_request_uri("http://localhost/uri");

            GivenAClientAuthzHeader("GET", "username", "wrongPassword", "realm", "nonce", "/uri");
            GivenAUser("username", "password");
            var authorizer = DependencyManager.GetService<DigestAuthorizerContributor>();

            authorizer.ReadCredentials(Context)
                .ShouldBe(PipelineContinuation.RenderNow);
            Context.OperationResult.ShouldBeOfType<OperationResult.Unauthorized>();
        }

        [Test]
        public void the_result_is_not_authorized_if_the_username_doesnt_match()
        {
            given_request_uri("http://localhost/uri");

            GivenAClientAuthzHeader("GET", "unknown", "password", "realm", "nonce", "/uri");
            GivenAUser("username", "password");
            var authorizer = DependencyManager.GetService<DigestAuthorizerContributor>();

            authorizer.ReadCredentials(Context)
                .ShouldBe(PipelineContinuation.RenderNow);
            Context.OperationResult.ShouldBeOfType<OperationResult.Unauthorized>();
        }
    }

    public class when_the_authentication_failed : openrasta_context
    {
        [Test]
        public void the_correct_response_is_returned() { }
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