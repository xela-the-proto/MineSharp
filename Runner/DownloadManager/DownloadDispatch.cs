using System.Net;
using System.Text.Json;
using Common.Enums;
using Common.Json;
using Common.Json.Structures;

namespace Runner.DownloadManager;

public class DownloadDispatch
{
    public static string MCJARS { get; } = $"https://mcjars.app/api/v2/builds/";

    public static void DownloadJar()
    {
      Console.WriteLine("Insert version");
      var version = Console.ReadLine();
      var client = new WebClient();
      var addres = MCJARS + ServerPlatform.VANILLA + $"/{version}";
      client.DownloadFile(addres,"temp.json");
      
      var structure = Deserializer.DeserializeObject<JarDownloadStructure>("C:\\Users\\thega\\RiderProjects\\MineSharp\\Runner\\bin\\Debug\\net9.0\\temp.json");
      
      client.DownloadFile(structure.builds.First().jarUrl,"server.jar");

    }
}