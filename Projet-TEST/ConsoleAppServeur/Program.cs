using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppServeur
{
    class Program
    {
        static void Main(string[] args)
        {
            Server serv = new Server(123);
            serv.start();

        }
    }
}
