using System.Collections.Generic;
using OpenBastard.Handlers;
using OpenBastard.Resources;
using OpenRasta.Configuration;

namespace OpenBastard
{
    public class Configurator : IConfigurationSource
    {
        public void Configure()
        {
            using (OpenRastaConfiguration.Manual)
            {
                ResourceSpace.Has.ResourcesOfType<Home>()
                    .AtUri("/")
                    .HandledBy<HomeHandler>()
                    .AsXmlSerializer();

                /* File upload resources */
                ResourceSpace.Has.ResourcesOfType<UploadedFile>()
                    .AtUri(Uris.FILES)
                    .And.AtUri(Uris.FILES_IFILE).Named("iFile")
                    .And.AtUri(Uris.FILES_COMPLEX_TYPE).Named("complexType")
                    .And
                    .AtUri("/files/{id}")
                    .And
                    .AtUri("/files/{fileName}")
                    .HandledBy<UploadedFileHandler>();

                ResourceSpace.Has.ResourcesOfType<IEnumerable<User>>()
                    .AtUri(Uris.USERS)
                    .HandledBy<UserListHandler>()
                    .AsXmlSerializer();

                ResourceSpace.Has.ResourcesOfType<User>()
                    .AtUri(Uris.USER)
                    .HandledBy<UserListHandler>()
                    .AsXmlSerializer();
            }
        }
    }

    public static class Uris
    {
        public const string FILES = "/files";
        public const string FILES_COMPLEX_TYPE = "/files/complexType";
        public const string FILES_IFILE = "/files/iFile";
        public const string HOME = "/";
        public const string USERS = "/users";
        public static string USER = "/users/{id}";
    }
}