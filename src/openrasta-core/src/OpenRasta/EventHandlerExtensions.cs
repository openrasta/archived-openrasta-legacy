using System;

namespace OpenRasta
{
    public static class EventHandlerExtensions
    {
        public static void Raise(this EventHandler handler, object src)
        {
            if (handler != null)
                handler(src, EventArgs.Empty);
        }

        public static void Raise(this EventHandler handler, object src, EventArgs args)
        {
            if (handler != null)
                handler(src, args);
        }

        public static void Raise<T>(this EventHandler<T> handler, object src, T args)
            where T : EventArgs
        {
            if (handler != null)
                handler(src, args);
        }
    }
}