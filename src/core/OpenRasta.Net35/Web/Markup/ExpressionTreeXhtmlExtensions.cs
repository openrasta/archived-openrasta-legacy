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
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using OpenRasta.Reflection;
using OpenRasta.Collections;
using OpenRasta.TypeSystem.ReflectionBased;
using OpenRasta.Web.Markup.Elements;
using OpenRasta.Web.Markup.Modules;

namespace OpenRasta.Web.Markup
{
    public static class ExpressionTreeXhtmlExtensions
    {
        public static IInputTextElement TextBox<T>(this IXhtmlAnchor hook, Expression<Func<T, object>> property)
        {
            return FillName(Document.CreateElement<IInputTextElement>("input").InputType(InputType.Text), property);
        }

        public static IInputTextElement TextBox(this IXhtmlAnchor hook, Expression<Func<object>> property)
        {
            return FillNameValue(Document.CreateElement<IInputTextElement>("input").InputType(InputType.Text), property);
        }

        public static ITextAreaElement TextArea(this IXhtmlAnchor hook, Expression<Func<object>> property)
        {
            var expressionTree = new PropertyPathForInstance<object>(property);

            return Document.CreateElement<ITextAreaElement>().Name(expressionTree.FullPath)[expressionTree.Value.ConvertToString()];
        }
        public static ITextAreaElement TextArea<T>(this IXhtmlAnchor hook, Expression<Func<T, object>> property)
        {
            var expressionTree = new PropertyPathForType<T,object>(property);

            return Document.CreateElement<ITextAreaElement>().Name(expressionTree.FullPath);
        }
        public static IInputTextElement Password(this IXhtmlAnchor hook, Expression<Func<object>> property)
        {
            return TextBox(hook, property).InputType(InputType.Password);
        }
        public static IInputTextElement Password<T>(this IXhtmlAnchor hook, Expression<Func<T, object>> property)
        {
            return TextBox<T>(hook, property).InputType(InputType.Password);
        }

        public static ISelectElement Select(this IXhtmlAnchor hook, Expression<Func<object>> propertyName, IEnumerable<string> options)
        {
            return Select(hook, propertyName, options.Select(val => Document.CreateElement<IOptionElement>()[val]).ToList());
        }
        public static ISelectElement Select(this IXhtmlAnchor hook, Expression<Func<object>> propertyName, IDictionary<string, string> options)
        {
            return Select(hook, propertyName, options.Select(kv => Document.CreateElement<IOptionElement>().Value(kv.Key)[kv.Value]).ToList());
        }
        public static ISelectElement Select(this IXhtmlAnchor hook, Expression<Func<object>> propertyName, IEnumerable<IOptionElement> options)
        {
            var et = new PropertyPathForInstance<object>(propertyName);


            var select = Document.CreateElement<ISelectElement>().Name(et.FullPath);
            //TODOD: Special case multiple values 
            if (et.Value != null)
            {
                var valueToFind = et.Value.ConvertToString();
                foreach(var option in options)
                    option.Selected = option.Value == valueToFind || option.InnerText == valueToFind;
            }
            options.ForEach(option => select.ChildNodes.Add(option));
            return select;
        }
        public static ISelectElement Select(this IXhtmlAnchor anchor, Expression<Func<object>> property)
        {
            var et = new PropertyPathForInstance<object>(property);
            
            Type enumType;
            bool isNullable=false;
            if (et.PropertyType.IsGenericType && et.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && (enumType = et.PropertyType.GetGenericArguments()[0]).IsEnum)
            {
                isNullable = true;
            }
            else if (et.PropertyType.IsEnum)
            {
                enumType = et.PropertyType;
            }
            else
            {
                throw new InvalidOperationException("Cannot automatically generate select entries if the type is not an enumeration or a nullable enumeration.");
            }
            
            var optionElements = Enum.GetNames(enumType)
                    .Select(x => Document.CreateElement<IOptionElement>().Value(x)[x]).ToList();

            if (isNullable)
                optionElements.Insert(0, Document.CreateElement<IOptionElement>()[string.Empty]);

            var select = Document.CreateElement<ISelectElement>().Name(et.FullPath);

            optionElements.ForEach(option => select.ChildNodes.Add(option));

            
            if (et.Value != null)
            {
                var valueToFind = et.Value.ConvertToString();
                foreach (var option in select.ChildNodes.Cast<IOptionElement>())
                    option.Selected = option.Value == valueToFind || option.InnerText == valueToFind;
            }
            else if (isNullable)
            {
                select.ChildNodes.Cast<IOptionElement>().First().Selected();
            }
            return select;
        }

        public static ISelectElement Select<T>(this IXhtmlAnchor hook, Expression<Func<T, object>> propertyName, IEnumerable<string> options)
        {
            return Select<T>(hook, propertyName,
                              options.Select(val => Document.CreateElement<IOptionElement>()[val]).ToList());
        }

        public static ISelectElement Select<T>(this IXhtmlAnchor hook, Expression<Func<T, object>> propertyName, IEnumerable<IOptionElement> options)
        {
            var et = new PropertyPathForType<T,object>(propertyName);
            var select = Document.CreateElement<ISelectElement>().Name(et.FullPath);
            //TODOD: Special case multiple values 
            options.ForEach(option => select.ChildNodes.Add(option));
            return select;
        }
        public static IInputCheckedElement CheckBox(this IXhtmlAnchor anchor, Expression<Func<bool>> property)
        {
            var et = new PropertyPathForInstance<bool>(property);
            var element = Document.CreateElement<IInputCheckedElement>("input").InputType(InputType.CheckBox).Name(et.FullPath);
            if ((bool)et.Value)
                element.Checked();
            return element;
        }
        static T FillName<T, TTarget,TProperty>(T element, Expression<Func<TTarget, TProperty>> property)
            where T : IInputElement
        {
            var expressionTree = new PropertyPathForType<TTarget,TProperty>(property);
            element.Name = expressionTree.FullPath;
            return element;

        }
        static T FillNameValue<T>(T element, Expression<Func<object>> property) where T:IInputElement
            
        {
            var expressionTree = new PropertyPathForInstance<object>(property);
            element.Name = expressionTree.FullPath;
            if (expressionTree.Value != null && !expressionTree.Value.Equals(expressionTree.PropertyType.GetDefaultValue()))
                element.Value = expressionTree.Value.ConvertToString();
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