using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Drawing;

namespace USG_Video_Service
{
    class VideoConnectionHandler
    {

        private Socket newsock = null;
        //private Boolean stopThread = false;
        private Socket client = null;
        private int port;

        public VideoConnectionHandler(int p)
        {
            this.port = p;
        }

        public void startHandler()
        {
            while (true)
            {
                startListening();
            }   
        }

        public void startListening()
        {
            ////////////////////////////////////////////

            Console.WriteLine("Video server is starting...");
            byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);

            if (newsock == null)
            {
                newsock = new Socket(AddressFamily.InterNetwork,
                                    SocketType.Stream, ProtocolType.Tcp);
            }

            if (newsock.IsBound == false && newsock != null)
            {
                newsock.Bind(ipep);
            }
            newsock.Listen(10);
            Console.WriteLine("Waiting for a video client...");

            client = newsock.Accept();
            IPEndPoint newclient = (IPEndPoint)client.RemoteEndPoint;
            Console.WriteLine("Video connected with {0} at port {1}",
                            newclient.Address, newclient.Port);
            int sent;

            while (SocketConnected(client) == true)
            {
                Bitmap bmp = TakeScreenshot();
                //Bitmap bmp = scrn.Clone(new System.Drawing.Rectangle(100, 100, 400, 400), scrn.PixelFormat);
                MemoryStream ms = new MemoryStream();
                // Save to memory using the Jpeg format
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                // read to end
                byte[] bmpBytes = ms.ToArray();
                bmp.Dispose();
                ms.Close();

                if (SocketConnected(client)) sent = SendVarData(client, bmpBytes);

                if (data.Length == 0)
                    newsock.Listen(10);             
            }
            Console.WriteLine("Disconnected from {0}", newclient.Address);
            //newsock.Shutdown(SocketShutdown.Both);
            //newsock.Disconnect(true);
            if (SocketConnected(client) == true)
            {
                client.Shutdown(SocketShutdown.Both);
                client.Disconnect(true);
            }
            //}

        }

        private static int SendVarData(Socket s, byte[] data)
        {
                int total = 0;
                int size = data.Length;
                int dataleft = size;
                int sent;

                byte[] datasize = new byte[4];
                datasize = BitConverter.GetBytes(size);
                if (SocketConnected(s)) sent = s.Send(datasize);

                while (total < size)
                {
                    if (SocketConnected(s)) sent = s.Send(data, total, dataleft, SocketFlags.None);
                    else sent = 0;
                    total += sent;
                    dataleft -= sent;
                }
                return total;
        }

        public static Bitmap TakeScreenshot()
        {
            System.Drawing.Rectangle totalSize = System.Drawing.Rectangle.Empty;


            //totalSize = System.Drawing.Rectangle.Union(totalSize, System.Windows.Forms.Screen.PrimaryScreen.Bounds);
            totalSize = System.Drawing.Rectangle.Union(totalSize, new Rectangle(new Point(0,0),new Size(400,400))); // w size zmieniamy rozmiar zrzutu ekranu

            Bitmap screenShotBMP = new Bitmap(totalSize.Width, totalSize.Height, System.Drawing.Imaging.PixelFormat.
                Format32bppArgb);

            Graphics screenShotGraphics = Graphics.FromImage(screenShotBMP);

            screenShotGraphics.CopyFromScreen(totalSize.X, totalSize.Y, -200, -30, totalSize.Size, CopyPixelOperation.SourceCopy);  // w 3. i 4. parametrze zmieniamy lokalizacje zrzutu

            screenShotGraphics.Dispose();

            return screenShotBMP;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found! - video service");
        }

        private static bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }

    }
}
