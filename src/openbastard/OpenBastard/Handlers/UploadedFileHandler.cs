using System;
using System.Collections.Generic;
using System.IO;
using OpenBastard.Resources;
using OpenRasta;
using OpenRasta.IO;
using OpenRasta.Web;

namespace OpenBastard.Handlers
{
    public class UploadedFileHandler
    {
        static readonly List<IFile> _files = new List<IFile>();

        public OperationResult Get(int id)
        {
            if (_files.Count < id)
                return new OperationResult.NotFound();
            var streamToSend = _files[id];
            streamToSend.OpenStream().Position = 0;

            
            return new OperationResult.OK(streamToSend);
        }

        /// <summary>
        /// Used to test that files sent as multipart/form-data in html forms get processed correctly
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public OperationResult Post(IEnumerable<IMultipartHttpEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (!entity.Headers.ContentDisposition.Disposition.EqualsOrdinalIgnoreCase("form-data"))
                    return new OperationResult.BadRequest { ResponseResource = "Sent a field that is not declared as form-data, cannot process" };
                return new OperationResult.SeeOther
                    {
                        RedirectLocation = typeof(UploadedFile).CreateUri(new { id = ReceiveStream(entity.ContentType, entity.Stream) })
                    };
            }
            return new OperationResult.BadRequest { ResponseResource = "Sent multiple files, cannot process the request" };
        }

        [HttpOperation(ForUriName = "iFile")]
        public OperationResult Post(IFile file)
        {
            return new OperationResult.SeeOther
                {
                    RedirectLocation = typeof(UploadedFile).CreateUri(new { id = ReceiveStream(file.ContentType, file.OpenStream()) })
                };
        }

        [HttpOperation(ForUriName = "complexType")]
        public OperationResult Post(UploadedFile uploadedFile)
        {
            return new OperationResult.SeeOther
                {
                    RedirectLocation = typeof(UploadedFile).CreateUri(new { id = ReceiveStream(uploadedFile.File.ContentType, uploadedFile.File.OpenStream()) })
                };
        }

        /// <summary>
        /// Used to test simple put requests using application/octet-stream
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public OperationResult Post(string fileName, Stream fileStream)
        {
            return new OperationResult.SeeOther
            {
                RedirectLocation = typeof(UploadedFile).CreateUri(new { id = ReceiveStream(MediaType.ApplicationOctetStream,fileStream) })
            };
        }

        int ReceiveStream(MediaType streamType, Stream stream)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            var file = new InMemoryFile(memoryStream) {ContentType = streamType};
            _files.Add(file);
            return _files.IndexOf(file);
        }
    }
}