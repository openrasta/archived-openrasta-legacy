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
using System.Diagnostics;
using OpenRasta.Configuration.MetaModel;
using OpenRasta.DI;

namespace OpenRasta.Configuration
{
    public static class OpenRastaConfiguration
    {
        static bool _isBeingConfigured;

        /// <summary>
        /// Creates a manual configuration of the resources supported by the application.
        /// </summary>
        public static IDisposable Manual
        {
            get
            {
                if (_isBeingConfigured)
                    throw new InvalidOperationException("Configuration is already happening on another thread.");

                _isBeingConfigured = true;

                return new FluentConfigurator();
            }
        }

        static void FinishConfiguration()
        {
            if (!_isBeingConfigured)
                throw new InvalidOperationException(
                    "Something went horribly wrong and the Configuration is deemed finish when it didn't even start!");

            DependencyManager.Pipeline.Initialize();
            _isBeingConfigured = false;
        }

        class FluentConfigurator : IDisposable
        {
            bool _disposed;

            ~FluentConfigurator()
            {
                Debug.Assert(_disposed, "The FluentConfigurator wasn't disposed properly.");
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
                var exceptions = new List<OpenRastaConfigurationException>();
                try
                {
                    var metaModelRepository = DependencyManager.GetService<IMetaModelRepository>();

                    metaModelRepository.Process();
                }
                finally
                {
                    FinishConfiguration();
                    _disposed = true;
                    if (exceptions.Count > 0)
                        throw new OpenRastaConfigurationException(exceptions);
                }
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