using System.Text.RegularExpressions;
namespace MudEngine.Library.System.PartOfSpeechTagging;

public partial class Tagger
{
    [GeneratedRegex(@"([0-9]*\.[0-9]+|[0-9]+)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex RegexNumeric();
    [GeneratedRegex(@"([\.\,\?\!])", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex RegexPunctuation();
    [GeneratedRegex(@"([ \t\n\r])", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex RegexWhiteSpace();
    public static List<PartOfSpeech> Tag(Corpus corpus, string arguments)
    {
        var partsOfSpeech = new List<PartOfSpeech>();
        try
        {
            var tokens = RegexNumeric().Split(arguments)
                .SelectMany(x => RegexWhiteSpace().Split(x))
                .SelectMany(x => RegexPunctuation().Split(x))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
            var ret = new string[tokens.Count];
            for (int i = 0, size = tokens.Count; i < size; i++)
            {
                if (!corpus.WordMap.TryGetValue(tokens[i], out var value) ||
                    !corpus.WordMap.ContainsKey(tokens[i].ToLower()))
                {
                    ret[i] = tokens[i].Length == 1 ? "^" : "NN";
                }
                else
                {
                    ret[i] = corpus.TagMap[value][0];
                }
            }
            var lastTag = string.Empty;
            for (var i = 0; i < tokens.Count; i++)
            {
                var token = ret[i];
                if (i > 0 && ret[i - 1] == "DT")
                {
                    if (token is "VBD" or "VBP" or "VB")
                    {
                        ret[i] = "NN";
                    }
                }
                if (ret[i].StartsWith('N') || ret[i].Equals("^"))
                {
                    if (tokens[i].IndexOf('.') > -1)
                    {
                        ret[i] = "^";
                    }
                    if (float.TryParse(tokens[i], out _))
                    {
                        ret[i] = "CD";
                    }
                }
                if (ret[i].StartsWith('N') && tokens[i].EndsWith("ed"))
                {
                    ret[i] = "VBN";
                }
                if (tokens[i].EndsWith("ly"))
                {
                    ret[i] = "RB";
                }
                if (ret[i].StartsWith("NN") && token.EndsWith("al"))
                {
                    ret[i] = "JJ";
                }
                if (i > 0 && ret[i].StartsWith("NN") &&
                    tokens[i - 1].Equals("would", StringComparison.OrdinalIgnoreCase))
                {
                    ret[i] = "VB";
                }
                if (ret[i].StartsWith("NN") && tokens[i].EndsWith("ing"))
                {
                    ret[i] = "VBG";
                }
                if (ret[i].StartsWith('N'))
                {
                    if (lastTag.StartsWith('N'))
                    {
                        partsOfSpeech[^1].Token += " " + tokens[i];
                    }
                    else
                    {
                        partsOfSpeech.Add(new PartOfSpeech(tokens[i], 1, ret[i]));
                    }
                    lastTag = ret[i];
                    continue;
                }
                if (ret[i].Equals("CD"))
                {
                    if (lastTag.StartsWith('N')
                        && int.TryParse(tokens[i], out var cardinal))
                    {
                        partsOfSpeech[^1].Index = cardinal;
                    }
                    continue;
                }
                partsOfSpeech.Add(new PartOfSpeech(tokens[i], 1, ret[i]));
                lastTag = ret[i];
            }
        }
        catch
        {
            // https://www.ling.upenn.edu/courses/Fall_2003/ling001/penn_treebank_pos.html
        }
        finally
        {
            if (partsOfSpeech.Count == 0)
            {
                partsOfSpeech.Add(new PartOfSpeech(arguments, 1, "NN"));
            }
        }
        return partsOfSpeech;
    }
}