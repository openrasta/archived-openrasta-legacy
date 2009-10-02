using System.Collections;
using System.Collections.Generic;

namespace OpenRasta.Configuration.MetaModel
{
    public interface IMetaModelRepository
    {
        IList<ResourceModel> ResourceRegistrations { get; set; }
        IList CustomRegistrations { get; set; }
        void Process();
    }
}