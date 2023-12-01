using System.Globalization;
using System.Text;
namespace MudEngine.HubServer.Processors;

public static class AsciiProcessor
{
    public static string Utf8ToAscii(this string line)
    {
        var normalizedString = line.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString.EnumerateRunes()
                     .Select(c => new {c, unicodeCategory = Rune.GetUnicodeCategory(c)})
                     .Where(t => t.unicodeCategory != UnicodeCategory.NonSpacingMark)
                     .Select(t => t.c))
        {
            stringBuilder.Append(c);
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}