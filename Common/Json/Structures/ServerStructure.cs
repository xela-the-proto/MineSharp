using Common.Enums;

namespace Common.Json.Structures;

public struct ServerStructure
{
    public Guid ServerGuid { get; set; }

    public string RootPath { get; set; }

    public string Version { get; set; }

    public ServerPlatform Platform { get; set; }
}