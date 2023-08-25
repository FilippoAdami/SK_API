// Declare the namespace for the WeatherForecast class
namespace SK_API
{
    // Define the Fill the Gaps class and its constructor with all the properties
    public class Fill_the_Gaps
    {
        // Define a property 'Date' of type DateOnly
        // 'DateOnly' is a struct in .NET that represents a date without a time component
        // The 'get' and 'set' accessors allow getting and setting the value of the 'Date' property
        public DateOnly Date { get; set; }

        // Define a property 'Temperature' of type double
        // This property represents the temperature of the prompt
        public double Temperature { get; set; }

        // Define a property 'NoW' of type int
        // This property represents the number of words that the text should have.
        // The actual number can vary from this by +- 30.
        public int NoW { get; set; }

        // Define a property 'NoG' of type int
        // This property represents the number of gaps that the text should have.
        public int NoG { get; set; }

        //Define a property 'NoD' of type int
        //This property represents the number of distractors that the text should have.
        public int NoD { get; set; }

        // Define a property 'Text' of type string
        // This property represents the text that the user should fill the gaps in.
        public string Text { get; set; }

        // Define a property 'TextWithGaps' of type string
        // This property represents the text with the gaps in it.
        public string TextWithGaps { get; set; }

        //Define a property 'Topic' of type string
        //This property represents the topic of the text.
        public string Topic { get; set; }

        //Define a property 'Type_of_text' of type string
        //This property represents the type of the text.
        public string Type_of_text { get; set; }

        //Define a property 'Level' of type string
        //This property represents the level of the text.
        public string Level { get; set; }

        //Define a property 'Words' which is an array of strings; it should have n_o_g + n_o_d elements.
        //This property represents the words from which to choose to fill the gaps in the text. (distractors included)
        public string[] Words { get; set; }

        //Define a constructor for the Fill the Gaps class
        public Fill_the_Gaps(string text, string t_w_g, string topic, string type_of_text, string level, int n_o_w, int n_o_g, int n_o_d, double temperature, string[] words)
        {
            Text = text;
            TextWithGaps = t_w_g;
            Topic = topic;
            Type_of_text = type_of_text;
            Level = level;
            NoW = n_o_w;
            NoG = n_o_g;
            NoD = n_o_d;
            Words = words;
            Temperature = temperature;
            Date = DateOnly.FromDateTime(DateTime.Now);
        }
        public override string ToString()
        {
            return $"Date: {Date}\nTemperature: {Temperature}\nNoW: {NoW}\nNoG: {NoG}\nNoD: {NoD}\nText: {Text}\nTextWithGaps: {TextWithGaps}\nTopic: {Topic}\nType_of_text: {Type_of_text}\nLevel: {Level}\nWords:\n(A){string.Join("\n(A)", Words)}";
        }
    }
}