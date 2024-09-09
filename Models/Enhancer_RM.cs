namespace SK_API
{
    public class EnhancerRM
    {
        public string Json_response { get; set; }
        public string Request { get; set; }
        public TypeOfResponse Type { get; set; }
        public string Language { get; set; }
        public double Temperature { get; set; }

        // Match parameter names with property names for deserialization
        public EnhancerRM(string Json_response, string Request, string Language, TypeOfResponse Type, double Temperature)
        {
            this.Json_response = Json_response;
            this.Request = Request;
            this.Type = Type;
            this.Language = Language;
            this.Temperature = Temperature;
        }
    }
}