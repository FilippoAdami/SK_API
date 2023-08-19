namespace SK_API{
    public class SummarizerRequestModel{
        public string Lesson { get; set; }

        public SummarizerRequestModel(string lesson)
        {
            Lesson = lesson;
        }        
    }
}