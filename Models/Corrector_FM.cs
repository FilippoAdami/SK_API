using Newtonsoft.Json;

namespace SK_API{
    public class CorrectedAnswer{
        public double Accuracy { get; set; }
        public string Correction { get; set; }

        public CorrectedAnswer(string response)
        {
            dynamic json = JsonConvert.DeserializeObject(response);
            // Assign properties if present, otherwise set them to default values
            Accuracy = json?["Accuracy"] ?? 0.0;
            Correction = json?["Correction"] ?? "null";
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
        
    }
}