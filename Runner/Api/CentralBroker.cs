using RestSharp;

namespace Runner.Api;

public class CentralBroker
{
    public static void UpdateServerStatus()
    {
        using (var client = new RestClient("http://localhost:5000/"))
        {
            
        }
    }
}