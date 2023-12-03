using System.Reflection;
using System.Text;
using Newtonsoft.Json;
namespace MudEngine.Library.System.PartOfSpeechTagging;

public class Corpus
{
    public Dictionary<string, int> WordMap { get; } = LoadResource<Dictionary<string, int>>("WordMap.json")
                                                      ?? new Dictionary<string, int>();
    public List<List<string>> TagMap { get; } = LoadResource<dynamic>("TagMap.json")
                                                    ?.TagMap.ToObject<List<List<string>>>()
                                                ?? new List<List<string>>();
    private static T? LoadResource<T>(string resourceName)
    {
        var resourceFileName = Assembly.GetExecutingAssembly().GetManifestResourceNames()
            .FirstOrDefault(file => file.EndsWith(resourceName));
        if (string.IsNullOrWhiteSpace(resourceFileName))
        {
            return default;
        }
        using var resourceData = string.IsNullOrWhiteSpace(resourceFileName)
            ? new MemoryStream(Array.Empty<byte>())
            : Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(resourceFileName);
        if (resourceData is null)
        {
            return default;
        }
        using var streamReader = new StreamReader(resourceData, Encoding.Default);
        var json = streamReader.ReadToEnd();
        return JsonConvert.DeserializeObject<T>(json);
    }
}