using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

using System.IO;


namespace ChatRoomServer
{
    class Program
    {
        static void Main(string[] args)
        {

            Server server = new Server();
            server.RunServer();

        }
    }
}
