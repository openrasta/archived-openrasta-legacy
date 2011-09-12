using System.Collections.Generic;
using OpenRasta.Web;

namespace OpenRasta.Configuration.MetaModel
{
    public class MediaTypeModel
    {
        public MediaTypeModel()
        {
            Extensions = new List<string>();
        }

        public ICollection<string> Extensions { get; set; }
        public MediaType MediaType { get; set; }
    }
}