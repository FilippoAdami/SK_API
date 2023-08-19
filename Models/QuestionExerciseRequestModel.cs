namespace SK_API{
    public class QuestionExcerciseRequestModel{
        public string Topic { get; set; }
        public string Level { get; set; }
        public double Temperature { get; set; }

        public QuestionExcerciseRequestModel(string topic, string level, double temperature)
        {
            Topic = topic;
            Level = level;
            Temperature = temperature;
        }
        
    }
}