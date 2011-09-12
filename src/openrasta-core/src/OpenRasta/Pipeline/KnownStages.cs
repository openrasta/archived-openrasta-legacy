using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRasta.Pipeline
{
    /// <summary>
    /// Contains all the known stages 
    /// </summary>
    public static class KnownStages
    {
        /// <summary>
        /// Represents the first stage in the pipeline.
        /// </summary>
        public interface IBegin : IPipelineContributor { }

        /// <summary>
        /// Represents the stage at which http authentication will take place
        /// </summary>
        public interface IAuthentication : IPipelineContributor { }

        /// <summary>
        /// Represents the stage at which the URI is matched to find a resource.
        /// </summary>
        public interface IUriMatching : IPipelineContributor { }

        /// <summary>
        /// Represents the stage at which handlers are selected for a resource.
        /// </summary>
        public interface IHandlerSelection : IPipelineContributor { }

        /// <summary>
        /// Represents the stage at which the operations available on a handler get created.
        /// </summary>
        public interface IOperationCreation : IPipelineContributor { }

        /// <summary>
        /// Represents the stage at which operations get filtered out and ignored.
        /// </summary>
        public interface IOperationFiltering : IPipelineContributor { }

        /// <summary>
        /// Represents the stage at which a codec is chosen to process the request entity.
        /// </summary>
        public interface ICodecRequestSelection : IPipelineContributor { }

        /// <summary>
        /// Represents the stage at which the operations get populated with data from the URI
        /// </summary>
        public interface IRequestDecoding : IPipelineContributor { }

        /// <summary>
        /// Represents the stage at which the operation is being executed.
        /// </summary>
        public interface IOperationExecution : IPipelineContributor { }

        /// <summary>
        /// Represents the stage at which the operation result gets executed.
        /// </summary>
        public interface IOperationResultInvocation : IPipelineContributor { }

        /// <summary>
        /// Represents the stage at which a codec is found to render the response resource.
        /// </summary>
        public interface ICodecResponseSelection : IPipelineContributor { }

        /// <summary>
        /// Represents the stage at which the codec is rendering the response resource.
        /// </summary>
        public interface IResponseCoding : IPipelineContributor { }

        /// <summary>
        /// Represents the end of the pipeline.
        /// </summary>
        public interface IEnd : IPipelineContributor { }


    }
}
