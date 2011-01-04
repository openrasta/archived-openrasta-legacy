namespace OpenRasta.TypeSystem.Surrogates
{
    /// <summary>
    /// Represents a builder, responsible to responding to surrogate building requests.
    /// </summary>
    public interface ISurrogateBuilder
    {
        /// <summary>
        /// Gets a value defining if this builder can provide a surrogate type for the provided member.
        /// </summary>
        /// <param name="member">The member for which a surrogate is requested.</param>
        /// <returns><c>true</c> if the builder can provide a surrogate, otherwise <c>false</c>.</returns>
        bool CanCreateFor(IMember member);

        /// <summary>
        /// Gets the surrogate type for a member.
        /// </summary>
        /// <param name="member">The member for which the surrogate type is to be provided.</param>
        /// <returns>The surrogated type.</returns>
        IType Create(IMember member);
    }
}