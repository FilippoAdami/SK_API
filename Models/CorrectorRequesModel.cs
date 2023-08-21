namespace SK_API{
    public class CorrectorRequestModel{
        public string Question { get; set; }
        public string Answer { get; set; }
        public double Temperature { get; set; }

        public CorrectorRequestModel(string question, string answer, double temperature)
        {
            Question = question;
            Answer = answer;
            Temperature = temperature;
        }
        
    }
}