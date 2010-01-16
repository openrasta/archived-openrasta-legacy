using System;
using OpenRasta.Web;

namespace OpenBastard.Environments
{
    public interface IEnvironment : IDisposable
    {
        /// <summary>
        /// Gets the name of the environment.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Creates a request object that can be used to configure the http request issued by the test runner
        /// </summary>
        /// <returns></returns>
        IClientRequest CreateRequest(string uri);

        /// <summary>
        /// Returns the response object that the test runner got after executing a request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IResponse ExecuteRequest(IClientRequest request);

        /// <summary>
        /// Initializes the environment for starting a test fixture
        /// </summary>
        void Initialize();
    }
}