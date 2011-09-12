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
using OpenRasta.TypeSystem;
using OpenRasta.Web;

namespace OpenRasta.Codecs
{
    public interface ICodecRepository : IEnumerable<CodecRegistration>
    {
        string[] RegisteredExtensions { get; }
        void Add(CodecRegistration descriptor);

        /// <summary>
        /// Selects the best codec for a given media type and a set of parameters to be resolved.
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="requiredTypes"></param>
        /// <param name="optionalTypes"></param>
        /// <returns>The codec registration and score matching the list of parameters.</returns>
        /// <remarks>
        /// <para>The score is calculated as the average distance of the codec to the parameter types.</para>
        /// <para>For example, if Customer inherits directly from Object, the distance between Object and Customer is 1,and the distance between Customer and itself is 0.</para>
        /// </remarks>
        CodecMatch FindMediaTypeReader(MediaType contentType, IEnumerable<IMember> requiredTypes, IEnumerable<IMember> optionalTypes);

        CodecRegistration FindByExtension(IMember resourceType, string extension);
        IEnumerable<CodecRegistration> FindMediaTypeWriter(IMember resourceType, IEnumerable<MediaType> contentTypes);
        void Clear();
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