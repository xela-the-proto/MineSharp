using Common.Enums;
using MineSharpAPI.Api;

namespace MineSharpAPI.Modules.Bodies;

/// <summary>
///Login body for email and password only
/// <summary>
public record LoginBody
{
    public string email { get; set; }
    public string password { get; set; }
    
}
/// <summary>
/// Record that contains stuff to feed into the command line to start the server
/// </summary>
public record RunnerBody
{
    public string version { get; set; }
    
    public string path  { get; set; }
    
    public int ram { get; set; }
    
    public ServerPlatform  platform { get; set; }
}

public record APIKeyCreationBody
{
    public string keyName { get; set; }

    public string Key { get; set; }

    public string OwnerID { get; set; }
}
