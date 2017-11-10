using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Barbeiros
{
    class Barbearia
    {
        static int barbeiro = 0; //1 == barbeiro esperando cliente
        static int cadeira = 0; //1 == cliente sentado na cadeira esperando o corte
        static int aberta = 0; //porta aberta pelo barbeiro, mas o cliente ainda não saiu
        static int maxCliente = 2;
        public static int contCliente = 0;
        public static int numero_clientes = 0;

        static SemaphoreSlim mutex;
        static Object barbeiro_disponivel = new Object();
        static Object cadeira_ocupada = new Object();
        static Object porta_aberta = new Object();
        static Object cliente_saiu = new Object();

        public static bool fimProcessos = false;

        public Barbearia()
        {
            mutex = new SemaphoreSlim(1, 1);
        }

        public void execute(int nClientes)
        {
            numero_clientes = nClientes;
            barbeiro joao = new barbeiro();
            List<cliente> clientes = new List<cliente>();

            for (int i = 0; i < nClientes; i++)
            {
                clientes.Add(new cliente());
            }
            Thread b = new Thread(joao.execute);
            Thread[] c = new Thread[nClientes];

            Random rdm = new Random();

            b.Start();

            for (int i = 0; i < nClientes; i++)
            {
                c[i] = new Thread(clientes[i].execute);
                //Thread.Sleep(rdm.Next(0, 5000));
                c[i].Name = "Cliente " + i.ToString();
                c[i].Start();
            }
        }

        public void corte_cabelo(string cliente)
        {
            Console.WriteLine(cliente + " entrou na barbearia.");

            mutex.Wait();
            if (contCliente == maxCliente)
            {
                mutex.Release();
                Console.WriteLine(cliente + " saiu da barbearia, pois estava lotada"); //desistir
                return;
            }
            contCliente++;
            mutex.Release();

            while (barbeiro == 0)
            {
                lock (barbeiro_disponivel)
                {
                    Monitor.Pulse(barbeiro_disponivel);
                }
            }
            barbeiro -= 1;
            cadeira += 1;
            lock (cadeira_ocupada)
            {
                Monitor.Pulse(cadeira_ocupada);
            }
            while (aberta == 0)
            {
                lock (porta_aberta)
                {
                    Monitor.Wait(porta_aberta);
                }
            }
            aberta -= 1;
            lock (cliente_saiu)
            {
                Monitor.Pulse(cliente_saiu);
            }

            Console.WriteLine(cliente + " terminou o corte, e saiu da barbearia.");
        }

        public void pegue_proximo_cliente()
        {
            barbeiro += 1;
            lock (barbeiro_disponivel)
            {
                Monitor.Pulse(barbeiro_disponivel);
            }

            while (cadeira == 0)
            {
                lock (cadeira_ocupada)
                {
                    Monitor.Wait(cadeira_ocupada);
                }
            }
            cadeira -= 1;
        }

        public void termine_corte()
        {
            aberta += 1;
            lock (porta_aberta)
            {
                Monitor.Pulse(porta_aberta);
            }
            while (aberta > 0)
            {
                lock (cliente_saiu)
                {
                    Monitor.Wait(cliente_saiu);
                }
            }

            mutex.Wait();
            contCliente--;
            mutex.Release();

            Console.WriteLine("Corte completo");
        }

    }

    class cliente : Barbearia
    {
        public cliente() { }

        public void execute()
        {
            corte_cabelo(Thread.CurrentThread.Name.ToString());
        }

    }

    class barbeiro : Barbearia
    {
        public barbeiro() { }

        public void execute()
        {

            for (; ; )
            {
                pegue_proximo_cliente();
                this.cortar_cabelo();
                termine_corte();
            }

            Console.WriteLine("Barbeiro terminou...");
        }

        public void cortar_cabelo()
        {
            Random rdm = new Random();
            Thread.Sleep(rdm.Next(0, 5000));
        }
    }
}
