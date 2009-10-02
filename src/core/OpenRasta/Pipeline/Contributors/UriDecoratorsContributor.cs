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
using System.Collections.Generic;
using System.Linq;
using OpenRasta.DI;
using OpenRasta.Web;
using OpenRasta.Pipeline;
using OpenRasta.Web.UriDecorators;

namespace OpenRasta.Pipeline.Contributors
{
    public class UriDecoratorsContributor : IPipelineContributor
    {
        readonly IDependencyResolver _resolver;

        public UriDecoratorsContributor(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public PipelineContinuation ProcessDecorators(ICommunicationContext context)
        {
            Uri currentUri = context.Request.Uri;
            IList<DecoratorPointer> decorators = CreateDecorators();

            /* Whenever we execute the decorators, each decorator gets a say in matching a url.
             * Whenever a decorator fails, it is ignored.
             * Whenever a decorator succeeds, it is marked as such so that its Apply() method gets called
             * Whenever a decorator that succeeded has changed the url, we reprocess all the decorators that failed before with the new url.
             * */
            for (int i = 0; i < decorators.Count; i++)
            {
                DecoratorPointer decorator = decorators[i];
                Uri processedUri;
                if (!decorator.Successful
                    && decorator.Decorator.Parse(currentUri, out processedUri))
                {
                    decorator.Successful = true;
                    if (currentUri != processedUri && processedUri != null)
                    {
                        currentUri = processedUri;
                        i = -1;
                        continue;
                    }
                }
            }
            foreach (var decorator in decorators)
            {
                if (decorator.Successful)
                    decorator.Decorator.Apply();
            }

            context.Request.Uri = currentUri;
            return PipelineContinuation.Continue;
        }

        public void Initialize(IPipeline pipelineRunner)
        {
            pipelineRunner.Notify(ProcessDecorators).Before<KnownStages.IUriMatching>();
        }

        IList<DecoratorPointer> CreateDecorators()
        {
            return _resolver.ResolveAll<IUriDecorator>()
                .Select(decorator => new DecoratorPointer { Decorator = decorator }).ToList();
        }

        class DecoratorPointer
        {
            public IUriDecorator Decorator { get; set; }
            public bool Successful { get; set; }
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