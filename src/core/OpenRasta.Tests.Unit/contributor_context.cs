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
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using OpenRasta.Codecs;
using OpenRasta.Collections;
using OpenRasta.DI;
using OpenRasta.Diagnostics;
using OpenRasta.Handlers;
using OpenRasta.Hosting;
using OpenRasta.Hosting.InMemory;
using OpenRasta.Security;
using OpenRasta.Testing;
using OpenRasta.TypeSystem;
using OpenRasta.TypeSystem.ReflectionBased;
using OpenRasta.Web;
using OpenRasta.Pipeline;

namespace OpenRasta.Tests
{
    public class openrasta_context : context
    {
        Dictionary<Type, Func<ICommunicationContext, PipelineContinuation>> _actions;
        InMemoryHost Host;

        public openrasta_context()
        {
            TypeSystem = new ReflectionBasedTypeSystem();
        }

        public PipelineContinuation Result { get; set; }

        protected ICodecRepository Codecs
        {
            get { return Resolver.Resolve<ICodecRepository>(); }
        }

        protected ICommunicationContext Context { get; private set; }
        protected bool IsContributorExecuted { get; set; }
        protected IPipeline Pipeline { get; private set; }

        protected InMemoryRequest Request
        {
            get { return Context.Request as InMemoryRequest; }
        }

        protected IDependencyResolver Resolver
        {
            get { return Host.Resolver; }
        }

        protected ITypeSystem TypeSystem { get; set; }

        protected IUriResolver UriResolver
        {
            get { return Resolver.Resolve<IUriResolver>(); }
        }

        public T given_pipeline_contributor<T>() where T : class, IPipelineContributor
        {
            return given_pipeline_contributor<T>(null);
        }

        public T given_pipeline_contributor<T>(Func<T> constructor) where T : class, IPipelineContributor
        {
            Pipeline = new SinglePipeline<T>(constructor, Resolver, _actions);
            Pipeline.Contributors[0].Initialize(Pipeline);
            return (T)Pipeline.Contributors[0];
        }

        public void given_registration_urimapping<T>(string uri)
        {
            UriResolver.AddUriMapping(uri, typeof(T).AssemblyQualifiedName, CultureInfo.CurrentCulture, null);
        }

        public void given_request_uri(string uri)
        {
            Context.Request.Uri = new Uri(uri);
        }

        public void then_contributor_is_executed()
        {
            IsContributorExecuted.ShouldBeTrue();
        }

        public PipelineContinuation when_sending_notification<TTrigger>()
        {
            IsContributorExecuted = _actions.ContainsKey(typeof(TTrigger));
            Result = _actions[typeof(TTrigger)](Context);
            return Result;
        }

        protected void given_pipeline_resourceKey<T1>()
        {
            Context.PipelineData.ResourceKey = typeof(T1).AssemblyQualifiedName;
        }


        protected void given_pipeline_selectedHandler<THandler>()
        {
            if (Context.PipelineData.SelectedHandlers == null)
                Context.PipelineData.SelectedHandlers = new List<IType>();

            Context.PipelineData.SelectedHandlers.Add(TypeSystem.FromClr<THandler>());
        }

        protected void given_pipeline_uriparams(NameValueCollection nameValueCollection)
        {
            if (Context.PipelineData.SelectedResource == null)
                Context.PipelineData.SelectedResource = new UriRegistration(null,null);
            Context.PipelineData.SelectedResource.UriTemplateParameters.Add(nameValueCollection);
        }

        protected void given_registration_codec<TCodec>()
        {
            CodecRegistration.FromCodecType(typeof(TCodec), TypeSystem).ForEach(x => Codecs.Add(x));
        }

        protected void given_registration_codec<TCodec, TResource>(string mediaTypes)
        {
            foreach (var contentType in MediaType.Parse(mediaTypes))
                Codecs.Add(CodecRegistration.FromResourceType(typeof(TResource),
                                                              typeof(TCodec),
                                                              TypeSystem,
                                                              contentType,
                                                              null,
                                                              null,
                                                              false));
        }

        protected void given_registration_handler<TResource, THandler>()
        {
            Resolver.Resolve<IHandlerRepository>().AddResourceHandler(typeof(TResource).AssemblyQualifiedName,
                                                                      TypeSystem.FromClr
                                                                          (typeof(THandler)));
        }

        protected void given_request_entity_body(byte[] bytes)
        {
            Request.Entity = new HttpEntity(new HttpHeaderDictionary(), new MemoryStream(bytes)) { ContentLength = bytes.Length };
        }

        protected void given_request_entity_body(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            Request.Entity = new HttpEntity(new HttpHeaderDictionary(), new MemoryStream(bytes)) { ContentLength = bytes.Length };
        }

        protected void given_request_header_accept(string p)
        {
            Context.Request.Headers["Accept"] = p;
        }

        protected void given_request_header_content_type(string mediaType)
        {
            if (mediaType == null)
                Context.Request.Entity.ContentType = null;
            else
                Context.Request.Entity.ContentType = new MediaType(mediaType);
        }

        protected void given_request_header_content_type(MediaType mediaType)
        {
            Context.Request.Entity.ContentType = mediaType;
        }

