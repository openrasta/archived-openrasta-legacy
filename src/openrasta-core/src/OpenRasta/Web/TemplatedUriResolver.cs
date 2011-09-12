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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using OpenRasta.Collections;
using OpenRasta.TypeSystem;

namespace OpenRasta.Web
{
    public class TemplatedUriResolver : IUriResolver, IUriTemplateParser
    {
        UriTemplateTable _templates = new UriTemplateTable();
        public ITypeSystem TypeSystem { get; set; }
        public int Count
        {
            get { return _templates.KeyValuePairs.Count; }
        }

        public bool IsReadOnly
        {
            get { return _templates.IsReadOnly; }
        }

        public TemplatedUriResolver()
        {
            TypeSystem = TypeSystems.Default;
        }
        /// <exception cref="InvalidOperationException">Cannot add a Uri mapping once the configuration has been done.</exception>
        /// <exception cref="ArgumentException">Cannot use a Type as the resourceKey. Use an <see cref="IType"/> instead or assign the <see cref="TypeSystem"/> property.</exception>
        public void Add(UriRegistration registration)
        {
            if (_templates.IsReadOnly)
                throw new InvalidOperationException("Cannot add a Uri mapping once the configuration has been done.");
            var resourceKey = EnsureIsNotType(registration.ResourceKey);
            var descriptor = new UrlDescriptor
            {
                Uri = new UriTemplate(registration.UriTemplate), 
                Culture = registration.UriCulture, 
                ResourceKey = resourceKey, 
                UriName = registration.UriName, 
                Registration = registration
            };
            _templates.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(descriptor.Uri, descriptor));
            _templates.BaseAddress = new Uri("http://localhost/").IgnoreAuthority();
        }

        public void Clear()
        {
            _templates = new UriTemplateTable();
        }

        public bool Contains(UriRegistration item)
        {
            return this.Any(x => x == item);
        }

        public void CopyTo(UriRegistration[] array, int arrayIndex)
        {
            this.ToList().CopyTo(array, arrayIndex);
        }

        public bool Remove(UriRegistration item)
        {
            var pairToRemove = _templates.KeyValuePairs
                .Where(x => ((UrlDescriptor)x.Value).Registration == item)
                .ToList();

            if (pairToRemove.Count > 0)
            {
                _templates.KeyValuePairs.Remove(pairToRemove[0]);
                return true;
            }

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<UriRegistration> GetEnumerator()
        {
            return _templates.KeyValuePairs.Select(x => ((UrlDescriptor)x.Value).Registration).GetEnumerator();
        }

        /// <exception cref="InvalidOperationException"><c>InvalidOperationException</c>.</exception>
        public Uri CreateUriFor(Uri baseAddress, object resourceKey, string uriName, NameValueCollection keyValues)
        {
            resourceKey = EnsureIsNotType(resourceKey);
            var template = FindBestMatchingTemplate(_templates, resourceKey, uriName, keyValues);

            if (template == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "No suitable Uri could be found for resource with key {0} with values {1}.", 
                        resourceKey, 
                        keyValues.ToHtmlFormEncoding()));
            }

            return template.BindByName(baseAddress, keyValues);
        }

        public UriRegistration Match(Uri uriToMatch)
        {
            if (uriToMatch == null)
                return null;
            var tableMatches = _templates.Match(uriToMatch.IgnoreSchemePortAndAuthority());
            if (tableMatches == null || tableMatches.Count == 0)
                return null;
            var urlDescriptor = (UrlDescriptor)tableMatches[0].Data;

            var result = new UriRegistration(urlDescriptor.Uri.ToString(), urlDescriptor.ResourceKey, urlDescriptor.UriName, urlDescriptor.Culture);
            foreach (var tableMatch in tableMatches)
            {
                var allVariables = new NameValueCollection
                {
                    tableMatch.PathSegmentVariables, 
                    tableMatch.QueryStringVariables
                };
                result.UriTemplateParameters.Add(allVariables);
            }

            return result;
        }

        public IEnumerable<string> GetQueryParameterNamesFor(string uriTemplate)
        {
            return new UriTemplate(uriTemplate).QueryStringVariableNames;
        }

        public IEnumerable<string> GetTemplateParameterNamesFor(string uriTemplate)
        {
            return new UriTemplate(uriTemplate).PathSegmentVariableNames;
        }

        static bool CompatibleKeys(object requestResourceKey, object templateResourceKey)
        {
            var requestType = requestResourceKey as IType;
            var templateType = templateResourceKey as IType;
            return (requestType != null &&
                    templateType != null &&
                    requestType.IsAssignableTo(templateType)) ||
                   requestResourceKey.Equals(templateResourceKey);
        }

        object EnsureIsNotType(object resourceKey)
        {
            var resourceType = resourceKey as Type;
            if (resourceType != null)
                resourceKey = TypeSystem.FromClr(resourceType);
            return resourceKey;
        }

        UriTemplate FindBestMatchingTemplate(UriTemplateTable templates, 
                                                    object resourceKey, 
                                                    string uriName, 
                                                    NameValueCollection keyValues)
        {
            resourceKey = EnsureIsNotType(resourceKey);
            var matchingTemplates =
                from template in templates.KeyValuePairs
                let descriptor = (UrlDescriptor)template.Value
                where CompatibleKeys(resourceKey, descriptor.ResourceKey)
                where UriNameMatches(uriName, descriptor.UriName)
                let templateParameters =
                    template.Key.PathSegmentVariableNames.Concat(template.Key.QueryStringVariableNames).ToList()
                let hasKeys = keyValues != null && keyValues.HasKeys()
                where (templateParameters.Count == 0) ||
                      (templateParameters.Count > 0
                       && hasKeys
                       && templateParameters.All(x => keyValues.AllKeys.Contains(x, StringComparison.OrdinalIgnoreCase)))
                orderby templateParameters.Count descending
                select template.Key;

            return matchingTemplates.FirstOrDefault();
        }

        static bool UriNameMatches(string requestUriName, string templateUriName)
        {
            return (!requestUriName.IsNullOrEmpty() &&
                    requestUriName.EqualsOrdinalIgnoreCase(templateUriName)) ||
                   (requestUriName.IsNullOrEmpty() &&
                    templateUriName.IsNullOrEmpty());
        }

        class UrlDescriptor
        {
            public CultureInfo Culture { get; set; }
            public UriRegistration Registration { get; set; }
            public object ResourceKey { get; set; }
            public UriTemplate Uri { get; set; }
            public string UriName { get; set; }
        }
    }
}

#region Full license
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion
