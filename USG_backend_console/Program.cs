using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USG_backend_console
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerRedirector sr = new ServerRedirector(13000);
            sr.start();
        }
    }
}
