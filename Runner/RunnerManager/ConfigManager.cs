using Common.Json.Structures;
using Common.Utils.Net;
using Newtonsoft.Json;

namespace Runner.RunnerManager;

public class ConfigManager
{
    public static RunnerPropertiesStructure WriteConfig(string file_root)
    {
        var properties = new RunnerPropertiesStructure
        {
            ip = IpFinder.findMachinePublicIp(),
            ShardGuid = Guid.NewGuid(),
            token = ""
        };
        File.WriteAllText(file_root + Path.DirectorySeparatorChar + "config.json", JsonConvert.SerializeObject(properties));
        
        return properties;
    }

    public static RunnerPropertiesStructure ReadConfig(string file_root)
    {
        return JsonConvert.DeserializeObject<RunnerPropertiesStructure>(
            File.ReadAllText(file_root + Path.DirectorySeparatorChar + "config.json"));
    }

}