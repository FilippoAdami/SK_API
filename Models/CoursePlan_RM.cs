namespace SK_API{
    public class CoursePlanRequestModel{
        public MaterialAnalysis Analysis { get; set; }
        public int NumberOfLessons { get; set; }
        public int LessonDuration { get; set; }
        public double Temperature { get; set; }
    
        public CoursePlanRequestModel(MaterialAnalysis analysis, int nol, int duration, double temperature){
            Analysis = analysis;
            NumberOfLessons = nol;
            LessonDuration = duration;
            Temperature = temperature;
        }

        public string TopicsBasicInfo(){
            string result = "";
            foreach (var topic in Analysis.MainTopics)
            {
                result += topic.BasicInfo() + "\n";
            }
            return result;
        }

        public CoursePlanRequestModel(){}
    }
}