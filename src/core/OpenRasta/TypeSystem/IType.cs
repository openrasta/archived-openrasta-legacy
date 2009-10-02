using System;
using System.Collections.Generic;
using OpenRasta.Binding;
using OpenRasta.DI;

namespace OpenRasta.TypeSystem
{
    public interface IType : IMember
    {
        bool TryCreateInstance<T>(IEnumerable<T> values, ValueConverter<T> converter, out object result);
        object CreateInstance();
        new ITypeBuilder CreateBuilder();
    }
    public interface IResolverAwareType : IType
    {
        object CreateInstance(IDependencyResolver resolver);
    }
}