using SK_API;

public class MaterialGeneratorRequestModel{
    public required string Topic { get; set; }
    public required int NumberOfWords { get; set; }
    public required TextLevel Level { get; set; }
    public required string LearningObjective { get; set; }

    public MaterialGeneratorRequestModel(string topic, int numberOfWords, TextLevel level, string learningObjective)
    {
        Topic = topic;
        NumberOfWords = numberOfWords;
        Level = level;
        LearningObjective = learningObjective;
    }
}