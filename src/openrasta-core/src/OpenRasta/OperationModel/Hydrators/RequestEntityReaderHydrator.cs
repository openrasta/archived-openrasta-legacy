using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenRasta.Binding;
using OpenRasta.Codecs;
using OpenRasta.Collections;
using OpenRasta.DI;
using OpenRasta.Diagnostics;
using OpenRasta.OperationModel.Hydrators.Diagnostics;
using OpenRasta.TypeSystem.ReflectionBased;
using OpenRasta.Web;
using OpenRasta.Pipeline;

namespace OpenRasta.OperationModel.Hydrators
{
    public class RequestEntityReaderHydrator : IOperationHydrator
    {
        readonly IRequest _request;
        readonly IDependencyResolver _resolver;

        public RequestEntityReaderHydrator(IDependencyResolver resolver, IRequest request)
        {
            Log = NullLogger<CodecLogSource>.Instance;
            ErrorCollector = NullErrorCollector.Instance;
            _resolver = resolver;
            _request = request;
        }

        public IErrorCollector ErrorCollector { get; set; }
        public ILogger<CodecLogSource> Log { get; set; }

        public IEnumerable<IOperation> Process(IEnumerable<IOperation> operations)
        {
            var operation = operations.Where(x => x.GetRequestCodec() != null)
                                .OrderByDescending(x => x.GetRequestCodec()).FirstOrDefault()
                            ?? operations.Where(x => x.Inputs.AllReady())
                                   .OrderByDescending(x => x.Inputs.CountReady()).FirstOrDefault();
            if (operation == null)
            {
                Log.OperationNotFound();
                yield break;
            }

            Log.OperationFound(operation);

            if (operation.GetRequestCodec() != null)
            {
                var codecInstance = CreateMediaTypeReader(operation);

                var codecType = codecInstance.GetType();
                Log.CodecLoaded(codecType);

                if (codecType.Implements(typeof(IKeyedValuesMediaTypeReader<>)))
                    if (TryAssignKeyedValues(_request.Entity, codecInstance, codecType, operation))
                    {
                        yield return operation;
                        yield break;
                    }

                if (codecType.Implements<IMediaTypeReader>())
                    if (!TryReadPayloadAsObject(_request.Entity, (IMediaTypeReader)codecInstance, operation))
                        yield break;
            }
            yield return operation;
        }

        static ErrorFrom<RequestEntityReaderHydrator> CreateErrorForException(Exception e)
        {
            return new ErrorFrom<RequestEntityReaderHydrator>
            {
                Message = "The codec failed to process the request entity. See the exception below.\r\n" + e,
                Exception = e
            };
        }


        ICodec CreateMediaTypeReader(IOperation operation)
        {
            return _resolver.Resolve(operation.GetRequestCodec().CodecRegistration.CodecType, UnregisteredAction.AddAsTransient) as ICodec;
        }

        bool TryAssignKeyedValues(IHttpEntity requestEntity, ICodec codec, Type codecType, IOperation operation)
        {
            Log.CodecSupportsKeyedValues();

            return codec.TryAssignKeyValues(requestEntity, operation.Inputs.Select(x => x.Binder), Log.KeyAssigned, Log.KeyFailed);
        }

        bool TryReadPayloadAsObject(IHttpEntity requestEntity, IMediaTypeReader reader, IOperation operation)
        {
            Log.CodecSupportsFullObjectResolution();
            foreach (var member in from m in operation.Inputs
                                   where m.Binder.IsEmpty
                                   select m)
            {
                Log.ProcessingMember(member);
                try
                {
                    var entityInstance = reader.ReadFrom(requestEntity,
                                                         member.Member.Type,
                                                         member.Member.Name);
                    Log.Result(entityInstance);

                    if (entityInstance != Missing.Value)
                    {
                        if (!member.Binder.SetInstance(entityInstance))
                        {
                            Log.BinderInstanceAssignmentFailed();
                            return false;
                        }
                        Log.BinderInstanceAssignmentSucceeded();
                    }
                }
                catch (Exception e)
                {
                    ErrorCollector.AddServerError(CreateErrorForException(e));
                    return false;
                }
            }
            return true;
        }
    }
}