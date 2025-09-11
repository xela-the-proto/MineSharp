using RestSharp;

namespace Common.Utils.Net;

public class HttpClientSingleton
{
    private HttpClientSingleton(){}

    private static RestClient _instance;
    
    private static readonly object _lock = new object();
    
    public static RestClient GetInstance(string value)
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new RestClient();
                }
            }
        }
        return _instance;
    }
}