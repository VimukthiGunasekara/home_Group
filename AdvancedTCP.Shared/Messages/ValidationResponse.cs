using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedTCP.Shared.Messages
{
    [Serializable]
    public class ValidationResponse : ResponseMessageBase
    {
        public ValidationResponse(RequestMessageBase request)
            : base(request)
        {

        }

        public bool IsValid { get; set; }
    }
}
