using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenRasta.Binding;
using OpenRasta.Collections;
using OpenRasta.DI;
using OpenRasta.Diagnostics;
using OpenRasta.IO;
using OpenRasta.OperationModel.Hydrators.Diagnostics;
using OpenRasta.Pipeline;
using OpenRasta.TypeSystem;
using OpenRasta.TypeSystem.ReflectionBased;
using OpenRasta.Web;

namespace OpenRasta.Codecs
{
    public abstract class AbstractMultipartFormDataCodec
    {
        const string FORMDATA_CACHE = "__MultipartFormDataCodec_FORMDATA_CACHED";

        // todo: inject the treshold from configuration, and be per-resource
        const int REQUEST_LENGTH_TRESHOLD = 80000;
        readonly byte[] _buffer = new byte[4096];
        readonly ICodecRepository _codecs;
        readonly IDependencyResolver _container;
        readonly PipelineData _pipeline;
        readonly ITypeSystem _typeSystem;

        protected AbstractMultipartFormDataCodec(ICommunicationContext context, 
                                                 ICodecRepository codecs, 
                                                 IDependencyResolver container, 
                                                 ITypeSystem typeSystem, 
                                                 IObjectBinderLocator binderLocator)
        {
            // temporary until IRequest / IResponse are moved to the container
            _pipeline = context.PipelineData;
            _codecs = codecs;
            _typeSystem = typeSystem;
            _container = container;
            BinderLocator = binderLocator;
            Log = NullLogger<CodecLogSource>.Instance;
        }

        public object Configuration { get; set; }
        public ILogger<CodecLogSource> Log { get; set; }

        protected IObjectBinderLocator BinderLocator { get; private set; }

        IDictionary<IHttpEntity, IDictionary<string, IList<IMultipartHttpEntity>>> Cache
        {
            get
            {
                return (_pipeline[FORMDATA_CACHE] ??
                        (_pipeline[FORMDATA_CACHE] =
                         new NullBehaviorDictionary<IHttpEntity, IDictionary<string, IList<IMultipartHttpEntity>>>()))
                       as IDictionary<IHttpEntity, IDictionary<string, IList<IMultipartHttpEntity>>>;
            }
        }

        public BindingResult ConvertValues(IMultipartHttpEntity entity, Type targetType)
        {
            object destination;
            var sourceMediaType = entity.ContentType ?? MediaType.TextPlain;

            var type = _typeSystem.FromClr(targetType);
            var mediaTypeReaderReg = _codecs.FindMediaTypeReader(sourceMediaType, new[] { type }, null);
            if (mediaTypeReaderReg != null)
            {
                
                var mediaTypeReader =
                    (ICodec)_container.Resolve(mediaTypeReaderReg.CodecRegistration.CodecType);
                if (mediaTypeReader is IMediaTypeReader)
                {
                    return BindingResult.Success(((IMediaTypeReader)mediaTypeReader).ReadFrom(entity, type, targetType.Name));
                }
                var binder = BinderLocator.GetBinder(type);
                if (mediaTypeReader.TryAssignKeyValues(entity, binder))
                    return binder.BuildObject();
            }

            // if no media type reader was found, try to parse to a string and convert from that.
            var stringType = _typeSystem.FromClr<string>();
            mediaTypeReaderReg = _codecs.FindMediaTypeReader(sourceMediaType, new[] { stringType }, null);

            if (entity.ContentType == null)
                entity.ContentType = MediaType.TextPlain;

            // defaults the entity to UTF-8 if none is specified, to account for browsers favouring using the charset of the origin page rather than the standard. Cause RFCs are too difficult to follow uh...
            if (entity.ContentType.CharSet == null)
                entity.ContentType.CharSet = "UTF-8";
            var plainTextReader = (IMediaTypeReader)_container.Resolve(mediaTypeReaderReg.CodecRegistration.CodecType);
            var targetString = plainTextReader.ReadFrom(entity, stringType, targetType.Name);
            destination = targetType.CreateInstanceFrom(targetString);

            return BindingResult.Success(destination);
        }

        public IEnumerable<KeyedValues<IMultipartHttpEntity>> ReadKeyValues(IHttpEntity entity)
        {
            foreach (string key in FormData(entity).Keys.ToArray())
            {
                var kv = new KeyedValues<IMultipartHttpEntity>(key, FormData(entity)[key], ConvertValues);

                yield return kv;

                if (kv.WasUsed)
                    FormData(entity).Remove(key);
            }
        }

        // Note that we store in the pipeline data because the same codec may be called for resolving several request entities
        protected IDictionary<string, IList<IMultipartHttpEntity>> FormData(IHttpEntity source)
        {
            if (Cache[source] == null)
            {
                Cache[source] = PreLoadAllParts(source);
            }
            return Cache[source];
        }

        static Stream CreateTempFile(out string filePath)
        {
            filePath = Path.GetTempFileName();
            return File.OpenWrite(filePath);
        }

        IDictionary<string, IList<IMultipartHttpEntity>> PreLoadAllParts(IHttpEntity source)
        {
            var multipartReader = new MultipartReader(source.ContentType.Boundary, source.Stream)
            {
                Log = Log
            };
            var formData =
                new NullBehaviorDictionary<string, IList<IMultipartHttpEntity>>(StringComparer.OrdinalIgnoreCase);
            foreach (var requestPart in multipartReader.GetParts())
            {
                if (requestPart.Headers.ContentDisposition != null &&
                    requestPart.Headers.ContentDisposition.Disposition.EqualsOrdinalIgnoreCase("form-data"))
                {
                    var memoryStream = new MemoryStream();
                    int totalRead = 0, lastRead;
                    while ((lastRead = requestPart.Stream.Read(_buffer, 0, _buffer.Length)) > 0)
                    {
                        totalRead += lastRead;
                        if (totalRead > REQUEST_LENGTH_TRESHOLD)
                        {
                            string filePath;
                            using (var fileStream = CreateTempFile(out filePath))
                            {
                                memoryStream.Position = 0;
                                var memoryStreamLength = memoryStream.CopyTo(fileStream);
                                fileStream.Write(_buffer, 0, lastRead);
                                var leftoverLength = requestPart.Stream.CopyTo(fileStream);
                            }
                            memoryStream = null;
                            requestPart.SwapStream(filePath);
                            break;
                        }
                        memoryStream.Write(_buffer, 0, lastRead);
                    }
                    if (memoryStream != null)
                    {
                        memoryStream.Position = 0;
                        requestPart.SwapStream(memoryStream);
                    }
                    var listOfEntities = formData[requestPart.Headers.ContentDisposition.Name]
                                         ??
                                         (formData[requestPart.Headers.ContentDisposition.Name] = new List<IMultipartHttpEntity>());
                    if (requestPart.ContentType == null)
                        requestPart.ContentType = MediaType.TextPlain;
                    listOfEntities.Add(requestPart);
                }
            }
            return formData;
        }
    }
}