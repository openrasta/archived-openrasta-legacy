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
using System.Xml;
using OpenRasta.Web.Markup.Attributes;
using OpenRasta.Web.Markup.Modules;

namespace OpenRasta.Web.Markup.Elements
{
    public class GenericElement : Element,
                                  IBodyElement, IHeadElement, IHtmlElement, ITitleElement, IHElement, IAddressElement, IBlockQuoteElement, IDivElement, IPreElement, IQElement,
                                  IAElement, IDlElement, IDtElement, IDdElement, IListElement, ILiElement, IEditElement, IImgElement, IFormElement, IInputCheckedElement,
                                  IInputTextElement, IInputImageElement, IOptionElement, IOptgroupElement, IObjectElement, IParamElement, IIFrameElement, IMetaElement,
                                  IStyleElement, ILinkElement, IScriptElement, INoScriptElement, ITextAreaElement, ISelectElement, ILabelElement, IFieldsetElement, ILegendElement,
                                  IButtonElement, ITableElement, ITrElement, ITdElement, ITHeadElement, ITBodyElement, ITFootElement, ICaptionElement, IColElement, IThElement
    {
        public GenericElement(string tagName)
            : base(tagName)
        {
        }

        public new GenericElement this[INode child]
        {
            get
            {
                ChildNodes.Add(child);
                return this;
            }
        }

        public GenericElement this[string child]
        {
            get { return this[new TextNode(child)]; }
        }
        IAddressElement IContentModel<IAddressElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        IAddressElement IContentModel<IAddressElement, IContentSetInline>.this[IContentSetInline child]
        {
            get { return this[child]; }
        }

        IAElement IContentModel<IAElement, IContentSetInline>.this[IContentSetInline child]
        {
            get { return this[child]; }
        }

        IAElement IContentModel<IAElement, string>.this[string child]
        {
            get { return this[child]; }
        }
        ILabelElement IContentModel<ILabelElement, string>.this[string child]
        {
            get { return this[child]; }
        }
        public char? AccessKey
        {
            get { return Attributes.GetAttribute<char?>("accesskey"); }
            set { Attributes.SetAttribute("accesskey", value); }
        }

        MediaType ITypeAttribute.Type
        {
            get { return Attributes.GetAttribute<MediaType>("type"); }
            set { Attributes.SetAttribute("type", value); }
        }

        public int? TabIndex
        {
            get { return Attributes.GetAttribute<int?>("tabindex"); }
            set { Attributes.SetAttribute("tabindex", value); }
        }

        public string CharSet
        {
            get { return Attributes.GetAttribute("charset"); }
            set { Attributes.SetAttribute("charset", value); }
        }

        public IList<string> Rel
        {
            get { return Attributes.GetAttribute<IList<string>>("rel"); }
            set { Attributes.SetAttribute("rel", value); }
        }

        public IList<string> Rev
        {
            get { return Attributes.GetAttribute<IList<string>>("rev"); }
            set { Attributes.SetAttribute("rev", value); }
        }

        string IHrefAttribute.HrefLang
        {
            get { return Attributes.GetAttribute("hreflang"); }
            set { Attributes.SetAttribute("hreflang", value); }
        }

        Uri IHrefAttribute.Href
        {
            get { return Attributes.GetAttribute<Uri>("href"); }
            set { Attributes.SetAttribute("href", value); }
        }

        IBlockQuoteElement IContentModel<IBlockQuoteElement, IContentSetHeading>.this[IContentSetHeading child]
        {
            get { return this[child]; }
        }

        IBlockQuoteElement IContentModel<IBlockQuoteElement, IContentSetBlock>.this[IContentSetBlock child]
        {
            get { return this[child]; }
        }

        IBlockQuoteElement IContentModel<IBlockQuoteElement, IContentSetList>.this[IContentSetList child]
        {
            get { return this[child]; }
        }

        IBodyElement IContentModel<IBodyElement, IContentSetHeading>.this[IContentSetHeading child]
        {
            get { return this[child]; }
        }

        IBodyElement IContentModel<IBodyElement, IContentSetBlock>.this[IContentSetBlock child]
        {
            get { return this[child]; }
        }

        IBodyElement IContentModel<IBodyElement, IContentSetList>.this[IContentSetList child]
        {
            get { return this[child]; }
        }

        public string ID
        {
            get { return Attributes.GetAttribute("id"); }
            set { Attributes.SetAttribute("id", value); }
        }

        public string Title
        {
            get { return Attributes.GetAttribute("title"); }
            set { Attributes.SetAttribute("title", value); }
        }

        public IList<string> Class
        {
            get { return Attributes.GetAttribute<IList<string>>("class"); }
        }

        public XmlSpace XmlSpace
        {
            get { return Attributes.GetAttribute<XmlSpace>("xml:space"); }
            set { Attributes.SetAttribute("xml:space", value); }
        }

        public Direction Dir
        {
            get { return Attributes.GetAttribute<Direction>("dir"); }
            set { Attributes.SetAttribute("dir", value); }
        }

        public string XmlLang
        {
            get { return Attributes.GetAttribute("lang"); }
            set { Attributes.SetAttribute("lang", value); }
        }

        public string Style
        {
            get { return Attributes.GetAttribute("style"); }
            set { Attributes.SetAttribute("style", value); }
        }

        IButtonElement IContentModel<IButtonElement, IContentSetList>.this[IContentSetList child]
        {
            get { return this[child]; }
        }

        IButtonElement IContentModel<IButtonElement, IContentSetHeading>.this[IContentSetHeading child]
        {
            get { return this[child]; }
        }

        IButtonElement IContentModel<IButtonElement, IContentSetBlock>.this[IContentSetBlock child]
        {
            get { return this[child]; }
        }

        IButtonElement IContentModel<IButtonElement, IContentSetInline>.this[IContentSetInline child]
        {
            get { return this[child]; }
        }

        public ButtonType Type
        {
            get { return Attributes.GetAttribute<ButtonType>("type"); }
            set { Attributes.SetAttribute("type", value); }
        }

        IDdElement IContentModel<IDdElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        IDdElement IContentModel<IDdElement, IContentSetFlow>.this[IContentSetFlow child]
        {
            get { return this[child]; }
        }

        IDivElement IContentModel<IDivElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        IDivElement IContentModel<IDivElement, IContentSetFlow>.this[IContentSetFlow child]
        {
            get { return this[child]; }
        }

        IDlElement IContentModel<IDlElement, IDtElement>.this[IDtElement child]
        {
            get { return this[child]; }
        }

        IDlElement IContentModel<IDlElement, IDdElement>.this[IDdElement child]
        {
            get { return this[child]; }
        }

        IDtElement IContentModel<IDtElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        IDtElement IContentModel<IDtElement, IContentSetInline>.this[IContentSetInline child]
        {
            get { return this[child]; }
        }

        IEditElement IContentModel<IEditElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        IEditElement IContentModel<IEditElement, IContentSetFlow>.this[IContentSetFlow child]
        {
            get { return this[child]; }
        }

        public DateTime? DateTime
        {
            get { return Attributes.GetAttribute<DateTime>("datetime"); }
            set { Attributes.SetAttribute("datetime", value); }
        }

        IButtonElement IContentModel<IButtonElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        IFormElement IContentModel<IFormElement, IContentSetHeading>.this[IContentSetHeading child]
        {
            get { return this[child]; }
        }

        IFormElement IContentModel<IFormElement, IContentSetList>.this[IContentSetList child]
        {
            get { return this[child]; }
        }

        IFormElement IContentModel<IFormElement, IContentSetBlock>.this[IContentSetBlock child]
        {
            get { return this[child]; }
        }

        IFormElement IContentModel<IFormElement, IFieldsetElement>.this[IFieldsetElement child]
        {
            get { return this[child]; }
        }

        public virtual IList<MediaType> Accept
        {
            get { return Attributes.GetAttribute<IList<MediaType>>("accept"); }
        }

        public virtual IList<string> AcceptCharset
        {
            get { return Attributes.GetAttribute<IList<string>>("accept-charset"); }
        }

        public virtual Uri Action
        {
            get { return Attributes.GetAttribute<Uri>("action"); }
            set { Attributes.SetAttribute("action", value); }
        }

        public virtual string Method
        {
            get { return Attributes.GetAttribute("method"); }
            set { Attributes.SetAttribute("method", value); }
        }

        public virtual MediaType EncType
        {
            get { return Attributes.GetAttribute<MediaType>("enctype"); }
            set { Attributes.SetAttribute("enctype", value); }
        }

        IHeadElement IContentModel<IHeadElement, ITitleElement>.this[ITitleElement child]
        {
            get { return this[child]; }
        }

        IHeadElement IContentModel<IHeadElement, ILinkElement>.this[ILinkElement child]
        {
            get { return this[child]; }
        }

        IHeadElement IContentModel<IHeadElement, IScriptElement>.this[IScriptElement child]
        {
            get { return this[child]; }
        }

        IHeadElement IContentModel<IHeadElement, IStyleElement>.this[IStyleElement child]
        {
            get { return this[child]; }
        }

        IHeadElement IContentModel<IHeadElement, IMetaElement>.this[IMetaElement child]
        {
            get { return this[child]; }
        }
        IHeadElement IContentModel<IHeadElement, IBaseElement>.this[IBaseElement child]
        {
            get { return this[child]; }
        }

        public IList<Uri> Profile
        {
            get { return Attributes.GetAttribute<IList<Uri>>("profile"); }
            set { Attributes.SetAttribute("profile", value); }
        }

        IInlineElement IContentModel<IInlineElement, IContentSetInline>.this[IContentSetInline child]
        {
            get { return this[child]; }
        }

        Uri ICiteAttribute.Cite
        {
            get { return Attributes.GetAttribute<Uri>("cite"); }
            set { Attributes.SetAttribute("cite", value); }
        }

        IHtmlElement IContentModel<IHtmlElement, IHeadElement>.this[IHeadElement child]
        {
            get { return this[child]; }
        }

        IHtmlElement IContentModel<IHtmlElement, IBodyElement>.this[IBodyElement child]
        {
            get { return this[child]; }
        }

        public string Version
        {
            get { return Attributes.GetAttribute("version"); }
            set { Attributes.SetAttribute("version", value); }
        }

        public Uri XmlNS
        {
            get { return Attributes.GetAttribute<Uri>("xmlns"); }
            set { Attributes.SetAttribute("xmlns", value); }
        }

        IIFrameElement IContentModel<IIFrameElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        IIFrameElement IContentModel<IIFrameElement, IContentSetFlow>.this[IContentSetFlow child]
        {
            get { return this[child]; }
        }

        public virtual bool FrameBorder
        {
            get { return Attributes.GetAttribute<bool>("frameborder"); }
            set { Attributes.SetAttribute("frameborder", value); }
        }

        public virtual int? MarginWidth
        {
            get { return Attributes.GetAttribute<int?>("marginwidth"); }
            set { Attributes.SetAttribute("marginwidth", value); }
        }

        public virtual int? MarginHeight
        {
            get { return Attributes.GetAttribute<int?>("marginheight"); }
            set { Attributes.SetAttribute("marginheight", value); }
        }

        public virtual Scrolling Scrolling
        {
            get { return Attributes.GetAttribute<Scrolling>("scrolling"); }
            set { Attributes.SetAttribute("scrolling", value); }
        }

        string IWidthAttribute.Width
        {
            get { return Attributes.GetAttribute("width"); }
            set { Attributes.SetAttribute("width", value); }
        }

        string IWidthHeightAttribute.Height
        {
            get { return Attributes.GetAttribute("height"); }
            set { Attributes.SetAttribute("height", value); }
        }

        public virtual string Alt
        {
            get { return Attributes.GetAttribute("alt"); }
            set { Attributes.SetAttribute("alt", value); }
        }

        public virtual Uri LongDesc
        {
            get { return Attributes.GetAttribute<Uri>("longdesc"); }
            set { Attributes.SetAttribute("longdesc", value); }
        }

        Uri ISrcAttribute.Src
        {
            get { return Attributes.GetAttribute<Uri>("src"); }
            set { Attributes.SetAttribute("src", value); }
        }

        public virtual bool ReadOnly
        {
            get { return Attributes.GetAttribute<bool>("readonly"); }
            set { Attributes.SetAttribute("readonly", value); }
        }

        public virtual bool Disabled
        {
            get { return Attributes.GetAttribute<bool>("disabled"); }
            set { Attributes.SetAttribute("disabled", value); }
        }

        public virtual string Name
        {
            get { return Attributes.GetAttribute("name"); }
            set { Attributes.SetAttribute("name", value); }
        }

        public virtual string Value
        {
            get { return Attributes.GetAttribute("value"); }
            set { Attributes.SetAttribute("value", value); }
        }

        InputType IInputElement.Type
        {
            get { return Attributes.GetAttribute<InputType>("type"); }
            set { Attributes.SetAttribute("type", value); }
        }

        public virtual bool Checked
        {
            get { return Attributes.GetAttribute<bool>("checked"); }
            set { Attributes.SetAttribute("checked", value); }
        }

        public virtual int? Size
        {
            get { return Attributes.GetAttribute<int?>("size"); }
            set { Attributes.SetAttribute("size", value); }
        }

        public virtual int? MaxLength
        {
            get { return Attributes.GetAttribute<int?>("maxlength"); }
            set { Attributes.SetAttribute("maxlength", value); }
        }

        ILabelElement IContentModel<ILabelElement, IContentSetInline>.this[IContentSetInline child]
        {
            get { return this[child]; }
        }

        public string For
        {
            get { return Attributes.GetAttribute("for"); }
            set { Attributes.SetAttribute("for", value); }
        }

        ILegendElement IContentModel<ILegendElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        ILegendElement IContentModel<ILegendElement, IContentSetInline>.this[IContentSetInline child]
        {
            get { return this[child]; }
        }

        ILiElement IContentModel<ILiElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        ILiElement IContentModel<ILiElement, IContentSetFlow>.this[IContentSetFlow child]
        {
            get { return this[child]; }
        }

        IListElement IContentModel<IListElement, ILiElement>.this[ILiElement child]
        {
            get { return this[child]; }
        }

        public virtual string Scheme
        {
            get { return Attributes.GetAttribute("scheme"); }
            set { Attributes.SetAttribute("scheme", value); }
        }

        public virtual string Content
        {
            get { return Attributes.GetAttribute("content"); }
            set { Attributes.SetAttribute("content", value); }
        }

        public virtual string HttpEquiv
        {
            get { return Attributes.GetAttribute("http-equiv"); }
            set { Attributes.SetAttribute("http-equiv", value); }
        }

        INoScriptElement IContentModel<INoScriptElement, IContentSetHeading>.this[IContentSetHeading child]
        {
            get { return this[child]; }
        }

        INoScriptElement IContentModel<INoScriptElement, IContentSetList>.this[IContentSetList child]
        {
            get { return this[child]; }
        }

        INoScriptElement IContentModel<INoScriptElement, IContentSetBlock>.this[IContentSetBlock child]
        {
            get { return this[child]; }
        }

        IObjectElement IContentModel<IObjectElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        IObjectElement IContentModel<IObjectElement, IContentSetFlow>.this[IContentSetFlow child]
        {
            get { return this[child]; }
        }

        IObjectElement IContentModel<IObjectElement, IParamElement>.this[IParamElement child]
        {
            get { return this[child]; }
        }

        public virtual IList<Uri> Archive
        {
            get { return Attributes.GetAttribute<IList<Uri>>("archive"); }
            set { Attributes.SetAttribute("archive", value); }
        }

        public virtual Uri ClassID
        {
            get { return Attributes.GetAttribute<Uri>("classid"); }
            set { Attributes.SetAttribute("classid", value); }
        }

        public virtual Uri CodeBase
        {
            get { return Attributes.GetAttribute<Uri>("codebase"); }
            set { Attributes.SetAttribute("codebase", value); }
        }

        public virtual MediaType CodeType
        {
            get { return Attributes.GetAttribute<MediaType>("codetype"); }
            set { Attributes.SetAttribute("codetype", value); }
        }

        public virtual Uri Data
        {
            get { return Attributes.GetAttribute<Uri>("data"); }
            set { Attributes.SetAttribute("data", value); }
        }

        public virtual bool Declare
        {
            get { return Attributes.GetAttribute<bool>("declare"); }
            set { Attributes.SetAttribute("declare", value); }
        }

        public virtual string StandBy
        {
            get { return Attributes.GetAttribute("standby"); }
            set { Attributes.SetAttribute("standby", value); }
        }

        IOptgroupElement IContentModel<IOptgroupElement, IOptionElement>.this[IOptionElement child]
        {
            get { return this[child]; }
        }

        public virtual string Label
        {
            get { return Attributes.GetAttribute("label"); }
            set { Attributes.SetAttribute("label", value); }
        }

        IOptionElement IContentModel<IOptionElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        public virtual bool Selected
        {
            get { return Attributes.GetAttribute<bool>("selected"); }
            set { Attributes.SetAttribute("selected", value); }
        }

        public virtual ParamValueType ValueType
        {
            get { return Attributes.GetAttribute<ParamValueType>("valuetype"); }
            set { Attributes.SetAttribute("valuetype", value); }
        }

        IPElement IContentModel<IPElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        IPElement IContentModel<IPElement, IContentSetInline>.this[IContentSetInline child]
        {
            get { return this[child]; }
        }

        IInlineElement IContentModel<IInlineElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        IScriptElement IContentModel<IScriptElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        public virtual bool Defer
        {
            get { return Attributes.GetAttribute<bool>("defer"); }
            set { Attributes.SetAttribute("defer", value); }
        }

        ISelectElement IContentModel<ISelectElement, IOptgroupElement>.this[IOptgroupElement child]
        {
            get { return this[child]; }
        }

        ISelectElement IContentModel<ISelectElement, IOptionElement>.this[IOptionElement child]
        {
            get { return this[child]; }
        }

        public bool? Multiple
        {
            get { return Attributes.GetAttribute<bool?>("multiple"); }
            set { Attributes.SetAttribute("multiple", value); }
        }

        IStyleElement IContentModel<IStyleElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        public virtual IList<string> Media
        {
            get { return Attributes.GetAttribute<IList<string>>("media"); }
        }

        ITextAreaElement IContentModel<ITextAreaElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        public virtual int? Cols
        {
            get { return Attributes.GetAttribute<int?>("cols"); }
            set { Attributes.SetAttribute("cols", value); }
        }

        public virtual int? Rows
        {
            get { return Attributes.GetAttribute<int?>("rows"); }
            set { Attributes.SetAttribute("rows", value); }
        }

        ITitleElement IContentModel<ITitleElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        IFieldsetElement IContentModel<IFieldsetElement, ILegendElement>.this[ILegendElement child]
        {
            get { return this[child]; }
        }

        IFieldsetElement IContentModel<IFieldsetElement, IContentSetFlow>.this[IContentSetFlow child]
        {
            get { return this[child]; }
        }

        IFieldsetElement IContentModel<IFieldsetElement, string>.this[string child]
        {
            get { return this[child]; }
        }
        IHElement IContentModel<IHElement, IContentSetInline>.this[IContentSetInline child]
        {
            get { return this[child]; }
        }
        IHElement IContentModel<IHElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        ITableElement IContentModel<ITableElement, ICaptionElement>.this[ICaptionElement child]
        {
            get { return this[child]; }
        }

        ITableElement IContentModel<ITableElement, IColElement>.this[IColElement child]
        {
            get { return this[child]; }
        }

        ITableElement IContentModel<ITableElement, IColGroupElement>.this[IColGroupElement child]
        {
            get { return this[child]; }
        }

        ITableElement IContentModel<ITableElement, ITHeadElement>.this[ITHeadElement child]
        {
            get { return this[child]; }
        }

        ITableElement IContentModel<ITableElement, ITBodyElement>.this[ITBodyElement child]
        {
            get { return this[child]; }
        }

        ITableElement IContentModel<ITableElement, ITFootElement>.this[ITFootElement child]
        {
            get { return this[child]; }
        }

        public int? Border
        {
            get { return Attributes.GetAttribute<int?>("border"); }
            set { Attributes.SetAttribute("border", value); }
        }

        public string CellPadding
        {
            get { return Attributes.GetAttribute("cellpadding"); }
            set { Attributes.SetAttribute("cellpadding", value); }
        }

        public string CellSpacing
        {
            get { return Attributes.GetAttribute("cellspacing"); }
            set { Attributes.SetAttribute("cellspacing", value); }
        }

        public Frame Frame
        {
            get { return Attributes.GetAttribute<Frame>("frame"); }
            set { Attributes.SetAttribute("frame", value); }
        }

        public Rules Rules
        {
            get { return Attributes.GetAttribute<Rules>("rules"); }
            set { Attributes.SetAttribute("rules", value); }
        }

        public string Summary
        {
            get { return Attributes.GetAttribute("summary"); }
            set { Attributes.SetAttribute("summary", value); }
        }

        public Alignment Align
        {
            get { return Attributes.GetAttribute<Alignment>("align"); }
            set { Attributes.SetAttribute("align", value); }
        }

        public char? Char
        {
            get { return Attributes.GetAttribute<char?>("char"); }
            set { Attributes.SetAttribute("char", value); }
        }

        public string CharOff
        {
            get { return Attributes.GetAttribute("charoff"); }
            set { Attributes.SetAttribute("cellspacing", value); }
        }

        public VerticalAlignment Valign
        {
            get { return Attributes.GetAttribute<VerticalAlignment>("valign"); }
            set { Attributes.SetAttribute("valign", value); }
        }

        ITrElement IContentModel<ITrElement, ITdElement>.this[ITdElement child]
        {
            get { return this[child]; }
        }

        ITrElement IContentModel<ITrElement, IThElement>.this[IThElement child]
        {
            get { return this[child]; }
        }

        public string Abbr
        {
            get { return Attributes.GetAttribute("abbr"); }
            set { Attributes.SetAttribute("abbr", value); }
        }

        public string Axis
        {
            get { return Attributes.GetAttribute("axis"); }
            set { Attributes.SetAttribute("axis", value); }
        }

        public int? ColSpan
        {
            get { return Attributes.GetAttribute<int?>("colspan"); }
            set { Attributes.SetAttribute("colspan", value); }
        }

        public int? RowSpan
        {
            get { return Attributes.GetAttribute<int?>("rowspan"); }
            set { Attributes.SetAttribute("rowspan", value); }
        }

        public IList<string> Headers
        {

            get { return Attributes.GetAttribute<IList<string>>("headers"); }
            set { Attributes.SetAttribute("headers", value); }
        }

        public Scope Scope
        {
            get { return Attributes.GetAttribute<Scope>("scope"); }
            set { Attributes.SetAttribute("scope", value); }
        }

        ITdElement IContentModel<ITdElement, IContentSetFlow>.this[IContentSetFlow child]
        {
            get { return this[child]; }
        }

        ITdElement IContentModel<ITdElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        ITHeadElement IContentModel<ITHeadElement, ITrElement>.this[ITrElement child]
        {
            get { return this[child]; }
        }

        ICaptionElement IContentModel<ICaptionElement, IContentSetInline>.this[IContentSetInline child]
        {
            get { return this[child]; }
        }

        ICaptionElement IContentModel<ICaptionElement, string>.this[string child]
        {
            get { return this[child]; }
        }

        public int? Span
        {
            get { return Attributes.GetAttribute<int?>("span"); }
            set { Attributes.SetAttribute("span", value); }
        }

        IThElement IContentModel<IThElement, IContentSetFlow>.this[IContentSetFlow child]
        {
            get { return this[child]; }
        }

        IThElement IContentModel<IThElement, string>.this[string child]
        {
            get { return this[child]; }
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