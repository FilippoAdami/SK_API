using System.Text.RegularExpressions;
using System.Text.Json;
using SK_API;
using Newtonsoft.Json;

public class MaterialAnalysis{
    public string Language { get; set; }
    public string MacroSubject { get; set; }
    public string Title { get; set; }
    public TextLevel PerceivedDifficulty { get; set; }
    public List<(string Topic, TypeOfAssignment type, string Explanation)> MainTopics { get; set; }

    //constructor with all the fields
    public MaterialAnalysis(string response)
    {
        dynamic json = JsonConvert.DeserializeObject(response);
        Console.WriteLine("Json: " + json);

        // Assign properties if present, otherwise set them to default values
        Language = json?["Language"] ?? "English";
        MacroSubject = json?["MacroSubject"] ?? "null";
        Title = json?["Title"] ?? "null";
        string perceivedDifficultyString = json?["PerceivedDifficulty"]?.ToString() ?? "high_school";
        TextLevel perceivedDifficulty;
        if (!Enum.TryParse(perceivedDifficultyString, true, out perceivedDifficulty))
        {
            perceivedDifficulty = TextLevel.high_school; // Default value if parsing fails
        }
        PerceivedDifficulty = perceivedDifficulty;
        MainTopics = new List<(string Topic, TypeOfAssignment Type, string Description)>();
        if (json?["MainTopics"] != null)
        {
            foreach (var topic in json["MainTopics"])
            {
                string topicName = topic?["Topic"] ?? "null";
                string typeString = topic?["Type"] ?? "theoretical";
                TypeOfAssignment type;
                Enum.TryParse(typeString, true, out type);
                string description = topic?["Description"] ?? "null";
                MainTopics.Add((topicName, type, description));
                Console.WriteLine("Found: Topic: " + topicName + " Type: " + type + " Description: " + description);
            }
        }
    }
    public string ToJson()
{
    var mainTopics = new List<Dictionary<string, object>>();

    foreach (var item in MainTopics)
    {
        var topicInfo = new Dictionary<string, object>
        {
            { "Topic", item.Topic },
            { "Type", item.type },
            { "Description", item.Explanation }
        };

        mainTopics.Add(topicInfo);
    }

    var obj = new
    {
        Language,
        MacroSubject,
        Title,
        PerceivedDifficulty,
        MainTopics = mainTopics
    };

    return JsonConvert.SerializeObject(obj);
}
}
