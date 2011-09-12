using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRasta.TypeSystem
{
    public class PathManager : IPathManager
    {
        public PathComponent GetPathType(IEnumerable<string> prefixes, string objectPath)
        {
            if (objectPath.IsNullOrEmpty())
                return Constructor();

            var components = ReadComponents(objectPath).ToList();
            var firstComponent = components.FirstOrDefault();

            if (firstComponent == null)
                return Constructor();
            if (firstComponent.Type == PathComponentType.Indexer
                || firstComponent.Type == PathComponentType.None)
                return Member(objectPath);
            if (prefixes.Contains(firstComponent.ParsedValue, StringComparer.OrdinalIgnoreCase))
            {
                if (components.Count == 1)
                    return Constructor();
                return Member(WriteComponents(components.Skip(1)));
            }

            return Member(objectPath);
        }

        public IEnumerable<PathComponent> ReadComponents(string objectPath)
        {
            if (string.IsNullOrEmpty(objectPath))
                yield break;
            var component = new StringBuilder();
            var result = new PathComponent();

            int processedCharacters;
            for (processedCharacters = 0; processedCharacters < objectPath.Length; processedCharacters++)
            {
                char ch = objectPath[processedCharacters];
                bool valueReady = false;
                if (ch == '.')
                    valueReady = true;
                if (ch == ':')
                {
                    if (component.Length == 0)
                    {
                        result.Type = PathComponentType.Indexer;
                        continue;
                    }

                    processedCharacters = processedCharacters > 0 ? processedCharacters - 1 : processedCharacters;
                    valueReady = true;
                }

                if (valueReady)
                {
                    result.ParsedValue = component.ToString();
                    if (result.Type == PathComponentType.None && processedCharacters > 0)
                        result.Type = PathComponentType.Member;
                    yield return result;
                    result = new PathComponent();
                    component = new StringBuilder();
                    continue;
                }

                component.Append(ch);
            }

            if (result.Type == PathComponentType.None && processedCharacters > 0)
                result.Type = PathComponentType.Member;
            if (component.Length == 0)
                result.Type = PathComponentType.None;
            result.ParsedValue = component.ToString();

            yield return result;
        }

        public string WriteComponents(IEnumerable<PathComponent> components)
        {
            var sb = new StringBuilder();
            foreach (var component in components)
            {
                if (component.Type == PathComponentType.Indexer)
                    sb.Append(':').Append(component.ParsedValue);
                else if (component.Type == PathComponentType.Member)
                {
                    if (sb.Length > 0) sb.Append('.');
                    sb.Append(component.ParsedValue);
                }
            }

            return sb.ToString();
        }

        static PathComponent Constructor()
        {
            return new PathComponent
            {
                Type = PathComponentType.Constructor
            };
        }

        static PathComponent Member(string objectPath)
        {
            return new PathComponent
            {
                ParsedValue = objectPath, 
                Type = PathComponentType.Member
            };
        }
    }
}