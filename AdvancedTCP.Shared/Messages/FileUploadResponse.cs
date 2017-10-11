using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedTCP.Shared.Messages
{
    [Serializable]
    public class FileUploadResponse : ResponseMessageBase
    {
        public FileUploadResponse(FileUploadRequest request)
            : base(request)
        {
            FileName = request.FileName;
            TotalBytes = request.TotalBytes;
            CurrentPosition = request.CurrentPosition;
            SourceFilePath = request.SourceFilePath;
            DestinationFilePath = request.DestinationFilePath;
            DeleteCallbackAfterInvoke = false;
        }

        public String FileName { get; set; }
        public long TotalBytes { get; set; }
        public int CurrentPosition { get; set; }
        public String SourceFilePath { get; set; }
        public String DestinationFilePath { get; set; }
    }
}
