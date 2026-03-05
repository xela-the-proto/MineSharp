using Common.Json.Structures;
using Common.Utils.Net;
using MineSharpAPI.Modules.Api;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using Serilog;

namespace Runner.RunnerManager;

public class ConfigManager
{
    public static RunnerPropertiesStructure WriteConfig(string file_root)
    {
        var properties = new RunnerPropertiesStructure
        {
            ip = IpFinder.findMachinePublicIp(),
            ShardGuid = Guid.NewGuid(),
            token = "",
            remote = "INSERT HERE THE HTTP URL OF THE API"
        };
        File.WriteAllText(Path.Combine(file_root, "config.json"), JsonConvert.SerializeObject(properties));

        return properties;
    }

    public static RunnerPropertiesStructure ReadConfig(string file_root)
    {
        return JsonConvert.DeserializeObject<RunnerPropertiesStructure>(
            File.ReadAllText(file_root + Path.DirectorySeparatorChar + "config.json"));
    }

    public void RegisterRunner()
    {
        RestResponse returnValue = null;
        try
        {
            var retryPolicy = Policy.Handle<HttpRequestException>(e =>
            {
                Log.Warning("Registering runner, attemtping to reach API");
                return true;
            }).WaitAndRetry(10, retry =>
            {
                Log.Warning($"Couldnt reach api {retry}");
                if (returnValue != null)
                {
                    Log.Fatal(returnValue.StatusCode.ToString());
                }
                else
                {
                    Log.Warning("No response from api");
                }

                return TimeSpan.FromSeconds(5);
            });

            returnValue = retryPolicy.Execute(Executable);
        }
        catch (HttpRequestException e)
        {
            Log.Fatal("Couldn't register runner. Is the API running? tried connecting 10 times");
            Log.Fatal(returnValue.StatusCode.ToString());
            Thread.Sleep(2000);
        }
        catch (Exception e)
        {
            Log.Fatal(e.Message);
            throw;
        }
    }

    private RestResponse Executable()
    {
        using (var client = new RestClient(Program.RUNNER_PROPERTIES.remote))
        {
            var response = client.Put(new RestRequest("/api/auth/registerRunner").AddBody(new Runners()
            {
                Id = Program.RUNNER_PROPERTIES.ShardGuid.ToString(),
                OpenPorts = new List<int>(),
                PublicIp = Program.RUNNER_PROPERTIES.ip,
                ServerHardware = new List<string>()
            }).AddHeader("x-api-key", Program.RUNNER_PROPERTIES.token));
            return response;
        }
        
    }

    public void CheckArgs(string[] args)
    {
        foreach (var s in args)
        {
            switch (s)
            {
                case "-IgnoreAutoRegister":
                    Program.RUNNER_PROPERTIES.ignoreAutoRegistration = true;
                    break;
                default:
                    Program.RUNNER_PROPERTIES.ignoreAutoRegistration = false;
                    break;
            }
        }
    }
}