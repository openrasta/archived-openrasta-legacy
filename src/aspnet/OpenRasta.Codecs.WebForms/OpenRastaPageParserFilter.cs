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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.UI;

namespace OpenRasta.Codecs.WebForms
{
    public class OpenRastaPageParserFilter : PageParserFilter
    {
        static readonly Dictionary<string, Type> TypeReplacements = new Dictionary<string, Type>
            {
                { "ResourceView(", typeof(ResourceView<>) }, 
                { "MasterView(", typeof(MasterPageView<>) }, 
                { "ResourceSubView(", typeof(ResourceSubView<>) }, 
                { "ResourceView", typeof(ResourceView) }, 
                { "ResourceSubView", typeof(ResourceSubView) }, 
                { "MasterView", typeof(MasterPageView) }
            };

        readonly List<string> _importedNamespaces = new List<string>();

        public override bool AllowCode
        {
            get { return true; }
        }

        public override int NumberOfControlsAllowed
        {
            get { return -1; }
        }

        public override int NumberOfDirectDependenciesAllowed
        {
            get { return -1; }
        }

        public override int TotalNumberOfDependenciesAllowed
        {
            get { return -1; }
        }

        public static Type GetTypeFromCSharpType(string typeName, IEnumerable<string> namespaces)
        {
            return new TypeBuilder("<", ">", namespaces).Parse(typeName);
        }

        public static Type GetTypeFromFriendlyType(string typeName, IEnumerable<string> namespaces)
        {
            int firstBracketPosition = typeName.IndexOf('(');
            string friendlyName = firstBracketPosition == -1 ? typeName : typeName.Substring(0, firstBracketPosition + 1);
            Type rootType;
            if (TypeReplacements.TryGetValue(friendlyName, out rootType))
            {
                if (firstBracketPosition != -1)
                {
                    string resourceTypeName = typeName.Substring(firstBracketPosition + 1, typeName.Length - firstBracketPosition - 2);
                    var resourceType = new TypeBuilder("(", ")", namespaces).Parse(resourceTypeName);
                    if (resourceType != null)
                        return rootType.MakeGenericType(resourceType);
                    RaiseResourceViewSyntaxError(resourceTypeName);
                }
                return rootType;
            }
            return null;
        }

        public override bool AllowBaseType(Type baseType)
        {
            return true;
        }

        public override bool AllowControl(Type controlType, ControlBuilder builder)
        {
            return true;
        }

        public override bool AllowServerSideInclude(string includeVirtualPath)
        {
            return true;
        }

        public override bool AllowVirtualReference(string referenceVirtualPath, VirtualReferenceType referenceType)
        {
            return true;
        }

        public string ParseInheritsAttribute(string originalAttribute)
        {
            if (BuildManager.GetType(originalAttribute, false, true) != null)
                return originalAttribute;
            var targetType = GetTypeFromFriendlyType(originalAttribute, Namespaces())
                             ?? GetTypeFromCSharpType(originalAttribute, Namespaces());

            if (targetType == null)
                RaiseResourceViewSyntaxError(originalAttribute);
            return targetType.AssemblyQualifiedName;
        }

        public override void PreprocessDirective(string directiveName, IDictionary attributes)
        {
            if (string.Compare(directiveName, "import", StringComparison.OrdinalIgnoreCase) == 0)
            {
                string ns = attributes["namespace"] as string;
                if (ns != null)
                    _importedNamespaces.Add(ns);
                return;
            }
            bool isPageInstruction = string.Compare(directiveName, "page", StringComparison.OrdinalIgnoreCase) == 0
                                     || string.Compare(directiveName, "master", StringComparison.OrdinalIgnoreCase) == 0
                                     || string.Compare(directiveName, "control", StringComparison.OrdinalIgnoreCase) == 0;

            if (isPageInstruction)
            {
                if (attributes.Contains("Inherits"))
                    attributes["Inherits"] = ParseInheritsAttribute((string)attributes["Inherits"]);
                if (attributes.Contains("Title"))
                {
                    var parentType = BuildManager.GetType((string)attributes["Inherits"], false);
                    if (parentType != null && (parentType.Namespace == typeof(ResourceView).Namespace))
                    {
                        attributes["PageTitle"] = attributes["Title"];
                        attributes.Remove("Title");
                    }
                }
            }
        }

