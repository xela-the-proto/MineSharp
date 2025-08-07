namespace Common.Enums;

public class ServerPlatform
{
    //This exists only in case we need to get the string form with ToString
    public string Platform { get; private set; }

    private ServerPlatform(string platform)
    {
        Platform  = platform;
    }
    
    public override string ToString()
    {
        return Platform;
    }
    
    public static ServerPlatform VANILLA => new("VANILLA");
    public static ServerPlatform PAPER => new("PAPER");
    public static ServerPlatform SPIGOT => new("SPIGOT");
    public static ServerPlatform FABRIC => new("FABRIC");
    public static ServerPlatform FORGE => new("FORGE");
    
   
}