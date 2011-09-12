#region License

/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */

#endregion

using System.Collections.Generic;
using OpenRasta.Web.Markup.Attributes;
using OpenRasta.Web.Markup.Attributes.Annotations;

// The Table module
// http://www.w3.org/TR/xhtml-modularization/abstract_modules.html#s_tablemodule

namespace OpenRasta.Web.Markup.Modules
{
    /// <summary>
    /// Represents the &lt;caption&gt; element.
    /// </summary>
    public interface ICaptionElement : IAttributesCommon,
                                       IContentModel<ICaptionElement, IContentSetInline>,
                                       IContentModel<ICaptionElement, string>
    {
    }

    /// <summary>
    /// Represents the &lt;table&gt; element.
    /// </summary>
    public interface ITableElement : IAttributesCommon,
                                     IWidthAttribute,
                                     IContentModel<ITableElement, ICaptionElement>,
                                     IContentModel<ITableElement, IColElement>,
                                     IContentModel<ITableElement, IColGroupElement>,
                                     IContentModel<ITableElement, ITHeadElement>,
                                     IContentModel<ITableElement, ITBodyElement>,
                                     IContentModel<ITableElement, ITFootElement>,
                                     IContentSetBlock

    {
        [Pixels]
        int? Border { get; set; }
        [Length]
        string CellPadding { get; set; }
        [Length]
        string CellSpacing { get; set; }
        [Frame]
        Frame Frame { get; set; }
        [Rules]
        Rules Rules { get; set; }
        [Text]
        string Summary { get; set; }
    }

    public interface ITableSectionElementBase : IAttributesCommon,
                                                IAlignAttribute,
                                                ICharAttribute,
                                                IValignAttribute
    {
    }

    /// <summary>
    /// Represents the &lt;thead&gt; element.
    /// </summary>
    public interface ITHeadElement : ITableSectionElementBase,
                                     IContentModel<ITHeadElement, ITrElement>
    {
    }

    /// <summary>
    /// Represents the &lt;tbody&gt; element.
    /// </summary>
    public interface ITBodyElement : ITableSectionElementBase,
                                     IContentModel<ITHeadElement, ITrElement>
    {
    }

    /// <summary>
    /// Represents the &lt;tfoot&gt; element.
    /// </summary>
    public interface ITFootElement : ITableSectionElementBase,
                                     IContentModel<ITHeadElement, ITrElement>
    {
    }

    /// <summary>
    /// Represents the &lt;tr&gt; element.
    /// </summary>
    public interface ITrElement : ITableSectionElementBase,
                                  IContentModel<ITrElement, ITdElement>,
                                  IContentModel<ITrElement, IThElement>
    {
    }

    public interface IColElementBase : IElement, 
                                       IAttributesCommon,
                                       IAlignAttribute,
                                       ICharAttribute,
                                       IValignAttribute
    {
        [Number(DefaultValue = "1")]
        int? Span { get; set; }
    }
    /// <summary>
    /// Defines the &lt;col&gt; element.
    /// </summary>
    public interface IColElement : IColElementBase
    {
    }

    /// <summary>
    /// Defines the &lt;colgroup&gt; element.
    /// </summary>
    public interface IColGroupElement : IColElementBase,
                                        IContentModel<IColGroupElement, IColElement>
    {
    }

    public interface ICellElementBase : IAttributesCommon,
                                        IAlignAttribute,
                                        ICharAttribute,
                                        IValignAttribute
    {
        [Text]
        string Abbr { get; set; }
        [CDATA]
        string Axis { get; set; }

        [Number(DefaultValue = "1")]
        int? ColSpan { get; set; }

        [Number(DefaultValue = "1")]
        int? RowSpan { get; set; }

        [IDREFS]
        IList<string> Headers { get; set; }

        [ScopeAttribute]
        Scope Scope { get; set; }
    }

    /// <summary>
    /// Represents the &lt;td&gt; element.
    /// </summary>
    public interface ITdElement : ICellElementBase,
                                  IContentModel<ITdElement, IContentSetFlow>,
                                  IContentModel<ITdElement, string>
    {
    }

    /// <summary>
    /// Represents the &lt;th&gt; element.
    /// </summary>
    public interface IThElement : ICellElementBase,
                                  IContentModel<IThElement, IContentSetFlow>,
                                  IContentModel<IThElement, string>
    {
    }

    public interface IValignAttribute
    {
        [VerticalAlignment]
        VerticalAlignment Valign { get; set; }
    }


    public interface ICharAttribute
    {
        [Character]
        char? Char { get; set; }
        [Length]
        string CharOff { get; set; }
    }

    public interface IAlignAttribute
    {
        [Alignment]
        Alignment Align { get; set; }
    }

    public enum Scope
    {
        Row,
        Col,
        RowGroup,
        ColGroup
    }
    public class ScopeAttribute : EnumAttributeCore
    {
        public ScopeAttribute() : base(Factory<Scope>){}
    }
    public enum Alignment
    {
        Left,
        Center,
        Right,
        Justify,
        Char
    }
    public class AlignmentAttribute : EnumAttributeCore
    {
        public AlignmentAttribute(): base(Factory<Alignment>){}
    }
    public enum VerticalAlignment
    {
        Top,
        Middle,
        Bottom,
        Baseline
    }
    public class VerticalAlignmentAttribute:EnumAttributeCore
    {
        public VerticalAlignmentAttribute(): base(Factory<VerticalAlignment>){}
    }
    public enum Rules
    {
        None,
        Groups,
        Rows,
        Cols,
        All
    }
    public class RulesAttribute : EnumAttributeCore
    {
        public RulesAttribute() : base(Factory<Rules>){}
    }
    public enum Frame
    {
        Void,
        Above,
        Below,
        Hsides,
        Lhs,
        Rhs,
        Vsides,
        Box,
        Borer
    }

    public class FrameAttribute : EnumAttributeCore
    {
        public FrameAttribute() : base(Factory<Frame>) { }
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