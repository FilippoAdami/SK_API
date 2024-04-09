using SK_API;

public class SummarizerRequestModel{
    public required string Material { get; set; }
    public required int NumberOfWords { get; set; }
    public required TextLevel Level { get; set; }

    public SummarizerRequestModel(string material, int numberOfWords, TextLevel level)
    {
        Material = material;
        NumberOfWords = numberOfWords;
        Level = level;
    }
}