using Common.Json.Structures;
using Newtonsoft.Json;
using Serilog;

namespace Common.Json;

public class Deserializer
{
    public static T DeserializeObject<T>(string path)
    {
        using (StreamReader file = File.OpenText(path))
        {
            JsonSerializer serializer = new JsonSerializer();
            
            
            T deserializedObject = (T)serializer.Deserialize(file, typeof(T)) ?? throw new NullReferenceException();
            Console.WriteLine("Deserialized object " + path);
            Console.WriteLine("Deserialized object " + typeof(T));
            
            return deserializedObject;
        }
    }
}