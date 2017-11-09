using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Barbeiros
{
    class Barbearia
    {
        private int numero_maximo_clientes = 0;
        private int contador_clientes = 0;

        private SemaphoreSlim mutex = new SemaphoreSlim(1, 1);
        private SemaphoreSlim cliente = new SemaphoreSlim(0);
        private SemaphoreSlim barbeiro = new SemaphoreSlim(0);
        private SemaphoreSlim clienteSatisfeito = new SemaphoreSlim(0);
        private SemaphoreSlim corteConcluido = new SemaphoreSlim(0);

        private int barbeiro_esperando = 0;
        private int cadeira = 0;
        private int aberta = 0;

        public void execute()
        {

        }

        public void processo_cliente()
        {
            this.mutex.Wait();

            if(this.contador_clientes == this.numero_maximo_clientes)
            {
                this.mutex.Release();
                return;
            }

            this.contador_clientes += 1;
            this.mutex.Release();

            this.cliente.Release();
            this.barbeiro.Release();

            this.corte_cabelo();

            this.clienteSatisfeito.Release();
            this.corteConcluido.Release();

            this.mutex.Wait();
            this.contador_clientes -= 1;
            this.mutex.Release();
        }

        public void processo_baribeiro()
        {
            while(true)
            {
                this.cliente.Wait();
                this.barbeiro.Release();

                this.cortar_cabelo();

                this.clienteSatisfeito.Wait();
                this.corteConcluido.Release();
            }
        }

        public void corte_cabelo()
        {
            //while(this.barbeiro_esperando == 0)
            //{

            //}
        }

        public void cortar_cabelo()
        {

        }
    }
}
