namespace DwFramework.Core.CommandLine;

public static class ConsoleExtension
{
    public static int Choose(string[] options)
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