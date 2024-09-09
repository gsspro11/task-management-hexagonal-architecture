using Poc.RabbitMQ.Configs;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.RabbitMQ
{
    internal class PocRabbitConnection : IDisposable
    {
        public PocRabbitMQConfig Config { get; set; }
        public IConnection Connection { get; set; }

        public void Dispose()
        {
            if (Connection != null && Connection.IsOpen)
                Connection.Close();

            Connection?.Dispose();
            Connection = null;
        }
    }
}
