namespace OpenRasta.Binding
{
    /// <summary>
    /// Represents a key and associated values.
    /// </summary>
    public abstract class KeyedValues
    {
        /// <summary>
        /// The key for which a value is provided.
        /// </summary>
        public string Key { get; protected set; }

        /// <summary>
        /// Sets a value defining if the KeyedValue was used during the binding process.
        /// </summary>
        public bool WasUsed { get; protected set; }

        /// <summary>
        /// Tries to set the value on the provided object binder.
        /// </summary>
        /// <param name="binder">The <see cref="IObjectBinder"/> to use when applying the value.</param>
        /// <returns><c>true</c> if the assignment was successful, otherwise <c>false</c>.</returns>
        public abstract bool SetProperty(IObjectBinder binder);
    }
}