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
using System.Linq;
using NUnit.Framework;
using OpenRasta;
using OpenRasta.DI;
using OpenRasta.Diagnostics;
using OpenRasta.Hosting.InMemory;
using OpenRasta.Pipeline.Contributors;
using OpenRasta.Pipeline.Diagnostics;
using OpenRasta.Testing;
using OpenRasta.Web;
using OpenRasta.Pipeline;

namespace PipelineRunner_Specification
{
    public class when_creating_the_pipeline : pipelinerunner_context
    {
        [Test]
        public void a_registered_contributor_gets_initialized_and_is_part_of_the_contributor_collection()
        {
            var pipeline = CreatePipeline(typeof(DummyContributor));
            pipeline.Contributors.OfType<DummyContributor>().FirstOrDefault()
                .ShouldNotBeNull();
        }

        public class DummyContributor : AfterContributor<KnownStages.IBegin>
        {
        }
    }

    public class when_accessing_the_contributors : pipelinerunner_context
    {
        [Test]
        public void the_contributor_list_always_contains_the_bootstrap_contributor()
        {
            var pipeline = CreatePipeline();
            pipeline.Contributors.OfType<KnownStages.IBegin>().FirstOrDefault()
                .ShouldNotBeNull();
        }

        [Test]
        public void the_contributor_list_is_read_only()
        {
            CreatePipeline().Contributors.IsReadOnly
                .ShouldBeTrue();
        }
    }

    // NOTE: Not sure if we really need those tests. They should be replaced by dependency analysis finding rootless elements
    // public class when_registering_for_pipeline_events : pipelinerunner_context
    // {
    // [Test]
    // public void registering_for_notification_before_the_bootstrap_results_in_an_error()
    // {
    // var pipeline = CreatePipeline();

    // Executing(() => pipeline.Notify(c=>PipelineContinuation.Continue).Before<KnownStages.IBegin>())
    // .ShouldThrow<InvalidOperationException>();
    // }

    // [Test]
    // public void the_type_on_which_notification_is_requested_must_have_been_registered_when_the_pipeline_was_created()
    // {
    // var pipeline = CreatePipeline();

    // Executing(() => pipeline.Notify(c => PipelineContinuation.Continue).After<KnownStages.IBegin>())
    // .ShouldThrow<ArgumentOutOfRangeException>();
    // }
    // }

    public class when_building_the_call_graph : pipelinerunner_context
    {
        [Test]
        public void a_second_contrib_registering_after_the_first_contrib_that_registers_after_the_boot_initializes_the_call_list_in_the_correct_order()
        {
            var pipeline = CreatePipeline(typeof(SecondIsAfterFirstContributor), 
                                          typeof(FirstIsAfterBootstrapContributor));
            pipeline.CallGraph.ShouldHaveSameElementsAs(new[]
            {
                typeof(BootstrapperContributor), 
                typeof(FirstIsAfterBootstrapContributor), 
                typeof(SecondIsAfterFirstContributor)
            }, 
                                                        (a, b) => a.Target.GetType() == b);
        }

        [Test]
        public void registering_all_the_contributors_results_in_a_correct_call_graph()
        {
            var pipeline = CreatePipeline(
                typeof(FirstIsAfterBootstrapContributor), 
                typeof(SecondIsAfterFirstContributor), 
                typeof(ThirdIsBeforeFirstContributor), 
                typeof(FourthIsAfterThridContributor));

            pipeline.CallGraph.ShouldHaveSameElementsAs(
                new[]
                {
                    typeof(BootstrapperContributor), 
                    typeof(ThirdIsBeforeFirstContributor), 
                    typeof(FourthIsAfterThridContributor), 
                    typeof(FirstIsAfterBootstrapContributor), 
                    typeof(SecondIsAfterFirstContributor)
                }, 
                (a, b) => a.Target.GetType() == b);
        }

        [Test]
        public void the_call_graph_cannot_be_recursive()
        {
            Executing(() => CreatePipeline(typeof(RecursiveA), typeof(RecursiveB)))
                .ShouldThrow<RecursionException>();
        }


        public class AfterAnyContributor : AfterContributor<IPipelineContributor>
        {
        }

        public class FirstIsAfterBootstrapContributor : AfterContributor<KnownStages.IBegin>
        {
        }

        public class FourthIsAfterThridContributor : AfterContributor<ThirdIsBeforeFirstContributor>
        {
        }

        public class RecursiveA : IPipelineContributor
        {
            public PipelineContinuation DoNothing(ICommunicationContext c)
            {
                return PipelineContinuation.Continue;
            }

            public void Initialize(IPipeline pipelineRunner)
            {
                pipelineRunner.Notify(DoNothing).After<KnownStages.IBegin>().And.After<RecursiveB>();
            }
        }

        public class RecursiveB : AfterContributor<RecursiveA>
        {
        }

        public class SecondIsAfterFirstContributor : AfterContributor<FirstIsAfterBootstrapContributor>
        {
        }

        public class SecondIsBeforeFirstContributor : BeforeContributor<FirstIsAfterBootstrapContributor>
        {
        }
        public class ThirdIsBeforeFirstContributor : BeforeContributor<FirstIsAfterBootstrapContributor>
        {
        }
    }

    public class when_executing_the_pipeline : pipelinerunner_context
    {
        [Test]
        public void contributors_get_executed()
        {
            var pipeline = CreatePipeline(typeof(WasCalledContributor));

            pipeline.Run(new InMemoryCommunicationContext());
            WasCalledContributor.WasCalled.ShouldBeTrue();
        }

        [Test]
        public void the_pipeline_must_have_been_initialized()
        {
            var pipeline = new PipelineRunner(new InternalDependencyResolver());
            Executing(() => pipeline.Run(new InMemoryCommunicationContext()))
                .ShouldThrow<InvalidOperationException>();
        }

        public class WasCalledContributor : IPipelineContributor
        {
            public static bool WasCalled;

            public PipelineContinuation Do(ICommunicationContext context)
            {
                WasCalled = true;
                return PipelineContinuation.Continue;
            }

            public void Initialize(IPipeline pipelineRunner)
            {
                pipelineRunner.Notify(Do).After<KnownStages.IBegin>();
            }
        }
    }

    public class pipelinerunner_context : context
    {
        protected IPipeline CreatePipeline(params Type[] contributorTypes)
        {
            var resolver = new InternalDependencyResolver();
            resolver.AddDependency<IPipelineContributor, BootstrapperContributor>();
            foreach (var type in contributorTypes)
                resolver.AddDependency(typeof(IPipelineContributor), type, DependencyLifetime.Singleton);
            var runner = new PipelineRunner(resolver) { PipelineLog = new TraceSourceLogger<PipelineLogSource>() };
            runner.Initialize();
            return runner;
        }
    }

    public class AfterContributor<T> : IPipelineContributor where T : IPipelineContributor
    {
        public PipelineContinuation DoNothing(ICommunicationContext c)
        {
            return PipelineContinuation.Continue;
        }

        public virtual void Initialize(IPipeline pipelineRunner)
        {
            pipelineRunner.Notify(DoNothing).After<T>();
        }
    }

    public class BeforeContributor<T> : IPipelineContributor where T : IPipelineContributor
    {
        public PipelineContinuation DoNothing(ICommunicationContext c)
        {
            return PipelineContinuation.Continue;
        }

        public virtual void Initialize(IPipeline pipelineRunner)
        {
            pipelineRunner.Notify(DoNothing).Before<T>();
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