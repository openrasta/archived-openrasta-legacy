#region License

/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */

#endregion

using OpenRasta.Web.Markup.Attributes;
using OpenRasta.Web.Markup.Modules;

// http://www.w3.org/TR/xhtml-modularization/abstract_modules.html#s_listmodule

namespace OpenRasta.Web.Markup.Elements
{
    /// <summary>
    /// Represents the &lt;dl&gt; element
    /// </summary>
    public interface IDlElement : IAttributesCommon,
                                  IContentSetList,
                                  IContentModel<IDlElement, IDtElement>,
                                  IContentModel<IDlElement, IDdElement>
    {
    }

    /// <summary>
    /// Represents the &lt;dt&gt; element
    /// </summary>
    public interface IDtElement : IAttributesCommon,
                                  IContentModel<IDtElement, string>,
                                  IContentModel<IDtElement, IContentSetInline>
    {
    }

    /// <summary>
    /// Represents the &lt;dd&gt; element
    /// </summary>
    public interface IDdElement : IAttributesCommon,
                                  IContentModel<IDdElement, string>,
                                  IContentModel<IDdElement, IContentSetFlow>
    {
    }

    /// <summary>
    /// Represents the &lt;ul&gt; and &lt;ol&gt; elements
    /// </summary>
    public interface IListElement : IAttributesCommon,
                                    IContentSetList,
                                    IContentModel<IListElement, ILiElement>
    {
    }
    /// <summary>
    /// Represents the &lt;li&gt; element
    /// </summary>
    public interface ILiElement : IAttributesCommon,
                                  IContentModel<ILiElement, string>,
                                  IContentModel<ILiElement, IContentSetFlow>
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