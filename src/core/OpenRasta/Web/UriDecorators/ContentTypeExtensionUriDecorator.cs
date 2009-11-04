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
using OpenRasta.Codecs;
using OpenRasta.TypeSystem;

namespace OpenRasta.Web.UriDecorators
{
    /// <summary>
    /// Decorates a uri with an extension, similar to a file system extension, to override content-type negotiation.
    /// </summary>
    /// <remarks>
    /// The extension is always processed at the end of the uri, and separated by a dot.
    /// The matching is done per-resource, based on the extension declared for the codec
    /// associated with a resource.
    /// </remarks>
    public class ContentTypeExtensionUriDecorator : IUriDecorator
    {
        readonly ICodecRepository _codecs;
        readonly ICommunicationContext _context;
        readonly IUriResolver _uris;
        UriRegistration _resourceMatch;
        CodecRegistration _selectedCodec;

        public ContentTypeExtensionUriDecorator(ICommunicationContext context, IUriResolver uris, ICodecRepository codecs, ITypeSystem typeSystem)
        {
            _context = context;
            _codecs = codecs;
            _uris = uris;
        }

        public void Apply()
        {
            // other decorators may change the url later on and the match will have the wrong values
            // the content type however shouldn't change
            var entity = _context.Response.Entity;
            _context.PipelineData.ResponseCodec = _selectedCodec;

            // TODO: Check if this still works. 
            entity.ContentType = _selectedCodec.MediaType;
        }

        public bool Parse(Uri uri, out Uri processedUri)
        {
            processedUri = null;

            var appBaseUri = _context.ApplicationBaseUri.EnsureHasTrailingSlash();
            var fakeBaseUri = new Uri("http://localhost/", UriKind.Absolute);

            var uriRelativeToAppBase = appBaseUri
                .MakeRelativeUri(uri)
                .MakeAbsolute(fakeBaseUri);
            // find the resource type for the uri
            string lastUriSegment = uriRelativeToAppBase.GetSegments()[uriRelativeToAppBase.GetSegments().Length - 1];

            int lastDot = lastUriSegment.LastIndexOf(".");

            if (lastDot == -1)
            {
                return false;
            }

            var uriWithoutExtension = ChangePath(uriRelativeToAppBase, 
                                                 srcUri =>
                                                 srcUri.AbsolutePath.Substring(0, srcUri.AbsolutePath.LastIndexOf(".")));

            _resourceMatch = _uris.Match(uriWithoutExtension);
            if (_resourceMatch == null)
                return false;

            string potentialExtension = lastUriSegment.Substring(lastDot + 1);

// _codecs.
            _selectedCodec = _codecs.FindByExtension(_resourceMatch.ResourceKey as IType, potentialExtension);

            if (_selectedCodec == null)
            {
                return false;
            }

            processedUri = fakeBaseUri.MakeRelativeUri(uriWithoutExtension)
                .MakeAbsolute(appBaseUri);

// TODO: Ensure that if the Accept: is not compatible with the overriden value a 406 is returned.
            return true;
        }

        Uri ChangePath(Uri uri, Func<Uri, string> getPath)
        {
            var builder = new UriBuilder(uri);
            builder.Path = getPath(uri);
            return builder.Uri;
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