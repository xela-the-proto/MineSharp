using System.Net;
using Common.Enums;
using Common.Json;
using Common.Json.Structures;
using Newtonsoft.Json;
using Serilog;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Runner.DownloadManager;

public class DownloadDispatch
{
    public static string MCJARS = $"https://mcjars.app/api/v2/builds/";


    public static void DownloadJar(string version, string path)
    {
      var client = new WebClient();
      var addres = MCJARS + ServerPlatform.VANILLA + Path.DirectorySeparatorChar + $"{version}";
      
      Log.Information("Downloading main manifest");
      Log.Information(Program.ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "temp.json");
      
      if (!Directory.Exists(Program.ABSOLUTE_SERVER_PATH))
      {
          Directory.CreateDirectory(Program.ABSOLUTE_SERVER_PATH);
      }else if (File.Exists(path))
      {
          Log.Information("server already exists");
          return;
      }
      client.DownloadFile(addres,Program.ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "temp.json");
      
      JarDownloadStructure structure = Deserializer.DeserializeObject<JarDownloadStructure>(Program.ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "temp.json");
     
      client.DownloadFile(structure.builds.First().jarUrl,Program.ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "server.jar");
    }
    
}