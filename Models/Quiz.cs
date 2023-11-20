namespace SK_API{
    public class Quiz{
        public string Language { get; set; }
        public DateOnly Date { get; set; }
        public string Level { get; set; }
        public double Temperature { get; set; }
        public int Nedd { get; set; }
        public int N_o_d { get; set; }
        public string Question { get; set; }
        public string Category { get; set; }
        public string CorrectAnswerIndex { get; set; }
        public string[] Answers { get; set; }
        public string Solution { get; set; }

        public Quiz(string language, string level, int nedd, int n_o_d, double temperature, string category, string question, string correctAnswerIndexes, string[] answers, string solution)
        {
            Language = language;
            Level = level;
            Nedd = nedd;
            N_o_d = n_o_d;
            Temperature = temperature;
            Question = question;
            Category = category;
            CorrectAnswerIndex = correctAnswerIndexes;
            Answers = answers;
            Solution = solution;
            Date = DateOnly.FromDateTime(DateTime.Now);
        }
        public override string ToString()
        {
            return $"Language: {Language}\nDate: {Date}\nLevel: {Level}\nTemperature: {Temperature}\nNedd: {Nedd}\nN_o_d: {N_o_d}\nCategory:{Category}: \nQuestion: {Question}\nCorrectAnswerIndex: {CorrectAnswerIndex}\nAnswers:\n(A){string.Join("\n(A)", Answers)}\nSolution: {Solution}";
        }
        
    }
}