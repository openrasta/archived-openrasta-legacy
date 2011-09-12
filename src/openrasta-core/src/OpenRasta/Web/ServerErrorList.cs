using System;
using System.Collections.ObjectModel;
using OpenRasta.Diagnostics;

namespace OpenRasta.Web
{
    public class ServerErrorList : Collection<Error>
    {
        ILogger _log;
        public ILogger Log
        {
            get { return _log; }
            set { _log = value ?? new NullLogger(); }
        }

        public ServerErrorList()
        {
            Log = new NullLogger();
        }
        protected override void InsertItem(int index, Error item)
        {
            if (item == null) throw new ArgumentNullException("item");

            if (item.Exception != null)
                Log.WriteException(item.Exception);
            else
                Log.WriteError(item.Message);
            base.InsertItem(index, item);
        }
    }
}