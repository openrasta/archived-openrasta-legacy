namespace OpenRasta.Configuration.Fluent
{
    public interface IRepeatableDefinition<TParent> : INoIzObject
    {
        TParent And { get; }
    }
}