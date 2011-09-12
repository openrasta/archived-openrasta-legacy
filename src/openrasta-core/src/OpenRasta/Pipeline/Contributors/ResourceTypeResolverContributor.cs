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
using OpenRasta.Diagnostics;
using OpenRasta.Web;
using OpenRasta.Pipeline;
using OpenRasta.Web.Internal;

namespace OpenRasta.Pipeline.Contributors
{
    public class ResourceTypeResolverContributor : KnownStages.IUriMatching
    {
        readonly IUriResolver _uriRepository;

        public ResourceTypeResolverContributor(IUriResolver uriRepository)
        {
            _uriRepository = uriRepository;
            Log = NullLogger.Instance;
        }

        public ILogger Log { get; set; }

        public void Initialize(IPipeline pipelineRunner)
        {
            pipelineRunner.Notify(ResolveResource).After<BootstrapperContributor>();
        }

        PipelineContinuation ResolveResource(ICommunicationContext context)
        {
            if (context.PipelineData.SelectedResource == null)
            {
                var uriToMath = context.GetRequestUriRelativeToRoot();
                var uriMatch = _uriRepository.Match(uriToMath);
                if (uriMatch != null)
                {
                    context.PipelineData.SelectedResource = uriMatch;
                    context.PipelineData.ResourceKey = uriMatch.ResourceKey;
                    context.Request.UriName = uriMatch.UriName;
                }
                else
                {
                    context.OperationResult = CreateNotFound(context);
                    return PipelineContinuation.RenderNow;
                }
            }
            else
            {
                Log.WriteInfo(
                    "Not resolving any resource as a resource with key {0} has already been selected.".With(
                        context.PipelineData.SelectedResource.ResourceKey));
            }
            return PipelineContinuation.Continue;
        }

        private OperationResult.NotFound CreateNotFound(ICommunicationContext context)
        {
            return new OperationResult.NotFound
            {
                Description =
                    "No registered resource could be found for "
                    + context.Request.Uri
            };
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