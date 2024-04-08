namespace SK_API{

    public class TranslatorRequestModel{
        public required string JSON { get; set; }
        public required string Language { get; set; }

        public TranslatorRequestModel(string json, string language)
        {
            JSON = json;
            Language = language;
        }
    }

}