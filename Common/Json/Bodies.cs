using Common.Enums;

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
    /// <summary>
    ///Note to self for next times, DO NOT MAKE THIS UPPERCASE, we leverage reflection
    /// to get the names of the variables to then be able to match them to the vars
    /// </summary>
    public string version { get; set; }
    
    public string path  { get; set; }
    
    public int ram { get; set; }
    
    public string  platform { get; set; }
    
    public string remoteUrl { get; set; }
    public bool eulaAccept { get; set; }
}


public record Server()
{
    public string id { get; set; }
    
    public string name { get; set; }
    
    public ServerStatus status { get; set; }
}




















