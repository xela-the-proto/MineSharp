using System.Net;
using System.Text.Json;
using Common.Enums;
using Common.Json;
using Common.Json.Structures;
using Serilog;

namespace Runner.DownloadManager;

public class DownloadDispatch
{
    public static string MCJARS = $"https://mcjars.app/api/v2/builds/";


    public static void DownloadJar(string version, string path)
    {
      var client = new WebClient();
      var addres = MCJARS + ServerPlatform.VANILLA + Path.DirectorySeparatorChar + $"{version}";
      
      Program.Log.Information("Downloading main manifest");
      Program.Log.Information(Program.ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "temp.json");
      
      if (!Directory.Exists(Program.ABSOLUTE_SERVER_PATH))
      {
          Directory.CreateDirectory(Program.ABSOLUTE_SERVER_PATH);
      }else if (File.Exists(path))
      {
          Program.Log.Information("server already exists");
          return;
      }
      client.DownloadFile(addres,Program.ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "temp.json");
      
      
      JarDownloadStructure structure = Deserializer.DeserializeObject<JarDownloadStructure>(Program.ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "temp.json");
      Console.WriteLine(structure.builds.Count);
      if (structure.builds.Count == 1)
      {
          Program.Log.Fatal("jar urls shown");
          Program.Log.Fatal(structure.builds.First().jarUrl);
      }
      else
      {
          Program.Log.Fatal("jar urls not shown");
          Program.Log.Fatal(structure.builds.First().jarUrl);
      }
      //client.DownloadFile(structure.builds.First().jarUrl,Program.ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "server.jar");
    }
}