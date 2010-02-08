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

// Defines the Forms module
// http://www.w3.org/TR/xhtml-modularization/abstract_modules.html#s_extformsmodule

namespace OpenRasta.Web.Markup.Modules
{
    /// <summary>
    /// Represents the &lt;form&gt; element.
    /// </summary>
    public interface IFormElement : IContentSetForm,
                                    IAttributesCommon,
                                    IAcceptAttribute,
                                    IContentModel<IFormElement, IContentSetHeading>,
                                    IContentModel<IFormElement, IContentSetList>,
                                    IContentModel<IFormElement, IContentSetBlock>,
                                    IContentModel<IFormElement, IFieldsetElement>
    {
        [Charsets("accept-charset")]
        IList<string> AcceptCharset { get; }

        [URI]
        Uri Action { get; set; }

        [CDATA(DefaultValue = "GET")]
        string Method { get; set; }

        [ContentType(DefaultValue = "application/www-url-formencoded")]
        MediaType EncType { get; set; }
    }

    public interface IInputElement : IAttributesCommon,
                                     IAcceptAttribute,
                                     IAccessKeyAttribute,
                                     IAltAttribute,
                                     IDisabledAttribute,
                                     INameAttribute,
                                     IReadOnlyAttribute,
                                     ITabIndexAttribute,
                                     IValueAttribute,
                                     IContentSetFormctrl
    {
        [InputType]
        InputType Type { get; set; }
    }
    public enum InputType
    {
        Text,
        Password,
        CheckBox,
        Radio,
        Submit,
        Reset,
        Hidden,
        Image,
        File
    }
    public class InputTypeAttribute:EnumAttributeCore
    {
        public InputTypeAttribute():base(Factory<InputType>){}
    }
    /// <summary>
    /// Represents the &lt;input type="text" /&gt; element.
    /// </summary>
    public interface IInputTextElement : IInputElement, ISizeAttribute
    {
        [Number]
        int? MaxLength { get; set; }
    }

    /// <summary>
    /// Represents the input tags checkbox and radio
    /// </summary>
    public interface IInputCheckedElement : IInputElement
    {
        [Boolean]
        bool Checked { get; set; }
    }

    /// <summary>
    /// Represents the &lt;input type="image" /&gt; element.
    /// </summary>
    public interface IInputImageElement : IInputElement, ISrcAttribute
    {
    }

    /// <summary>
    /// Represents the &lt;select&gt; element.
    /// </summary>
    public interface ISelectElement : IAttributesCommon,
                                      IDisabledAttribute,
                                      INameAttribute,
                                      ISizeAttribute,
                                      ITabIndexAttribute,
                                      IContentSetFormctrl,
                                      IContentModel<ISelectElement, IOptgroupElement>,
                                      IContentModel<ISelectElement, IOptionElement>
    {
        [Boolean]
        bool? Multiple { get; set; }
    }

    /// <summary>
    /// Represents the &lt;optgroup&gt; element.
    /// </summary>
    public interface IOptgroupElement : IAttributesCommon,
                                        IDisabledAttribute,
                                        ILabelAttribute,
                                        IContentModel<IOptgroupElement, IOptionElement>
    {
    }

    /// <summary>
    /// Represents the &lt;option&gt; element.
    /// </summary>
    public interface IOptionElement : IAttributesCommon,
                                      IDisabledAttribute,
                                      ILabelAttribute,
                                      IValueAttribute,
                                      IContentModel<IOptionElement, string>
    {
        [Boolean]
        bool Selected { get; set; }
    }

    /// <summary>
    /// Represents the &lt;textarea&gt; element.
    /// </summary>
    public interface ITextAreaElement : IAttributesCommon,
                                        IAccessKeyAttribute,
                                        IDisabledAttribute,
                                        INameAttribute,
                                        IReadOnlyAttribute,
                                        ITabIndexAttribute,
                                        IContentSetFormctrl,
                                        IContentModel<ITextAreaElement, string>
    {
        [Number]
        int? Cols { get; set; }

        [Number]
        int? Rows { get; set; }
    }

    /// <summary>
    /// Represents the &lt;button&gt; element.
    /// </summary>
    public interface IButtonElement : IAttributesCommon,
                                      IAccessKeyAttribute,
                                      IDisabledAttribute,
                                      INameAttribute,
                                      ITabIndexAttribute,
                                      IValueAttribute,
                                      IContentSetFormctrl,
                                      IContentModel<IButtonElement, string>,
                                      IContentModel<IButtonElement, IContentSetList>,
                                      IContentModel<IButtonElement, IContentSetHeading>,
                                      IContentModel<IButtonElement, IContentSetBlock>,
                                      IContentModel<IButtonElement, IContentSetInline>
    {
        [ButtonTypeAttribute]
        ButtonType Type { get; set; }
    }
    /// <summary>
    /// Represents the &lt;fieldset&gt; element.
    /// </summary>
    public interface IFieldsetElement : IAttributesCommon,
                                        IContentSetForm,
                                        IContentModel<IFieldsetElement, string>,
                                        IContentModel<IFieldsetElement, ILegendElement>,
                                        IContentModel<IFieldsetElement, IContentSetFlow>
    {
    }
    /// <summary>
    /// Represents the &lt;label&gt; element.
    /// </summary>
    public interface ILabelElement : IAttributesCommon,
                                     IAccessKeyAttribute,
                                     IContentSetFormctrl,
                                     IContentModel<ILabelElement, IContentSetInline>,
                                     IContentModel<ILabelElement, string>
    {
        [IDREF]
        string For { get; set; }
    }
    /// <summary>
    /// Represents the &lt;legend&gt; element.
    /// </summary>
    public interface ILegendElement : IAttributesCommon,
                                      IAccessKeyAttribute,
                                      IContentModel<ILegendElement, string>,
                                      IContentModel<ILegendElement, IContentSetInline>
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