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
using System.Text;
using OpenRasta.Collections;
using OpenRasta.Handlers;
using System.Text.RegularExpressions;

namespace OpenRasta.Web.UriDecorators
{
    public class PathSegmentAsParameterUriDecorator : IUriDecorator
    {
        private static Regex segmentRegex = new Regex(";(?<segment>[a-zA-Z0-9-=]+)", RegexOptions.Compiled);
        IHandlerRepository _handlers;
        ICommunicationContext _context;
        string[] matchingSegments = null;
        public PathSegmentAsParameterUriDecorator(ICommunicationContext context, IHandlerRepository handlers)
        {
            _context = context;
            _handlers = handlers;
        }
        public bool Parse(Uri uri, out Uri processedUri)
        {
            string[] uriSegments = uri.Segments;
            string lastSegment = uriSegments[uriSegments.Length - 1];
            var matches = segmentRegex.Matches(lastSegment);
            if (matches.Count > 0)
            {
                matchingSegments = new string[matches.Count];
                for (int i = 0; i < matches.Count; i++)
                {
                    matchingSegments[i] = matches[i].Groups["segment"].Value;
                }
                UriBuilder builder = new UriBuilder(uri);
                
                builder.Path = string.Join("",uriSegments,1, uriSegments.Length-2) + segmentRegex.Replace(lastSegment, "");
                processedUri = builder.Uri;
                return true;
            }
            processedUri = uri;
            return false;
        }

        public void Apply()
        {
            _context.Request.CodecParameters.AddRange(matchingSegments);
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
