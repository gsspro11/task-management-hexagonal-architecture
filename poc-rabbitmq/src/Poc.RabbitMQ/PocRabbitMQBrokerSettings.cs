namespace Poc.RabbitMQ;

public class PocRabbitMQBrokerSettings
{
    public PocRabbitMQBrokerConfig[] Brokers { get; set; }
}

public class PocRabbitMQBrokerConfig
{
    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = "/";
    public string HostName { get; set; }
    public int Port { get; set; }
    public bool IsSsl { get; set; } = false;

    public Uri Uri { get; set; }
    public int RetryCount { get; set; }
    public int RetryIntervalSeconds { get; set; }


    public Dictionary<string, PocRabbitMQQueueSettings> Queues { get; set; } = null!;
}

public class PocRabbitMQQueueSettings
{
    public string Queue { get; private set; }
    public string QueueFailed { get; set; }
    public ushort MaxConcurrentMessages { get; private set; } = 1;

    public void SetQueue(string queue) => Queue = queue;
    public void SetQueueFailed(string queueFailed) => QueueFailed = queueFailed;
    public void SetMaxConcurrentMessages(ushort maxConcurrentMessages) => MaxConcurrentMessages = maxConcurrentMessages;
}
