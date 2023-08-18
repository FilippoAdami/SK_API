
namespace SK_API{
    public class QuizExerciseRequestModel{
        public string Topic { get; set; }
        public string Level { get; set; }
        public double Temperature { get; set; }
        public int Nedd { get; set; }
        public int N_o_d { get; set; }

        public QuizExerciseRequestModel(string topic, string level, int nedd, int n_o_d, double temperature)
        {
            Topic = topic;
            Level = level;
            Nedd = nedd;
            N_o_d = n_o_d;
            Temperature = temperature;
        }
    }
}