        public override bool ProcessCodeConstruct(CodeConstructType codeType, string code)
        {
            return false;
        }

        static IEnumerable<string> AssemblyNames()
        {
            var section = WebConfigurationManager.GetSection("system.web/compilation") as CompilationSection;
            if (section == null) yield break;

            foreach (AssemblyInfo nsInfo in section.Assemblies)
                yield return nsInfo.Assembly;
            foreach (var ns in AppDomain.CurrentDomain.GetAssemblies())
                yield return ns.FullName;
        }

        static Type FindType(string typeName, IEnumerable<string> namespaces, IEnumerable<string> assemblyNames)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (string ns in namespaces)
            {
                if (typeName.IndexOf(',') != -1 && (type = Type.GetType(ns + "." + typeName)) != null)
                    return type;
                if (typeName.IndexOf(',') == -1)
                    foreach (string assembly in assemblyNames)
                        if ((type = Type.GetType(ns + "." + typeName + ", " + assembly)) != null)
                            return type;
            }
            return null;
        }

        static void RaiseResourceViewSyntaxError(string resourceName)
        {
            throw new TypeLoadException(
                "The resource named {0} couldn't be found.\r\nTry importing the namespace this resource exists in in the namespaces tag in web.config, or by inserting an @Import directive before the @Page directive.\r\nAlternatively, fully qualify the type."
                    .With(resourceName));
        }

        IEnumerable<string> Namespaces()
        {
            foreach (string ns in _importedNamespaces)
                yield return ns;

            var section = WebConfigurationManager.GetSection("system.web/pages") as PagesSection;
            if (section == null) yield break;

            foreach (NamespaceInfo nsInfo in section.Namespaces)
                yield return nsInfo.Namespace;
            if (Process.GetCurrentProcess().ProcessName == "devenv")
            {
                var allNamespaces = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                    from type in assembly.GetTypes()
                                    select type.Namespace;

                foreach (string ns in allNamespaces.Distinct())
                    yield return ns;
            }
        }

        class TypeBuilder
        {
            readonly string _genericTypeDefClose;
            readonly string _genericTypeDefOpen;
            readonly IEnumerable<string> _namespaces;

            public TypeBuilder(string genericTypeDefOpen, string genericTypeDefClose, IEnumerable<string> namespaces)
            {
                _genericTypeDefOpen = genericTypeDefOpen;
                _namespaces = namespaces;
                _genericTypeDefClose = genericTypeDefClose;
            }

            public Type Parse(string typeName)
            {
                try
                {
                    var stack = new Stack<TypeDef>();
                    stack.Push(new TypeDef(_namespaces));
                    for (int i = 0; i < typeName.Length; i++)
                    {
                        if (typeName[i] == _genericTypeDefOpen[0])
                            stack.Push(new TypeDef(_namespaces));
                        else if (typeName[i] == _genericTypeDefClose[0])
                        {
                            var current = stack.Pop();
                            stack.Peek().GenericTypeArguments.Add(current.BuildType());
                        }
                        else if (typeName[i] == ',')
                        {
                            var type = stack.Pop();
                            if (stack.Count == 0) // we're not inside a typedef
                                return type.BuildType();
                            stack.Peek().GenericTypeArguments.Add(type.BuildType());
                            stack.Push(new TypeDef(_namespaces));
                        }
                        else
                        {
                            stack.Peek().TypeName.Append(typeName[i]);
                        }
                    }

                    return stack.Pop().BuildType();
                }
                catch
                {
                    return null;
                }
            }
        }

        class TypeDef
        {
            public readonly List<Type> GenericTypeArguments = new List<Type>();
            public readonly StringBuilder TypeName = new StringBuilder();
            readonly IEnumerable<string> _namespaces;

            public TypeDef(IEnumerable<string> namespaces)
            {
                _namespaces = namespaces;
            }

            public Type BuildType()
            {
                if (GenericTypeArguments.Count == 0)
                    return FindType(TypeName.ToString(), _namespaces, AssemblyNames());
                string potentialTypeName = TypeName + "`" + GenericTypeArguments.Count;

                var type = FindType(potentialTypeName, _namespaces, AssemblyNames());
                if (type == null || !type.IsGenericTypeDefinition)
                    return null;
                return type.MakeGenericType(GenericTypeArguments.ToArray());
            }
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
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion