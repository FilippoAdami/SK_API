namespace SK_API{
    public class SummarizerRequestModel{
        public string Lesson { get; set; }
        public string Level { get; set; }
        public int NoW { get; set; }

        public SummarizerRequestModel(string lesson, string level, int now)
        {
            Lesson = lesson;
            Level = level;
            NoW = now;
        }        
    }
}