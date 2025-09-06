using Common.Json.Structures;
using Common.Utils.Net;
using Newtonsoft.Json;

namespace Runner.RunnerManager;

public class ConfigManager
{
    public static void WriteConfig()
    {
        var properties = new RunnerPropertiesStructure
        {
            ip = IpFinder.findLocalMachineIp(),
            ShardGuid = Guid.NewGuid()
        };
        File.WriteAllText("config.json", JsonConvert.SerializeObject(properties));

    }
    
    
}