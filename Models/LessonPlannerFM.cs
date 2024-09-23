using Newtonsoft.Json;

namespace SK_API
{
    public class LessonPlan : IGeneralClass
    {
        public List<Node> LessonPlanNodes { get; set; }

        public LessonPlan(string json)
        {
            // Deserialize the JSON to match the structure with a "lesson_plan" field.
            var lessonPlanWrapper = JsonConvert.DeserializeObject<LessonPlanWrapper>(json);
            LessonPlanNodes = lessonPlanWrapper?.LessonPlan ?? new List<Node>();
        }

        public string ToJSON()
        {
            // Serialize back into a structure that includes "lesson_plan".
            var lessonPlanWrapper = new LessonPlanWrapper { LessonPlan = LessonPlanNodes };
            return JsonConvert.SerializeObject(lessonPlanWrapper);
        }
    }

    public class Node
    {
        public string Type { get; set; }
        public string Topic { get; set; }
        public string Details { get; set; }
        public string Description { get; set; }

        public Node(string type, string topic, string details, string description)
        {
            Type = type;
            Topic = topic;
            Details = details;
            Description = description;
        }
    }

    // Wrapper class to match the "lesson_plan" field in the JSON.
    public class LessonPlanWrapper
    {
        [Newtonsoft.Json.JsonProperty("lesson_plan")]
        public List<Node> LessonPlan { get; set; }
    }
}
