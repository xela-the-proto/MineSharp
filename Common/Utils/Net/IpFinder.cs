using System.Net;
using RestSharp;
using Serilog;

namespace Common.Utils.Net;

public class IpFinder
{
    public static string findMachinePublicIp()
    {
        Log.Verbose("finding machine public ip");
        var client = new RestClient("https://icanhazip.com/");
        var response = client.Get(new RestRequest());
        return response.Content.Replace(@"\n","") ?? throw new HttpRequestException("Couldn't contact icanhazip.com to get public ip!");
    }
}