using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chat = System.Net;

namespace ChatServer
{
    class ChatDictionary
    {
        public Dictionary<string, Chat.Sockets.TcpClient> ClientsByName = new Dictionary<string, Chat.Sockets.TcpClient>();
        public Dictionary<Chat.Sockets.TcpClient, string> ClientsByNumber = new Dictionary<Chat.Sockets.TcpClient, string>();
    }
}
