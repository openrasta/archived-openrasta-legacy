using OpenRasta.Binding;
using OpenRasta.TypeSystem;

namespace OpenRasta.OperationModel
{
    public class InputMember
    {
        public InputMember(IMember member, IObjectBinder binder, bool isOptional)
        {
            Member = member;
            Binder = binder;
            IsOptional = isOptional;
        }

        public IObjectBinder Binder { get; private set; }
        public bool IsOptional { get; private set; }

        public bool IsReadyForAssignment
        {
            get { return IsOptional || !Binder.IsEmpty; }
        }

        public IMember Member { get; private set; }
    }
}