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
using System.Diagnostics.CodeAnalysis;
using System.Web.UI;
using OpenRasta.DI;
using OpenRasta.Web;
using OpenRasta.Web.Markup;
using OpenRasta.Web.Markup.Elements;
using OpenRasta.Web.Markup.Modules;
using XhtmlTextWriter = OpenRasta.Web.Markup.XhtmlTextWriter;

namespace OpenRasta.Codecs.WebForms
{
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules","SA1300")]
    public class ResourceView : Page, IXhtmlAnchorSite
    {
        XhtmlAnchor _xhtmlAnchor;

        public IAElement a
        {
            get { return Document.CreateElement<IAElement>(); }
        }

        public IInlineElement abbr
        {
            get { return Document.CreateElement<IInlineElement>("abbr"); }
        }

        public IInlineElement acronym
        {
            get { return Document.CreateElement<IInlineElement>("acronym"); }
        }

        public IAddressElement address
        {
            get { return Document.CreateElement<IAddressElement>(); }
        }

        public IInlineElement b
        {
            get { return Document.CreateElement<IInlineElement>("b"); }
        }

        public IBaseElement @base
        {
            get { return Document.CreateElement<IBaseElement>(); }
        }

        public IBdoElement bdo
        {
            get { return Document.CreateElement<IBdoElement>(); }
        }

        public IInlineElement big
        {
            get { return Document.CreateElement<IInlineElement>("big"); }
        }

        public IBlockQuoteElement blockquote
        {
            get { return Document.CreateElement<IBlockQuoteElement>(); }
        }

        public IBodyElement body
        {
            get { return Document.CreateElement<IBodyElement>(); }
        }

        public IEmptyElement br
        {
            get { return Document.CreateElement<IEmptyElement>("br"); }
        }

        public IButtonElement button
        {
            get { return Document.CreateElement<IButtonElement>(); }
        }

        public ICaptionElement caption
        {
            get { return Document.CreateElement<ICaptionElement>(); }
        }

        public IInlineElement cite
        {
            get { return Document.CreateElement<IInlineElement>("cite"); }
        }

        public IInlineElement code
        {
            get { return Document.CreateElement<IInlineElement>("code"); }
        }

        public IColElement col
        {
            get { return Document.CreateElement<IColElement>(); }
        }

        public IColGroupElement colgroup
        {
            get { return Document.CreateElement<IColGroupElement>(); }
        }

        public IDdElement dd
        {
            get { return Document.CreateElement<IDdElement>(); }
        }

        public IEditElement del
        {
            get { return Document.CreateElement<IEditElement>("del"); }
        }

        public IInlineElement dfn
        {
            get { return Document.CreateElement<IInlineElement>("dfn"); }
        }

        public IDivElement div
        {
            get { return Document.CreateElement<IDivElement>(); }
        }

        public IDlElement dl
        {
            get { return Document.CreateElement<IDlElement>(); }
        }

        public IDtElement dt
        {
            get { return Document.CreateElement<IDtElement>(); }
        }

        public IInlineElement em
        {
            get { return Document.CreateElement<IInlineElement>("em"); }
        }

        public IList<Error> Errors { get; set; }

        public IFieldsetElement fieldset
        {
            get { return Document.CreateElement<IFieldsetElement>(); }
        }

        public IFormElement form
        {
            get { return Document.CreateElement<IFormElement>(); }
        }

        public IHElement h1
        {
            get { return Document.CreateElement<IHElement>("h1"); }
        }

        public IHElement h2
        {
            get { return Document.CreateElement<IHElement>("h2"); }
        }

        public IHElement h3
        {
            get { return Document.CreateElement<IHElement>("h3"); }
        }

        public IHElement h4
        {
            get { return Document.CreateElement<IHElement>("h4"); }
        }

        public IHElement h5
        {
            get { return Document.CreateElement<IHElement>("h5"); }
        }

        public IHElement h6
        {
            get { return Document.CreateElement<IHElement>("h5"); }
        }

        public IHeadElement head
        {
            get { return Document.CreateElement<IHeadElement>(); }
        }

        public IEmptyElement hr
        {
            get { return Document.CreateElement<IEmptyElement>("hr"); }
        }

        public IHtmlElement html
        {
            get { return Document.CreateElement<IHtmlElement>(); }
        }

        public IInlineElement i
        {
            get { return Document.CreateElement<IInlineElement>("i"); }
        }

        public IIFrameElement iframe
        {
            get { return Document.CreateElement<IIFrameElement>(); }
        }

        public IImgElement img
        {
            get { return Document.CreateElement<IImgElement>(); }
        }

        public IInputCheckedElement inputCheckbox
        {
            get { return Document.CreateElement<IInputCheckedElement>("input").InputType(InputType.CheckBox); }
        }

        public IInputElement inputFile
        {
            get { return Document.CreateElement<IInputElement>("input").InputType(InputType.File); }
        }

        public IInputElement inputHidden
        {
            get { return Document.CreateElement<IInputElement>("input").InputType(InputType.Hidden); }
        }

        public IInputImageElement inputImage
        {
            get { return Document.CreateElement<IInputImageElement>("input").InputType(InputType.Image); }
        }

