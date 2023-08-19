namespace SK_API{
    public class LessonRequestModel{
        public string Topic { get; set; }
        public string Level { get; set; }
        public double Temperature { get; set; }

        public LessonRequestModel(string topic, string level, double temperature)
        {
            Topic = topic;
            Level = level;
            Temperature = temperature;
        }
        
    }
}