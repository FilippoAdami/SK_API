namespace SK_API{
    public class LessonRequestModel{
        public string Language { get; set; }
        public string Topic { get; set; }
        public TextLevel Level { get; set; }
        public double Temperature { get; set; }

        public LessonRequestModel(string language, string topic, TextLevel level, double temperature)
        {
            Language = language;
            Topic = topic;
            Level = level;
            Temperature = temperature;
        }
        
    }
}