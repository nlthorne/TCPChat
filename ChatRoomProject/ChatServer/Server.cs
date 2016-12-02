using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;


namespace ChatRoomServer
{
    public class Server
    {

        public static Dictionary<string, TcpClient> chatUsers;
        public static Queue<string> messageQueue;
        public static BinaryTree userTree;

        public Server()
        {
            chatUsers = new Dictionary<string, TcpClient>();
            messageQueue = new Queue<string>();
            userTree = new BinaryTree();
        }
        public void RunServer()
        {
            TcpListener serverSocket = null;
            try
            {
                serverSocket = new TcpListener(IPAddress.Any, 9001);
                serverSocket.Start();
                WriteMessageToServer("Chat Server Initiated ...");

                Thread monitorConnections = new Thread(MonitorConnections);
                monitorConnections.Start();
                while (true)
                {
                    //Thread broadcastMessages = new Thread(() => BroadcastMessageQueue());
                    //broadcastMessages.Start();
                    TcpClient clientSocket = serverSocket.AcceptTcpClient();
                    Thread clientThread = new Thread(() => InitializeClient(clientSocket));
                    clientThread.Start();
                }
            }
            catch (Exception error)
            {
                WriteMessageToServer(error.ToString());
            }
            finally
            {
                if (serverSocket != null)
                {
                    serverSocket.Stop();
                }
                WriteMessageToServer("Chat Server shutting down.");
                Broadcast("System", "Chat Server shutting down.");
                Console.ReadLine();
            }

        }

        public void InitializeClient(TcpClient clientSocket)
        {
            try
            {
                byte[] bytesSent = new byte[4096];
                string userName = "";
                NetworkStream networkStream = clientSocket.GetStream();
                bytesSent = Encoding.ASCII.GetBytes("Welcome.\nYour first message will be used as your name.\nEnter 'exit' to close the application.");
                networkStream.Write(bytesSent, 0, bytesSent.Length);

                userName = GetUserName(networkStream);
                if (userName != "exit")
                {
                    MonitorClient client = new MonitorClient();
                    client.startClient(clientSocket, userName);
                    userTree.Insert(userName, clientSocket);
                    chatUsers.Add(userName, clientSocket);
                    SendWelcomeMessage(userName, networkStream);

                }

            }
            catch (Exception error)
            {
                WriteMessageToServer(error.ToString());
            }

        }

        public static void Broadcast(string userName, string message)
        {
            try
            {
                foreach (KeyValuePair<string, TcpClient> user in chatUsers)
                {
                    if (userName != user.Key)
                    {
                        byte[] bytesOut = null;
                        bytesOut = Encoding.ASCII.GetBytes(message);
                        NetworkStream broadcast = user.Value.GetStream();
                        broadcast.Write(bytesOut, 0, bytesOut.Length);
                        broadcast.Flush();
                    }

                }
                
            }
            catch (Exception error)
            {
                WriteMessageToServer(error.ToString());
            }

        }

        public static void BroadcastMessageQueue(string userName)
        {
            while (messageQueue.Count > 0)
            {
                byte[] bytesOut = System.Text.Encoding.ASCII.GetBytes(messageQueue.Dequeue());

                foreach (KeyValuePair<string, TcpClient> user in chatUsers)
                {
                    if (user.Key != userName)
                    {
                        NetworkStream broadcast = user.Value.GetStream();
                        broadcast.Write(bytesOut, 0, bytesOut.Length);
                        broadcast.Flush();
                    }

                }
            }
        }

        private void MonitorConnections()
        {
            while (true)
            {
                try
                {
                    if (userTree.Count() > 0)
                    {
                        foreach (Node node in userTree)
                        {
                            if (!node.tcpClient.Connected)
                            {
                                chatUsers.Remove(node.name);

                                //Broadcast("Server", node.name + " has been disconnected.");
                                messageQueue.Enqueue(node.name + " has been disconnected.");
                                BroadcastMessageQueue(node.name);
                            }
                        }

                    }
                    userTree = new BinaryTree();

                    foreach (KeyValuePair<string, TcpClient> user in chatUsers)
                    {
                        userTree.Insert(user.Key, user.Value);
                    }
                }
                catch (Exception error)
                {
                    WriteMessageToServer(error.ToString());
                }
            }

        }

        private string GetUserName(NetworkStream networkStream)
        {
            byte[] bytesFrom = new byte[4096];
            byte[] bytesSent = new byte[4096];
            networkStream.Read(bytesFrom, 0, bytesFrom.Length);
            string userName = Encoding.ASCII.GetString(bytesFrom);
            userName = userName.Substring(0, userName.IndexOf("\0"));
            if (userName != "exit")
            {
                while (chatUsers.ContainsKey(userName))
                {
                    bytesSent = Encoding.ASCII.GetBytes("Name already in use. Please enter a different name.");
                    networkStream.Write(bytesSent, 0, bytesSent.Length);
                    networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    userName = Encoding.ASCII.GetString(bytesFrom);
                    userName = userName.Substring(0, userName.IndexOf("\0"));
                }

            }
            return userName;
        }

        public static void WriteMessageToServer(string message)
        {
            Console.WriteLine(message);
        }

        private static void SendWelcomeMessage(string userName, NetworkStream networkStream)
        {
            byte[] bytesSent = new byte[4096];
            WriteMessageToServer(userName + " has joined.");
            bytesSent = Encoding.ASCII.GetBytes("Welcome, " + userName);
            networkStream.Write(bytesSent, 0, bytesSent.Length);
            DisplayCurrentUsers(networkStream);
            messageQueue.Enqueue(userName + " has joined.");
            BroadcastMessageQueue(userName);
        }

        private static void DisplayCurrentUsers(NetworkStream networkStream)
        {
            string message = "Users online: ";
            byte[] bytesSent = new byte[4096];

            message += string.Format("{0}", string.Join(", ", chatUsers.Keys));
            bytesSent = Encoding.ASCII.GetBytes(message);
            networkStream.Write(bytesSent, 0, bytesSent.Length);
        }
    }

}

