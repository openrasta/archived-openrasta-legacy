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
using OpenRasta.Codecs;
using OpenRasta.Diagnostics;
using OpenRasta.TypeSystem;
using OpenRasta.Web;
using OpenRasta.Pipeline;

namespace OpenRasta.Pipeline.Contributors
{
    public class ResponseEntityCodecResolverContributor : KnownStages.ICodecResponseSelection
    {
        const string HEADER_ACCEPT = "Accept";
        readonly ICodecRepository _codecs;
        readonly ITypeSystem _typeSystem;

        public ResponseEntityCodecResolverContributor(ICodecRepository repository, ITypeSystem typeSystem)
        {
            _codecs = repository;
            _typeSystem = typeSystem;
        }

        public ILogger Log { get; set; }

        public PipelineContinuation FindResponseCodec(ICommunicationContext context)
        {
            if (context.Response.Entity.Instance == null || context.PipelineData.ResponseCodec != null)
            {
                LogNoResponseEntity();
                return PipelineContinuation.Continue;
            }

            string acceptHeader = context.Request.Headers[HEADER_ACCEPT];

            IEnumerable<MediaType> acceptedContentTypes =
                MediaType.Parse(string.IsNullOrEmpty(acceptHeader) ? "*/*" : acceptHeader);
            IType responseEntityType = _typeSystem.FromInstance(context.Response.Entity.Instance);

            IEnumerable<CodecRegistration> sortedCodecs = _codecs.FindMediaTypeWriter(responseEntityType,
                                                                                      acceptedContentTypes);
            int codecsCount = sortedCodecs.Count();
            CodecRegistration negotiatedCodec = sortedCodecs.FirstOrDefault();

            if (negotiatedCodec != null)
            {
                LogCodecSelected(responseEntityType, negotiatedCodec, codecsCount);
                context.Response.Entity.ContentType = negotiatedCodec.MediaType.WithoutQuality();
                context.PipelineData.ResponseCodec = negotiatedCodec;
            }
            else
            {
                context.OperationResult = ResponseEntityHasNoCodec(acceptHeader, responseEntityType);
                return PipelineContinuation.RenderNow;
            }
            return PipelineContinuation.Continue;
        }

        public void Initialize(IPipeline pipeline)
        {
            pipeline.Notify(FindResponseCodec).After<KnownStages.IOperationResultInvocation>();
        }

        static OperationResult.ResponseMediaTypeUnsupported ResponseEntityHasNoCodec(string acceptHeader,
                                                                                     IType responseEntityType)
        {
            return new OperationResult.ResponseMediaTypeUnsupported
            {
                Title = "The response from the server could not be sent in any format understood by the UA.",
                Description =
                    string.Format(
                    "Content-type negotiation failed. Resource {0} doesn't have any codec for the content-types in the accept header:\r\n{1}",
                    responseEntityType,
                    acceptHeader)
            };
        }

        void LogCodecSelected(IType responseEntityType, CodecRegistration negotiatedCodec, int codecsCount)
        {
            Log.WriteInfo(
                "Selected codec {0} out of {1} codecs for entity of type {2} and negotiated media type {3}.".
                    With(negotiatedCodec.CodecType.Name,
                         codecsCount,
                         responseEntityType.Name,
                         negotiatedCodec.MediaType));
        }

        void LogNoResponseEntity()
        {
            Log.WriteInfo(
                "No response codec was searched for. The response entity is null or a response codec is already set.");
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