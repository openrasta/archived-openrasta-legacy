using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRasta.Codecs.WebForms
{
    [CLSCompliant(false)]
    public class ResourceView<T> : ResourceView
    {
        public T Resource { get; set; }
    }
}
