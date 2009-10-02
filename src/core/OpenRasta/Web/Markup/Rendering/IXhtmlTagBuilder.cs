#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion
using OpenRasta.Web.Markup.Elements;
using OpenRasta.Web.Markup.Modules;

namespace OpenRasta.Web.Markup.Rendering
{
    public interface IXhtmlTagBuilder
    {
        IBodyElement body { get; }
        IHeadElement head { get; }
        IHtmlElement html { get; }
        ITitleElement title { get; }
        IHElement h1 { get; }
        IHElement h2 { get; }
        IHElement h3 { get; }
        IHElement h4 { get; }
        IHElement h5 { get; }
        IHElement h6 { get; }
        IAddressElement address { get; }
        IBlockQuoteElement blockquote { get; }
        IDivElement div { get; }
        IPElement p { get; }
        IPreElement pre { get; }
        IInlineElement abbr { get; }
        IInlineElement acronym { get; }
        IEmptyElement br { get; }
        IInlineElement cite { get; }
        IInlineElement code { get; }
        IInlineElement dfn { get; }
        IInlineElement em { get; }
        IInlineElement kbd { get; }
        IQElement q { get; }
        IInlineElement samp { get; }
        IInlineElement span { get; }
        IInlineElement strong { get; }
        IInlineElement var { get; }
        IAElement a { get; }
        IDlElement dl { get; }
        IDtElement dt { get; }
        IDdElement dd { get; }
        IListElement ul { get; }
        IListElement ol { get; }
        ILiElement li { get; }
        IInlineElement b { get; }
        IInlineElement big { get; }
        IEmptyElement hr { get; }
        IInlineElement i { get; }
        IInlineElement small { get; }
        IInlineElement sub { get; }
        IInlineElement sup { get; }
        IInlineElement tt { get; }
        IEditElement del { get; }
        IEditElement ins { get; }
        IImgElement img { get; }
        IFormElement form { get; }
        IInputTextElement inputText { get; }
        IInputTextElement inputPassword { get; }
        IInputCheckedElement inputCheckbox { get; }
        IInputCheckedElement inputRadio { get; }
        IInputElement inputSubmit { get; }
        IInputElement inputReset { get; }
        IInputElement inputFile { get; }
        IInputElement inputHidden { get; }
        IInputImageElement inputImage { get; }
        ISelectElement select { get; }
        IOptionElement option { get; }
        ITextAreaElement textarea { get; }
        IButtonElement button { get; }
        IFieldsetElement fieldset { get; }
        ILabelElement label { get; }
        ILegendElement legend { get; }
        IOptgroupElement optgroup { get; }
        IObjectElement @object { get; }
        IParamElement param { get; }
        IIFrameElement iframe { get; }
        IMetaElement meta { get; }
        IStyleElement style { get; }
        ILinkElement link { get; }
        IScriptElement script { get; }
        INoScriptElement noscript { get; }
        IBaseElement @base { get; }
        IBdoElement bdo { get; }
        ICaptionElement caption { get; }
        ITableElement table { get; }
        ITrElement tr { get; }
        IThElement th { get; }
        ITdElement td { get; }
        ITHeadElement thead { get; }
        ITBodyElement tbody { get; }
        ITFootElement tfoot { get; }
        IColElement col { get; }
        IColGroupElement colgroup { get; }
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
