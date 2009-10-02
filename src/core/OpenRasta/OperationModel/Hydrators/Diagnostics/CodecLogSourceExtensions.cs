using System;
using System.Reflection;
using OpenRasta.Binding;
using OpenRasta.Diagnostics;

namespace OpenRasta.OperationModel.Hydrators.Diagnostics
{
    public static class CodecLogSourceExtensions
    {
        public static void BinderInstanceAssignmentFailed(this ILogger<CodecLogSource> logger)
        {
            logger.WriteDebug("The binder failed to assign the instance to the parameter.");
        }

        public static void BinderInstanceAssignmentSucceeded(this ILogger<CodecLogSource> logger)
        {
            logger.WriteDebug("The binder successfully assigned the instance to the parameter.");
        }

        public static void CodecLoaded(this ILogger<CodecLogSource> logger, Type readerInstanceConcreteType)
        {
            logger.WriteInfo("Loaded codec {0}", readerInstanceConcreteType);
        }

        public static void CodecSupportsFullObjectResolution(this ILogger<CodecLogSource> logger)
        {
            logger.WriteDebug("Switching to full object media type reading.");
        }

        public static void CodecSupportsKeyedValues(this ILogger<CodecLogSource> logger)
        {
            logger.WriteInfo("Codec supports IKeyedValuesMediaTypeReader. Processing parameters.");
        }

        public static void KeyAssigned(this ILogger<CodecLogSource> logger, KeyedValues keyValues)
        {
            logger.WriteDebug("Key {0} was successfully assigned.", keyValues.Key);
        }

        public static void KeyFailed(this ILogger<CodecLogSource> logger, KeyedValues keyValues)
        {
            logger.WriteDebug("Key {0} was not successfully assigned.", keyValues.Key);
        }

        public static void OperationFound(this ILogger<CodecLogSource> logger, IOperation operation)
        {
            if (operation.GetRequestCodec() != null)
                logger.WriteInfo("Operation {0} was selected with a codec score of {1}", operation, operation.GetRequestCodec().Score);
            else
                logger.WriteInfo("Operation {0} was selected without a codec.", operation);
        }

        public static void OperationNotFound(this ILogger<CodecLogSource> logger)
        {
            logger.WriteError("No operation was found.");
        }

        public static void ProcessingMember(this ILogger<CodecLogSource> logger, InputMember member)
        {
        }

        public static void Result(this ILogger<CodecLogSource> logger, object result)
        {
            if (result == Missing.Value)
                logger.WriteInfo("No result returned from the codec.");
            else if (result == null)
                logger.WriteInfo("Null result was returned by the codec.", result);
            else
                logger.WriteInfo("Result of type {0} was returned by the codec.", result.GetType());
        }

        public static void TryAssignValue(this ILogger<CodecLogSource> logger, InputMember member)
        {
            logger.WriteDebug("Trying to assign key and values to {0} {1}.", member.Member.TypeName, member.Member.Name);
        }
    }
}