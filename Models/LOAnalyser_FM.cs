using Newtonsoft.Json;

namespace SK_API{
    public class LOAnalysis : IGeneralClass{
        public BloomLevel BloomLevel { get; set; }
        public string MacroSubject { get; set; }
        public TextLevel Level { get; set; }
        public string Topic { get; set; }

        public LOAnalysis(string response)
        {
            dynamic responseData = JsonConvert.DeserializeObject(response);
        
            // Assign properties if present, otherwise set them to default values
            
            // Bloom's Level
            if (responseData?["BloomLevel"] != null)
            {
                BloomLevel = Enum.Parse<BloomLevel>((string)responseData["BloomLevel"], true);
            }
            else
            {
                BloomLevel = BloomLevel.Remembering; // Default value
            }

            // Macro-Subject
            MacroSubject = responseData?["MacroSubject"] ?? "";

            // Level
            if (responseData?["Level"] != null)
            {
                Level = Enum.Parse<TextLevel>((string)responseData["Level"], true);
            }
            else
            {
                Level = TextLevel.primary_school; // Default value
            }

            // Topic
            Topic = responseData?["Topic"] ?? "";
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
