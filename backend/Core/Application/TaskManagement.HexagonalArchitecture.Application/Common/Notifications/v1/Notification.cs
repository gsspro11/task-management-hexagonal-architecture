using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TaskManagement.HexagonalArchitecture.Application.Common.Notifications.v1
{
    [Keyless]
    public class Notification
    {
        [JsonProperty(Order = -3)]
        public string Timestamp { get; }

        [JsonProperty(Order = -2)]
        public string Key { get; }

        [JsonProperty(Order = -1)]
        public string Message { get; }

        public Notification(string key, string message)
        {
            Key = key;
            Message = message;
            Timestamp = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
    }
}
