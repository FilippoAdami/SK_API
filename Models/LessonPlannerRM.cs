using Newtonsoft.Json;

namespace SK_API{
    public class LessonPlannerRequestModel{
        public List<TopicAndExplanation> MainTopics { get; set; }
        public string Language { get; set; }
        public string MacroSubject { get; set; }
        public string Title { get; set; }
        public TextLevel Level { get; set; }
        public string LearningObjective { get; set; }
        public BloomLevel BloomLevel { get; set; }
        public string Context { get; set; }
        public double Temperature { get; set; }

        // parameterless constructor
        public LessonPlannerRequestModel() { }
    
        public LessonPlannerRequestModel(List<TopicAndExplanation> topics, string language, string macroSubject, string title, TextLevel level, string learningObjective, BloomLevel bloomLevel, string context, double temperature){
            MainTopics = new List<TopicAndExplanation>(topics);
            Language = language;
            MacroSubject = macroSubject;
            Title = title;
            Level = level;
            LearningObjective = learningObjective;
            BloomLevel = bloomLevel;
            Context = context;
            Temperature = temperature;
        }
    }
}