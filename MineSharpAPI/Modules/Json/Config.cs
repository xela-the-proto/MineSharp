using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Serilog;

namespace MineSharpAPI.Json;

public class ConfigJson
{
    public string ip { get; set; }
    public string port { get; set; }
    public string AuthPrivateKey { get; set; }
    public string AuthAudience { get; set; }
    public string AuthIssuer { get; set; }
}