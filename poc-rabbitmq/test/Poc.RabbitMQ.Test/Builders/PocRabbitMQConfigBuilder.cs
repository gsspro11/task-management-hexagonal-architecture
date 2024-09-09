using Poc.RabbitMQ.Configs;

namespace Poc.RabbitMQ.Test.Builders
{
    public class PocRabbitMQConfigBuilder
    {
        private readonly PocRabbitMQConfig config;

        public PocRabbitMQConfigBuilder()
        {
            config = new PocRabbitMQConfig();
        }

        public PocRabbitMQConfigBuilder WithUri(Uri uri)
        {
            config.SetUri(uri);
            return this;
        }

        public PocRabbitMQConfigBuilder WithSetCredentials(string username, string password)
        {
            config.SetCredentials(username, password);
            return this;
        }

        public PocRabbitMQConfigBuilder WithSetServer(string? hostName, int port, bool isSsl = false, int retryCount = 3, int retryIntervalSeconds = 3)
        {
            config.SetServer(hostName, port);
            return this;
        }

        public PocRabbitMQConfigBuilder WithSetServerParameter(ushort requestedChannelMax, uint requestedFrameMax, TimeSpan requestedHeartbeat,
                                 bool useBackgroundThreadsForIO)
        {
            config.SetServerParameter(requestedChannelMax, requestedFrameMax, requestedHeartbeat, useBackgroundThreadsForIO);
            return this;
        }


        public PocRabbitMQConfig Build()
        {
            return config;
        }
    }
}
