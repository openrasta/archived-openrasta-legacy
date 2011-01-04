using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpenRasta.Hosting.AspNet
{
    public class HttpHandlerRegistration
    {
        readonly Regex _pathRegex;

        public HttpHandlerRegistration(string verb, string path, string type)
        {
            Type = type;
            Methods = verb.Split(',').Select(x => x.Trim());
            Path = path;
            _pathRegex = new Regex("^" + Regex.Escape(path).Replace("\\*", ".*") + "/?$");
        }

        public IEnumerable<string> Methods { get; private set; }
        public string Path { get; private set; }
        public string Type { get; private set; }

        public bool Matches(string httpMethod, Uri path)
        {
            if (!Methods.Contains("*") && !Methods.Any(x => string.CompareOrdinal(x, httpMethod) == 0))
                return false;

            bool simpleMatch = _pathRegex.IsMatch(path.PathAndQuery);
            if (simpleMatch) return true;

            return path.Segments.Any(x => _pathRegex.IsMatch(x));
        }
    }
}