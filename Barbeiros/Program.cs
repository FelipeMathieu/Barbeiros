using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Barbeiros
{
    class Program
    {
        static void Main(string[] args)
        {
            Barbearia barbearia = new Barbearia();

            barbearia.execute(8);

            Console.ReadLine();
            return;
        }
    }
}
