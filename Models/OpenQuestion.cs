namespace SK_API{
    public class OpenQuestion{
        public DateOnly Date { get; set; }
        public string Topic { get; set; }
        public string Level { get; set; }
        public double Temperature { get; set; }
        public string Question { get; set; }
        public string CorrectAnswer { get; set; }

        public OpenQuestion(string topic, string level, double temperature, string question, string correctAnswer)
        {
            Topic = topic;
            Level = level;
            Temperature = temperature;
            Question = question;
            CorrectAnswer = correctAnswer;
            Date = DateOnly.FromDateTime(DateTime.Now);
        }
        public override string ToString()
        {
            return $"Date: {Date}\nTopic: {Topic}\nLevel: {Level}\nTemperature: {Temperature}\nQuestion: {Question}\nCorrectAnswer: {CorrectAnswer}";
        }
        
    }
}