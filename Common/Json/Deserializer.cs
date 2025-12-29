using Newtonsoft.Json;

namespace Common.Json;

public class Deserializer
{
    public static T DeserializeObject<T>(string path)
    {
        var fileStream = new StreamReader(new FileStream(path, FileMode.Open));
       
            var deserializedObj = (T)JsonConvert.DeserializeObject<T>(
                fileStream.ReadToEnd()) ?? throw new NullReferenceException();
            Console.WriteLine("Deserialized object " + path);
            Console.WriteLine("Deserialized object " + typeof(T));
        fileStream.Close();
        fileStream.Dispose();
        return deserializedObj;
        
    }
}