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
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace OpenRasta
{
    public class UriTemplate
    {
        const string WILDCARD_TEXT = "*";
        Dictionary<string, UrlSegment> _pathSegmentVariables;
        List<UrlSegment> _segments;
        Dictionary<string, QuerySegment> _queryStringVariables;
        Uri _templateUri;

        public UriTemplate(string template)
        {
            _templateUri = ParseTemplate(template);
            _segments = ParseSegments(_templateUri);
            _pathSegmentVariables = ParseSegmentVariables(_segments);
            _queryStringVariables = ParseQueryStringVariables(_templateUri);

            PathSegmentVariableNames = new ReadOnlyCollection<string>(new List<string>(_pathSegmentVariables.Keys));
            QueryStringVariableNames = new ReadOnlyCollection<string>(new List<string>(GetQueryStringVariableNames(_queryStringVariables)));
        }

        public ReadOnlyCollection<string> PathSegmentVariableNames { get; private set; }
        public ReadOnlyCollection<string> QueryStringVariableNames { get; private set; }

        IEnumerable<string> GetQueryStringVariableNames(Dictionary<string, QuerySegment> valueCollection)
        {
            foreach (var qsegment in valueCollection)
                if (qsegment.Value.Type == SegmentType.Variable)
                    yield return qsegment.Value.Value;
        }

        static Dictionary<string, UrlSegment> ParseSegmentVariables(List<UrlSegment> _segments)
        {
            var returnDic = new Dictionary<string, UrlSegment>();
            foreach (UrlSegment segment in _segments)
            {
                if (segment.Type == SegmentType.Variable)
                    returnDic.Add(segment.Text.ToUpperInvariant(), segment);
            }
            return returnDic;
        }

        static Dictionary<string, QuerySegment> ParseQueryStringVariables(Uri templateUri)
        {
            string queries = templateUri.Query;
            string[] pairs = queries.Split('&');
            var nc = new Dictionary<string, QuerySegment>();
            foreach (string value in pairs)
            {
                string unescapedString = Uri.UnescapeDataString(value);
                if (unescapedString.Length == 0)
                    continue;
                int variableStart = unescapedString[0] == '?' ? 1 : 0;

                int equalSignPosition = unescapedString.IndexOf('=');
                if (equalSignPosition != -1)
                {
                    string val = unescapedString.Substring(equalSignPosition + 1);
                    string valAsVariable = GetVariableName(val);
                    var segment = new QuerySegment
                    {
                        Key = unescapedString.Substring(variableStart, equalSignPosition - variableStart),
                        Value = valAsVariable ?? val,
                        Type = valAsVariable == null ? SegmentType.Literal : SegmentType.Variable
                    };
                    nc[segment.Key] = segment;
                }
            }
            return nc;
        }

        static List<UrlSegment> ParseSegments(Uri _templateUri)
        {
            var pasedSegments = new List<UrlSegment>();
#if !SILVERLIGHT
            string[] originalSegments = _templateUri.Segments;
#else
            var originalSegments = _templateUri.AbsolutePath.Split('/');
#endif
            foreach (string segmentText in originalSegments)
            {
                UrlSegment parsedSegment;
                string unescapedSegment = Uri.UnescapeDataString(segmentText);
                string sanitizedSegment = unescapedSegment.Replace("/", string.Empty);
                bool trailingSeparator = unescapedSegment.Length - sanitizedSegment.Length > 0;
                string variableName;
                if (sanitizedSegment == string.Empty) // this is the '/' returned by Uri which we don't care much for
                    continue;
                if ((variableName = GetVariableName(unescapedSegment)) != null)
                    parsedSegment = new UrlSegment {Text = variableName, OriginalText = sanitizedSegment, Type = SegmentType.Variable, TrailingSeparator = trailingSeparator};
                else if (string.Compare(unescapedSegment, WILDCARD_TEXT, StringComparison.OrdinalIgnoreCase) == 0)
                    parsedSegment = new UrlSegment {Text = WILDCARD_TEXT, OriginalText = sanitizedSegment, Type = SegmentType.Wildcard};
                else
                    parsedSegment = new UrlSegment {Text = sanitizedSegment, OriginalText = sanitizedSegment, Type = SegmentType.Literal, TrailingSeparator = trailingSeparator};

                pasedSegments.Add(parsedSegment);
            }
            return pasedSegments;
        }

        static string GetVariableName(string segmentText)
        {
            segmentText = segmentText.Replace("/", string.Empty).Trim();

            string result = null;
            if (segmentText.Length > 2 && segmentText[0] == '{' && segmentText[segmentText.Length - 1] == '}')
                result = segmentText.Substring(1, segmentText.Length - 2);

            return result;
        }

        static Uri ParseTemplate(string template)
        {
            return new Uri(new Uri("http://uritemplateimpl"), template);
        }

        public Uri BindByName(Uri baseAddress, NameValueCollection parameters)
        {
            if (baseAddress == null)
                throw new ArgumentNullException("baseAddress", "The base Uri needs to be provided for a Uri to be generated.");


            baseAddress = SanitizeUriAsBaseUri(baseAddress);

            var path = new StringBuilder();
            
            foreach (UrlSegment segment in _segments)
            {
                if (segment.Type == SegmentType.Literal)
                    path.Append(segment.Text);
                else if (segment.Type == SegmentType.Variable)
                {
                    path.Append(parameters[segment.Text.ToUpperInvariant()]);
                }

                if (segment.TrailingSeparator)
                    path.Append('/');
            }
            if (_queryStringVariables.Count > 0)
            {
                path.Append('?');
                foreach (var querySegment in _queryStringVariables)
                {
                    path.Append(querySegment.Value.Key).Append("=").Append(parameters[querySegment.Value.Value]).Append(";");
                }
                path.Remove(path.Length - 1, 1);
            }
            
            return new Uri(baseAddress, path.ToString());
        }

        static Uri SanitizeUriAsBaseUri(Uri address)
        {
            var builder = new UriBuilder(address);
            builder.Fragment = null;
            builder.Query = null;
            if (!builder.Path.EndsWith("/"))
                builder.Path += "/";
            return builder.Uri;
        }

        public Uri BindByPosition(Uri baseAddress, params string[] values)
        {
            baseAddress = SanitizeUriAsBaseUri(baseAddress);
            var path = new StringBuilder();
            int paramPosition = 0;
            foreach (UrlSegment segment in _segments)
            {
                if (segment.Type == SegmentType.Literal)
                    path.Append(segment.Text);
                else if (segment.Type == SegmentType.Variable)
                {
                    string param = paramPosition < values.Length ? values[paramPosition++] : segment.Text;
                    path.Append(param);
                }

                if (segment.TrailingSeparator)
                    path.Append('/');
            }
            return new Uri(baseAddress, path.ToString());
        }

        public bool IsEquivalentTo(UriTemplate other)
        {
            if (other == null)
                return false;
            if (_segments.Count != other._segments.Count)
                return false;
            if (_queryStringVariables.Count != other._queryStringVariables.Count)
                return false;
            for (int i = 0; i < _segments.Count; i++)
            {
                UrlSegment thisSegment = _segments[i];
                UrlSegment otherSegment = other._segments[i];
                if (thisSegment.Type != otherSegment.Type)
                    return false;
                if (thisSegment.Type == SegmentType.Literal && thisSegment.Text != otherSegment.Text)
                    return false;
            }
            foreach (var thisSegment in _queryStringVariables)
            {
                if (!other._queryStringVariables.ContainsKey(thisSegment.Key))
                    return false;
                QuerySegment otherSegment = other._queryStringVariables[thisSegment.Key];

                if (thisSegment.Value.Type != otherSegment.Type)
                    return false;
                if (thisSegment.Value.Type == SegmentType.Literal && thisSegment.Value.Value != otherSegment.Value)
                    return false;
            }
            return true;
        }

        public UriTemplateMatch Match(Uri baseAddress, Uri candidate)
        {
            if (baseAddress == null || candidate == null)
                return null;
            if (baseAddress.GetLeftPart(UriPartial.Authority) != candidate.GetLeftPart(UriPartial.Authority))
                return null;

            var baseUriSegments = baseAddress.Segments.Select(s=> RemoveTrailingSlash(s));
            var candidateSegments = new List<string>(candidate.Segments.Select(x=>RemoveTrailingSlash(x)));
            
            foreach(var baseUriSegment in baseUriSegments)
                if (baseUriSegment == candidateSegments[0])
                    candidateSegments.RemoveAt(0);

            if (candidateSegments.Count > 0 && candidateSegments[0] == string.Empty)
                candidateSegments.RemoveAt(0);

            if (candidateSegments.Count != _segments.Count)
                return null;

            var boundVariables = new NameValueCollection(_pathSegmentVariables.Count);
            for (int i = 0; i < _segments.Count; i++)
            {
                string segment = candidateSegments[i];
                
                var candidateSegment = new {Text = segment, ProposedSegment = _segments[i]};

                candidateSegments[i] = candidateSegment.Text;

                if (candidateSegment.ProposedSegment.Type == SegmentType.Literal
                    && string.Compare(candidateSegment.ProposedSegment.Text, segment, StringComparison.OrdinalIgnoreCase) != 0)
                    return null;
                if (candidateSegment.ProposedSegment.Type == SegmentType.Wildcard)
                    throw new NotImplementedException("Not finished wildcards implementation yet");
                if (candidateSegment.ProposedSegment.Type == SegmentType.Variable)
                    boundVariables.Add(candidateSegment.ProposedSegment.Text, Uri.UnescapeDataString(candidateSegment.Text));
            }

            var queryMatches = new NameValueCollection();
            Dictionary<string, QuerySegment> requestQueryString = ParseQueryStringVariables(candidate);
            var queryParams = new Collection<string>();

            foreach (QuerySegment querySegment in _queryStringVariables.Values)
            {
                // if you match text exactly
                if (querySegment.Type == SegmentType.Literal && (!requestQueryString.ContainsKey(querySegment.Key)
                                                                 || requestQueryString[querySegment.Key].Value != querySegment.Value))
                    return null;
                else if (querySegment.Type == SegmentType.Variable)
                {
                    if (requestQueryString.ContainsKey(querySegment.Key))
                    {
                        queryMatches[querySegment.Value] = requestQueryString[querySegment.Key].Value;
                    }
                }
                queryParams.Add(querySegment.Key);
            }
            return new UriTemplateMatch
            {
                BaseUri = baseAddress,
                Data = 0,
                PathSegmentVariables = boundVariables,
                QueryParameters = queryParams,
                QueryStringVariables = queryMatches,
                RelativePathSegments = new Collection<string>(candidateSegments),
                RequestUri = candidate,
                Template = this,
                WildcardPathSegments = new Collection<string>()
            };
        }
         string RemoveTrailingSlash(string str)
        {
            return str.LastIndexOf('/') == str.Length - 1 ? str.Substring(0, str.Length - 1) : str;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (UrlSegment segment in _segments)
            {
                hash ^= segment.OriginalText.GetHashCode();
            }
            return hash;
        }

        public override string ToString()
        {
            return Uri.UnescapeDataString(_templateUri.AbsolutePath);
        }

        class QuerySegment
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public SegmentType Type { get; set; }
        }

        enum SegmentType
        {
            Wildcard,
            Variable,
            Literal
        }

        class UrlSegment
        {
            public string OriginalText { get; set; }
            public string Text { get; set; }
            public SegmentType Type { get; set; }
            public bool TrailingSeparator { get; set; }
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
