using Common.Json.Structures;
using Newtonsoft.Json;

namespace Common.Json;

public class Deserializer
{
    public static T DeserializeObject<T>(string path)
    {
        using (StreamReader file = File.OpenText(path))
        {
            JsonSerializer serializer = new JsonSerializer();
            
            T deserializedObject = (T)serializer.Deserialize(file, typeof(T)) ?? throw new NullReferenceException();

            return deserializedObject;
        }
    }
}