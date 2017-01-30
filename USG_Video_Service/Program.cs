using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USG_Video_Service
{
    class Program
    {
        static void Main(string[] args)
        {
            VideoConnectionHandler vch = new VideoConnectionHandler(9050);
            vch.startHandler();
        }
    }
}
