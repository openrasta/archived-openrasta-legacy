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
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using OpenRasta.Collections;

namespace OpenRasta.Web
{
    /// <summary>
    /// Represents an internet media-type as defined by RFC 2046.
    /// </summary>
    public class MediaType : ContentType, IComparable<MediaType>, IEquatable<MediaType>
    {
        private const int MOVE_DOWN = -1;
        private const int MOVE_UP = 1;

        public bool Equals(MediaType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.TopLevelMediaType.Equals(TopLevelMediaType) &&
                   other.Subtype.Equals(Subtype) &&
                   ParametersAreEqual(other);
        }

        bool ParametersAreEqual(MediaType other)
        {
            if (other.Parameters.Count != Parameters.Count) return false;
            foreach (string parameter in other.Parameters.Keys)
                if (!Parameters.ContainsKey(parameter) || Parameters[parameter] != other.Parameters[parameter])
                    return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as MediaType);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result * 397) ^ (TopLevelMediaType != null ? TopLevelMediaType.GetHashCode() : 0);
                result = (result * 397) ^ (Subtype != null ? Subtype.GetHashCode() : 0);
                foreach (string parameterName in Parameters.Keys)
                    result = (result * 397) ^ (parameterName + Parameters[parameterName]).GetHashCode();
                return result;
            }
        }

        public static bool operator ==(MediaType left, MediaType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MediaType left, MediaType right)
        {
            return !Equals(left, right);
        }

        public static readonly MediaType ApplicationOctetStream = new MediaType("application/octet-stream");
        public static readonly MediaType ApplicationXWwwFormUrlencoded = new MediaType("application/x-www-form-urlencoded");
        public static readonly MediaType Html = new MediaType("text/html");
        public static readonly MediaType Json = new MediaType("application/json");
        public static readonly MediaType MultipartFormData = new MediaType("multipart/form-data");
        public static readonly MediaType TextPlain = new MediaType("text/plain");
        public static readonly MediaType Xhtml = new MediaType("application/xhtml+xml");
        public static readonly MediaType XhtmlFragment = new MediaType("application/vnd.openrasta.htmlfragment+xml");
        public static readonly MediaType Xml = new MediaType("application/xml");
        public static readonly MediaType Javascript = new MediaType("text/javascript");

        private float _quality;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaType"/> class.
        /// </summary>
        /// <param name="contentType">A <see cref="T:System.String"/>, for example, "text/plain; charset=us-ascii", that contains the internet media type, subtype, and optional parameters.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="contentType"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="contentType"/> is <see cref="F:System.String.Empty"/> ("").
        /// </exception>
        /// <exception cref="T:System.FormatException">
        /// 	<paramref name="contentType"/> is in a form that cannot be parsed.
        /// </exception>
        public MediaType(string contentType)
            : base(contentType)
        {
            if (Parameters.ContainsKey("q"))
            {
                float floatResult;
                _quality = float.TryParse(Parameters["q"], NumberStyles.Float, CultureInfo.InvariantCulture, out floatResult) ? Math.Min(1, Math.Max(0, floatResult)) : 0F;
            }
            else
            {
                _quality = 1.0F;
            }
            int slashPos = MediaType.IndexOf('/');
            int semiColumnPos = MediaType.IndexOf(';', slashPos);

            TopLevelMediaType = MediaType.Substring(0, slashPos).Trim();
            Subtype =
                MediaType.Substring(slashPos + 1,
                                    (semiColumnPos != -1 ? semiColumnPos : MediaType.Length) - slashPos - 1).Trim();
        }

        public float Quality
        {
            get { return _quality; }
            private set
            {
                _quality = value;
                if (value != 1.0f)
                    Parameters["q"] = value.ToString("0.###");
                else if (Parameters.ContainsKey("q"))
                    Parameters.Remove("q");
            }
        }
        public string TopLevelMediaType { get; private set; }
        public string Subtype { get; private set; }
        public bool IsWildCard { get { return IsTopLevelWildcard && IsSubtypeWildcard; } }
        public bool IsTopLevelWildcard
        {
            get { return TopLevelMediaType == "*"; }
        }
        public bool IsSubtypeWildcard
        {
            get { return Subtype == "*"; }
        }

        public int CompareTo(MediaType other)
        {
            if (other == null)
                return MOVE_UP;
            if (Equals(other))
                return 0;

            // first, always move down */*
            if (IsWildCard)
            {
                if (other.IsWildCard)
                    return 0;
                return MOVE_DOWN;
            }

            // then sort by quality
            if (Quality != other.Quality)
                return Quality > other.Quality ? MOVE_UP : MOVE_DOWN;

            // then, if the quality is the same, always move application/xml at the end
            if (MediaType == "application/xml")
                return MOVE_DOWN;
            if (other.MediaType == "application/xml")
                return MOVE_UP;

            if (TopLevelMediaType != other.TopLevelMediaType)
            {
                if (IsTopLevelWildcard)
                    return MOVE_DOWN;
                if (other.IsTopLevelWildcard)
                    return MOVE_UP;
                return TopLevelMediaType.CompareTo(other.TopLevelMediaType);
            }

            if (Subtype != other.Subtype)
            {
                if (IsSubtypeWildcard)
                    return MOVE_DOWN;
                if (other.IsSubtypeWildcard)
                    return MOVE_UP;
                return Subtype.CompareTo(other.Subtype);
            }
            return 0;
        }

        public MediaType WithQuality(float quality)
        {
            var newMediaType = new MediaType(ToString());
            newMediaType.Quality = quality;
            return newMediaType;
        }
        public MediaType WithoutQuality()
        {
            var newMediatype = new MediaType(ToString());
            newMediatype.Quality = 1.0f;
            return newMediatype;
        }

        public bool Matches(MediaType typeToMatch)
        {
            return (typeToMatch.IsTopLevelWildcard || IsTopLevelWildcard
                    || TopLevelMediaType == typeToMatch.TopLevelMediaType)
                   && (typeToMatch.IsSubtypeWildcard || IsSubtypeWildcard || Subtype == typeToMatch.Subtype);
        }

        public static IEnumerable<MediaType> Parse(string contentTypeList)
        {
            if (contentTypeList == null)
                return new List<MediaType>();

            return from mediaTypeComponent in contentTypeList.Split(',')
                   let mediatype = new MediaType(mediaTypeComponent.Trim())
                   orderby mediatype descending
                   select mediatype;
        }

        public class MediaTypeEqualityComparer : IEqualityComparer<MediaType>
        {
            public bool Equals(MediaType x, MediaType y) { return x.Matches(y); }

            public int GetHashCode(MediaType obj) { return obj.TopLevelMediaType.GetHashCode() ^ obj.Subtype.GetHashCode(); }
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