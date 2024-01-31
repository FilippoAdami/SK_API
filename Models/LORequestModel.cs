namespace SK_API{
    public class LORequestModel{
        public string Language { get; set; }
        public  ExperienceLevel EducatorExperience { get; set; }
        public  ExperienceLevel LearnerExperience { get; set; }
        public  Dimension Dimension { get; set; }
        public TextLevel EducationContext { get; set; }
        public string LearningContext { get; set; }
        public string Skills { get; set; }
        public BloomLevel BloomLevel { get; set; }
        public string[] Verbs { get; set; }
        public double Temperature { get; set; }

        public LORequestModel(string language, ExperienceLevel educatorExperience, ExperienceLevel learnerExperience, Dimension dimension, TextLevel educationContext, string learningContext, string skills, BloomLevel bloomLevel, string[] verbs, double temperature)
        {
            Language = language;
            EducatorExperience = educatorExperience;
            LearnerExperience = learnerExperience;
            Dimension = dimension;
            EducationContext = educationContext;
            LearningContext = learningContext;
            Skills = skills;
            BloomLevel = bloomLevel;
            this.Verbs = verbs;
            Temperature = temperature;
        }
        
    }
}