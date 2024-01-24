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
            return $"1.)Language: {Language}\n2.)Date: {Date}\n3.)Level: {Level}\n4.)Type of question: {Type}\n5.)Category: {Category}\n6.)Temperature: {Temperature}\n7.)Question: {Question}\n8.)CorrectAnswer: {CorrectAnswer}";
        }
        
    }
}