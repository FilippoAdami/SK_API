namespace SK_API{

    public class LORM{
        public string Topic { get; set; }
        public string Context { get; set; }
        public TextLevel Level { get; set; }

        public LORM(string topic, string context, TextLevel level)
        {
            Topic = topic;
            Context = context;
            Level = level;
        }
    }

}
