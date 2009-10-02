namespace OpenRasta.Binding
{
    public abstract class KeyedValues
    {
        public string Key { get; protected set; }
        public bool WasUsed { get; protected set; }
        public abstract bool SetProperty(IObjectBinder binder);
    }
}