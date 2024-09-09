namespace SK_API{
    public class LessonPlan : IGeneralClass{
        public List<Nodes> Nodes { get; set; }

        public LessonPlan(string json)
        {
            var lessonPlan = Newtonsoft.Json.JsonConvert.DeserializeObject<LessonPlanJson>(json);
            Nodes = lessonPlan.lesson_plan.Select(x => new Nodes(x.type == "Activity", x.topic, x.description, x.duration)).ToList();
        }

        public string ToJSON(){
            return Newtonsoft.Json.JsonConvert.SerializeObject(Nodes);
        }
    }

    public class Nodes {
        public bool Type { get; set; }
        public string Topic { get; set; }
        public string Details { get; set; }
        public int Duration { get; set; }

        public Nodes(bool type, string topic, string details, int duration){
            Type = type;
            Topic = topic;
            Details = details;
            Duration = duration;
        }
    }

    public class LessonPlanJson
    {
        public List<NodeJson> lesson_plan { get; set; }
    }

    public class NodeJson
    {
        public string type { get; set; }
        public string topic { get; set; }
        public string description { get; set; }
        public int duration { get; set; }
    }
}
