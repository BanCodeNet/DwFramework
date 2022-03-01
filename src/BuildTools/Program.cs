using System.Xml;
using System.Diagnostics;

namespace BuildTools;

class Program
{
    static void Main(params string[] args)
    {
        var root = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent;
        var projects = Directory.GetDirectories(Path.Combine(root.FullName, "src"))
            .Select(item => new DirectoryInfo(item))
            .Where(item => item.Name.StartsWith("DwFramework"))
            .ToArray();
        if (projects.Length <= 0)
        {
            Console.WriteLine("‼️src目录中未找到可以构建的项目");
            return;
        }
        Console.WriteLine("开始构建Nuget包,请选择需要构建的项目:");
        var selected = Choose(projects.Select(item => item.Name).ToArray());
        var project = projects[selected];
        var projectFile = Path.Combine(project.FullName, $"{project.Name}.csproj");
        if (!File.Exists(projectFile))
        {
            Console.WriteLine("‼️项目文件不存在");
            return;
        }
        Console.Write($"是否为大版本构建(Y[y]/N[n]):");
        var isChangeBuildVersion = new[] { "Y", "y" }.Contains(Console.ReadLine());
        Console.Write($"使用后缀:");
        var suffix = Console.ReadLine();
        var version = "";
        try
        {
            var document = new XmlDocument();
            document.Load(projectFile);
            var nodeList = document.GetElementsByTagName("TargetFramework");
            if (nodeList.Count <= 0) throw new Exception();
            version = nodeList[0].InnerText.Replace("net", "");
            nodeList = document.GetElementsByTagName("Version");
            if (nodeList.Count <= 0) throw new Exception();
            var currentVersion = nodeList[0].InnerText.Split("-")[0].Split(".");
            if (currentVersion.Length < 4) currentVersion = new string[] { "0", "0", "0", "0" };
            if (!int.TryParse(currentVersion[2], out var releaseVersion) || !int.TryParse(currentVersion[3], out var buildVersion)) throw new Exception();
            version = $"{version}.{releaseVersion + (isChangeBuildVersion ? 1 : 0)}.{(isChangeBuildVersion ? 0 : buildVersion + 1)}{(!string.IsNullOrEmpty(suffix) ? $"-" : "")}{suffix}";
            nodeList[0].InnerText = version;
            document.Save(projectFile);
        }
        catch
        {
            Console.WriteLine("‼️修改项目文件失败");
            return;
        }
        try
        {
            var defaultOutputPath = Path.Combine(root.FullName, "nuget");
            Console.Write($"输出目录({defaultOutputPath}):");
            var outputPath = Console.ReadLine();
            using var process = Process.Start(new ProcessStartInfo("dotnet",
                $"pack -c Release -o {(string.IsNullOrEmpty(outputPath) ? defaultOutputPath : outputPath)} {projectFile}"
            )
            {
                RedirectStandardOutput = true
            });
            if (process == null) throw new Exception();
            Console.WriteLine("------------- 开始构建 --------------");
            //开始读取
            using (var sr = process.StandardOutput)
            {
                while (!sr.EndOfStream)
                {
                    Console.WriteLine(sr.ReadLine());
                }
            }
            Console.WriteLine("------------- 结束构建 --------------");
        }
        catch
        {
            Console.WriteLine("Nuget构建失败");
            return;
        }
    }

    private static int Choose(string[] options)
    {
        int selected = 0;
        bool done = false;
        while (!done)
        {
            for (int i = 0; i < options.Length; i++)
            {
                if (selected == i)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("> ");
                }
                else
                {
                    Console.Write("  ");
                }
                Console.WriteLine($"{i}. {options[i]}");
                Console.ResetColor();
            }
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow:
                    selected = Math.Max(0, selected - 1);
                    break;
                case ConsoleKey.DownArrow:
                    selected = Math.Min(options.Length - 1, selected + 1);
                    break;
                case ConsoleKey.Enter:
                    done = true;
                    break;
            }
            if (!done) Console.CursorTop = Console.CursorTop - options.Length;
        }
        return selected;
    }
}