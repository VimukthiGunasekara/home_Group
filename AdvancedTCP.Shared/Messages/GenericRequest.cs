using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedTCP.Shared.Messages
{
    [Serializable]
    public class GenericRequest : RequestMessageBase
    {
        internal MemoryStream InnerMessage { get; set; }

        public GenericRequest()
        {
            InnerMessage = new MemoryStream();
        }

        public GenericRequest(RequestMessageBase request)
            : this()
        {
            BinaryFormatter f = new BinaryFormatter();
            f.Serialize(InnerMessage, request);
            InnerMessage.Position = 0;
        }

        public GenericRequest ExtractInnerMessage()
        {
            BinaryFormatter f = new BinaryFormatter();
            f.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
            return f.Deserialize(InnerMessage) as GenericRequest;
        }
    }
}
