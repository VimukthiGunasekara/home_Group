using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


public static class TcpClientMethods
{
    public static String GetIP(this TcpClient client)
    {
        return ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
    }
}



