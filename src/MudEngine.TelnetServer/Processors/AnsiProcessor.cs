using System.Text;
namespace MudEngine.TelnetServer.Processors;

public static class AnsiProcessor
{
    public static string ReplaceAnsiTags(this string data)
    {
        return new StringBuilder(data)
            .Replace("[RESET]", "\x1B[0m")
            .Replace("[BOLD]", "\x1B[1m")
            .Replace("[BLACK]", "\x1B[30m")
            .Replace("[RED]", "\x1B[31m")
            .Replace("[GREEN]", "\x1B[32m")
            .Replace("[YELLOW]", "\x1B[33m")
            .Replace("[BLUE]", "\x1B[34m")
            .Replace("[MAGENTA]", "\x1B[35m")
            .Replace("[CYAN]", "\x1B[36m")
            .Replace("[WHITE]", "\x1B[37m")
            .Replace("[BBLACK]", "\x1B[40m")
            .Replace("[BRED]", "\x1B[41m")
            .Replace("[BGREEN]", "\x1B[42m")
            .Replace("[BYELLOW]", "\x1B[43m")
            .Replace("[BBLUE]", "\x1B[44m")
            .Replace("[BMAGENTA]", "\x1B[45m")
            .Replace("[BCYAN]", "\x1B[46m")
            .Replace("[BWHITE]", "\x1B[47m")
            .Replace("[CR]", "\r\n")
            .ToString();
    }
}