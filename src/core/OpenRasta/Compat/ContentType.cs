#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion
#if SILVERLIGHT
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace System.Net.Mime
{
    public class ContentType
    {

        public string Boundary 
        {
            get { return Parameters["boundary"]; }
            set { Parameters["boundary"] = value; }
        }
        public string CharSet
        {
            get { return Parameters["charset"]; }
            set { Parameters["charset"] = value; }
        }
        public string Name
        {
            get { return Parameters["charset"]; }
            set { Parameters["charset"] = value; }
        }
        public string MediaType { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public ContentType(string contentType)
        {
            int colPos = contentType.IndexOf(";");
            MediaType = contentType.Substring(0, colPos == -1 ? contentType.Length : colPos);
            Parameters = new Dictionary<string, string>();
            if (colPos > 0)
                ParseNextParameter(contentType, colPos+1);
        }

        private void ParseNextParameter(string contentType, int start)
        {
            if (start > contentType.Length)
                return;
            int nextColumn = contentType.IndexOf(";", start);
            ParseParameter(contentType.Substring(start, nextColumn == -1 ? contentType.Length : nextColumn));
            ParseNextParameter(contentType, nextColumn + 1);
        }

        private void ParseParameter(string p)
        {
            int equalPos = p.IndexOf("=");
            if (equalPos == -1)
                return;
            Parameters.Add(p.Substring(0, equalPos), p.Substring(equalPos + 1));
        }

    }
}
#endif
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
