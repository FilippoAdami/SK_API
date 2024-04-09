namespace SK_API{

    public class TranslatorRequestModel{
        public string JSON { get; set; }
        public string Language { get; set; }
        
        public TranslatorRequestModel(string json, string language)
        {
            JSON = json;
            Language = language;
        }
    }

}