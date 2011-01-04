using System;
using OpenRasta.Codecs;
using OpenRasta.Configuration.MetaModel;
using OpenRasta.TypeSystem;

namespace OpenRasta.Configuration.Fluent.Implementation
{
    public class ResourceDefinition : IResourceDefinition, 
                                      IHandlerParentDefinition, 
                                      IHandlerForResourceWithUriDefinition
    {
        readonly ITypeSystem _typeSystem;

        public ResourceDefinition(ITypeSystem typeSystem, ResourceModel resourceRegistration)
        {
            Registration = resourceRegistration;
            _typeSystem = typeSystem;
        }

        public IHandlerParentDefinition And
        {
            get { return this; }
        }

        public ResourceModel Registration { get; private set; }

        /// <exception cref="InvalidOperationException">Cannot make a resource URI-less if a URI is already registered.</exception>
        public ICodecParentDefinition WithoutUri
        {
            get
            {
                if (Registration.Uris.Count > 0)
                    throw new InvalidOperationException(
                        "Cannot make a resource URI-less if a URI is already registered.");
                return new CodecParentDefinition(this);
            }
        }

        public ICodecDefinition TranscodedBy<TCodec>(object configuration) where TCodec : ICodec
        {
            return new CodecDefinition(this, typeof(TCodec), configuration);
        }

        public ICodecDefinition TranscodedBy(Type codecType, object configuration)
        {
            return new CodecDefinition(this, codecType, configuration);
        }

        public IHandlerForResourceWithUriDefinition HandledBy<T>()
        {
            return HandledBy(_typeSystem.FromClr(typeof(T)));
        }

        public IHandlerForResourceWithUriDefinition HandledBy(Type type)
        {
            return HandledBy(_typeSystem.FromClr(type));
        }

        public IHandlerForResourceWithUriDefinition HandledBy(IType type)
        {
            if (type == null) throw new ArgumentNullException("type");
            Registration.Handlers.Add(type);
            return this;
        }

        /// <exception cref="ArgumentNullException"><c>uri</c> is null.</exception>
        public IUriDefinition AtUri(string uri)
        {
            if (uri == null) throw new ArgumentNullException("uri");
            return new UriDefinition(this, uri);
        }
    }
}