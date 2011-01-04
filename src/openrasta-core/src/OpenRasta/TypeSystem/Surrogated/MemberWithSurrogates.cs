using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRasta.TypeSystem.Surrogated
{
    public abstract class MemberWithSurrogates : WrappedMember

    {
        readonly IMember _wrappedMember;

        protected MemberWithSurrogates(IMember wrappedMember, IEnumerable<IType> alienTypes)
            : base(wrappedMember)
        {
            _wrappedMember = wrappedMember;
            AlienTypes = alienTypes.Select(x => (IType)new AlienType(x, this)).ToList();
        }

        protected IEnumerable<IType> AlienTypes { get; set; }

        public override IProperty GetIndexer(string parameter)
        {
            return CachedProperty(parameter, ()=> AlienTypes.Select(x => Reroot(x.GetIndexer(parameter))).FirstOrDefault()
                           ?? base.GetIndexer(parameter));
        }

        public override IProperty GetProperty(string name)
        {
            return CachedProperty(name, ()=> AlienTypes.Select(x => Reroot(x.GetProperty(name))).FirstOrDefault()
                           ?? base.GetProperty(name));
        }
    }
}