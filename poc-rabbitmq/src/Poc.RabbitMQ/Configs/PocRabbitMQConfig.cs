
namespace Poc.RabbitMQ.Configs;

public sealed class PocRabbitMQConfig
{

    public string UserName { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public string VirtualHost { get; private set; } = "/";
    public string HostName { get; private set; }
    public int Port { get; private set; }
    public bool IsCredentialsProvided { get; private set; } = false;
    public bool IsSsl { get; private set; } = false;

    public Uri Uri { get; private set; }
    internal string ClientProvidedName { get; private set; } = string.Empty;
    public IDictionary<string, object> ClientProperties { get; private set; }
    public ushort RequestedChannelMax { get; private set; }
    public uint RequestedFrameMax { get; private set; }
    public TimeSpan RequestedHeartbeat { get; private set; }
    public bool UseBackgroundThreadsForIO { get; private set; }
    public int RetryCount { get; internal set; }
    public int RetryIntervalSeconds { get; internal set; }

    public uint PrefetchSizeQos { get; internal set; } = 0;
    public ushort PrefetchCountQos { get; internal set; } = 1;
    public bool GlobalQos { get; internal set; } = false;

    public Dictionary<string, PocRabbitMQQueueConfig> QueueConfig { get; internal set; } = null!;


    internal void SetCredentials(string username, string password)
    {
        UserName = username;
        Password = password;
        IsCredentialsProvided = true;
    }
    internal void SetServer(string hostName, int port, bool isSsl = false, int retryCount = 3, int retryIntervalSeconds = 3,
        string virtualHost = "/")
    {
        HostName = hostName;
        Port = port;
        IsSsl = isSsl;
        RetryCount = retryCount;
        RetryIntervalSeconds = retryIntervalSeconds;
        VirtualHost = virtualHost;
    }

    internal void SetQos(uint prefetchSizeQos, ushort prefetchCountQos, bool globalQos)
    {
        PrefetchSizeQos = prefetchSizeQos;
        PrefetchCountQos = prefetchCountQos;
        GlobalQos = globalQos;  
    }

    internal void SetServerParameter(ushort requestedChannelMax, uint requestedFrameMax, TimeSpan requestedHeartbeat,
                                     bool useBackgroundThreadsForIO)
    {
        RequestedChannelMax = requestedChannelMax;
        RequestedFrameMax = requestedFrameMax;
        RequestedHeartbeat = requestedHeartbeat;
        UseBackgroundThreadsForIO = useBackgroundThreadsForIO;
    }

    internal void SetClientProvidedName(string clientProvidedName)
    {
        ClientProvidedName = clientProvidedName;
    }

    internal void SetUri(Uri uri)
    {
        Uri = uri;
    }

    internal void SetQueueConfig(Dictionary<string, PocRabbitMQQueueConfig> queueConfig)
    {
        QueueConfig = queueConfig;
    }
}

public class PocRabbitMQQueueConfig
{
    public string Queue { get; set; }
    public string QueueFailed { get; set; }
}
