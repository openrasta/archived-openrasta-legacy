namespace OpenRasta.TypeSystem.Surrogated
{
    public abstract class AlienMember : WrappedMember
    {
        readonly IMember _realMember;

        public AlienMember(IMember alienMember, IMember realMember) : base(alienMember)
        {
            _realMember = realMember;
        }

        public override IProperty GetIndexer(string parameter)
        {
            return WrapProperty(base.GetIndexer(parameter));
        }

        public override IProperty GetProperty(string name)
        {
            return WrapProperty(base.GetProperty(name));
        }

        IProperty WrapProperty(IProperty result)
        {
            if (result == null) return null;
            if (!(result is AlienOwnedProperty))
                return new AlienOwnedProperty(this, _realMember, result);
            return result;
        }
    }
}