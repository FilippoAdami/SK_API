using Newtonsoft.Json;

namespace SK_API
{
    public class SimpleText : IGeneralClass
    {
        public string Text { get; set; }

        public SimpleText(string text)
        {
            Text = text;
        }

        public string ToJSON()
        {
            return Text;
        }
    }
}