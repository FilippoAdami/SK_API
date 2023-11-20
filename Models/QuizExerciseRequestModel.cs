
namespace SK_API{
    public class QuizExerciseRequestModel{
        public string Language { get; set; }
        public bool Type { get; set; }
        public string Text { get; set; }
        public TextLevel Level { get; set; }
        public QuestionCategory Category { get; set; }
        public double Temperature { get; set; }
        public int N_o_ca { get; set; }
        public int Nedd { get; set; }
        public int N_o_d { get; set; }

        public QuizExerciseRequestModel(string language, string text, bool type, TextLevel level, QuestionCategory category, int n_o_ca, int nedd, int n_o_d, double temperature)
        {
            Language = language;
            Type = type;
            Text = text;
            Level = level;
            Category = category;
            N_o_ca = n_o_ca;
            Nedd = nedd;
            N_o_d = n_o_d;
            Temperature = temperature;
        }
    }
}