using System;

namespace OpenRasta
{
    public class Error
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}