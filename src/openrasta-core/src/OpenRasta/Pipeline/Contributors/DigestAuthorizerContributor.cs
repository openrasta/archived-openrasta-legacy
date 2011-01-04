#region License

/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

// Digest Authentication implementation
//  Inspired by mono's implenetation, rewritten for OpenRasta.
// Original authors:
//  Greg Reinacker (gregr@rassoc.com)
//  Sebastien Pouliot (spouliot@motus.com)
// Portions (C) 2002-2003 Greg Reinacker, Reinacker & Associates, Inc. All rights reserved.
// Portions (C) 2003 Motus Technologies Inc. (http://www.motus.com)
// Original source code available at
// http://www.rassoc.com/gregr/weblog/stories/2002/07/09/webServicesSecurityHttpDigestAuthenticationWithoutActiveDirectory.html
using System;
using System.Linq;
using System.Security.Principal;
using OpenRasta.DI;
using OpenRasta.Security;
using OpenRasta.Web;
using OpenRasta.Pipeline;

namespace OpenRasta.Pipeline.Contributors
{
    public class DigestAuthorizerContributor : IPipelineContributor
    {
        readonly IDependencyResolver _resolver;
        IAuthenticationProvider _authentication;

        public DigestAuthorizerContributor(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public void Initialize(IPipeline pipelineRunner)
        {
            pipelineRunner.Notify(ReadCredentials)
                .After<KnownStages.IBegin>()
                .And
                .Before<KnownStages.IHandlerSelection>();

            pipelineRunner.Notify(WriteCredentialRequest)
                .After<KnownStages.IOperationResultInvocation>()
                .And
                .Before<KnownStages.IResponseCoding>();
        }

        public PipelineContinuation ReadCredentials(ICommunicationContext context)
        {
            if (!_resolver.HasDependency(typeof(IAuthenticationProvider)))
                return PipelineContinuation.Continue;

            _authentication = _resolver.Resolve<IAuthenticationProvider>();

            DigestHeader authorizeHeader = GetDigestHeader(context);

            if (authorizeHeader == null)
                return PipelineContinuation.Continue;

            string digestUri = GetAbsolutePath(authorizeHeader.Uri);

            if (digestUri != context.Request.Uri.AbsolutePath)
                return ClientError(context);

            Credentials creds = _authentication.GetByUsername(authorizeHeader.Username);

            if (creds == null)
                return NotAuthorized(context);
            var checkHeader = new DigestHeader(authorizeHeader)
            {
                Password = creds.Password,
                Uri = authorizeHeader.Uri
            };
            string hashedDigest = checkHeader.GetCalculatedResponse(context.Request.HttpMethod);

            if (authorizeHeader.Response == hashedDigest)
            {
                IIdentity id = new GenericIdentity(creds.Username, "Digest");
                context.User = new GenericPrincipal(id, creds.Roles);
                return PipelineContinuation.Continue;
            }
            return NotAuthorized(context);
        }

        static DigestHeader GetDigestHeader(ICommunicationContext context)
        {
            string header = context.Request.Headers["Authorization"];
            return string.IsNullOrEmpty(header) ? null : DigestHeader.Parse(header);
        }
        static bool HasDigestHeader(ICommunicationContext context)
        {
            return GetDigestHeader(context) != null;
        }
        static PipelineContinuation ClientError(ICommunicationContext context)
        {
            context.OperationResult = new OperationResult.BadRequest();
            return PipelineContinuation.RenderNow;
        }

        static string GetAbsolutePath(string uri)
        {
            uri = uri.TrimStart();

            if (uri.StartsWith("http://") || uri.StartsWith("https://"))
            {
                return new Uri(uri).AbsolutePath;
            }
            return uri.Any(ch => ch > 127) ? Uri.EscapeUriString(uri) : uri;
        }

        static PipelineContinuation NotAuthorized(ICommunicationContext context)
        {
            context.OperationResult = new OperationResult.Unauthorized();
            return PipelineContinuation.RenderNow;
        }

        static PipelineContinuation WriteCredentialRequest(ICommunicationContext context)
        {
            if (context.OperationResult is OperationResult.Unauthorized)
            {
                context.Response.Headers["WWW-Authenticate"] =
                    new DigestHeader
                    {
                        Realm = "Digest Authentication",
                        QualityOfProtection = "auth",
                        Nonce = "nonce",
                        Stale = false,
                        Opaque = "opaque"
                    }
                        .ServerResponseHeader;
            }
            return PipelineContinuation.Continue;
        }
    }
}

#region Full license

// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion