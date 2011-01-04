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
using OpenRasta.Web.Markup.Modules;

namespace OpenRasta.Web.Markup
{
    public static class FormModuleExtensions
    {
        public static T AcceptCharset<T>(this T element, string charset) where T:IFormElement
        {
            element.AcceptCharset.Add(charset);
            return element;
        }
        public static T Action<T>(this T element, string actionUri) where T:IFormElement
        {
            element.Action = new Uri(actionUri);
            return element;
        }
        public static T Action<T>(this T element, Uri actionUri) where T:IFormElement
        {
            element.Action = actionUri;
            return element;
        }
        public static T Method<T>(this T element, string httpMethod) where T:IFormElement
        {
            element.Method = httpMethod;
            return element;
        }
        public static T EncType<T>(this T element, string mediaType) where T:IFormElement
        {
            element.EncType = new MediaType(mediaType);
            return element;
        }
        public static T EncType<T>(this T element, MediaType mediaType) where T : IFormElement
        {
            element.EncType = mediaType;
            return element;
        }


        public static T InputType<T>(this T element, InputType inputType) where T:IInputElement
        {
            element.Type = inputType;
            return element;
        }
        public static T MaxLength<T>(this T element, int maxLength) where T:IInputTextElement
        {
            element.MaxLength = maxLength;
            return element;
        }
        public static T Checked<T>(this T element) where T:IInputCheckedElement
        {
            element.Checked = true;
            return element;
        }
        public static T Multiple<T>(this T element) where T:ISelectElement
        {
            element.Multiple = true;
            return element;
        }
        public static T Selected<T>(this T element) where T : IOptionElement
        {
            element.Selected = true;
            return element;
        }
        public static T Cols<T>(this T element, int columns) where T:ITextAreaElement
        {
            element.Cols = columns;
            return element;
        }
        public static T Rows<T>(this T element, int rows) where T : ITextAreaElement
        {
            element.Rows = rows;
            return element;
        }
        public static T InputType<T>(this T element, ButtonType type) where T:IButtonElement
        {
            element.Type = type;
            return element;
        }
        public static T Submit<T>(this T element) where T:IButtonElement
        {
            return element.InputType(ButtonType.Submit);
        }
        public static T Reset<T>(this T element) where T : IButtonElement
        {
            return element.InputType(ButtonType.Reset);
        }
        public static T For<T>(this T element, string id) where T:ILabelElement
        {
            element.For = id;
            return element;
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
