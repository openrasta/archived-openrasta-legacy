#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion
using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OpenRasta.Collections;
using OpenRasta.Hosting.AspNet.Tests.Integration;
using OpenRasta.IO;

[SetUpFixture]
public class FileCopySetup
{
    public static DirectoryInfo TempFolder;
    [SetUp]
    public void CopyFiles()
    {
        TempFolder = PrepareFolderStructure();
        //Debugger.Launch();
    }

    static DirectoryInfo PrepareFolderStructure()
    {
        var assembly = typeof(aspnet_server_context).Assembly;
        var rootFolder = Path.GetDirectoryName(assembly.Location);

        var tempFolder = CreateTempFolder();

        var filesToCopy = Directory.GetFiles(rootFolder);
        foreach (var file in filesToCopy)
        {
            var source = file;
            var destination = Path.Combine(Path.Combine(tempFolder.FullName, "bin"), Path.GetFileName(source));

            Console.WriteLine("Copying " + file);
            File.Copy(source, destination);
        }
        using (var webConfig = assembly.GetManifestResourceStream("OpenRasta.Hosting.AspNet.Tests.Integration.Web.config"))
        {
            var content = webConfig.ReadToEnd();

            File.WriteAllBytes(Path.Combine(tempFolder.FullName, "web.config"), content);
        }

        return tempFolder;
    }
    static DirectoryInfo CreateTempFolder()
    {
        var tempFolder = Path.GetTempPath();
        string createdFolder;
        int count = 0;
        do
        {
            createdFolder = Path.Combine(tempFolder, "_ORTEST_" + count++);
        } while (Directory.Exists(createdFolder));
        Console.WriteLine("Creating {0}", createdFolder);
        var tempRoot = Directory.CreateDirectory(createdFolder);
        Console.WriteLine("Creating " + Directory.CreateDirectory(Path.Combine(createdFolder, "bin")).FullName);

        return tempRoot;
    }
    [TearDown]
    public void DeleteFiles()
    {
        try
        {
            TempFolder.GetDirectories()
                .SelectMany(subfolders => subfolders.GetFiles())
                .Concat(TempFolder.GetFiles())
                .ForEach(f =>
                {
                    try
                    {
                        f.Delete();
                    }
                    catch
                    {
                    }
                });

            TempFolder.Delete(true);
        }catch
        {
        }
    }
}

#region Full license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
