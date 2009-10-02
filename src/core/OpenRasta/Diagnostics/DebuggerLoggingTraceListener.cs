#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System.Diagnostics;
using System.Linq;

namespace OpenRasta.Diagnostics
{
    public class DebuggerLoggingTraceListener : TraceListener
    {
        public DebuggerLoggingTraceListener()
            : base("DebuggerLoggingTraceListener")
        {
        }

        public override bool IsThreadSafe
        {
            get { return false; }
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            WriteAll(eventCache, eventType, id, data.ToString());
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            string message = string.Join(", ", data.Select(obj => obj.ToString()).ToArray());
            WriteAll(eventCache, eventType, id, message);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            WriteAll(eventCache, eventType, id, format.With(args));
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            WriteAll(eventCache, eventType, id, message);
        }

        public override void Write(string message)
        {
            if (Debugger.IsLogging())
            {
                if (NeedIndent)
                    WriteIndent();
                Debugger.Log(0, "OpenRasta", message);
            }
        }

        public override void WriteLine(string message)
        {
            if (Debugger.IsLogging())
            {
                if (NeedIndent)
                    WriteIndent();
                Debugger.Log(0, "OpenRasta", message + "\r\n");
                NeedIndent = true;
            }
        }

        void UpdateIndent()
        {
            IndentLevel = Trace.CorrelationManager.LogicalOperationStack.Count;
        }

        void WriteAll(TraceEventCache eventCache, TraceEventType eventType, int id, string message)
        {
            UpdateIndent();
            WriteLine("{4}-[{0}] {1}({2}) {3}".With(eventCache.DateTime.ToString("u"), eventType.ToString(), id, message, eventCache.ThreadId));
        }
    }
}

#region Full license
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion