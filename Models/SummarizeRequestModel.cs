namespace SK_API{
    public class SummarizerRequestModel{
        public string Lesson { get; set; }
        public int NoW { get; set; }

        public SummarizerRequestModel(string lesson, int now)
        {
            Lesson = lesson;
            NoW = now;
        }        
    }
}