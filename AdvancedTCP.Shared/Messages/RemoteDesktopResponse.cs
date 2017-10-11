using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedTCP.Shared.Messages
{
    [Serializable]
    public class RemoteDesktopResponse : ResponseMessageBase
    {
        public RemoteDesktopResponse(RequestMessageBase request)
            : base(request)
        {
            DeleteCallbackAfterInvoke = false;
        }

        public MemoryStream FrameBytes { get; set; }
        public bool Cancel { get; set; }
    }
}
