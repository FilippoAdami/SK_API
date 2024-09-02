using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SK_API;

public class MaterialAnalysis
{
    public string Language { get; set; }
    public string MacroSubject { get; set; }
    public string Title { get; set; }
    public TextLevel PerceivedDifficulty { get; set; }
    public List<MainTopic> MainTopics { get; set; }

    // Constructor that directly deserializes the response
    public MaterialAnalysis(string response)
    {
        // Deserialize the JSON directly into the object
        var json = JsonConvert.DeserializeObject<MaterialAnalysis>(response);

        // If any of the properties are missing, fallback to default values
        Language = json?.Language ?? "English";
        MacroSubject = json?.MacroSubject ?? "Unknown Subject";
        Title = json?.Title ?? "Untitled Course";
        PerceivedDifficulty = json?.PerceivedDifficulty ?? TextLevel.high_school;
        MainTopics = json?.MainTopics ?? new List<MainTopic>();
    }

    // Parameterless constructor for manual object creation
    public MaterialAnalysis()
    {
        MainTopics = new List<MainTopic>();
    }

    public string BasicTopicsInfo()
    {
        string result = "";
        foreach (var topic in MainTopics)
        {
            result += topic.BasicInfo() + "\n";
        }
        return result;
    }

    public string TopicsFullInfo()
    {
        string result = "";
        foreach (var topic in MainTopics)
        {
            result += topic.ToString() + "\n";
        }
        return result;
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}


// Class to represent a Main Topic
public class MainTopic
{
    public string Topic { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public BloomLevel Bloom { get; set; }
    public string Start { get; set; }
    public List<string> Keywords { get; set; }

    public MainTopic()
    {
        Keywords = new List<string>();
    }

    public MainTopic(string topic, string type, string description, BloomLevel bloom, string start, List<string> keywords)
    {
        Topic = topic;
        Type = type;
        Description = description;
        Bloom = bloom;
        Start = start;
        Keywords = keywords ?? new List<string>();
    }

    public override string ToString()
    {
        return $"Topic: {Topic}, Type: {Type}, Description: {Description}, Bloom: {Bloom}, Start: {Start}, Keywords: {string.Join(", ", Keywords)};";
    }

    public string BasicInfo()
    {
        return $"Topic: {Topic}, Description: {Description}, Bloom Level: {Bloom}";
    }
}
