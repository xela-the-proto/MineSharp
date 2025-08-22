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
      Program.ABSOLUTE_SERVER_PATH = path.Substring(0, path.LastIndexOf(@"\") + 1);
      var client = new WebClient();
      var addres = MCJARS + ServerPlatform.VANILLA + $"/{version}";
      
      Console.WriteLine("Downloading main manifest");
      Console.WriteLine(Program.ABSOLUTE_SERVER_PATH);
      
      if (!Directory.Exists(Program.ABSOLUTE_SERVER_PATH))
      {
          Directory.CreateDirectory(Program.ABSOLUTE_SERVER_PATH);
      }else if (File.Exists(path))
      {
          Console.WriteLine("server already exists");
          return;
      }
      client.DownloadFile(addres,Program.ABSOLUTE_SERVER_PATH + "\\temp.json");
      
      
      var structure = Deserializer.DeserializeObject<JarDownloadStructure>(Program.ABSOLUTE_SERVER_PATH + ".\\temp.json");
      Console.WriteLine("Downloading jar");
      client.DownloadFile(structure.builds.First().jarUrl,Program.ABSOLUTE_SERVER_PATH + "\\server.jar");
    }
}