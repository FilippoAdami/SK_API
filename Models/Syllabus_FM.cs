using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SK_API
{
    public class Syllabus : IGeneralClass
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Objectives { get; set; }
        public List<string> Goals { get; set; }
        public List<Tuple<string, string>> Topics { get; set; }
        public List<string> Prerequisites { get; set; }

        // Constructor to create a Syllabus object from JSON
        public Syllabus(string json){
            Console.WriteLine("json: " + json);
            try
            {
                var jsonObject = JObject.Parse(json);

                Title = jsonObject["CourseTitle"]?.ToString() ?? "Title";
                Description = jsonObject["CourseDescription"]?.ToString() ?? "Description";
                
                Objectives = jsonObject["LearningOutcomes"]?.ToObject<List<string>>() ?? new List<string>();
                Goals = jsonObject["CourseGoals"]?.ToObject<List<string>>() ?? new List<string>();

                Topics = new List<Tuple<string, string>>();
                var topicsArray = jsonObject["CourseTopics"]?.ToArray();
                if (topicsArray != null)
                {
                    foreach (var topic in topicsArray)
                    {
                        var topicTitle = topic["Topic"]?.ToString() ?? "Topic Title";
                        var topicDescription = topic["Description"]?.ToString() ?? "Topic Description";
                        Topics.Add(new Tuple<string, string>(topicTitle, topicDescription));
                    }
                }

                Prerequisites = jsonObject["Prerequisites"]?.ToObject<List<string>>() ?? new List<string>();
            }
            catch (JsonException ex)
            {
                throw new ArgumentException("Error deserializing JSON: " + ex.Message, ex);
            }
        }

        // Parameterless constructor for manual object creation
        public Syllabus()
        {
            Objectives = new List<string>();
            Goals = new List<string>();
            Topics = new List<Tuple<string, string>>();
            Prerequisites = new List<string>();
        }

        // Method to serialize the Syllabus object to JSON, the tuples must be printed as Topic and Description
        public string ToJSON()
        {
            var jsonObject = new JObject
            {
                ["CourseTitle"] = Title,
                ["CourseDescription"] = Description,
                ["LearningOutcomes"] = JToken.FromObject(Objectives),
                ["CourseGoals"] = JToken.FromObject(Goals)
            };

            var topicsArray = new JArray();
            foreach (var topic in Topics)
            {
                var topicObject = new JObject
                {
                    ["Topic"] = topic.Item1,
                    ["Description"] = topic.Item2
                };
                topicsArray.Add(topicObject);
            }
            jsonObject["CourseTopics"] = topicsArray;

            jsonObject["Prerequisites"] = JToken.FromObject(Prerequisites);

            return jsonObject.ToString();
        }
    }
}
