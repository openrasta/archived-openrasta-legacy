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
using OpenRasta.Web;

namespace OpenRasta.IO
{
    public interface IFile
    {
        MediaType ContentType { get; }
        string FileName { get; }
        long Length { get; }
        Stream OpenStream();
    }
    [Obsolete("IReiceivedFile has been depreciated. Please use the IFile interface instead.")]
    public interface IReceivedFile : IFile
    {
        string OriginalName { get; }
    }
    public interface IDownloadableFile : IFile
    {
        DownloadableFileOptions Options { get; }
    }
    [Flags]
    public enum DownloadableFileOptions
    {
        Open,
        Save
    }
#pragma warning disable 0618
    public class InMemoryFile : IFile, IReceivedFile
    {
        public InMemoryFile() : this(new MemoryStream())
        {
        }

        public InMemoryFile(Stream stream)
        {
            _stream = stream;
            ContentType = MediaType.ApplicationOctetStream;
        }

        readonly Stream _stream;
        public MediaType ContentType { get; set; }
        public string FileName { get; set; }
        public long Length { get; set; }
        public Stream OpenStream()
        {
            _stream.Position = 0;
            return _stream;
        }

        string IReceivedFile.OriginalName
        {
            get { return FileName; }
        }
    }
#pragma warning restore 0618
    public class InMemoryDownloadableFile : InMemoryFile, IDownloadableFile
    {
        public InMemoryDownloadableFile()
        {
            Options = DownloadableFileOptions.Save | DownloadableFileOptions.Open;
        }
        public DownloadableFileOptions Options { get; set; }
    }
}

#region Full license

// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion