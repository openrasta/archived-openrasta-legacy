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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace OpenRasta.Configuration
{
    [Serializable]
    public class OpenRastaConfigurationException : Exception
    {
        public OpenRastaConfigurationException()
        {
        }

        public OpenRastaConfigurationException(string message)
            : base(message)
        {
        }

        public OpenRastaConfigurationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public OpenRastaConfigurationException(IList<OpenRastaConfigurationException> exceptions)
            : base("Several configuration errors were reported. See below.\n" + GetInnerExceptionMessages(exceptions))
        {
            InnerExceptions = exceptions;
        }

#if !__OW_PROFILE_sl30__ && !__OW_PROFILE_sl40__
        protected OpenRastaConfigurationException(
            SerializationInfo info, 
            StreamingContext context)
            : base(info, context)
        {
        }

        public IList<OpenRastaConfigurationException> InnerExceptions { get; private set; }
#endif

        static string GetInnerExceptionMessages(IList<OpenRastaConfigurationException> exceptions)
        {
            var finalString = new StringBuilder();
            foreach (var exception in exceptions)
            {
                finalString.AppendLine(exception.ToString());
                finalString.AppendLine("-------------------------");
            }
            return finalString.ToString();
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