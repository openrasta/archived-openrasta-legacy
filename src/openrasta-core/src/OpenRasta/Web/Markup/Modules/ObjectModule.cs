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

// The object module
// http://www.w3.org/TR/xhtml-modularization/abstract_modules.html#s_objectmodule
namespace OpenRasta.Web.Markup.Elements
{
    /// <summary>
    /// Represents the &lt;object&gt; element
    /// </summary>
    public interface IObjectElement : IAttributesCommon,
                                      INameAttribute,
                                      IWidthHeightAttribute,
                                      ITabIndexAttribute,
                                      ITypeAttribute,
                                      IContentModel<IObjectElement, string>,
                                      IContentModel<IObjectElement, IContentSetFlow>,
                                      IContentModel<IObjectElement, IParamElement>,
                                      IContentSetInline
    {
        [URIs]
        IList<Uri> Archive { get; set; }

        [URI]
        Uri ClassID { get; set; }

        [URI]
        Uri CodeBase { get; set; }

        [ContentType]
        MediaType CodeType { get; set; }

        [URI]
        Uri Data { get; set; }

        [Boolean]
        bool Declare { get; set; }

        [CDATA]
        string StandBy { get; set; }
    }

    /// <summary>
    /// Represents the &lt;param&gt; element
    /// </summary>
    public interface IParamElement : IElement,
                                     IIDAttribute,
                                     INameAttribute,
                                     ITypeAttribute,
                                     IValueAttribute
    {
        [ParamValueTypeAttribute]
        ParamValueType ValueType { get; set; }
    }

    public class ParamValueTypeAttributeAttribute : PrimaryTypeAttributeCore
    {
        public ParamValueTypeAttributeAttribute()
            : base(Factory<ParamValueType>)
        {
        }
    }

    public enum ParamValueType
    {
        Data,
        Ref,
        Object
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