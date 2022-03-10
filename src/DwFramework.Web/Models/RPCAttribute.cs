using System.IO;
namespace DwFramework.Web;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class RPCAttribute : Attribute { }