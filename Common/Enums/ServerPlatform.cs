namespace Common.Enums;

public class ServerPlatform
{
    private ServerPlatform(string platform)
    {
        Platform = platform;
    }

    //This exists only in case we need to get the string form with ToString
    public string Platform { get; }

    public static ServerPlatform VANILLA => new("VANILLA");
    public static ServerPlatform PAPER => new("PAPER");
    public static ServerPlatform SPIGOT => new("SPIGOT");
    public static ServerPlatform FABRIC => new("FABRIC");
    public static ServerPlatform FORGE => new("FORGE");

    public override string ToString()
    {
        return Platform;
    }

    public static ServerPlatform getPlatformFromString(string platform)
    {
        platform = platform.ToLower();

        switch (typeof(ServerPlatform))
        {
        }

        return FABRIC;
    }
}