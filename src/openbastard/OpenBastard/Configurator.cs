using System;
using System.Collections.Generic;
using OpenBastard.Handlers;
using OpenBastard.Resources;
using OpenRasta.Configuration;
using OpenRasta.DI;
using OpenRasta.Security;

namespace OpenBastard
{
    public class Configurator : IConfigurationSource
    {
        public void Configure()
        {
            using (OpenRastaConfiguration.Manual)
            {
                ResourceSpace.Uses.CustomDependency<IAuthenticationProvider, StaticAuthenticationProvider>(DependencyLifetime.Singleton);
                ResourceSpace.Has.ResourcesOfType<Home>()
                    .AtUri("/")
                    .HandledBy<HomeHandler>()
                    .AsXmlSerializer();

                /* File upload resources */
                ResourceSpace.Has.ResourcesOfType<UploadedFile>()
                    .AtUri(Uris.FILES)
                    .And.AtUri(Uris.FILES_IFILE).Named("IFile")
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
                    .HandledBy<UserHandler>()
                    .AsXmlSerializer();
            }
        }
    }
}