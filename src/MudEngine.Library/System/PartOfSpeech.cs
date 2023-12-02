namespace MudEngine.Library.System;

public class PartOfSpeech(string name, int index, string type)
{
    public string? Token { get; set; } = name;
    public int Index { get; set; } = index;
    public string Type { get; set; } = type;
}