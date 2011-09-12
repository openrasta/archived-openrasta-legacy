using System.Collections.Generic;
using OpenRasta.Web.Markup;
using OpenRasta.Web.Markup.Elements;

namespace OpenRasta.Codecs
{
    public class HtmlErrorPage : Element
    {
        public Element Root { get; set; }

        public HtmlErrorPage(IEnumerable<Error> errors)
        {
            var exceptionBlock = GetExceptionBlock(errors);
            Root = this
                [html
                     [head[title["OpenRasta encountered an error."]]]
                     [body
                          [div.Class("errorList")[exceptionBlock]]
                     ]
                ];
        }

        IDlElement GetExceptionBlock(IEnumerable<Error> errors)
        {
            var exceptionBlock = dl;

            foreach (var error in errors)
            {
                exceptionBlock = exceptionBlock
                    [dt[error.Title]]
                    [dd[pre[error.Message]]];
            }
            return exceptionBlock;
        }
    }
}