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
using OpenRasta.Web.Markup.Attributes;
using OpenRasta.Web.Markup.Attributes.Annotations;

// The Metainformation module
// http://www.w3.org/TR/xhtml-modularization/abstract_modules.html#s_iframemodule
namespace OpenRasta.Web.Markup.Elements
{
    /// <summary>
    /// Represents the &lt;meta&gt; element.
    /// </summary>
    public interface IMetaElement : IElement,
        IAttributesI18N,
        IIDAttribute
    {
        [CDATA]
        string Scheme { get; set; }
        [CDATA]
        string Content { get; set; }
        [NMTOKEN("http-equiv")]
        string HttpEquiv { get; set; }
        // we don't use the INameAttribute because it's defined as a CDATA for forms
        // and on meta <meta> it's defined as an NMTOKEN...
        [NMTOKEN]
        string Name { get; set; }
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
