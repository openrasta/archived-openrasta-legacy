using System.Collections.Generic;

namespace OpenRasta.Web
{
    public interface IUriTemplateParser
    {
        IEnumerable<string> GetQueryParameterNamesFor(string uriTemplate);

        IEnumerable<string> GetTemplateParameterNamesFor(string uriTemplate);
    }
}