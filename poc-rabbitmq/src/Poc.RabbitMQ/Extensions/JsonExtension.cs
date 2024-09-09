using System.Text;
using System.Text.Json;

namespace Poc.RabbitMQ.Extensions;

public static class JsonExtension
{
    public static string Serialize<T>(this T data, JsonSerializerOptions serializerOptions = null)
    {
        if (data is null) return null!;

        return JsonSerializer.Serialize(data, serializerOptions);
    }


    public static T? Deserialize<T>(string data)
    {
        if (String.IsNullOrEmpty(data))
            return default(T);

        return JsonSerializer.Deserialize<T>(data);
    }

    public static T? Deserialize<T>(string data, JsonSerializerOptions serializerOptions = null)
    {
        if (String.IsNullOrEmpty(data))
            return default(T);

        return JsonSerializer.Deserialize<T>(data, serializerOptions);
    }

    public static T? Deserialize<T>(byte[] data)
    {
        return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
    }

    public static T? Deserialize<T>(byte[] data, JsonSerializerOptions serializerOptions = null)
    {
        return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data), serializerOptions);
    }
}
