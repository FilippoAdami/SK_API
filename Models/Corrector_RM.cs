namespace SK_API{
    public class CorrectorRequestModel
    {
        public string Question { get; set; }
        public string ExpectedAnswer { get; set; }
        public string Answer { get; set; }
        public int Temperature { get; set; }

        public CorrectorRequestModel(string question, string expectedAnswer, string answer, int temperature)
        {
            Question = question;
            ExpectedAnswer = expectedAnswer;
            Answer = answer;
            Temperature = temperature;
        }
    }
}