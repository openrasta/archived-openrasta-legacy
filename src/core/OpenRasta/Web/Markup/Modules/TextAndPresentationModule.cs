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

// Text module and presentation module
// http://www.w3.org/TR/xhtml-modularization/abstract_modules.html#s_textmodule
// http://www.w3.org/TR/xhtml-modularization/abstract_modules.html#s_presentationmodule
namespace OpenRasta.Web.Markup.Elements
{
    /// <summary>
    /// Represents the &lt;address&gt; element.
    /// </summary>
    public interface IAddressElement : IAttributesCommon,
                                       IContentSetBlock,
                                       IContentModel<IAddressElement, string>,
                                       IContentModel<IAddressElement, IContentSetInline>
    {
    }

    /// <summary>
    /// Represents the &lt;blockquote&gt; element.
    /// </summary>
    public interface IBlockQuoteElement : IAttributesCommon,
                                          ICiteAttribute,
                                          IContentSetBlock,
                                          IContentModel<IBlockQuoteElement, IContentSetHeading>,
                                          IContentModel<IBlockQuoteElement, IContentSetBlock>,
                                          IContentModel<IBlockQuoteElement, IContentSetList>
    {
    }

    /// <summary>
    /// Represents the &lt;div&gt; element.
    /// </summary>
    public interface IDivElement : IAttributesCommon,
                                   IContentSetBlock,
                                   IContentModel<IDivElement, string>,
                                   IContentModel<IDivElement, IContentSetFlow>
    {
    }


    /// <summary>
    /// Represents the &lt;p&gt; element.
    /// </summary>
    public interface IPElement : IAttributesCommon,
                                 IContentSetBlock,
                                 IContentModel<IPElement, string>,
                                 IContentModel<IPElement, IContentSetInline>
    {
    }

    /// <summary>
    /// Represents the &lt;pre&gt; element.
    /// </summary>
    public interface IPreElement : IPElement
    {
    }

    /// <summary>
    /// Represents the &lt;q&gt; element.
    /// </summary>
    public interface IQElement : IInlineElement,
                                 ICiteAttribute
    {

    }

    /// <summary>
    /// Represents the &lt;abbr&gt;, &lt;acronym&gt;, &lt;b&gt;, &lt;big&gt;, &lt;cite&gt;, &lt;code&gt;, &lt;dfn&gt;, &lt;em&gt;, &lt;i&gt;, &lt;kbd&gt;, &lt;samp&gt;, &lt;small&gt;, &lt;span&gt;, &lt;strong&gt;, &lt;sub&gt;, &lt;sup&gt;, &lt;tt&gt; and &lt;var&gt; elements.
    /// </summary>
    public interface IInlineElement : IContentSetInline,
                                      IAttributesCommon,
                                      IContentModel<IInlineElement, string>,
                                      IContentModel<IInlineElement, IContentSetInline>
    {
    }
    /// <summary>
    /// Represents the &lt;h1&gt;, &lt;h2&gt;, &lt;h3&gt;, &lt;h4&gt;, &lt;h5&gt; and &lt;h6&gt; elements.
    /// </summary>
    public interface IHElement : IContentSetHeading,
                                 IAttributesCommon,
                                 IContentModel<IHElement, string>,
                                 IContentModel<IHElement, IContentSetInline>
    {
    }

    /// <summary>
    /// Represents the &lt;img&gt;, &lt;hr&gt; and &lt;br&gt; elements
    /// </summary>
    public interface IEmptyElement :
        IContentSetInline,
        IAttributesCore
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