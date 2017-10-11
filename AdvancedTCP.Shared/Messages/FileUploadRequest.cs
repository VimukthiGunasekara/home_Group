using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedTCP.Shared.Messages
{
    [Serializable]
    public class FileUploadRequest : RequestMessageBase
    {
        public FileUploadRequest()
        {
            BufferSize = 1024;
        }

        public FileUploadRequest(FileUploadResponse response)
            : this()
        {
            CallbackID = response.CallbackID;
            FileName = response.FileName;
            TotalBytes = response.TotalBytes;
            CurrentPosition = response.CurrentPosition;
            SourceFilePath = response.SourceFilePath;
            DestinationFilePath = response.DestinationFilePath;
        }

        public String FileName { get; set; }
        public long TotalBytes { get; set; }
        public int CurrentPosition { get; set; }
        public String SourceFilePath { get; set; }
        public String DestinationFilePath { get; set; }
        public Byte[] BytesToWrite { get; set; }
        public int BufferSize { get; set; }
    }
}
