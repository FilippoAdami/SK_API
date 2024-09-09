using Newtonsoft.Json;

namespace SK_API{
    public class CorrectedAnswer : IGeneralClass{
        public double Accuracy { get; set; }
        public string Correction { get; set; }

        public CorrectedAnswer(string response)
        {
            dynamic json = JsonConvert.DeserializeObject(response);
            // Assign properties if present, otherwise set them to default values
            Accuracy = json?["Accuracy"] ?? 0.0;
            Correction = json?["Correction"] ?? "null";
            if (Accuracy == 0.2){
                Accuracy = 0.0;
            } else if (Accuracy == 0.4 || Accuracy == 0.6){
                Accuracy = 0.5;
            } else if (Accuracy == 0.8){
                Accuracy = 1.0;
            }
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
        
    }
}