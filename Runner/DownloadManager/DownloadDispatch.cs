using System.Net;
using System.Text.Json;
using Common.Enums;
using Common.Json;
using Common.Json.Structures;

namespace Runner.DownloadManager;

public class DownloadDispatch
{
    public static string MCJARS { get; } = $"https://mcjars.app/api/v2/builds/";

    public static void DownloadJar(string version, string path)
    {
      var client = new WebClient();
      var addres = MCJARS + ServerPlatform.VANILLA + $"/{version}";
      
      Console.WriteLine("Downloading main manifest");
      Console.WriteLine(path);
      client.DownloadFile(addres,path + "\\temp.json");
      
      
      var structure = Deserializer.DeserializeObject<JarDownloadStructure>(path + ".\\temp.json");
      Console.WriteLine("Downloading jar");
      client.DownloadFile(structure.builds.First().jarUrl,path + "\\server.jar");
      
    }
}