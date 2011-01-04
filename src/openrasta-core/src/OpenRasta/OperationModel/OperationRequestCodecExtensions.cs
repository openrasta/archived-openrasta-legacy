using OpenRasta.Codecs;

namespace OpenRasta.OperationModel
{
    public static class OperationRequestCodecExtensions
    {
        const string REQUEST_CODEC = "_REQUEST_CODEC";

        /// <summary>
        /// Gets The codec used to read the request.
        /// </summary>
        public static CodecMatch GetRequestCodec(this IOperation operation)
        {
            return operation.ExtendedProperties[REQUEST_CODEC] as CodecMatch;
        }

        public static void SetRequestCodec(this IOperation operation, CodecMatch codecMatch)
        {
            operation.ExtendedProperties[REQUEST_CODEC] = codecMatch;
        }
    }
}