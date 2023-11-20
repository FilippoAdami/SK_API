using Microsoft.SemanticKernel;
using System;
using SK_API;

// Declare the namespace for the WeatherForecast class
namespace SK_API
{
    public class FillTheGapsRequestModel
    {
        public string Language { get; set; }
        public string Text { get; set; }
        public TextLevel Level { get; set; }
        public int N_o_w { get; set; }
        public int N_o_g { get; set; }
        public int N_o_d { get; set; }
        public double Temperature { get; set; }

        public FillTheGapsRequestModel(string language, string text, TextLevel level, int n_o_w, int n_o_g, int n_o_d, double temperature)
        {
            Language = language;
            Text = text;
            Level = level;
            N_o_w = n_o_w;
            N_o_g = n_o_g;
            N_o_d = n_o_d;
            Temperature = temperature;
        }
    }
}