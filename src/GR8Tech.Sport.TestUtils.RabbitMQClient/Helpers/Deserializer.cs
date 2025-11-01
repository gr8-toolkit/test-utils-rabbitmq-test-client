using Newtonsoft.Json;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace GR8Tech.Sport.TestUtils.RabbitMQClient.Helpers;

public static class Deserializer
{
    public static bool IsMessageCanBeDeserialized(this string jsonString) => jsonString switch 
    { 
        "null" => false, 
        null => false,
        "" => false,
        _ => true
    };

    public static T GetDeserializedObject<T>(this string jsonString, bool throwOnError = true)
    {
        try
        {
            var settings = new JsonSerializerSettings();
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;
            settings.Error += OnError;
            var result = JsonConvert.DeserializeObject<T>(jsonString, settings);
            if (result == null) throw new ApplicationException("Unable to deserialize Json string to:" + typeof(T));

            return result;
        }
        catch (Exception e)
        {
            if (!throwOnError) return default;

            throw new ApplicationException($"Unable to deserialize JSON string {typeof(T)}. Exception {e}");
        }
    }

    private static void OnError(object sender, ErrorEventArgs e) => Console.WriteLine(e.ErrorContext.Error);
}