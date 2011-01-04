#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using OpenRasta.Binding;
using OpenRasta.Codecs;
using OpenRasta.CodeDom.Compiler;
using OpenRasta.Collections;
using OpenRasta.Configuration.MetaModel;
using OpenRasta.Configuration.MetaModel.Handlers;
using OpenRasta.DI;
using OpenRasta.Diagnostics;
using OpenRasta.Handlers;
using OpenRasta.OperationModel;
using OpenRasta.OperationModel.CodecSelectors;
using OpenRasta.OperationModel.Filters;
using OpenRasta.OperationModel.Hydrators;
using OpenRasta.OperationModel.Interceptors;
using OpenRasta.OperationModel.MethodBased;
using OpenRasta.Pipeline;
using OpenRasta.Pipeline.Contributors;
using OpenRasta.TypeSystem;
using OpenRasta.TypeSystem.ReflectionBased;
using OpenRasta.TypeSystem.Surrogated;
using OpenRasta.TypeSystem.Surrogates;
using OpenRasta.TypeSystem.Surrogates.Static;
using OpenRasta.Web;

namespace OpenRasta.Configuration
{
    public class DefaultDependencyRegistrar : IDependencyRegistrar
    {
        protected Type PathManagerType;

        public DefaultDependencyRegistrar()
        {
            CodecTypes = new List<Type>();
            PipelineContributorTypes = new List<Type>();
            CodeSnippetModifierTypes = new List<Type>();
            TraceSourceListenerTypes = new List<Type>();
            MetaModelHandlerTypes = new List<Type>();
            MethodFilterTypes = new List<Type>();
            OperationFilterTypes = new List<Type>();
            OperationHydratorTypes = new List<Type>();
            OperationCodecSelectorTypes = new List<Type>();
            SurrogateBuilders = new List<Type>();
            LogSourceTypes = new List<Type>();

            SetTypeSystem<ReflectionBasedTypeSystem>();
            SetMetaModelRepository<MetaModelRepository>();
            SetUriResolver<TemplatedUriResolver>();
            SetCodecRepository<CodecRepository>();
            SetHandlerRepository<HandlerRepository>();
            SetPipeline<PipelineRunner>();
            SetLogger<TraceSourceLogger>();
            SetErrorCollector<OperationContextErrorCollector>();
            SetObjectBinderLocator<DefaultObjectBinderLocator>();
            SetOperationCreator<MethodBasedOperationCreator>();
            SetOperationExecutor<OperationExecutor>();
            SetOperationInterceptorProvider<SystemAndAttributesOperationInterceptorProvider>();
            SetPathManager<PathManager>();

            AddMethodFilter<TypeExclusionMethodFilter<object>>();

            AddDefaultCodecs();
            AddDefaultContributors();
            AddCSharpCodeSnippetModifiers();
            AddDefaultMetaModelHandlers();
            AddOperationFilters();
            AddOperationHydrators();
            AddOperationCodecResolvers();
            AddLogSources();
            AddSurrogateBuilders();
        }

        protected IList<Type> CodeSnippetModifierTypes { get; private set; }

        protected Type CodecRepositoryType { get; set; }
        protected IList<Type> CodecTypes { get; private set; }
        protected Type ErrorCollectorType { get; set; }
        protected Type HandlerRepositoryType { get; set; }
        protected IList<Type> LogSourceTypes { get; set; }
        protected Type LogSourcedLoggerType { get; set; }
        protected Type LoggerType { get; set; }
        protected IList<Type> MetaModelHandlerTypes { get; private set; }
        protected Type MetaModelRepositoryType { get; set; }
        protected IList<Type> MethodFilterTypes { get; set; }
        protected IList<Type> OperationCodecSelectorTypes { get; set; }
        protected Type OperationCreatorType { get; set; }
        protected Type OperationExecutorType { get; set; }
        protected IList<Type> OperationFilterTypes { get; set; }
        protected IList<Type> OperationHydratorTypes { get; set; }
        protected Type OperationInterceptorProviderType { get; set; }
        protected Type ParameterBinderLocatorType { get; set; }
        protected IList<Type> PipelineContributorTypes { get; private set; }
        protected Type PipelineType { get; set; }
        protected IList<Type> SurrogateBuilders { get; private set; }
        protected IList<Type> TraceSourceListenerTypes { get; private set; }
        protected Type TypeSystemType { get; set; }
        protected Type UriResolverType { get; set; }

