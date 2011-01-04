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
using OpenRasta.Web.Markup.Attributes;
using OpenRasta.Web.Markup.Attributes.Annotations;
using OpenRasta.Web.Markup.Modules;

// Structure module
// http://www.w3.org/TR/xhtml-modularization/abstract_modules.html#s_structuremodule
namespace OpenRasta.Web.Markup.Elements
{
    /// <summary>
    /// Represents the &lt;html&gt; element.
    /// </summary>
    public interface IHtmlElement : IContentModel<IHtmlElement, IHeadElement>,
                                    IContentModel<IHtmlElement, IBodyElement>,
                                    IAttributesI18N,
                                    IIDAttribute
    {
        [CDATA]
        string Version { get; set; }
        [URI("http://www.w3.org/1999/xhtml",true)]
        Uri XmlNS { get; set; }
    }

    /// <summary>
    /// Represents the &lt;title&gt; element.
    /// </summary>
    public interface ITitleElement : IIDAttribute,
                                     IAttributesI18N,
                                     IContentModel<ITitleElement, string>
    {
    }

    /// <summary>
    /// Represents the &lt;head&gt; element.
    /// </summary>
    public interface IHeadElement : IIDAttribute,
                                    IAttributesI18N,
                                    IContentModel<IHeadElement, ITitleElement>,
                                    IContentModel<IHeadElement, ILinkElement>,
                                    IContentModel<IHeadElement, IScriptElement>,
                                    IContentModel<IHeadElement, IStyleElement>,
                                    IContentModel<IHeadElement, IBaseElement>,
                                    IContentModel<IHeadElement, IMetaElement>
    {
        [URIs]
        IList<Uri> Profile { get; set; }
    }

    /// <summary>
    /// Represents the &lt;body&gt; element.
    /// </summary>
    public interface IBodyElement : IContentModel<IBodyElement, IContentSetHeading>,
                                    IContentModel<IBodyElement, IContentSetBlock>,
                                    IContentModel<IBodyElement, IContentSetList>,
                                    IAttributesCommon
    {
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
