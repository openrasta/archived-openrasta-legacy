using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRasta.Web;
using OpenRasta.Web.Markup.Elements;

namespace OpenRasta.Codecs
{
    public class OperationResultPage : Element
    {
        public OperationResultPage(OperationResult result)
        {
            Root = this
                [html
                     [head[title[result.Title]]]
                     [body
                        [h1[result.Title]]
                        [p[result.Description]]
                     ]
                ];
        }

        protected Element Root { get; set; }
    }
}
