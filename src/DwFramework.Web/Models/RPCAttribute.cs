using System.IO;
namespace DwFramework.Web;

[AttributeUsage(AttributeTargets.Class)]
public sealed class RPCAttribute : Attribute
{
    public RPCAttribute() { }
}