using Newtonsoft.Json;

namespace Common.Json;

public class Deserializer
{
    public static T DeserializeObject<T>(string path)
    {
        var file = File.ReadAllText(path);
        

        var deserializedObject = (T)JsonConvert.DeserializeObject<T>(file) ?? throw new NullReferenceException();
        Console.WriteLine("Deserialized object " + path);
        Console.WriteLine("Deserialized object " + typeof(T));

        return deserializedObject;
    }
}