        public void AddCodeSnippetModifier<T>() where T : ICodeSnippetModifier
        {
            CodeSnippetModifierTypes.Add(typeof(T));
        }

        public void AddCodec<T>() where T : ICodec
        {
            CodecTypes.Add(typeof(T));
        }

        public void AddMetaModelHandler<T>() where T : IMetaModelHandler
        {
            MetaModelHandlerTypes.Add(typeof(T));
        }

        public void AddMethodFilter<T>() where T : IMethodFilter
        {
            MethodFilterTypes.Add(typeof(T));
        }

        public void AddOperationCodecSelector<T>()
        {
            OperationCodecSelectorTypes.Add(typeof(T));
        }

        public void AddPipelineContributor<T>() where T : IPipelineContributor
        {
            PipelineContributorTypes.Add(typeof(T));
        }

        public void AddSurrogateBuilders()
        {
            SurrogateBuilders.Add(typeof(ListIndexerSurrogateBuilder));
            SurrogateBuilders.Add(typeof(DateTimeSurrogate));
        }

        public void SetCodecRepository<T>() where T : ICodecRepository
        {
            CodecRepositoryType = typeof(T);
        }

        public void SetErrorCollector<T>()
        {
            ErrorCollectorType = typeof(T);
        }

        public void SetHandlerRepository<T>() where T : IHandlerRepository
        {
            HandlerRepositoryType = typeof(T);
        }

        public void SetLogger<T>() where T : ILogger
        {
            LoggerType = typeof(T);
        }

        public void SetMetaModelRepository<T>()
        {
            MetaModelRepositoryType = typeof(T);
        }

        public void SetObjectBinderLocator<T>() where T : IObjectBinderLocator
        {
            ParameterBinderLocatorType = typeof(T);
        }

        public void SetOperationExecutor<T>()
        {
            OperationExecutorType = typeof(T);
        }

        public void SetOperationInterceptorProvider<T>()
        {
            OperationInterceptorProviderType = typeof(T);
        }

        public void SetPathManager<T>()
        {
            PathManagerType = typeof(T);
        }

        public void SetPipeline<T>() where T : IPipeline
        {
            PipelineType = typeof(T);
        }

        public void SetTypeSystem<T>() where T : ITypeSystem
        {
            TypeSystemType = typeof(T);
        }

        public void SetUriResolver<T>() where T : IUriResolver
        {
            UriResolverType = typeof(T);
        }

        public virtual void Register(IDependencyResolver resolver)
        {
            RegisterCoreComponents(resolver);
            RegisterSurrogateBuilders(resolver);
            RegisterLogging(resolver);
            RegisterMetaModelHandlers(resolver);
            RegisterContributors(resolver);
            RegisterCodeSnippeModifiers(resolver);
            RegisterMethodFilters(resolver);
            RegisterOperationModel(resolver);
            RegisterLogSources(resolver);
            RegisterCodecs(resolver);
        }

        protected virtual void AddCSharpCodeSnippetModifiers()
        {
            AddCodeSnippetModifier<MarkupElementModifier>();
            AddCodeSnippetModifier<UnencodedOutputModifier>();
        }

        protected virtual void AddDefaultMetaModelHandlers()
        {
            AddMetaModelHandler<TypeRewriterMetaModelHandler>();
            AddMetaModelHandler<CodecMetaModelHandler>();
            AddMetaModelHandler<HandlerMetaModelHandler>();
            AddMetaModelHandler<UriRegistrationMetaModelHandler>();
            AddMetaModelHandler<DependencyRegistrationMetaModelHandler>();
        }

