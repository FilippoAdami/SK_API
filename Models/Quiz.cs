namespace SK_API{
    public class Quiz{
        public DateOnly Date { get; set; }
        public string Topic { get; set; }
        public string Level { get; set; }
        public double Temperature { get; set; }
        public int Nedd { get; set; }
        public int N_o_d { get; set; }
        public string Question { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string[] Answers { get; set; }

        public Quiz(string topic, string level, int nedd, int n_o_d, double temperature, string question, int correctAnswerIndex, string[] answers)
        {
            Topic = topic;
            Level = level;
            Nedd = nedd;
            N_o_d = n_o_d;
            Temperature = temperature;
            Question = question;
            CorrectAnswerIndex = correctAnswerIndex;
            Answers = answers;
            Date = DateOnly.FromDateTime(DateTime.Now);
        }
        public override string ToString()
        {
            return $"Date: {Date}\n Topic: {Topic}\nLevel: {Level}\nTemperature: {Temperature}\nNedd: {Nedd}\nN_o_d: {N_o_d}\nQuestion: {Question}\nCorrectAnswerIndex: {CorrectAnswerIndex}\nAnswers:\n(A){string.Join("\n(A)", Answers)}";
        }
        
    }
}