public class AnalyserRequestModel{
    public required string Material { get; set; }

    public AnalyserRequestModel(string material)
    {
        Material = material;
    }
}
