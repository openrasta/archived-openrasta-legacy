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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using OpenRasta.Collections.Specialized;
using OpenRasta.DI;
using OpenRasta.Diagnostics;
using OpenRasta.Pipeline.Diagnostics;
using OpenRasta.Web;

namespace OpenRasta.Pipeline
{
    public class PipelineRunner : IPipeline
    {
        readonly IList<IPipelineContributor> _contributors = new List<IPipelineContributor>();
        readonly ICollection<Notification> _notificationRegistrations = new List<Notification>();


        readonly IDependencyResolver _resolver;
        IEnumerable<ContributorCall> _callGraph;

        public PipelineRunner(IDependencyResolver resolver)
        {
            Contributors = new ReadOnlyCollection<IPipelineContributor>(_contributors);

            _resolver = resolver;
            PipelineLog = NullLogger<PipelineLogSource>.Instance;
            Log = NullLogger.Instance;
        }

        public IList<IPipelineContributor> Contributors { get; private set; }

        public bool IsInitialized { get; private set; }
        public ILogger<PipelineLogSource> PipelineLog { get; set; }
        public ILogger Log { get; set; }

        IEnumerable<ContributorCall> IPipeline.CallGraph
        {
            get { return _callGraph; }
        }
        void CheckPipelineIsInitialized()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("The pipeline has not been initialized and cannot run.");
        }
        public void Initialize()
        {
            if (IsInitialized)
                return;
            using (PipelineLog.Operation(this, "Initializing the pipeline."))
            {
                foreach (var item in _resolver.ResolveAll<IPipelineContributor>())
                {
                    PipelineLog.WriteDebug("Initialized contributor {0}.", item.GetType().Name);
                    _contributors.Add(item);
                }
                _callGraph = GenerateCallGraph();
            }
            IsInitialized = true;
            PipelineLog.WriteInfo("Pipeline has been successfully initialized.");
        }

        public IPipelineExecutionOrder Notify(Func<ICommunicationContext, PipelineContinuation> action)
        {
            if (IsInitialized)
            {
                PipelineLog.WriteWarning("A pipeline registration through Notify() has been done after the pipeline was initialized. Ignoring.");
                return new Notification(this,action);
            }
            var notification = new Notification(this, action);
            _notificationRegistrations.Add(notification);

            return notification;
        }


        public void Run(ICommunicationContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            CheckPipelineIsInitialized();

            if (context.PipelineData.PipelineStage == null)
                context.PipelineData.PipelineStage = new PipelineStage(this);
            RunCallGraph(context, context.PipelineData.PipelineStage);
        }
        void RunCallGraph(ICommunicationContext context, PipelineStage stage)
        {
            lock (stage)
            {
                foreach (var contrib in stage)
                {
                    if (!CanBeExecuted(contrib))
                        continue;
                    stage.CurrentState = ExecuteContributor(context, contrib);
                    switch (stage.CurrentState)
                    {
                        case PipelineContinuation.Abort:
                            AbortPipeline(context);
                            goto case PipelineContinuation.RenderNow;
                        case PipelineContinuation.RenderNow:
                            RenderNow(context, stage);
                            break;
                        case PipelineContinuation.Finished:
                            FinishPipeline(context);
                            return;

                    }
                }
            }
        }
        void RenderNow(ICommunicationContext context, PipelineStage stage)
        {
            PipelineLog.WriteDebug("Pipeline is in RenderNow mode.");
            if (!stage.ResumeFrom<KnownStages.IOperationResultInvocation>())
            {
                if (stage.OwnerStage != null)
                {
                    PipelineLog.WriteError("Trying to launch nested pipeline to render error failed.");
                    AttemptCatastrophicErrorNotification(context);
                    return;
                }
                using (PipelineLog.Operation(this, "Rendering contributor has already been executed. Calling a nested pipeline to render the error."))
                {
                    var nestedPipeline = new PipelineStage(this, stage);
                    if (!nestedPipeline.ResumeFrom<KnownStages.IOperationResultInvocation>())
                        throw new InvalidOperationException("Could not find an IOperationResultInvocation in the new pipeline.");
                    RunCallGraph(context, nestedPipeline);
                }
            }
        }

        static void AttemptCatastrophicErrorNotification(ICommunicationContext context)
        {
            try
            {
                string fatalError = "An error in one of the rendering components of OpenRasta prevents the error message from being sent back.";
                context.Response.StatusCode = 500;
                context.Response.Entity.ContentLength = fatalError.Length;
                context.Response.Entity.Stream.Write(Encoding.ASCII.GetBytes(fatalError), 0, fatalError.Length);
                context.Response.WriteHeaders();
            }
            catch
            {}
    }

        bool CanBeExecuted(ContributorCall call)
        {
            if (call.Action == null)
            {
                PipelineLog.WriteWarning("Contributor call for {0} had a null Action.", call.ContributorTypeName);
                return false;
            }
            return true;
        }

        protected virtual void AbortPipeline(ICommunicationContext context)
        {
            PipelineLog.WriteError("Aborting the pipeline and rendering the errors.");
            context.OperationResult = new OperationResult.InternalServerError
            {
                Title =
                    "The request could not be processed because of a fatal error. See log below.",
                ResponseResource = context.ServerErrors
            };
            context.PipelineData.ResponseCodec = null;
            context.Response.Entity.Instance = context.ServerErrors;
            context.Response.Entity.Codec = null;
            context.Response.Entity.ContentLength = null;

            Log.WriteError("An error has occurred and the processing of the request has stopped.\r\n{0}", context.ServerErrors.Aggregate(string.Empty, (str, error) => str + "\r\n" + error.ToString()));
        }

