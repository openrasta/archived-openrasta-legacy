using System;
using OpenRasta.TypeSystem;

namespace OpenRasta.Configuration.Fluent
{
    public interface IHandlerParentDefinition : INoIzObject
    {
        IHandlerForResourceWithUriDefinition HandledBy<T>();
        IHandlerForResourceWithUriDefinition HandledBy(Type type);
        IHandlerForResourceWithUriDefinition HandledBy(IType type);
    }
}