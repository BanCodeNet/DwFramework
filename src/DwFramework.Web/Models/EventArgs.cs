namespace DwFramework.Web;

public sealed class OnConnectEventArgs : EventArgs
{

}

public sealed class OnCloceEventArgs : EventArgs
{

}

public sealed class OnSendEventArgs : EventArgs
{
    public byte[] Data { get; init; }
}

public sealed class OnReceiveEventArgs : EventArgs
{
    public byte[] Data { get; init; }
}

public sealed class OnErrorEventArgs : EventArgs
{
    public Exception Exception { get; init; }
}