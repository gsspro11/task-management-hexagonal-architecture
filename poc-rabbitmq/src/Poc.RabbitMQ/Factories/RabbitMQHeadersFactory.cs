using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;

namespace Poc.RabbitMQ.Factories
{
    [ExcludeFromCodeCoverage]
    public static class RabbitMQHeadersFactory
    {
        public static IBasicProperties CreateSimpleHeader(string key, string value, IModel channel)
        {
            IBasicProperties props = channel.CreateBasicProperties();

            props.ContentType = "UTF8";
            props.DeliveryMode = 2;
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add(key, value);

            return props;
        }


        public static IBasicProperties CreateMultipleHeader(Dictionary<string, object> keyValuePairs, IModel channel)
        {
            IBasicProperties props = channel.CreateBasicProperties();

            props.ContentType = "UTF8";
            props.DeliveryMode = 2;
            props.Headers = keyValuePairs;
            return props;
        }
    }
}