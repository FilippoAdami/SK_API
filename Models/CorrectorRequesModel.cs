namespace SK_API{
    public class CorrectorRequestModel{
        public string Language { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public double Temperature { get; set; }

        public CorrectorRequestModel(string language, string question, string answer, double temperature)
        {
            Language = language;
            Question = question;
            Answer = answer;
            Temperature = temperature;
        }
        
    }
}