        protected virtual void AddLogSources()
        {
            LogSourcedLoggerType = typeof(TraceSourceLogger<>);
            LogSourceTypes.AddRange(Assembly.GetExecutingAssembly().GetTypes().Where(x => !x.IsAbstract && !x.IsInterface && x.IsAssignableTo<ILogSource>()));
        }

        protected virtual void AddOperationCodecResolvers()
        {
            AddOperationCodecSelector<RequestCodecSelector>();
        }

        protected virtual void AddOperationFilter<T>() where T : IOperationFilter
        {
            OperationFilterTypes.Add(typeof(T));
        }

        protected virtual void AddOperationFilters()
        {
            AddOperationFilter<HttpMethodOperationFilter>();
            AddOperationFilter<UriNameOperationFilter>();
            AddOperationFilter<UriParametersFilter>();
        }

        protected virtual void AddOperationHydrator<T>()
        {
            OperationHydratorTypes.Add(typeof(T));
        }

        protected virtual void AddOperationHydrators()
        {
            AddOperationHydrator<RequestEntityReaderHydrator>();
        }

        protected virtual void RegisterCodeSnippeModifiers(IDependencyResolver resolver)
        {
            CodeSnippetModifierTypes.ForEach(x => resolver.AddDependency(typeof(ICodeSnippetModifier), x, DependencyLifetime.Transient));
        }

        protected virtual void RegisterCodecs(IDependencyResolver resolver)
        {
            var repo = resolver.Resolve<ICodecRepository>();
            var typeSystem = resolver.Resolve<ITypeSystem>();
            foreach (Type codecType in CodecTypes)
            {
                if (!resolver.HasDependency(codecType))
                    resolver.AddDependency(codecType, DependencyLifetime.Transient);
                IEnumerable<CodecRegistration> registrations = CodecRegistration.FromCodecType(codecType, typeSystem);
                registrations.ForEach(repo.Add);
            }
        }

        protected virtual void RegisterContributors(IDependencyResolver resolver)
        {
            PipelineContributorTypes.ForEach(x => resolver.AddDependency(typeof(IPipelineContributor), x, DependencyLifetime.Singleton));
        }

        protected virtual void RegisterCoreComponents(IDependencyResolver resolver)
        {
            resolver.AddDependency(typeof(ITypeSystem), TypeSystemType, DependencyLifetime.Singleton);
            resolver.AddDependency(typeof(IMetaModelRepository), MetaModelRepositoryType, DependencyLifetime.Singleton);
            resolver.AddDependency(typeof(IUriResolver), UriResolverType, DependencyLifetime.Singleton);
            resolver.AddDependency(typeof(ICodecRepository), CodecRepositoryType, DependencyLifetime.Singleton);
            resolver.AddDependency(typeof(IHandlerRepository), HandlerRepositoryType, DependencyLifetime.Singleton);
            resolver.AddDependency(typeof(IPipeline), PipelineType, DependencyLifetime.Singleton);
            resolver.AddDependency(typeof(IObjectBinderLocator), ParameterBinderLocatorType, DependencyLifetime.Singleton);
            resolver.AddDependency(typeof(IOperationCreator), OperationCreatorType, DependencyLifetime.Transient);
            resolver.AddDependency(typeof(IOperationExecutor), OperationExecutorType, DependencyLifetime.Transient);
            resolver.AddDependency(typeof(IErrorCollector), ErrorCollectorType, DependencyLifetime.Transient);
            resolver.AddDependency(typeof(IOperationInterceptorProvider), OperationInterceptorProviderType, DependencyLifetime.Transient);
            resolver.AddDependency(typeof(IPathManager), PathManagerType, DependencyLifetime.Singleton);
            resolver.AddDependency(typeof(ISurrogateProvider), typeof(SurrogateBuilderProvider), DependencyLifetime.Singleton);
        }

        [Conditional("DEBUG")]
        protected virtual void RegisterDefaultTraceListener(IDependencyResolver resolver)
        {
            if (!resolver.HasDependencyImplementation(typeof(TraceListener), typeof(DebuggerLoggingTraceListener)))
                resolver.AddDependency(typeof(TraceListener), typeof(DebuggerLoggingTraceListener), DependencyLifetime.Transient);
        }

