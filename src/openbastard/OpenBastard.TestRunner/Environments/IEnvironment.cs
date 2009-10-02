using OpenRasta.Web;

namespace OpenBastard.Scenarios
{
    public interface IEnvironment
    {
        /// <summary>
        /// Creates a reqest object that can be used to configure the http request issued by the test runner
        /// </summary>
        /// <returns></returns>
        IRequest CreateRequest(string uri);

        /// <summary>
        /// Returns the response ojbect that the test runner got after executing a request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IResponse ExecuteRequest(IRequest request);

        /// <summary>
        /// Initializes the environment for starting a test fixture
        /// </summary>
        void Initialize();

        void Dispose();

        string Name { get; }
    }
}