using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedTCP.Client.UI.MessagesExtensions
{
    [Serializable]
    public class CalcMessageResponse : Shared.Messages.GenericResponse
    {
        public CalcMessageResponse(CalcMessageRequest request)
            : base(request)
        {

        }

        public int Result { get; set; }
    }
}
