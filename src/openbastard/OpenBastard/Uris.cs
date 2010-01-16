using System.Diagnostics.CodeAnalysis;

namespace OpenBastard
{
    public static class Uris
    {
        public const string FILES = "/files";
        public const string FILES_COMPLEX_TYPE = "/files/complexType";
        public const string FILES_IFILE = "/files/iFile";
        public const string HOME = "/";
        public const string USERS = "/users";
        public const string USER = "/users/{id}";
        public static string User(int id)
        {
            return "/users/" + id;
        }
    }
}