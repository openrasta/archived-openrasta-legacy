using System;

namespace OpenRasta
{
    public class Error
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public override string ToString()
        {
            return "{0}\r\nMessage:\r\n{1}\r\n".With(Title, Message) + Exception != null ? "Exception:\r\n{0}".With(Exception) : string.Empty;
        }
    }
}