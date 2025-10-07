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
        Log.Information(Path.Combine(path, "temp.json"));

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        else if (File.Exists(Path.Combine(path, "server.jar")))
        {
            Log.Information("server already exists");
            return;
        }

        var buildManifestFileInBytes = client.DownloadData(buildDownRequest);
        File.WriteAllBytesAsync(Path.Combine(path, "temp.json"), buildManifestFileInBytes ??
                                                                 throw new NullReferenceException(
                                                                     "Got empty array for build manifest!"));

        JarDownloadStructure structure =
            Deserializer.DeserializeObject<JarDownloadStructure>(Path.Combine(path, "temp.json"));

        var serverJar = client.DownloadDataAsync(new RestRequest(structure.builds.First().jarUrl)).Result;
        File.WriteAllBytesAsync(Path.Combine(path, "server.jar"), serverJar ??
                                                                  throw new NullReferenceException(
                                                                      "Got empty array for java file!"));
        File.WriteAllTextAsync(Path.Combine(path, "guid.txt"), Guid.NewGuid().ToString());
    }
}