using System;
using System.Text;
using System.Threading;
using System.Net.Sockets;


namespace ChatRoomClient
{
    class Client
    {

        TcpClient clientStream = new TcpClient();
        NetworkStream serverStream = default(NetworkStream);

        public void RunClient()
        {
            try
            {
                clientStream.Connect("localhost", 9001);

                while (clientStream.Connected)
                {
                    Thread receiveMessage = new Thread(ReceiveMessage);
                    receiveMessage.Start();
                    serverStream = clientStream.GetStream();
                    string message = Console.ReadLine();

                    byte[] outMessage = Encoding.ASCII.GetBytes(message);
                    serverStream.Write(outMessage, 0, outMessage.Length);
                    serverStream.Flush();

                    if (message.ToLower() == "exit")
                    {
                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception)
            {
                DisplayMessage("Unable to connect to server.\nPlease restart client.");
                Console.ReadLine();


            }
            finally
            {
                if (clientStream != null)
                {
                    clientStream.Close();
                }

                if (serverStream != null)
                {
                    serverStream.Close();
                }
            }
        }


        private void ReceiveMessage()
        {

            try
            {
                while (true)
                {
                    byte[] bytesFrom = new byte[4096];
                    serverStream.Read(bytesFrom, 0, bytesFrom.Length);
                    string message = Encoding.ASCII.GetString(bytesFrom);
                    message = message.Substring(0, message.IndexOf("\0"));
                    DisplayMessage(message);
                }
            }
            catch (Exception)
            {
                DisplayMessage("Connection error detected. Please restart client.");

            }
        }

        private static void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }

    }
}