        protected virtual void RegisterLogSources(IDependencyResolver resolver)
        {
            LogSourceTypes.ForEach(x => resolver.AddDependency(typeof(ILogger<>).MakeGenericType(x), LogSourcedLoggerType.MakeGenericType(x), DependencyLifetime.Transient));
        }

        protected virtual void RegisterLogging(IDependencyResolver resolver)
        {
            resolver.AddDependency(typeof(ILogger), LoggerType, DependencyLifetime.Singleton);

            RegisterTraceSourceLiseners(resolver);
            RegisterDefaultTraceListener(resolver);
        }

        protected virtual void RegisterMetaModelHandlers(IDependencyResolver resolver)
        {
            MetaModelHandlerTypes.ForEach(x => resolver.AddDependency(typeof(IMetaModelHandler), x, DependencyLifetime.Transient));
        }

        protected virtual void RegisterMethodFilters(IDependencyResolver resolver)
        {
            MethodFilterTypes.ForEach(x => resolver.AddDependency(typeof(IMethodFilter), x, DependencyLifetime.Transient));
        }

        protected virtual void RegisterOperationModel(IDependencyResolver resolver)
        {
            OperationFilterTypes.ForEach(x => resolver.AddDependency(typeof(IOperationFilter), x, DependencyLifetime.Transient));
            OperationHydratorTypes.ForEach(x => resolver.AddDependency(typeof(IOperationHydrator), x, DependencyLifetime.Transient));
            OperationCodecSelectorTypes.ForEach(x => resolver.AddDependency(typeof(IOperationCodecSelector), x, DependencyLifetime.Transient));
        }

        protected virtual void RegisterSurrogateBuilders(IDependencyResolver resolver)
        {
            SurrogateBuilders.ForEach(x => resolver.AddDependency(typeof(ISurrogateBuilder), x, DependencyLifetime.Transient));
        }

        protected virtual void RegisterTraceSourceLiseners(IDependencyResolver resolver)
        {
            TraceSourceListenerTypes.ForEach(x => resolver.AddDependency(typeof(TraceListener), x, DependencyLifetime.Transient));
        }

        protected void SetOperationCreator<T>() where T : IOperationCreator
        {
            OperationCreatorType = typeof(T);
        }


        protected virtual void AddDefaultCodecs()
        {
            AddCodec<HtmlErrorCodec>();
            AddCodec<TextPlainCodec>();
            AddCodec<ApplicationXWwwFormUrlencodedKeyedValuesCodec>();
            AddCodec<ApplicationXWwwFormUrlencodedObjectCodec>();
            AddCodec<MultipartFormDataObjectCodec>();
            AddCodec<MultipartFormDataKeyedValuesCodec>();
            AddCodec<ApplicationOctetStreamCodec>();
            AddCodec<OperationResultCodec>();
        }

        protected virtual void AddDefaultContributors()
        {
            AddPipelineContributor<ResponseEntityCodecResolverContributor>();
            AddPipelineContributor<ResponseEntityWriterContributor>();
            AddPipelineContributor<BootstrapperContributor>();
            AddPipelineContributor<HttpMethodOverriderContributor>();
            AddPipelineContributor<UriDecoratorsContributor>();

            AddPipelineContributor<ResourceTypeResolverContributor>();
            AddPipelineContributor<HandlerResolverContributor>();

            AddPipelineContributor<AuthenticationContributor>();
            AddPipelineContributor<AuthenticationChallengerContributor>();

            AddPipelineContributor<OperationCreatorContributor>();
            AddPipelineContributor<OperationFilterContributor>();
            AddPipelineContributor<OperationHydratorContributor>();
            AddPipelineContributor<OperationCodecSelectorContributor>();
            AddPipelineContributor<OperationInvokerContributor>();
            AddPipelineContributor<OperationResultInvokerContributor>();

            AddPipelineContributor<OperationInterceptorContributor>();

            AddPipelineContributor<EndContributor>();
        }
    }
}

#region Full license
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion
