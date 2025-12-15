using Common.Enums;
using Common.Json;
using Common.Json.Structures;
using RestSharp;
using Serilog;

namespace Runner.DownloadManager;

public class DownloadDispatch
{
    public static string MCJARS = "https://mcjars.app/api/v2/";
    public static string SERVER_ROOT = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
        "QuickMCServers");

    public static void DownloadJar(string version, string path)
    {
        var client = new RestClient(MCJARS);
        var realPath = Path.Combine(SERVER_ROOT, path);
        var jarBuildManifestAddres = "/builds/" + ServerPlatform.VANILLA + "/" + $"{version}";
        var buildDownRequest = new RestRequest(jarBuildManifestAddres);

        Log.Information("Downloading main manifest");
        Log.Information(Path.Combine(SERVER_ROOT, "temp.json"));

        if (!Directory.Exists(realPath))
        {
            Directory.CreateDirectory(realPath);
        }
        else if (File.Exists(Path.Combine(realPath, "server.jar")))
        {
            Log.Information("server already exists");
            return;
        }

        var buildManifestFileInBytes = client.DownloadData(buildDownRequest);
        File.WriteAllBytesAsync(Path.Combine(realPath, "temp.json"), buildManifestFileInBytes ??
                                                                     throw new NullReferenceException(
                                                                         "Got empty array for build manifest!"));

        var structure =
            Deserializer.DeserializeObject<JarDownloadStructure>(Path.Combine(realPath, "temp.json"));

        var serverJar = client.DownloadDataAsync(new RestRequest(structure.builds.First().jarUrl)).Result;
        File.WriteAllBytesAsync(Path.Combine(realPath, "server.jar"), serverJar ??
                                                                      throw new NullReferenceException(
                                                                          "Got empty array for java file!"));
        File.WriteAllTextAsync(Path.Combine(realPath, "guid.txt"), Guid.NewGuid().ToString());
    }
}