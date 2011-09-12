using System;
using System.Collections;
using System.Collections.Generic;
using OpenRasta.Collections;

namespace OpenRasta.Pipeline
{
    public class PipelineStage : IEnumerable<ContributorCall>
    {
        public PipelineStage OwnerStage { get; set; }
        readonly ResumableIterator<ContributorCall, Type> _enumerator;

        public PipelineStage(IPipeline pipeline)
        {
            _enumerator = new ResumableIterator<ContributorCall, Type>(
                new List<ContributorCall>(pipeline.CallGraph).GetEnumerator(),
                x => x.Target != null ? x.Target.GetType() : null,
                (contributorType, key) => key != null && key.IsAssignableFrom(contributorType));
        }

        public PipelineStage(PipelineRunner pipeline, PipelineStage ownerStage)
            : this(pipeline)
        {
            OwnerStage = ownerStage;
        }

        public PipelineContinuation CurrentState { get; set; }

        public bool ResumeFrom<T>() where T:IPipelineContributor
        {
            
            return _enumerator.ResumeFrom(typeof(T));
        }
        public void SuspendAfter<T>() where T:IPipelineContributor
        {
            _enumerator.SuspendAfter(typeof(T));
        }
        public IEnumerator<ContributorCall> GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}