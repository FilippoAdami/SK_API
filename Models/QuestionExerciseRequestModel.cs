namespace SK_API{
    public class QuestionExcerciseRequestModel{
        public string Topic { get; set; }
        public TextLevel Level { get; set; }
        public double Temperature { get; set; }

        public QuestionExcerciseRequestModel(string topic, TextLevel level, double temperature)
        {
            Topic = topic;
            Level = level;
            Temperature = temperature;
        }
        
    }
}