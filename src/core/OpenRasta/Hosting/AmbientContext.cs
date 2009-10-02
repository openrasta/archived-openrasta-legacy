using System.Collections;
using System.Runtime.Remoting.Messaging;

namespace OpenRasta.Hosting
{
    public class AmbientContext
    {
        readonly Hashtable _items = new Hashtable();

        public static AmbientContext Current
        {
            get { return CallContext.HostContext as AmbientContext; }
        }

        public object this[string key]
        {
            get { return _items[key]; }
            set { _items[key] = value; }
        }
    }
}