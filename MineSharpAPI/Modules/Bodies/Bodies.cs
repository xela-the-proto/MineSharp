using MineSharpAPI.Api;

namespace MineSharpAPI.Modules.Bodies;

/*
 * Record per le richieste di get put ecc. speigo in quei file il perchè
 */
public record LoginBody
{
    public string email { get; set; }
    public string password { get; set; }
    
}

public record RunnerBody
{
    public string version { get; set; }
    
    public string path  { get; set; }
    
    public int ram { get; set; }
}

public record WsMessage
{
    public string RunnerId { get; set; }
    public string Message { get; set; }
}
