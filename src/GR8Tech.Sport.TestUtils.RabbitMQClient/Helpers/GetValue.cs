using System.Text;

namespace GR8Tech.Sport.TestUtils.RabbitMQClient.Helpers;

public static class TryGetValueHelper
{
    public static string GetValue(this IDictionary<string, object> headers, string key)
    {
        object returnValue;
        if(!headers.TryGetValue(key, out returnValue))
        {
            return string.Empty;
        }
        return Encoding.UTF8.GetString((byte[])returnValue);
    }
}