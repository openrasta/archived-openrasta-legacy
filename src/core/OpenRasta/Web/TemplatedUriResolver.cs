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
using OpenRasta.TypeSystem.ReflectionBased;

namespace OpenRasta.Web
{
    public class TemplatedUriResolver : IUriResolver, IUriTemplateParser
    {
        UriTemplateTable _templates = new UriTemplateTable();

        public TemplatedUriResolver()
        {
            TypeSystem = new ReflectionBasedTypeSystem();
        }

        /// <summary>
        /// The TypeSystem to use for any resource key that is a type
        /// </summary>
        public ITypeSystem TypeSystem { get; set; }

        /// <exception cref="InvalidOperationException">There is no Uri mapping to the resource you requested.</exception>
        /// <exception cref="ArgumentNullException"><c>resourceKey</c> is null.</exception>
        public Uri CreateUriFor(Uri baseAddress, object resourceKey, string[] parameters)
        {
            if (resourceKey == null) throw new ArgumentNullException("resourceKey");
            var templatePair =
                _templates.KeyValuePairs.FirstOrDefault(pair => ((UrlDescriptor)pair.Value).ResourceKey == resourceKey);

            if (templatePair.Key == null)
                throw new InvalidOperationException("There is no Uri mapping to the resource you requested.");
            return templatePair.Key.BindByPosition(_templates.BaseAddress, parameters).ReplaceAuthority(baseAddress);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var pair in _templates.KeyValuePairs)
            {
                yield return
                    new KeyValuePair<string, object>(pair.Key.ToString(), ((UrlDescriptor)pair.Value).ResourceKey);
            }
        }

        /// <exception cref="InvalidOperationException">Cannot add a Uri mapping once the configuration has been done.</exception>
        /// <exception cref="ArgumentException">Cannot use a Type as the resourceKey. Use an IType instead or assign the TypeSystem property.</exception>
        public void AddUriMapping(string uri, object resourceKey, CultureInfo ci, string uriName)
        {
            if (_templates.IsReadOnly)
                throw new InvalidOperationException("Cannot add a Uri mapping once the configuration has been done.");

            resourceKey = EnsureTypeSystemUsage(resourceKey);

            var descriptor = new UrlDescriptor
            {
                Uri = new UriTemplate(uri), 
                Culture = ci, 
                ResourceKey = resourceKey, 
                UriName = uriName
            };
            _templates.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(descriptor.Uri, descriptor));
            _templates.BaseAddress = new Uri("http://localhost/").IgnoreAuthority();
        }

        public void Clear()
        {
            _templates = new UriTemplateTable();
        }

        /// <exception cref="InvalidOperationException"><c>InvalidOperationException</c>.</exception>
        public Uri CreateUriFor(Uri baseAddress, object resourceKey, string uriName, NameValueCollection keyValues)
        {
            resourceKey = EnsureTypeSystemUsage(resourceKey);
            var template = FindBestMatchingTemplate(_templates, resourceKey, uriName, keyValues);

            if (template == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "No suitable uri could be found for resource with key {0} with values {1}.", 
                        resourceKey, 
                        keyValues.ToHtmlFormEncoding()));
            }
            return template.BindByName(_templates.BaseAddress, keyValues).ReplaceAuthority(baseAddress);
        }

        public UriRegistration Match(Uri uriToMatch)
        {
            if (uriToMatch == null)
                return null;
            var tableMatches = _templates.Match(uriToMatch.IgnorePortAndAuthority());
            if (tableMatches == null || tableMatches.Count == 0)
                return null;
            var urlDescriptor = (UrlDescriptor)tableMatches[0].Data;

            var result = new UriRegistration(urlDescriptor.Uri.ToString(), urlDescriptor.ResourceKey, urlDescriptor.UriName, urlDescriptor.Culture);
            foreach (var tableMatch in tableMatches)
            {
                var allVariables = new NameValueCollection
                {
                    tableMatch.BoundVariables, 
                    tableMatch.QueryParameters
                };
                result.UriTemplateParameters.Add(allVariables);
            }
            return result;
        }

        public IEnumerable<string> GetQueryParameterNamesFor(string uriTemplate)
        {
            return new UriTemplate(uriTemplate).QueryValueVariableNames;
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

        static bool UriNameMatches(string requestUriName, string templateUriName)
        {
            return (!requestUriName.IsNullOrEmpty() &&
                    requestUriName.EqualsOrdinalIgnoreCase(templateUriName)) ||
                   (requestUriName.IsNullOrEmpty() &&
                    templateUriName.IsNullOrEmpty());
        }

        object EnsureTypeSystemUsage(object resourceKey)
        {
            var resourceType = resourceKey as Type;

            if (resourceType != null && TypeSystem == null)
                throw new ArgumentException("Cannot use a Type as the resourceKey. Use an IType instead or assign the TypeSystem property.");

            if (resourceType != null)
                resourceKey = TypeSystem.FromClr(resourceType);
            return resourceKey;
        }

        UriTemplate FindBestMatchingTemplate(UriTemplateTable templates, 
                                             object resourceKey, 
                                             string uriName, 
                                             NameValueCollection keyValues)
        {
            resourceKey = EnsureTypeSystemUsage(resourceKey);
            var matchingTemplates =
                from template in templates.KeyValuePairs
                let descriptor = (UrlDescriptor)template.Value
                where CompatibleKeys(resourceKey, descriptor.ResourceKey)
                where UriNameMatches(uriName, descriptor.UriName)
                let templateParameters =
                    template.Key.PathSegmentVariableNames.Concat(template.Key.QueryValueVariableNames).ToList()
                let hasKeys = keyValues != null && keyValues.HasKeys()
                where (templateParameters.Count == 0) ||
                      (templateParameters.Count > 0
                       && hasKeys
                       && templateParameters.All(x => keyValues.AllKeys.Contains(x, StringComparison.OrdinalIgnoreCase)))
                orderby templateParameters.Count descending
                select template.Key;

            return matchingTemplates.FirstOrDefault();
        }

        class UrlDescriptor
        {
            public CultureInfo Culture { get; set; }
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