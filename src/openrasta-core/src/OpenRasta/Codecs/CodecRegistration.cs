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
using OpenRasta.Collections;
using OpenRasta.TypeSystem;
using OpenRasta.Web;

namespace OpenRasta.Codecs
{
    public class CodecRegistration : IEquatable<CodecRegistration>
    {
        public CodecRegistration(Type codecType, object resourceKey, MediaType mediaType)
            : this(codecType, resourceKey, false, mediaType, null, null, false)
        {
        }

        public CodecRegistration(Type codecType, 
                                 object resourceKey, 
                                 bool isStrictRegistration, 
                                 MediaType mediaType, 
                                 IEnumerable<string> extensions, 
                                 object codecConfiguration, 
                                 bool isSystem)
        {
            CheckArgumentsAreValid(codecType, resourceKey, mediaType, isStrictRegistration);
            ResourceKey = resourceKey;
            MediaType = mediaType;
            CodecType = codecType;
            Extensions = new List<string>();
            if (extensions != null)
                Extensions.AddRange(extensions);
            Configuration = codecConfiguration;
            IsSystem = isSystem;
            IsStrict = isStrictRegistration;
        }

        public Type CodecType { get; private set; }
        public object Configuration { get; private set; }
        public IList<string> Extensions { get; private set; }
        public bool IsStrict { get; private set; }

        /// <summary>
        /// Defines if the codec is to be preserved between configuration refreshes because it is part of the
        /// OpenRasta framework.
        /// </summary>
        public bool IsSystem { get; private set; }

        public MediaType MediaType { get; private set; }
        public object ResourceKey { get; private set; }

        public IType ResourceType
        {
            get { return ResourceKey as IType; }
        }

        public static IEnumerable<CodecRegistration> FromCodecType(Type codecType, ITypeSystem typeSystem)
        {
            var resourceTypeAttributes =
                codecType.GetCustomAttributes(typeof(SupportedTypeAttribute), true).Cast<SupportedTypeAttribute>();
            var mediaTypeAttributes =
                codecType.GetCustomAttributes(typeof(MediaTypeAttribute), true).Cast<MediaTypeAttribute>();
            return from resourceTypeAttribute in resourceTypeAttributes
                   from mediaType in mediaTypeAttributes
                   let isStrictRegistration = IsStrictRegistration(resourceTypeAttribute.Type)
                   let resourceType =
                       isStrictRegistration ? GetStrictType(resourceTypeAttribute.Type) : resourceTypeAttribute.Type
                   select new CodecRegistration(
                       codecType, 
                       typeSystem.FromClr(resourceType), 
                       isStrictRegistration, 
                       mediaType.MediaType, 
                       mediaType.Extensions, 
                       null, 
                       true);
        }

        public static CodecRegistration FromResourceType(Type resourceType, 
                                                         Type codecType, 
                                                         ITypeSystem typeSystem, 
                                                         MediaType mediaType, 
                                                         IEnumerable<string> extensions, 
                                                         object codecConfiguration, 
                                                         bool isSystem)
        {
            bool isStrict = false;
            if (IsStrictRegistration(resourceType))
            {
                resourceType = GetStrictType(resourceType);
                isStrict = true;
            }
            return new CodecRegistration(codecType, 
                                         typeSystem.FromClr(resourceType), 
                                         isStrict, 
                                         mediaType, 
                                         extensions, 
                                         codecConfiguration, 
                                         isSystem);
        }

        public static Type GetStrictType(Type registration)
        {
            return registration.GetGenericArguments()[0];
        }

        public static bool IsStrictRegistration(Type type)
        {
            return type.IsGenericType
                   && type.GetGenericTypeDefinition() == typeof(Strictly<>);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(CodecRegistration)) return false;
            return Equals((CodecRegistration)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = ResourceKey != null ? ResourceKey.GetHashCode() : 0;
                result = (result * 397) ^ (CodecType != null ? CodecType.GetHashCode() : 0);
                result = (result * 397) ^ (MediaType != null ? MediaType.GetHashCode() : 0);
                result = (result * 397) ^ IsStrict.GetHashCode();
                result = (result * 397) ^ (Extensions != null ? Extensions.GetHashCode() : 0);
                result = (result * 397) ^ (Configuration != null ? Configuration.GetHashCode() : 0);
                result = (result * 397) ^ IsSystem.GetHashCode();
                return result;
            }
        }

        public bool Equals(CodecRegistration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ResourceKey, ResourceKey) && Equals(other.CodecType, CodecType) && Equals(other.MediaType, MediaType) && other.IsStrict.Equals(IsStrict) &&
                   Equals(other.Extensions, Extensions) && Equals(other.Configuration, Configuration) && other.IsSystem.Equals(IsSystem);
        }

        /// <exception cref="ArgumentException">Cannot do a strict registration on resources with keys that are not types.</exception>
        /// <exception cref="ArgumentNullException"><c>mediaType</c> is null.</exception>
        static void CheckArgumentsAreValid(Type codecType, 
                                           object resourceKey, 
                                           MediaType mediaType, 
                                           bool isStrictRegistration)
        {
            if (codecType == null)
                throw new ArgumentNullException("codecType", "codecType is null.");
            if (resourceKey == null)
                throw new ArgumentNullException("resourceKey", "resourceKey is null.");
            if (mediaType == null)
                throw new ArgumentNullException("mediaType", "mediaType is null.");
            if (resourceKey is Type)
                throw new ArgumentException("If using a type as a resourceKey, use an IType instead.", "resourceKey");
            if (isStrictRegistration && !(resourceKey is IType))
                throw new ArgumentException(
                    "Cannot do a strict registration on resources with keys that are not types.", "isStrictRegistration");
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