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

namespace OpenRasta.Diagnostics
{
    public class NullLogger : ILogger
    {
        static readonly ILogger INSTANCE = new NullLogger();
        static readonly OperationCookie COOKIE = new OperationCookie();

        public static ILogger Instance
        {
            get { return INSTANCE; }
        }

        public IDisposable Operation(object source, string name)
        {
            return COOKIE;
        }

        public void WriteDebug(string message, params object[] format)
        {
        }

        public void WriteError(string message, params object[] format)
        {
        }

        public void WriteException(Exception e)
        {
        }

        public void WriteInfo(string message, params object[] format)
        {
        }

        public void WriteWarning(string message, params object[] format)
        {
        }

        class OperationCookie : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }

    public class NullLogger<T> : NullLogger, ILogger<T> where T : class, ILogSource
    {
        static readonly ILogger<T> INSTANCE = new NullLogger<T>();

        public new static ILogger<T> Instance
        {
            get { return INSTANCE; }
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