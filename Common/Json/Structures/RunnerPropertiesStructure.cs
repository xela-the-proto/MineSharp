namespace Common.Json.Structures;

public struct RunnerPropertiesStructure
{
    public Guid ShardGuid { get; set; }
    public string ip { get; set; }
    public string token { get; set; }
    public string remote { get; set; }
}