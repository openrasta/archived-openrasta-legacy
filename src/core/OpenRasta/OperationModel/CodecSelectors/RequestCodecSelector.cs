using System.Collections.Generic;
using System.Linq;
using OpenRasta.Codecs;
using OpenRasta.Collections;
using OpenRasta.Diagnostics;
using OpenRasta.OperationModel.Hydrators;
using OpenRasta.Web;

namespace OpenRasta.OperationModel.CodecSelectors
{
    /// <summary>
    /// Resolves a compatible codec for an operation, and filters out operations
    /// that do not have a codec capable of processing the request.
    /// </summary>
    public class RequestCodecSelector : IOperationCodecSelector
    {
        readonly ICodecRepository _codecRepository;
        readonly IRequest _request;

        public RequestCodecSelector(ICodecRepository codecRepository, IRequest request)
        {
            _codecRepository = codecRepository;
            _request = request;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public IEnumerable<IOperation> Process(IEnumerable<IOperation> operations)
        {
            if (_request.Entity.ContentLength.IsNullOr(0))
                return LogSelected(operations.Where(operation => operation.Inputs.AllReady()));

            var requestMediaType = _request.Entity.ContentType ?? DetectMediaType();

            var results = from operation in operations
                          let requiredMembers = from member in operation.Inputs.Required()
                                                where !member.IsReadyForAssignment
                                                select member.Member
                          let optionalMembers = from member in 
                                                    operation.Inputs.Optional().Union(
                                                    operation.Inputs.Required()
                                                        .Where(x => x.IsReadyForAssignment))
                                                where member.IsReadyForAssignment
                                                select member.Member
                          let hasMembersToFill = optionalMembers.Any() || requiredMembers.Any()
                          let selectedCodec = hasMembersToFill
                                                  ? _codecRepository.FindMediaTypeReader(
                                                        requestMediaType,
                                                        requiredMembers,
                                                        optionalMembers)
                                                  : null
                          where !hasMembersToFill || selectedCodec != null
                          select new
                          {
                              operation,
                              codec = selectedCodec
                          };

            results.ForEach(x => x.operation.SetRequestCodec(x.codec));

            return LogSelected(results.OrderByDescending(x=>x.codec).Select(x => x.operation));
        }

        MediaType DetectMediaType()
        {
            return MediaType.ApplicationOctetStream;
        }

        IEnumerable<IOperation> LogSelected(IEnumerable<IOperation> selectedOps)
        {
            foreach (var op in selectedOps)
            {
                if (op.GetRequestCodec() == null)
                    Logger.WriteInfo("Operation {0} selected with {1} required members and {2} optional members, without request codec",
                                     op,
                                     op.Inputs.Required().Count(),
                                     op.Inputs.Optional().Count());
                else
                    Logger.WriteInfo("Operation {0} selected with {1} required members and {2} optional members, with codec {3} with score {4}.",
                                     op,
                                     op.Inputs.Required().Count(),
                                     op.Inputs.Optional().Count(),
                                     op.GetRequestCodec().CodecRegistration.CodecType.Name,
                                     op.GetRequestCodec().Score);
                yield return op;
            }
        }
    }
}