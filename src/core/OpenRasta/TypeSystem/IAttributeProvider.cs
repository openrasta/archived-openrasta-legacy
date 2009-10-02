using System;
using System.Collections.Generic;

namespace OpenRasta.TypeSystem
{
    public interface IAttributeProvider
    {
        T FindAttribute<T>() where T : class;
        IEnumerable<T> FindAttributes<T>() where T : class;
    }
}