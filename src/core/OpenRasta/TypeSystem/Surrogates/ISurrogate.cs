namespace OpenRasta.TypeSystem.Surrogates
{
    /// <summary>
    /// Represents a surrogate instance
    /// </summary>
    public interface ISurrogate
    {
        /// <summary>
        /// Gets or sets the instance of the object being surrogated.
        /// </summary>
        object Value { get; set; }
    }
}