        protected virtual PipelineContinuation ExecuteContributor(ICommunicationContext context, ContributorCall call)
        {
            using (PipelineLog.Operation(this, "Executing contributor {0}.{1}".With(call.ContributorTypeName, call.Action.Method.Name)))
            {
                PipelineContinuation nextStep;
                try
                {
                    nextStep = call.Action(context);
                }
                catch (Exception e)
                {
                    context.ServerErrors.Add(new Error
                    {
                        Title = "Fatal error",
                        Message = "An exception was thrown while processing a pipeline contributor",
                        Exception = e
                    });
                    nextStep = PipelineContinuation.Abort;
                }
                return nextStep;
            }
        }

        protected virtual void FinishPipeline(ICommunicationContext context)
        {
            PipelineLog.WriteInfo("Pipeline finished.");
        }


        IEnumerable<ContributorCall> GenerateCallGraph()
        {
            var bootstrapper = _contributors.OfType<KnownStages.IBegin>().Single();
            var tree = new DependencyTree<ContributorNotification>(
                new ContributorNotification(bootstrapper, new Notification(this, null)));

            foreach (var contrib in _contributors.Where(x=>x != bootstrapper))
            {
                _notificationRegistrations.Clear();
                using (PipelineLog.Operation(this, "Initializing contributor {0}.".With(contrib.GetType().Name)))
                    contrib.Initialize(this);
                foreach (var reg in _notificationRegistrations.DefaultIfEmpty(new Notification(this, null)))
                {
                    tree.CreateNode(new ContributorNotification(contrib, reg));
                }
            }
            foreach (var notificationNode in tree.Nodes)
            {
                foreach (var parentNode in GetCompatibleTypes(tree,
                                                              notificationNode,
                                                              notificationNode.Value.Notification.AfterTypes))
                    parentNode.ChildNodes.Add(notificationNode);
                foreach (var childNode in GetCompatibleTypes(tree,
                                                             notificationNode,
                                                             notificationNode.Value.Notification.BeforeTypes))
                    childNode.ParentNodes.Add(notificationNode);
            }
            var graph = tree.GetCallGraph().Select(x =>
                                                   new ContributorCall(x.Value.Contributor, x.Value.Notification.Target, x.Value.Notification.Description));
            LogContributorCallChainCreated(graph);
            return graph;
        }

        static IEnumerable<DependencyNode<ContributorNotification>> GetCompatibleTypes(DependencyTree<ContributorNotification> tree,
                                                                                DependencyNode<ContributorNotification> notificationNode,
                                                                                IEnumerable<Type> beforeTypes)
        {
            return from childType in beforeTypes
                   from compatibleNode in tree.Nodes
                   where compatibleNode != notificationNode
                         && childType.IsAssignableFrom(compatibleNode.Value.Contributor.GetType())
                   select compatibleNode;
        }
        IEnumerable<IPipelineContributor> GetContributorsOfType(Type contributorType)
        {
            return from contributor in _contributors
                   where contributorType.IsAssignableFrom(contributor.GetType())
                   select contributor;
        }

        void LogContributorCallChainCreated(IEnumerable<ContributorCall> callGraph)
        {
            PipelineLog.WriteInfo("Contributor call chain has been processed and results in the following pipeline:");
            int pos = 0;
            foreach (var contributor in callGraph)
                PipelineLog.WriteInfo("{0} {1}", pos++, contributor.ContributorTypeName);
        }

        void VerifyContributorIsRegistered(Type contributorType)
        {
            if (!GetContributorsOfType(contributorType).Any())
                throw new ArgumentOutOfRangeException("There is no registered contributor matching type "
                                                      + contributorType.FullName);
        }

        struct ContributorNotification
        {
            public readonly IPipelineContributor Contributor;
            public readonly Notification Notification;

            public ContributorNotification(IPipelineContributor contributor, Notification notification)
            {
                Notification = notification;
                Contributor = contributor;
            }
        }

        class Notification : IPipelineExecutionOrder, IPipelineExecutionOrderAnd
        {
            readonly ICollection<Type> _after = new List<Type>();
            readonly ICollection<Type> _before = new List<Type>();
            readonly PipelineRunner _runner;

            public Notification(PipelineRunner runner, Func<ICommunicationContext, PipelineContinuation> action)
            {
                _runner = runner;
                Target = action;
            }

            public ICollection<Type> AfterTypes
            {
                get { return _after; }
            }

            public IPipelineExecutionOrder And
            {
                get { return this; }
            }

            public ICollection<Type> BeforeTypes
            {
                get { return _before; }
            }

            public string Description
            {
                get { return Target != null && Target.Target != null ? Target.Target.GetType().Name : null; }
            }

            public Func<ICommunicationContext, PipelineContinuation> Target { get; private set; }

            public IPipelineExecutionOrderAnd After(Type contributorType)
            {
                _runner.VerifyContributorIsRegistered(contributorType);
                AfterTypes.Add(contributorType);
                return this;
            }

            public IPipelineExecutionOrderAnd Before(Type contributorType)
            {
                _runner.VerifyContributorIsRegistered(contributorType);
                BeforeTypes.Add(contributorType);
                return this;
            }
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
