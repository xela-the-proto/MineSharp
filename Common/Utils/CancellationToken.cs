using Common.Enums;

namespace MineSharpAPI.Modules.Helpers;

//TODO: causes a stack overflow when subscribing
public class RichCancellationToken : CancellationTokenSource
{
    public string ExitReason { get; set; }
    public event EventHandler<CancellationEventArgs> Changed;
    
    protected void OnChanged(CancellationEventArgs e) {
        if (Changed != null)
        {
            Changed.Invoke(this, e);
        }
    }    
    
    public ServerStatus CurrentServerStatus
    {
        get { return CurrentServerStatus;}
        set
        {
            CurrentServerStatus = value;
            OnChanged(new CancellationEventArgs(this.ExitReason, this.Token,value));
        }
    }
}

public class CancellationEventArgs : EventArgs
{
    public CancellationEventArgs(string reason, CancellationToken token, ServerStatus currentServerStatus)
    {
        Reason = reason;
        Token = token;
        CurrentServerStatus = currentServerStatus;
    }

    public string Reason { get; set; }
    public CancellationToken Token { get; set; }
    public ServerStatus CurrentServerStatus { get; set; }
}