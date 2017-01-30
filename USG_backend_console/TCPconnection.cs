using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace USG_backend_console
{
    class TCPconnection
    {

        private String IPaddr;
        private int port;
        Socket s = null;

        public TCPconnection(String IP, int po)
        {
            this.IPaddr = IP;
            this.port = po;
            connect();
        }

        private void connect()
        {
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAdd = System.Net.IPAddress.Parse(this.IPaddr);
            IPEndPoint remoteEP = new IPEndPoint(ipAdd, this.port);

            s.Connect(remoteEP);
        }

        public void disconnect()
        {
            s.Disconnect(true);
            s.Close();
        }

        public void send(String msg)
        {
            byte[] byData = System.Text.Encoding.ASCII.GetBytes(msg);
            s.Send(byData);
        }


    }
}
