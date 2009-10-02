using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRasta.Web.Markup.Modules;

namespace OpenRasta.Web.Markup
{
    public static class TableModuleExtensions
    {
        public static T Border<T>(T element, int border)
            where T:ITableElement
        {
            element.Border = border;
            return element;
        }
        public static T CellPadding<T>(T element, int padding)
            where T : ITableElement
        {
            element.CellPadding = padding.ToString();
            return element;
        }
        public static T CellPadding<T>(T element, string padding)
                    where T : ITableElement
        {
            element.CellPadding = padding;
            return element;
        }


        public static T CellSpacing<T>(T element, int spacing)
            where T : ITableElement
        {
            element.CellSpacing = spacing.ToString();
            return element;
        }


        public static T CellSpacing<T>(T element, string spacing)
            where T : ITableElement
        {
            element.CellSpacing = spacing;
            return element;
        }
        public static T Frame<T>(T element, Frame frame)
            where T : ITableElement
        {
            element.Frame = frame;
            return element;
        }
        public static T Rules<T>(T element, Rules rules)
           where T : ITableElement
        {
            element.Rules = rules;
            return element;
        }
        public static T Summary<T>(T element, string summary)
            where T : ITableElement
        {
            element.Summary = summary;
            return element;
        }

        public static T Span<T>(T element, int span)
            where T : IColElementBase
        {
            element.Span = span;
            return element;
        }
        public static T Abbr<T>(T element, string abbr)
            where T : ICellElementBase
        {
            element.Abbr = abbr;
            return element;
        }
        public static T Axis<T>(T element, string axis)
            where T : ICellElementBase
        {
            element.Axis = axis;
            return element;
        }
        public static T ColSpan<T>(T element, int colspan)
           where T : ICellElementBase
        {
            element.ColSpan = colspan;
            return element;
        }

        public static T RowSpan<T>(T element, int rowspan)
           where T : ICellElementBase
        {
            element.RowSpan = rowspan;
            return element;
        }
        public static T Headers<T>(T element, string idref)
           where T : ICellElementBase
        {
            element.Headers.Add(idref);
            return element;
        }

        public static T Scope<T>(T element, Scope scope)
           where T : ICellElementBase
        {
            element.Scope = scope;
            return element;
        }


        public static T Valign<T>(T element, VerticalAlignment alignment)
           where T : IValignAttribute
        {
            element.Valign = alignment;
            return element;
        }
        public static T Align<T>(T element, Alignment alignment)
           where T : IAlignAttribute
        {
            element.Align = alignment;
            return element;
        }

        public static T Char<T>(T element, char ch)
          where T : ICharAttribute
        {
            element.Char = ch;
            return element;
        }
        public static T CharOff<T>(T element, string length)
          where T : ICharAttribute
        {
            element.CharOff = length;
            return element;
        }
        public static T CharOff<T>(T element, int length)
          where T : ICharAttribute
        {
            element.CharOff = length.ToString();
            return element;
        }
    }
}
