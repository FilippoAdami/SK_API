namespace SK_API{
    public class CorrectedAnswer{
        public string Language { get; set; }
        public DateOnly Date { get; set; }
        public double Accuracy { get; set; }
        public string Correct_answer { get; set; }
        public string Correction { get; set; }
        public double Temperature { get; set; }

        public CorrectedAnswer(string language, double accuracy, string correct_answer, string correction, double temperature)
        {
            Language = language;
            Accuracy = accuracy;
            Correct_answer = correct_answer;
            Correction = correction;
            Temperature = temperature;
            Date = DateOnly.FromDateTime(DateTime.Now);
        }

        public override string ToString()
        {
            return $"1.)Language: {Language}\n2.)Date: {Date}\n3.)Accuracy: {Accuracy}\n4.)Correct_answer: {Correct_answer}\n5.)Correction: {Correction}\n6.)Temperature: {Temperature}";
        }
        
    }
}