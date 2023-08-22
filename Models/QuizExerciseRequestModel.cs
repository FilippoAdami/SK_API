
namespace SK_API{
    public class QuizExerciseRequestModel{
        public bool Type { get; set; }
        public string Topic { get; set; }
        public TextLevel Level { get; set; }
        public double Temperature { get; set; }
        public int Nedd { get; set; }
        public int N_o_d { get; set; }

        public QuizExerciseRequestModel(bool type, string topic, TextLevel level, int nedd, int n_o_d, double temperature)
        {
            Type = type;
            Topic = topic;
            Level = level;
            Nedd = nedd;
            N_o_d = n_o_d;
            Temperature = temperature;
        }
    }
}