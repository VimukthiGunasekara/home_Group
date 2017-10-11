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
    public class GenericResponse : ResponseMessageBase
    {
        internal MemoryStream InnerMessage { get; set; }

        public GenericResponse(GenericRequest request)
            : base(request)
        {
            InnerMessage = new MemoryStream();
        }

        public GenericResponse(GenericResponse response)
            : this(new GenericRequest())
        {
            CallbackID = response.CallbackID;
            BinaryFormatter f = new BinaryFormatter();
            f.Serialize(InnerMessage, response);
            InnerMessage.Position = 0;
        }

        public GenericResponse ExtractInnerMessage()
        {
            BinaryFormatter f = new BinaryFormatter();
            f.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
            return f.Deserialize(InnerMessage) as GenericResponse;
        }
    }
}
