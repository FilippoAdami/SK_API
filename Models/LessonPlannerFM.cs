namespace SK_API{
    public class LessonPlan : IGeneralClass{
        public List<Nodes> Nodes { get; set; }

        public LessonPlan(string json)
        {
            var lessonPlan = Newtonsoft.Json.JsonConvert.DeserializeObject<LessonPlan>(json);
            Nodes = lessonPlan?.Nodes != null ? new List<Nodes>(lessonPlan.Nodes) : new List<Nodes>();
        }

        public string ToJSON(){
            return Newtonsoft.Json.JsonConvert.SerializeObject(Nodes);
        }
    }

    public class Nodes {
        public bool Type { get; set; }
        public string Topic { get; set; }
        public string Details { get; set; }
        public string Description { get; set; }

        public Nodes(bool type, string topic, string details, string description){
            Type = type;
            Topic = topic;
            Details = details;
            Description = description;
        }
    }
}
