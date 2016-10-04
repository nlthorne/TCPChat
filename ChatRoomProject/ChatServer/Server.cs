using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;
using Chat = System.Net;
using System.Collections;

namespace ChatServer
{
    class Server
    {
        System.Net.Sockets.TcpListener chatServer;
        ChatDictionary userDictionary = new ChatDictionary();
        Listener listener = new Listener();
        Writer writer = new Writer();
        Reader reader = new Reader();

        public void ConnectClients()
        {
            while (true)
            {
                listener.serverListener.Start();
                if (listener.serverListener.Pending())
                {
                    Chat.Sockets.TcpClient connectedClient = listener.serverListener.AcceptTcpClient();
                    ChatRoom chatRoom = new ChatRoom(connectedClient, userDictionary, this);
                }
            }
        }
    }
}
