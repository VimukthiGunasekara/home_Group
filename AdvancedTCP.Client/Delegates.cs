using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedTCP.Client
{
    public class Delegates
    {
        public delegate void SessionRequestDelegate(Client client, EventArguments.SessionRequestEventArguments args);
        public delegate void FileUploadRequestDelegate(Client client, EventArguments.FileUploadRequestEventArguments args);
    }
}
