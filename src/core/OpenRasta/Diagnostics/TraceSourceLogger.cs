#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System;
using System.Diagnostics;

namespace OpenRasta.Diagnostics
{
    public class TraceSourceLogger<T> : TraceSourceLogger, ILogger<T> where T : ILogSource
    {
        public TraceSourceLogger()
            : base(new TraceSource(LogSource<T>.Category))
        {
        }
    }

    public class TraceSourceLogger : ILogger
    {
        readonly TraceSource _source;

        public TraceSourceLogger() : this(new TraceSource("openrasta"))
        {
        }

        public TraceSourceLogger(TraceSource source)
        {
            _source = source;
            _source.Listeners.Remove("Default");

            var listener = new DebuggerLoggingTraceListener
            {
                Name = "OpenRasta", 
                TraceOutputOptions =
                    TraceOptions.DateTime | TraceOptions.ThreadId |
                    TraceOptions.LogicalOperationStack
            };

            _source.Listeners.Add(listener);

            _source.Switch = new SourceSwitch("OpenRasta", "All");
        }

        public IDisposable Operation(object source, string name)
        {
            _source.TraceData(TraceEventType.Start, 1, "Entering {0}: {1}".With(source.GetType().Name, name));
            Trace.CorrelationManager.StartLogicalOperation(source.GetType().Name);

            return new OperationCookie { Initiator = source, Source = _source };
        }

        public void WriteDebug(string message, params object[] format)
        {
            _source.TraceData(TraceEventType.Verbose, 0, message.With(format));
        }

        public void WriteError(string message, params object[] format)
        {
            _source.TraceData(TraceEventType.Error, 0, message.With(format));
        }

        public void WriteException(Exception e)
        {
            if (e == null)
                return;
            WriteError("An error of type {0} has been thrown", e.GetType());
            foreach (string line in e.ToString().Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                WriteError(line);
        }

        public void WriteInfo(string message, params object[] format)
        {
            _source.TraceData(TraceEventType.Information, 0, message.With(format));
        }

        public void WriteWarning(string message, params object[] format)
        {
            _source.TraceData(TraceEventType.Warning, 0, message.With(format));
        }

        class OperationCookie : IDisposable
        {
            public object Initiator { get; set; }
            public TraceSource Source { get; set; }

            public void Dispose()
            {
                Trace.CorrelationManager.StopLogicalOperation();
                Source.TraceData(TraceEventType.Stop, 1, "Exiting {0}".With(Initiator.GetType().Name));
            }
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