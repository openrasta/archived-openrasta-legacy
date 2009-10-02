using System;
using System.Collections.Generic;
using OpenRasta.Binding;

namespace OpenRasta.TypeSystem
{
    public interface IMember : IComparable<IMember>, IAttributeProvider
    {
        string Name { get; }
        string TypeName { get; }

        IProperty GetProperty(string propertyName);

        IProperty GetLocalIndexer(string parameter);
        IProperty GetLocalProperty(string name);

        IList<IMethod> GetMethods();
        IMethod GetMethod(string methodName);

        bool CanSetValue(object value);
        bool IsAssignableTo(IMember member);

        ITypeSystem TypeSystem { get; set; }

        IMemberBuilder CreateBuilder();

        /// <summary>
        /// Returns the type of the current member
        /// </summary>
        IType Type { get; }
        bool IsCollection { get; }
    }
}