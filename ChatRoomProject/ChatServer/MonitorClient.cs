using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;


namespace ChatRoomServer
{
    public class MonitorClient
    {
        TcpClient clientSocket;
        string userName;

        public void startClient(TcpClient ClientSocket, string userName)
        {
            this.clientSocket = ClientSocket;
            this.userName = userName;

            Thread clientThread = new Thread(Communicate);
            clientThread.Start();
        }

        private void Communicate()
        {

            try
            {
                while (clientSocket.Connected)
                {
                    byte[] bytesFrom = new byte[4096];
                    string userInput = null;
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, bytesFrom.Length);

                    userInput = Encoding.ASCII.GetString(bytesFrom);

                    userInput = userInput.Substring(0, userInput.IndexOf("\0"));

                    if (userInput.ToLower() == "exit")
                    {
                        ProcessExitingClient(userName);
                        break;
                    }
                    else
                    {
                        userInput = "<" + userName + ">" + userInput;
                        Server.WriteMessageToServer(userInput);
                        Server.messageQueue.Enqueue(userInput);
                        Server.BroadcastMessageQueue(userName);
                        //Server.Broadcast(userName, userInput);
                    }

                }

            }

            catch (Exception error)
            {
                Server.WriteMessageToServer(error.ToString());
            }
            finally
            {
                if (clientSocket != null)
                {
                    clientSocket.Close();
                }

            }
        }

        private void ProcessExitingClient(string userName)
        {
            string message = userName + " has left.";

            Server.userTree.Delete(userName);
            Server.chatUsers.Remove(userName);

            Server.WriteMessageToServer(message);
            Server.messageQueue.Enqueue(message);
            Server.BroadcastMessageQueue(userName);
            //Server.Broadcast(userName, message);
        }

    }
}
