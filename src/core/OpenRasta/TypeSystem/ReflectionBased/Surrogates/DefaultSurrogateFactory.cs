using System;
using System.Collections.Generic;
using OpenRasta.DI;

namespace OpenRasta.TypeSystem.ReflectionBased.Surrogates
{
    public class DefaultSurrogateFactory : ISurrogateFactory
    {
        public IProperty FindSurrogate(IProperty property)
        {
            var reflectionProperty = property as ReflectionBasedProperty;
            if (reflectionProperty == null)
                return null;

            Type surrogateType = GetSurrogateType(reflectionProperty.TargetType);

            if (surrogateType != null)
                return new SurrogatedTypeReflectionProperty(surrogateType, reflectionProperty);
            return null;
        }

        public IType FindSurrogate(IType type)
        {
            return FindSurrogate<IType,ReflectionBasedType>(type, (surrogate, t) => new ReflectionBasedSurrogatedType(surrogate, t));
        }
        T FindSurrogate<T, TBase>(T type, Func<Type,TBase,T> newType)
            where TBase : class, T
            where T: class, IMember
        {
            var reflectionType = type as TBase;
            if (reflectionType == null) return null;
            if (type is INativeMember == false)
                return null;

            var nativeType = ((INativeMember)type).NativeType;

            var surrogateType = GetSurrogateType(nativeType);

            if (surrogateType != null)
                return newType(surrogateType, reflectionType);
            return null;
        }
        Type GetSurrogateType(Type targetType)
        {
            Type surrogateType = null;
            Type enumerableInterface = targetType.FindInterface(typeof(IList<>))
                                       ?? targetType.FindInterface(typeof(IEnumerable<>))
                                          ?? targetType.FindInterface(typeof(ICollection<>));

            if (enumerableInterface != null)
                surrogateType = typeof(ListIndexerSurrogate<>).MakeGenericType(enumerableInterface.GetGenericArguments()[0]);
            if (targetType == typeof(DateTime))
                surrogateType = typeof(DateTimeSurrogate);
            return surrogateType;
        }
    }
}