        protected void given_request_httpmethod(string method)
        {
            Context.Request.HttpMethod = method;
        }

        protected void given_response_entity(object responseEntity, Type codecType, string contentType)
        {
            Context.Response.Entity.ContentType = new MediaType(contentType);

            Context.PipelineData.ResponseCodec = CodecRegistration.FromResourceType(responseEntity == null ? typeof(object) : responseEntity.GetType(),
                                                                                    codecType,
                                                                                    TypeSystem,
                                                                                    new MediaType(contentType),
                                                                                    null,
                                                                                    null,
                                                                                    false);
            given_response_entity(responseEntity);
        }

        protected void given_response_entity(object responseEntity)
        {
            Context.Response.Entity.Instance = responseEntity;
            Context.OperationResult = new OperationResult.OK { ResponseResource = responseEntity };
        }

        protected void GivenAUser(string username, string password)
        {
            var provider = Resolver.Resolve<IAuthenticationProvider>() as InMemAuthenticationProvider;
            provider.Passwords[username] = password;
        }
        protected TestErrorCollector Errors { get; private set; }
        protected override void SetUp()
        {
            base.SetUp();
            Host = new InMemoryHost(null);
            Pipeline = null;
            _actions = new Dictionary<Type, Func<ICommunicationContext, PipelineContinuation>>();
            var manager = Host.HostManager;
            if (!Resolver.HasDependency(typeof(IAuthenticationProvider)))
                Resolver.AddDependency<IAuthenticationProvider, InMemAuthenticationProvider>();
            Resolver.AddDependencyInstance(typeof(IErrorCollector), Errors = new TestErrorCollector());
            
            manager.SetupCommunicationContext(Context = new InMemoryCommunicationContext());
            DependencyManager.SetResolver(Resolver);
        }

        protected override void TearDown()
        {
            base.TearDown();
            DependencyManager.UnsetResolver();
        }

        public class InMemAuthenticationProvider : IAuthenticationProvider
        {
            public Dictionary<string, string> Passwords = new Dictionary<string, string>();

            public Credentials GetByUsername(string username)
            {
                if (username == null || !Passwords.ContainsKey(username))
                    return null;
                return new Credentials
                {
                    Username = username,
                    Password = Passwords[username],
                    Roles = new string[0]
                };
            }
        }

        public class SinglePipeline<T> : IPipeline, IPipelineExecutionOrder, IPipelineExecutionOrderAnd where T : class, IPipelineContributor
        {
            internal Dictionary<Type, Func<ICommunicationContext, PipelineContinuation>> _actions;
            internal List<IPipelineContributor> _list;
            internal IDependencyResolver _resolver;
            Func<ICommunicationContext, PipelineContinuation> _lastNotification;

            public SinglePipeline(Func<T> creator,
                                  IDependencyResolver resolver,
                                  Dictionary<Type, Func<ICommunicationContext, PipelineContinuation>> actions)
            {
                ContextData = new PipelineData();
                _resolver = resolver;
                if (!_resolver.HasDependency(typeof(T)))
                    _resolver.AddDependency<T>();
                _list = new List<IPipelineContributor> { creator != null ? creator() : resolver.Resolve<T>() };
                _actions = actions;
            }

            public IPipelineExecutionOrder And
            {
                get { return this; }
            }

            public IEnumerable<ContributorCall> CallGraph
            {
                get
                {
                    foreach (var kv in _actions)
                        yield return new ContributorCall { Action = kv.Value, Target = _list[0] };
                }
            }

            public PipelineData ContextData { get; private set; }

            public IList<IPipelineContributor> Contributors
            {
                get { return _list; }
            }

            public bool IsInitialized { get; private set; }


            public void Initialize()
            {
                IsInitialized = true;
            }

            public IPipelineExecutionOrder Notify(Func<ICommunicationContext, PipelineContinuation> notification)
            {
                _lastNotification = notification;
                return this;
            }

            public void RegisterAsRenderStage(IPipelineContributor renderContributor)
            {
            }

            public void Run(ICommunicationContext context)
            {
            }

            public void RunCallGraph(ICommunicationContext context, bool renderNow)
            {
            }

            public IPipelineExecutionOrderAnd After(Type contributorType)
            {
                _actions[contributorType] = _lastNotification;
                return this;
            }

            public IPipelineExecutionOrderAnd Before(Type contributorType)
            {
                _actions[contributorType] = _lastNotification;
                return this;
            }
        }

        protected void given_request_uriName(string uriName)
        {
            if (Context.PipelineData.SelectedResource == null)
                Context.PipelineData.SelectedResource = new UriRegistration(null, null, uriName, null);
            else
            {
                var r = Context.PipelineData.SelectedResource;
                Context.PipelineData.SelectedResource = new UriRegistration(r.UriTemplate,r.ResourceKey,uriName,r.UriCulture);
            }
        }
    }

    public class TestErrorCollector : IErrorCollector
    {
        public IList<Error> Errors { get; private set; }

        public TestErrorCollector()
        {
            Errors = new List<Error>();
        }
        public void AddServerError(Error error)
        {
            Errors.Add(error);
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