          ***********************************************
          *           OpenRasta source code             *
          *                                             *
          ***********************************************


The code in OpenRasta is split in various modules that can be built independently.

The latest description of each modules can be found at
          http://trac.caffeine-it.com/openrasta/wiki/Doc/Modules

OpenRasta is split in various modules that can be built independently.

Each module has its own solution file, and each module (except for core) requires the core
libraries to have been compiled through the build process.

You can trigger an initial build by using the make.bat file that is provided at the root of
the source tree.

core
  The Core module contains the core and portable functionality in OpenRasta.
aspnet
  The AspNet module contains the asp.net hosting and the WebForms codec.
castle
  The Castle module provides support for using Castle Windsor as a container
sharpview
  The SharpView module contains support for a new C# view engine that ships with OpenRasta.
install
  The Install module contains the code for the installer of OpenRasta.
vside
  The vside module contains the visual studio project and item templates.

