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
using System.Collections.ObjectModel;
using System.Linq;
using OpenRasta.Collections;

namespace OpenRasta
{
    public class UriTemplateTable
    {
        private readonly List<KeyValuePair<UriTemplate, object>> _keyValuePairs;
        private ReadOnlyCollection<KeyValuePair<UriTemplate, object>> _keyValuePairsReadOnly;

        public UriTemplateTable() : this(null, null)
        {
        }

        public UriTemplateTable(IEnumerable<KeyValuePair<UriTemplate, object>> keyValuePairs)
            : this(null, keyValuePairs)
        {
        }

        public UriTemplateTable(Uri baseAddress)
            : this(baseAddress, null)
        {
        }

        public UriTemplateTable(Uri baseAddress, IEnumerable<KeyValuePair<UriTemplate, object>> keyValuePairs)
        {
            BaseAddress = baseAddress;
            _keyValuePairs = keyValuePairs != null ? new List<KeyValuePair<UriTemplate, object>>(keyValuePairs) : new List<KeyValuePair<UriTemplate, object>>();
        }

        public Uri BaseAddress { get; set; }

        public bool IsReadOnly { get; private set; }

        public IList<KeyValuePair<UriTemplate, object>> KeyValuePairs
        {
            get { return IsReadOnly ? _keyValuePairsReadOnly : (IList<KeyValuePair<UriTemplate, object>>) _keyValuePairs; }
        }

        /// <exception cref="InvalidOperationException">You need to set a BaseAddress before calling MakeReadOnly</exception>
        public void MakeReadOnly(bool allowDuplicateEquivalentUriTemplates)
        {
            if (BaseAddress == null)
                throw new InvalidOperationException("You need to set a BaseAddress before calling MakeReadOnly");
            if (!allowDuplicateEquivalentUriTemplates)
                EnsureAllTemplatesAreDifferent();
            IsReadOnly = true;
            _keyValuePairsReadOnly = _keyValuePairs.AsReadOnly();
        }

        public Collection<UriTemplateMatch> Match(Uri uri)
        {
            int lastMaxLiteralSegmentCount = 0;
            var matches = new Collection<UriTemplateMatch>();
            foreach (var template in KeyValuePairs)
            {
                UriTemplateMatch potentialMatch = template.Key.Match(BaseAddress, uri);

                if (potentialMatch != null)
                {
                    // this calculates and keep only what matches the maximum possible amount of literal segments
                    int currentMaxLiteralSegmentCount = potentialMatch.RelativePathSegments.Count
                                                        - potentialMatch.WildcardPathSegments.Count;
                    for (int i = 0; i < potentialMatch.PathSegmentVariables.Count; i++)
                        if (potentialMatch.QueryParameters == null ||
                            potentialMatch.QueryStringVariables[potentialMatch.PathSegmentVariables.GetKey(i)] == null)
                            currentMaxLiteralSegmentCount -= 1;

                    potentialMatch.Data = template.Value;

                    if (currentMaxLiteralSegmentCount > lastMaxLiteralSegmentCount)
                    {
                        lastMaxLiteralSegmentCount = currentMaxLiteralSegmentCount;
                    }
                    else if (currentMaxLiteralSegmentCount < lastMaxLiteralSegmentCount)
                    {
                        continue;
                    }

                    matches.Add(potentialMatch);
                }
            }

            return SortByMatchQuality(matches).ToCollection();
        }

        IEnumerable<UriTemplateMatch> SortByMatchQuality(Collection<UriTemplateMatch> matches)
        {
            return from m in matches
                   let missingQueryStringParameters = Math.Abs(m.QueryStringVariables.Count - m.QueryParameters.Count)
                   let matchedVariables = m.PathSegmentVariables.Count + m.QueryStringVariables.Count
                   let literalSegments = m.RelativePathSegments.Count - m.PathSegmentVariables.Count
                   orderby literalSegments descending, matchedVariables descending, missingQueryStringParameters
                   select m;
        }

        /// <exception cref="UriTemplateMatchException">Several matching templates were found.</exception>
        public UriTemplateMatch MatchSingle(Uri uri)
        {
            UriTemplateMatch singleMatch = null;
            foreach (var segmentKey in KeyValuePairs)
            {
                UriTemplateMatch potentialMatch = segmentKey.Key.Match(BaseAddress, uri);
                if (potentialMatch != null && singleMatch != null)
                    throw new UriTemplateMatchException("Several matching templates were found.");
                if (potentialMatch != null)
                {
                    singleMatch = potentialMatch;
                    singleMatch.Data = segmentKey.Value;
                }
            }
            return singleMatch;
        }

        /// <exception cref="InvalidOperationException">Two equivalent templates were found.</exception>
        private void EnsureAllTemplatesAreDifferent()
        {
            // highly unoptimized, but good enough for now. It's an O(n!) in all cases
            // if you wnat to implement a sort algorythm on this, be my guest. It's only called
            // once per application lifecycle so not sure there's much value.
            for (int i = 0; i < _keyValuePairs.Count; i++)
            {
                KeyValuePair<UriTemplate, object> rootKey = _keyValuePairs[i];
                for (int j = i + 1; j < _keyValuePairs.Count; j++)
                    if (rootKey.Key.IsEquivalentTo(_keyValuePairs[j].Key))
                        throw new InvalidOperationException("Two equivalent templates were found.");
            }
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
