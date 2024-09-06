namespace SK_API{

    public class SyllabusRM{
        public MaterialAnalysis Analysis { get; set; }
        public BloomLevel BloomLevel { get; set; }

        public SyllabusRM(MaterialAnalysis analysis, BloomLevel bloomLevel){
            Analysis = analysis;
            BloomLevel = bloomLevel;
        }
    }

}
