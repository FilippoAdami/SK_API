namespace SK_API{
    public class OpenQuestion{
        public string Language { get; set; }
        public DateOnly Date { get; set; }
        public string Type { get; set; }
        public string Level { get; set; }
        public string Category { get; set; }
        public double Temperature { get; set; }
        public string Question { get; set; }
        public string CorrectAnswer { get; set; }

        public OpenQuestion(string language, string type, string level, string category, double temperature, string question, string correctAnswer)
        {
            Language = language;
            Type = type;
            Level = level;
            Category = category;
            Temperature = temperature;
            Question = question;
            CorrectAnswer = correctAnswer;
            Date = DateOnly.FromDateTime(DateTime.Now);
        }
        public override string ToString()
        {
            return $"Language: {Language}\nDate: {Date}\nLevel: {Level}\nType of question: {Type}\nCategory: {Category}\nTemperature: {Temperature}\nQuestion: {Question}\nCorrectAnswer: {CorrectAnswer}";
        }
        
    }
}