using System;
using System.Globalization;
using OpenRasta.Configuration.MetaModel;
using OpenRasta.TypeSystem;

namespace OpenRasta.Configuration.Fluent.Implementation
{
    public class UriDefinition : IUriDefinition
    {
        readonly ResourceDefinition _resourceDefinition;
        readonly UriModel _uriModel;

        public UriDefinition(ResourceDefinition resourceDefinition, string uri)
        {
            _resourceDefinition = resourceDefinition;
            _uriModel = new UriModel { Uri = uri };
            _resourceDefinition.Registration.Uris.Add(_uriModel);
        }

        public IResourceDefinition And
        {
            get { return _resourceDefinition; }
        }

        public IHandlerForResourceWithUriDefinition HandledBy<T>()
        {
            return _resourceDefinition.HandledBy<T>();
        }

        public IHandlerForResourceWithUriDefinition HandledBy(Type type)
        {
            return _resourceDefinition.HandledBy(type);
        }

        public IHandlerForResourceWithUriDefinition HandledBy(IType type)
        {
            return _resourceDefinition.HandledBy(type);
        }

        public IUriDefinition InLanguage(string language)
        {
            _uriModel.Language = language == null
                                     ? CultureInfo.InvariantCulture
                                     : CultureInfo.GetCultureInfo(language);
            return this;
        }

        public IUriDefinition Named(string uriName)
        {
            _uriModel.Name = uriName;
            return this;
        }
    }
}