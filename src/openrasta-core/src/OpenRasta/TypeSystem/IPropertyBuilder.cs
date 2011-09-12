namespace OpenRasta.TypeSystem
{
    public interface IPropertyBuilder : IMemberBuilder
    {
        int IndexAtCreation { get; set; }
        IProperty Property { get;  }
        IMemberBuilder Owner { get;  }
    }
}