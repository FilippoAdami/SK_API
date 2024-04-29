namespace SK_API{
    public class AbstractNodeRequestModel{
        public string Language { get; set; }
        public string MacroSubject { get; set; }
        public string Title { get; set; }
        public TextLevel Level { get; set; }
        public string Correction { get; set; }
        public double Temperature { get; set; }
    
        public AbstractNodeRequestModel(string language, string macroSubject, string title, TextLevel level, string correction, double temperature){
            Language = language;
            MacroSubject = macroSubject;
            Title = title;
            Level = level;
            Correction = correction;
            Temperature = temperature;
        }
    }
}