        public IInputTextElement inputPassword
        {
            get { return Document.CreateElement<IInputTextElement>("input").InputType(InputType.Password); }
        }

        public IInputCheckedElement inputRadio
        {
            get { return Document.CreateElement<IInputCheckedElement>("input").InputType(InputType.Radio); }
        }

        public IInputElement inputReset
        {
            get { return Document.CreateElement<IInputElement>("input").InputType(InputType.Reset); }
        }

        public IInputElement inputSubmit
        {
            get { return Document.CreateElement<IInputElement>("input").InputType(InputType.Submit); }
        }

        public IInputTextElement inputText
        {
            get { return Document.CreateElement<IInputTextElement>("input").InputType(InputType.Text); }
        }

        public IEditElement ins
        {
            get { return Document.CreateElement<IEditElement>("ins"); }
        }

        public IInlineElement kbd
        {
            get { return Document.CreateElement<IInlineElement>("kbd"); }
        }

        public ILabelElement label
        {
            get { return Document.CreateElement<ILabelElement>(); }
        }

        public ILegendElement legend
        {
            get { return Document.CreateElement<ILegendElement>(); }
        }

        public ILiElement li
        {
            get { return Document.CreateElement<ILiElement>(); }
        }

        public ILinkElement link
        {
            get { return Document.CreateElement<ILinkElement>(); }
        }

        public IMetaElement meta
        {
            get { return Document.CreateElement<IMetaElement>(); }
        }

        public INoScriptElement noscript
        {
            get { return Document.CreateElement<INoScriptElement>(); }
        }

        public IObjectElement @object
        {
            get { return Document.CreateElement<IObjectElement>(); }
        }

        public IListElement ol
        {
            get { return Document.CreateElement<IListElement>("ol"); }
        }

        public IOptgroupElement optgroup
        {
            get { return Document.CreateElement<IOptgroupElement>(); }
        }

        public IOptionElement option
        {
            get { return Document.CreateElement<IOptionElement>(); }
        }

        public IPElement p
        {
            get { return Document.CreateElement<IPElement>(); }
        }

        public string PageTitle
        {
            get { return base.Title; }
            set { base.Title = value; }
        }

        public IParamElement param
        {
            get { return Document.CreateElement<IParamElement>(); }
        }

        public IPreElement pre
        {
            get { return Document.CreateElement<IPreElement>(); }
        }

        public IQElement q
        {
            get { return Document.CreateElement<IQElement>(); }
        }

        public IDependencyResolver Resolver { get; set; }

        public IInlineElement samp
        {
            get { return Document.CreateElement<IInlineElement>("samp"); }
        }

        public IScriptElement script
        {
            get { return Document.CreateElement<IScriptElement>(); }
        }

        public ISelectElement select
        {
            get { return Document.CreateElement<ISelectElement>(); }
        }

        public IInlineElement small
        {
            get { return Document.CreateElement<IInlineElement>("small"); }
        }

        public IInlineElement span
        {
            get { return Document.CreateElement<IInlineElement>("span"); }
        }

        public IInlineElement strong
        {
            get { return Document.CreateElement<IInlineElement>("strong"); }
        }

        public IStyleElement style
        {
            get { return Document.CreateElement<IStyleElement>(); }
        }

        public IInlineElement sub
        {
            get { return Document.CreateElement<IInlineElement>("sub"); }
        }

        public IInlineElement sup
        {
            get { return Document.CreateElement<IInlineElement>("sup"); }
        }

        public ITableElement table
        {
            get { return Document.CreateElement<ITableElement>(); }
        }

        public ITBodyElement tbody
        {
            get { return Document.CreateElement<ITBodyElement>(); }
        }

        public ITdElement td
        {
            get { return Document.CreateElement<ITdElement>(); }
        }

        public ITextAreaElement textarea
        {
            get { return Document.CreateElement<ITextAreaElement>(); }
        }

        public ITFootElement tfoot
        {
            get { return Document.CreateElement<ITFootElement>(); }
        }

        public IThElement th
        {
            get { return Document.CreateElement<IThElement>(); }
        }

        public ITHeadElement thead
        {
            get { return Document.CreateElement<ITHeadElement>(); }
        }

        public ITitleElement title
        {
            get { return Document.CreateElement<ITitleElement>(); }
        }

        public ITrElement tr
        {
            get { return Document.CreateElement<ITrElement>(); }
        }

        public IInlineElement tt
        {
            get { return Document.CreateElement<IInlineElement>("tt"); }
        }

        public IListElement ul
        {
            get { return Document.CreateElement<IListElement>("ul"); }
        }

        public IInlineElement var
        {
            get { return Document.CreateElement<IInlineElement>("var"); }
        }

        public IXhtmlAnchor Xhtml
        {
            get
            {
                if (_xhtmlAnchor == null)
                    _xhtmlAnchor = new XhtmlAnchor(Resolver, new XhtmlTextWriter(Response.Output), () => User);
                return _xhtmlAnchor;
            }
        }

        public IDisposable scope(IContentModel element)
        {
            return IXhtmlAnchorSiteExtensions.scope(this, element);
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