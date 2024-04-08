using Newtonsoft.Json;

namespace SK_API{
    public class LOFM{
        public List<string> Remembering { get; set; }
        public List<string> Understanding { get; set; }
        public List<string> Applying { get; set; }
        public List<string> Analyzing { get; set; }
        public List<string> Evaluating { get; set; }
        public List<string> Creating { get; set; }

        public LOFM(string response)
        {
            dynamic responseData = JsonConvert.DeserializeObject(response);

            // Convert JArrays to Lists<string>
            Remembering = responseData.Remembering != null ? responseData.Remembering.ToObject<List<string>>() : new List<string>();
            Understanding = responseData.Understanding != null ? responseData.Understanding.ToObject<List<string>>() : new List<string>();
            Applying = responseData.Applying != null ? responseData.Applying.ToObject<List<string>>() : new List<string>();
            Analyzing = responseData.Analyzing != null ? responseData.Analyzing.ToObject<List<string>>() : new List<string>();
            Evaluating = responseData.Evaluating != null ? responseData.Evaluating.ToObject<List<string>>() : new List<string>();
            Creating = responseData.Creating != null ? responseData.Creating.ToObject<List<string>>() : new List<string>();
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
