namespace SK_API
{
    public class CoursePlan
    {
        public List<Lesson> Plan { get; set; }

        public CoursePlan(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                Plan = new List<Lesson>();
            }
            else
            {
                var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<CoursePlan>(json);
                Plan = deserialized?.Plan != null ? new List<Lesson>(deserialized.Plan) : new List<Lesson>();
            }
        }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

    public class Lesson
    {
        public string Title { get; set; }
        public List<string> Topics { get; set; }

        public Lesson(string title, List<string> topics)
        {
            Title = title;
            Topics = new List<string>(topics);
        }
    }
}