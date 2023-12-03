using System.Reflection;
using System.Text;
using Newtonsoft.Json;
namespace MudEngine.Library.System.PartOfSpeechTagging;

public class Corpus
{
    public Corpus()
    {
        WordMap = LoadWordMap("WordMap.json");
        TagMap = LoadTagMap("TagMap.json");
    }
    public Dictionary<string, int> WordMap { get; }
    public List<List<string>> TagMap { get; }
    private static Stream? GetFileFromResource(string resourceName)
    {
        var resourceFileName = Assembly.GetExecutingAssembly().GetManifestResourceNames()
            .FirstOrDefault(file => file.EndsWith(resourceName));
        return string.IsNullOrWhiteSpace(resourceFileName)
            ? new MemoryStream(Array.Empty<byte>())
            : Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceFileName);
    }
    private static T? LoadJsonFile<T>(Stream? resourceData)
    {
        if (resourceData is null)
        {
            return default;
        }
        using var r = new StreamReader(resourceData, Encoding.Default);
        var json = r.ReadToEnd();
        return JsonConvert.DeserializeObject<T>(json);
    }
    private List<List<string>> LoadTagMap(string resourceName)
    {
        return LoadJsonFile<dynamic>(GetFileFromResource(resourceName))?
                   .TagMap
                   .ToObject<List<List<string>>>()
               ?? new List<List<string>>();
    }
    private Dictionary<string, int> LoadWordMap(string resourceName)
    {
        return LoadJsonFile<Dictionary<string, int>>(GetFileFromResource(resourceName))
               ?? new Dictionary<string, int>();
    }
}