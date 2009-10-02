#region License

/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */

#endregion

namespace OpenRasta.Web.Markup.Modules
{
    /// <summary>
    /// <para>Represents the set of Inline elements.</para>
    /// <para>
    /// Inline Elements: &lt;a&gt;, &lt;del&gt;, &lt;ins&gt;, &lt;iframe&gt;, &lt;script&gt;, &lt;noscript&gt; and &lt;object&gt;
    /// </para>
    /// <para>
    /// Empty elements: &lt;hr&gt;, &lt;br&gt; and &lt;img&gt;</para>
    /// <para>
    /// Text elements: &lt;abbr&gt;, &lt;acronym&gt;, &lt;b&gt;, &lt;big&gt;, &lt;cite&gt;, &lt;code&gt;, &lt;dfn&gt;, &lt;em&gt;, &lt;i&gt;, &lt;kbd&gt;, &lt;samp&gt;, &lt;small&gt;, &lt;span&gt;, &lt;strong&gt;, &lt;sub&gt;, &lt;sup&gt;, &lt;tt&gt; and &lt;var&gt;
    /// </para>
    /// </summary>
    public interface IContentSetInline : IContentSetFlow
    {
    }

    /// <summary>
    /// <para>Represents the set of Heading elements.</para>
    /// <para>
    /// Header elements: &lt;h1&gt;, &lt;h2&gt;, &lt;h3&gt;, &lt;h4&gt;, &lt;h5&gt;, &lt;h6&gt;
    /// </para>
    /// </summary>
    public interface IContentSetHeading : IContentSetFlow
    {
    }

    /// <summary>
    /// <para>Represents the set of Block elements.</para>
    /// <para>
    /// Block elements: &lt;address&gt;, &lt;blockquote&gt;, &lt;div&gt;, &lt;p&gt;, &lt;pre&gt;, &lt;script&gt;, &lt;noscript&gt;
    /// </para>
    /// <para>
    /// Form elements: &lt;form&gt; and &lt;fieldset&gt;
    /// </para>
    /// <para>
    /// Table elements: &lt;table&gt;
    /// </para>
    /// </summary>
    public interface IContentSetBlock : IContentSetFlow
    {
    }

    /// <summary>
    /// <para>Represents the set of list elements</para>
    /// <para>
    /// List elements: &lt;dl&gt;, &lt;ol&gt; and &lt;ul&gt;
    /// </para>
    /// </summary>
    public interface IContentSetList : IContentSetFlow
    {
    }

    /// <summary>
    /// <para>Represens the set of Flow elements</para>
    /// <para>
    /// Block elements: &lt;address&gt;, &lt;blockquote&gt;, &lt;div&gt;, &lt;p&gt;, &lt;pre&gt;, &lt;script&gt;, &lt;noscript&gt;</para>
    /// <para>
    /// Form elements: &lt;form&gt; and &lt;fieldset&gt;
    /// </para>
    /// <para>
    /// Table elements: &lt;table&gt;
    /// </para>
    /// <para>
    /// List elements: &lt;dl&gt;, &lt;ol&gt; and &lt;ul&gt;
    /// </para>
    /// <para>
    /// Header elements: &lt;h1&gt;, &lt;h2&gt;, &lt;h3&gt;, &lt;h4&gt;, &lt;h5&gt;, &lt;h6&gt;
    /// </para>
    /// <para>
    /// Inline Elements: &lt;a&gt;, &lt;del&gt;, &lt;ins&gt;, &lt;iframe&gt;, &lt;script&gt;, &lt;noscript&gt; and &lt;object&gt;
    /// </para>
    /// <para>
    /// Empty elements: &lt;hr&gt;, &lt;br&gt; and &lt;img&gt;</para>
    /// <para>
    /// Text elements: &lt;abbr&gt;, &lt;acronym&gt;, &lt;b&gt;, &lt;big&gt;, &lt;cite&gt;, &lt;code&gt;, &lt;dfn&gt;, &lt;em&gt;, &lt;i&gt;, &lt;kbd&gt;, &lt;samp&gt;, &lt;small&gt;, &lt;span&gt;, &lt;strong&gt;, &lt;sub&gt;, &lt;sup&gt;, &lt;tt&gt; and &lt;var&gt;
    /// </para>
    /// </summary>
    public interface IContentSetFlow : IElement
    {
    }

    /// <summary>
    /// <para>Represents the set of form elements.</para>
    /// <para>
    /// Form elements: &lt;form&gt; and &lt;fieldset&gt;
    /// </para>
    /// </summary>
    public interface IContentSetForm :
        IContentSetBlock
    {
    }

    /// <summary>
    /// <para>Represents the set of form control elements.</para>
    /// <para>
    /// Form control elements: &lt;input&gt;, &lt;select&gt;, &lt;textarea&gt;, &lt;label&gt; and &lt;button&gt;
    /// </para>
    /// </summary>
    public interface IContentSetFormctrl :
        IContentSetInline
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