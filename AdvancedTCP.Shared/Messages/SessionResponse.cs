using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedTCP.Shared.Messages
{
    [Serializable]
    public class SessionResponse : ResponseMessageBase
    {
        public SessionResponse(RequestMessageBase request)
            : base(request)
        {

        }

        public bool IsConfirmed { get; set; }
        public String Email { get; set; }
    }
}
