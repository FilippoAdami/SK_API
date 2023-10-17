namespace SK_API{
    public class QuestionExcerciseRequestModel{
        public string Text { get; set; }
        public TextLevel Level { get; set; }
        public double Temperature { get; set; }

        public QuestionExcerciseRequestModel(string text, TextLevel level, double temperature)
        {
            Text = text;
            Level = level;
            Temperature = temperature;
        }
        
    }
}