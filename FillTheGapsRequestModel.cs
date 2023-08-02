// Declare the namespace for the WeatherForecast class
namespace SK_API
{
    public class FillTheGapsRequestModel
    {
        public string Text { get; set; }
        public string Topic { get; set; }
        public string Level { get; set; }
        public int N_o_w { get; set; }
        public int N_o_g { get; set; }
        public int N_o_d { get; set; }
        public int Temperature { get; set; }
        public string[] Gaps { get; set; }
        public string[] Distractors { get; set; }

        public FillTheGapsRequestModel(string text, string topic, string level, int n_o_w, int n_o_g, int n_o_d, int temperature, string[] gaps, string[] distractors)
        {
            Text = text;
            Topic = topic;
            Level = level;
            N_o_w = n_o_w;
            N_o_g = n_o_g;
            N_o_d = n_o_d;
            Temperature = temperature;
            Gaps = gaps;
            Distractors = distractors;
        }
    }
}