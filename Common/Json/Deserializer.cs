using Common.Json.Structures;
using Newtonsoft.Json;
using Serilog;

namespace Common.Json;

public class Deserializer
{
    public static T DeserializeObject<T>(string path)
    {
        var file = File.ReadAllText(path);
        
        JsonSerializer serializer = new JsonSerializer();
        
        
        T deserializedObject = (T)JsonConvert.DeserializeObject<T>(file) ?? throw new NullReferenceException();
        Console.WriteLine("Deserialized object " + path);
        Console.WriteLine("Deserialized object " + typeof(T));
        
        return deserializedObject;
        
    }
}