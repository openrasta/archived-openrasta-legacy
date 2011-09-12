using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenRasta.Binding;
using OpenRasta.TypeSystem.Surrogates;

namespace OpenRasta.TypeSystem.Surrogated
{
    /// <summary>
    /// Represents a property defined on an alien object.
    /// </summary>
    [DebuggerDisplay("{global::OpenRasta.TypeSystem.DebuggerStrings.Property(_alienProperty)}")]
    public class AlienOwnedProperty : WrappedMember, IProperty
    {
        readonly IProperty _alienProperty;
        readonly IMember _alienType;

        public AlienOwnedProperty(IMember alienOwner, IMember nativeOwner, IProperty alienProperty)
            : base(alienProperty)
        {
            // the alienOwner is the original type we receive from the type system, before it's wrapped.
            // the nativeOwner is the original type we have a surrogate for, before it was wrapped.
            Owner = nativeOwner;
            _alienProperty = alienProperty;
            _alienType = alienOwner;
        }

        public bool CanWrite
        {
            get { return _alienProperty.CanWrite; }
        }

        public IMember Owner { get; private set; }

        public object[] PropertyParameters
        {
            get { return _alienProperty.PropertyParameters; }
        }

        public override IProperty GetIndexer(string parameter)
        {
            return new WrappedProperty(this, base.GetIndexer(parameter));
        }

        public override IProperty GetProperty(string name)
        {
            return new WrappedProperty(this, base.GetProperty(name));
        }

        public IPropertyBuilder CreateBuilder(IMemberBuilder parent)
        {
            return new AlienPropertyBuilder(_alienType, parent, _alienProperty);
        }

        public IEnumerable<IMember> GetCallStack()
        {
            IMember current = this;
            while (current != null)
            {
                yield return current;
                var currentIsProp = current as IProperty;
                if (currentIsProp != null)
                    current = currentIsProp.Owner;
                else
                    break;
            }
        }

        public object GetValue(object target)
        {
            return ExecuteOnTarget(target, x => _alienProperty.GetValue(x));
        }

        public bool TrySetValue(object target, object value)
        {
            return ExecuteOnTarget(target, x => _alienProperty.TrySetValue(x, value));
        }

        public bool TrySetValue<T>(object target, IEnumerable<T> values, ValueConverter<T> converter)
        {
            return ExecuteOnTarget(target, x => _alienProperty.TrySetValue(x, values, converter));
        }

        ISurrogate CreateAlien()
        {
            return (ISurrogate)_alienProperty.Owner.Type.CreateInstance();
        }

        TResult ExecuteOnTarget<TResult>(object target, Func<object, TResult> action)
        {
            // if the target is of the same type as the real type for this property,
            // then we need to instantiate a surrogate to apply the value
            if (Owner.TypeSystem.FromInstance(target).IsAssignableTo(Owner))
            {
                var surrogate = CreateAlien();
                surrogate.Value = target;
                return action(surrogate);
            }
            // if the target is the surrogate type, then we can execute straight on the
            // surrogate instance
            
            if (_alienProperty.Owner.TypeSystem.FromInstance(target).IsAssignableTo(_alienProperty.Owner.Type))
            {
                return action(target);
            }

            throw new InvalidOperationException();
        }
    }
}