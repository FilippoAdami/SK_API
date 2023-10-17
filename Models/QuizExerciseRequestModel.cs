
namespace SK_API{
    public class QuizExerciseRequestModel{
        public bool Type { get; set; }
        public string Text { get; set; }
        public TextLevel Level { get; set; }
        public double Temperature { get; set; }
        public int Nedd { get; set; }
        public int N_o_d { get; set; }

        public QuizExerciseRequestModel(string text, bool type, TextLevel level, int nedd, int n_o_d, double temperature)
        {
            Type = type;
            Text = text;
            Level = level;
            Nedd = nedd;
            N_o_d = n_o_d;
            Temperature = temperature;
        }
    }
}