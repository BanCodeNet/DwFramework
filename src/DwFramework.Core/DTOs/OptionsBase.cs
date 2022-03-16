using CommandLine;

namespace DwFramework.Core;

public abstract class OptionsBase
{
    [Option('e', "env", Default = "Development", HelpText = "运行环境")]
    public string? Environment { get; set; } = null;
}