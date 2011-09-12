using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRasta.Reflection;

namespace OpenRasta
{
    public static class ObjectPaths
    {

        public static void Add(object o, PropertyPath path)
        {
            ObjectPathsCore[o] = path;
        }

        public static void Remove(object o)
        {
            ObjectPathsCore.Remove(o);
        }
        public static PropertyPath Get(object o)
        {
            PropertyPath result;
            return ObjectPathsCore.TryGetValue(o, out result) ? result : null;
        }
        [ThreadStatic]
        static Dictionary<object,PropertyPath> _objectPaths;
        static Dictionary<object, PropertyPath> ObjectPathsCore
        {
            get
            {
                if (_objectPaths == null)
                    _objectPaths = new Dictionary<object, PropertyPath>();
                return _objectPaths;
            }
        }
    }
}
