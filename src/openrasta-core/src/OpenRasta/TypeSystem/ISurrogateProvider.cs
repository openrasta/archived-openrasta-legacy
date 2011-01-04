namespace OpenRasta.TypeSystem
{
    /// <summary>
    /// Represents a provider responsible for creating surrogates for members.
    /// </summary>
    public interface ISurrogateProvider
    {
        /// <summary>
        /// Tries to find a surrogate for the <see cref="IProperty"/> or <see cref="IType"/> defined in member.
        /// </summary>
        /// <typeparam name="T">Either an <see cref="IProperty"/> or an <see cref="IType"/>.</typeparam>
        /// <param name="member">An instance of T for which to get a surrogate.</param>
        /// <returns>The member surrogated, the original member if none were found, or <c>null</c> if an error occured.</returns>
        T FindSurrogate<T>(T member) where T : IMember;
    }
}