namespace OpenRasta.Configuration.Fluent
{
    public interface IRepeatableDefinition<TParent>
    {
        TParent And { get; }
    }
}