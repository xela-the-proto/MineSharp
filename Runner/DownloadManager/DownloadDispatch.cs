using System.Globalization;
using System.Net;
using Common.Enums;
using Common.Json;
using Common.Json.Structures;
using Newtonsoft.Json;
using RestSharp;
using Serilog;

namespace Runner.DownloadManager;

public class DownloadDispatch
{
    public static string MCJARS = $"https://mcjars.app/api/v2/";


    public static void DownloadJar(string version, string path)
    {
      var client = new RestClient(MCJARS);
      
      string jarBuildManifestAddres = "/builds/" + ServerPlatform.VANILLA + "/" + $"{version}";
      RestRequest buildDownRequest = new RestRequest(jarBuildManifestAddres);
      
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
      var buildManifestFileInBytes = client.DownloadData(buildDownRequest);
      File.WriteAllBytesAsync(Program.ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "temp.json",buildManifestFileInBytes ?? 
          throw new NullReferenceException("Got empty array for build manifest!"));
      
      JarDownloadStructure structure = Deserializer.DeserializeObject<JarDownloadStructure>(Program.ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "temp.json");

      var serverJar = client.DownloadDataAsync(new RestRequest(structure.builds.First().jarUrl)).Result;
      File.WriteAllBytesAsync(Program.ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "server.jar", serverJar ?? 
          throw new NullReferenceException("Got empty array for java file!"));
    }
    
}