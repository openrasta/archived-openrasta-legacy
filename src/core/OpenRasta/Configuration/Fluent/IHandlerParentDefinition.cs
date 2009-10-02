using System;
using OpenRasta.TypeSystem;

namespace OpenRasta.Configuration.Fluent
{
    public interface IHandlerParentDefinition
    {
        IHandlerForResourceWithUriDefinition HandledBy<T>();
        IHandlerForResourceWithUriDefinition HandledBy(Type type);
        IHandlerForResourceWithUriDefinition HandledBy(IType type);
    }
}