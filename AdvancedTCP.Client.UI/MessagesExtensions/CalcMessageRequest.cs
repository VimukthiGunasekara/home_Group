﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedTCP.Client.UI.MessagesExtensions
{
    [Serializable]
    public class CalcMessageRequest : Shared.Messages.GenericRequest
    {
        public int A { get; set; }
        public int B { get; set; }
    }
}
