using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Windows;
namespace FinalUi
{
    class ServerV
    {
        TcpListener serversocket;
        TcpListener clientsocket;
        int requestCount;
        public ServerV()
        {
            this.serversocket = new TcpListener(Configs.Default.Ports);
            TcpClient clientSocket = default(TcpClient);
            clientSocket = this.serversocket.AcceptTcpClient();
            Console.WriteLine(" >> Accept connection from client");
            MessageBox.Show("Accept connecion");
            requestCount = 0;

            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    byte[] bytesFrom = new byte[10025];
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    Console.WriteLine(" >> Data from client - " + dataFromClient);
                    string serverResponse = "Last Message from client" + dataFromClient;
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                    Console.WriteLine(" >> " + serverResponse);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public void  startConnection()
        {
             serversocket.Start();
        }
        public  void closeConnection()
        {
            this.serversocket.Stop();
        }
    }
}

