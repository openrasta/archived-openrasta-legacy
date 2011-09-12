using OpenRasta.Authentication;
using OpenRasta.DI;
using OpenRasta.Diagnostics;
using OpenRasta.Web;

namespace OpenRasta.Pipeline.Contributors
{
    public class AuthenticationChallengerContributor : IPipelineContributor
    {
        readonly IDependencyResolver _resolver;
        public ILogger Log { get; set; }

        public AuthenticationChallengerContributor(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public void Initialize(IPipeline pipelineRunner)
        {
            pipelineRunner.Notify(ChallengeIfUnauthorized)
                .After<KnownStages.IOperationExecution>()
                .And
                .Before<KnownStages.IResponseCoding>();
        }

        private PipelineContinuation ChallengeIfUnauthorized(ICommunicationContext context)
        {
            if (context.OperationResult is OperationResult.Unauthorized)
            {
                var supportedSchemes = _resolver.ResolveAll<IAuthenticationScheme>();

                foreach (var scheme in supportedSchemes)
                {
                    scheme.Challenge(context.Response);
                }
            }

            return PipelineContinuation.Continue;
        }
    }
}