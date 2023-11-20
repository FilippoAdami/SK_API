namespace SK_API{
    public class QuestionExcerciseRequestModel{
        public string Language { get; set; }
        public string Text { get; set; }
        public TypeOfQuestion Type { get; set; }
        public TextLevel Level { get; set; }
        public QuestionCategory Category { get; set; }
        public double Temperature { get; set; }

        public QuestionExcerciseRequestModel(string language, string text, TypeOfQuestion type, QuestionCategory category, TextLevel level, double temperature)
        {
            Language = language;
            Text = text;
            Type = type;
            Level = level;
            Category = category;
            Temperature = temperature;
        }
        
    }
}