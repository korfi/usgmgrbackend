using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace USG_backend_console
{
    class ServerRedirector
    {

        private int port;
        Byte[] bytes = new Byte[256];
        AutomationHandler ah = new AutomationHandler();

        public ServerRedirector(int p)
        {
            this.port = p;
        }

        public void start()
        {
            IPAddress[] ipTab = Array.FindAll(
                Dns.GetHostEntry(string.Empty).AddressList,
                a => a.AddressFamily == AddressFamily.InterNetwork);
            IPAddress ipAdd = ipTab[0];
            TcpListener serverSocket = new TcpListener(ipAdd, this.port);
            TcpClient clientSocket = default(TcpClient);
            serverSocket.Start();
            while (true)
            {
                Console.Write("Waiting for a connection at " + ipAdd.ToString() + "\r\n");
                clientSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine("Connected!\r\n");

                int i;

                int commandCharSize = 4;
                NetworkStream stream = clientSocket.GetStream();
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    try {
                        Console.WriteLine("Bytes read");
                        // Translate data bytes to a ASCII string.
                        string dataLeft = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine(String.Format("Received: {0}", dataLeft));

                        int currDataPos = 0;
                        while (dataLeft.Length >= commandCharSize)
                        {
                            string currData = dataLeft.Substring(currDataPos, commandCharSize);
                            currDataPos += commandCharSize;
                            dataLeft = currData.Substring(currDataPos, dataLeft.Length-currDataPos);

                            ah.HandleStringCommand(currData, clientSocket.Client.RemoteEndPoint.ToString());

                            // Process the data sent by the client.
                            currData = currData.ToUpper();

                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(currData);

                            // Send back a response.
                            stream.Write(msg, 0, msg.Length);
                            Console.WriteLine(String.Format("Sent: {0}", currData));
                        }
                    }
                    catch (Exception ex) { Console.WriteLine("Couldn't process sending/receiving data: " + ex.Message); };
                }              

                clientSocket.Close();

            }
        }

    }
}
