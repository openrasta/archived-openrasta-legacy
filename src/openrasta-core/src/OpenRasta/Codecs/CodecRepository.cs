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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenRasta.Collections;
using OpenRasta.TypeSystem;
using OpenRasta.TypeSystem.ReflectionBased;
using OpenRasta.Web;

namespace OpenRasta.Codecs
{
    public class CodecRepository : ICodecRepository
    {
        readonly MediaTypeDictionary<CodecRegistration> _codecs = new MediaTypeDictionary<CodecRegistration>();

        public string[] RegisteredExtensions
        {
            get { return _codecs.SelectMany(reg => reg.Extensions).ToArray(); }
        }

        public void Add(CodecRegistration codecRegistration)
        {
            _codecs.Add(codecRegistration.MediaType, codecRegistration);
        }

        public void Clear()
        {
        }

        public CodecRegistration FindByExtension(IMember resourceMember, string extension)
        {
            foreach (var codecRegistration in _codecs)
            {
                var codecResourceType = codecRegistration.ResourceType;
                if (codecRegistration.Extensions.Contains(extension, StringComparison.OrdinalIgnoreCase))
                {
                    if (codecRegistration.IsStrict && resourceMember.Type.CompareTo(codecResourceType) == 0)
                        return codecRegistration;

                    if (resourceMember.Type.CompareTo(codecResourceType) >= 0)
                        return codecRegistration;
                }
            }

            return null;
        }

        /// <exception cref="ArgumentNullException"><c>requestedMediaType</c> is null.</exception>
        public CodecMatch FindMediaTypeReader(MediaType requestedMediaType, 
                                              IEnumerable<IMember> requiredMembers, 
                                              IEnumerable<IMember> optionalMembers)
        {
            if (requestedMediaType == null)
                throw new ArgumentNullException("requestedMediaType");
            if (requiredMembers == null)
                throw new ArgumentNullException("requiredMembers");
            var codecMatches = new List<CodecMatch>();

            var readerCodecs = from codec in _codecs.Matching(requestedMediaType)
                               where codec.CodecType.Implements<IMediaTypeReader>() ||
                                     codec.CodecType.Implements(typeof(IKeyedValuesMediaTypeReader<>))
                               select codec;

            foreach (var codec in readerCodecs)
            {
                float totalDistanceToRequiredParameters = 0;
                if (requiredMembers.Any())
                {
                    totalDistanceToRequiredParameters = CalculateScoreFor(requiredMembers, codec);
                    if (totalDistanceToRequiredParameters == -1)
                        continue; // the codec cannot resolve the required parameters
                }
                int totalDistanceToOptionalParameters = 0;
                int totalOptionalParametersCompatibleWithCodec = 0;
                if (optionalMembers != null)
                    foreach (var optionalType in optionalMembers)
                    {
                        int typeScore = CalculateDistance(optionalType, codec);
                        if (typeScore > -1)
                        {
                            totalDistanceToOptionalParameters += typeScore;
                            totalOptionalParametersCompatibleWithCodec++;
                        }
                    }

                float averageScore = totalDistanceToRequiredParameters + totalDistanceToOptionalParameters;

                codecMatches.Add(new CodecMatch(codec, 
                                                averageScore, 
                                                requiredMembers.Count() + totalOptionalParametersCompatibleWithCodec));
            }
            if (codecMatches.Count == 0)
                return null;

            codecMatches.Sort();
            codecMatches.Reverse();
            return codecMatches[0];
        }

        public IEnumerable<CodecRegistration> FindMediaTypeWriter(IMember resourceType, 
                                                                  IEnumerable<MediaType> requestedMediaTypes)
        {
            var orderedMediaTypes = requestedMediaTypes.OrderByDescending(mt => mt);
            var mediaTypesByQuality = orderedMediaTypes.GroupBy(key => key.Quality);
            return mediaTypesByQuality
                .Aggregate(new List<CodecRegistration>(), AppendMediaTypeWriterFor(resourceType));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var reg in _codecs.Distinct())
                yield return reg;
        }

        IEnumerator<CodecRegistration> IEnumerable<CodecRegistration>.GetEnumerator()
        {
            foreach (var reg in _codecs.Distinct())
                yield return reg;
        }

        static int CalculateDistance(IMember member, CodecRegistration registration)
        {
            if (registration.ResourceType == null)
                return -1;
            if (registration.IsStrict)
                return (member.Type.CompareTo(registration.ResourceType) == 0) ? 0 : -1;
            return member.Type.CompareTo(registration.ResourceType);
        }

        Func<IEnumerable<CodecRegistration>, IGrouping<float, MediaType>, IEnumerable<CodecRegistration>> AppendMediaTypeWriterFor(IMember resourceType)
        {
            return (source, mediaTypes) => source.Concat(FindMediaTypeWriterFor(mediaTypes, resourceType));
        }

        float CalculateScoreFor(IEnumerable<IMember> types, CodecRegistration registration)
        {
            float score = 0;

            foreach (var requestedType in types)
            {
                int typeComparison = CalculateDistance(requestedType.Type, registration);
                if (typeComparison == -1)
                    return -1;
                float typeScore = 1f / (1f + typeComparison);
                score += typeScore;
            }
            return score;
        }

        IEnumerable<CodecRegistration> FindMediaTypeWriterFor(IEnumerable<MediaType> mediaTypes, IMember resourceType)
        {
            return from mediaType in mediaTypes
                   from codec in _codecs.Matching(mediaType)
                   where codec.CodecType.Implements<IMediaTypeWriter>()
                   let match = new CodecMatch(codec, CalculateScoreFor(new[] { resourceType }, codec), int.MaxValue)
                   where match.Score >= 0
                   orderby match descending
                   select codec;
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