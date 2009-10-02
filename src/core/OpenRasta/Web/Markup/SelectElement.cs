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
using OpenRasta.Collections;

namespace OpenRasta.Web.Markup
{
    public class SelectElement //: ContainerElement<SelectElement,OptionElement>, INameValueElement
    {
        public SelectElement()
        {
        }

        // : base("select") { }

        //public string Name
        //{
        //    get { return Attributes["name"]; }
        //    set
        //    {
        //        if (Attributes.ContainsKey("name") && value == null)
        //            Attributes.Remove("name");
        //        else
        //            Attributes["name"] = value;
        //    }
        //}
        //string _pendingValue;
        //public string Value
        //{
        //    get
        //    {
        //        if (_pendingValue != null) return _pendingValue;
        //        var option = SelectedOption;
        //        return option != null ? option.Value : null;
        //    }
        //    set
        //    {
        //        var optionTag = Children.FirstOrDefault(o=>o.Value == value);
        //        if (optionTag != null)
        //        {
        //            _pendingValue = null;
        //            optionTag.Selected = true;
        //        }
        //        else
        //            _pendingValue = value;
        //    }
        //}
        //private OptionElement SelectedOption { get { return Children.FirstOrDefault(o => o.Selected == true); } }
        //protected override void OnChildAdded(OptionElement child)
        //{
        //    if (_pendingValue != null)
        //    {
        //        child.Selected = (_pendingValue != null && child.Value == _pendingValue);
        //    }
        //    else
        //        child.Selected = false;
        //    base.OnChildAdded(child);
        //}
        //protected override void OnChildRemoved(OptionElement child)
        //{
        //    if (child.Value == Value)
        //        _pendingValue = child.Value;
        //    base.OnChildRemoved(child);
        //}
        //public override void WriteAll(IXhtmlWriter writer)
        //{
        //    if (_pendingValue != null)
        //        throw new InvalidOperationException("An <option> wasn't found with value {0}".With(_pendingValue));
        //    base.WriteAll(writer);
        //}
        //public int? Size { get { return GetAttribute<int>("size"); } set { SetAttribute("size", value); } }
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