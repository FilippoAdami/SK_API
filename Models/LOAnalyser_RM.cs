namespace SK_API{

    public class LOAnalyserRequestModel{
        public required string LearningObjective { get; set; }

        public LOAnalyserRequestModel(string learningObjective)
        {
            LearningObjective = learningObjective;
        }
    }

}
