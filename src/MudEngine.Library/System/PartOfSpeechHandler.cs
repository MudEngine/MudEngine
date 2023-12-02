using MudEngine.Database.DataTransferObjects.System;
using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.Tokenize;
namespace MudEngine.Library.System;

public class PartOfSpeechHandler
{
    private const string _posModel = @"Resources\EnglishPOS.nbin";
    private EnglishRuleBasedTokenizer? _tokenizer;
    private EnglishMaximumEntropyPosTagger? _posTagger;
    public IEnumerable<PartOfSpeech> GetPartsOfSpeech(string arguments)
    {
        var partsOfSpeech = new List<PartOfSpeech>();
        try
        {
            _tokenizer ??= new EnglishRuleBasedTokenizer(true);
            var path = Path.GetDirectoryName(typeof(BaseCommand).Assembly.Location)!;
            _posTagger ??= new EnglishMaximumEntropyPosTagger(Path.Combine(path, _posModel));
            var tokens = _tokenizer.Tokenize(arguments);
            if (tokens.Length > 0)
            {
                var tags = _posTagger.Tag(tokens);
                if (tags.Length >= tokens.Length)
                {
                    var lastTag = false;
                    for (var index = 0; index < tags.Length; index++)
                    {
                        var tag = tags[index];
                        if (tag.StartsWith('N'))
                        {
                            if (lastTag)
                            {
                                partsOfSpeech[^1].Token += " " + tokens[index];
                            }
                            else
                            {
                                partsOfSpeech.Add(new PartOfSpeech(tokens[index], 1, "NN"));
                            }
                            lastTag = true;
                            continue;
                        }
                        if (tag.Equals("CD") && lastTag && int.TryParse(tokens[index], out var cardinal))
                        {
                            partsOfSpeech[^1].Index = cardinal;
                            continue;
                        }
                        partsOfSpeech.Add(new PartOfSpeech(tokens[index], 1, tag));
                        lastTag = false;
                    }
                }
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