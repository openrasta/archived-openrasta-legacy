using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRasta.Web.Markup.Modules;

namespace OpenRasta.Web.Markup
{
    public static class TableModuleExtensions
    {
        public static T Border<T>(this T element, int border)
            where T:ITableElement
        {
            element.Border = border;
            return element;
        }
        public static T CellPadding<T>(this T element, int padding)
            where T : ITableElement
        {
            element.CellPadding = padding.ToString();
            return element;
        }
        public static T CellPadding<T>(this T element, string padding)
                    where T : ITableElement
        {
            element.CellPadding = padding;
            return element;
        }


        public static T CellSpacing<T>(this T element, int spacing)
            where T : ITableElement
        {
            element.CellSpacing = spacing.ToString();
            return element;
        }


        public static T CellSpacing<T>(this T element, string spacing)
            where T : ITableElement
        {
            element.CellSpacing = spacing;
            return element;
        }
        public static T Frame<T>(this T element, Frame frame)
            where T : ITableElement
        {
            element.Frame = frame;
            return element;
        }
        public static T Rules<T>(this T element, Rules rules)
           where T : ITableElement
        {
            element.Rules = rules;
            return element;
        }
        public static T Summary<T>(this T element, string summary)
            where T : ITableElement
        {
            element.Summary = summary;
            return element;
        }

        public static T Span<T>(this T element, int span)
            where T : IColElementBase
        {
            element.Span = span;
            return element;
        }
        public static T Abbr<T>(this T element, string abbr)
            where T : ICellElementBase
        {
            element.Abbr = abbr;
            return element;
        }
        public static T Axis<T>(this T element, string axis)
            where T : ICellElementBase
        {
            element.Axis = axis;
            return element;
        }
        public static T ColSpan<T>(this T element, int colspan)
           where T : ICellElementBase
        {
            element.ColSpan = colspan;
            return element;
        }

        public static T RowSpan<T>(this T element, int rowspan)
           where T : ICellElementBase
        {
            element.RowSpan = rowspan;
            return element;
        }
        public static T Headers<T>(this T element, string idref)
           where T : ICellElementBase
        {
            element.Headers.Add(idref);
            return element;
        }

        public static T Scope<T>(this T element, Scope scope)
           where T : ICellElementBase
        {
            element.Scope = scope;
            return element;
        }


        public static T Valign<T>(this T element, VerticalAlignment alignment)
           where T : IValignAttribute
        {
            element.Valign = alignment;
            return element;
        }
        public static T Align<T>(this T element, Alignment alignment)
           where T : IAlignAttribute
        {
            element.Align = alignment;
            return element;
        }

        public static T Char<T>(this T element, char ch)
          where T : ICharAttribute
        {
            element.Char = ch;
            return element;
        }
        public static T CharOff<T>(this T element, string length)
          where T : ICharAttribute
        {
            element.CharOff = length;
            return element;
        }
        public static T CharOff<T>(this T element, int length)
          where T : ICharAttribute
        {
            element.CharOff = length.ToString();
            return element;
        }
    }
}
