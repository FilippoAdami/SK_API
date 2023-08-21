namespace SK_API{
    public class CorrectedAnswer{
        public DateOnly Date { get; set; }
        public double Accuracy { get; set; }
        public string Correct_answer { get; set; }
        public string Correction { get; set; }
        public double Temperature { get; set; }

        public CorrectedAnswer(double accuracy, string correct_answer, string correction, double temperature)
        {
            Accuracy = accuracy;
            Correct_answer = correct_answer;
            Correction = correction;
            Temperature = temperature;
            Date = DateOnly.FromDateTime(DateTime.Now);
        }

        public override string ToString()
        {
            return $"Date: {Date}\nAccuracy: {Accuracy}\nCorrect_answer: {Correct_answer}\nCorrection: {Correction}\nTemperature: {Temperature}";
        }
        
    }
}