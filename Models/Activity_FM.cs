using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace SK_API{
    public class ActivityFinalModel{
        public string Assignment {get;set;}
        public string Plus {get;set;}
        public List<string> Solutions {get;set;}
        public List<string> Distractors {get;set;}
        public List<string> EasilyDiscardableDistractors {get;set;}

        // Constructor
        public ActivityFinalModel(string exercise)
        {
            dynamic exerciseData = JsonConvert.DeserializeObject(exercise);
            // Assign properties if present, otherwise set them as empty strings
            Assignment = exerciseData?.Assignment ?? "";
            Plus = exerciseData?.Plus ?? "";
            Solutions = new List<string>();
            if (exerciseData?.Solutions != null)
            {
                Solutions = exerciseData.Solutions.ToObject<List<string>>();
            }
            else if (exerciseData?.Solution != null)
                {
                    Solutions.Add((string)exerciseData.Solution);
                }
            // Distractors
            if (exerciseData?.Distractors != null)
            {
                Distractors = exerciseData.Distractors.ToObject<List<string>>();
            }
            else
            {
                Distractors = new List<string>();
            }
            // EasilyDiscardableDistractors
            if (exerciseData?.EasilyDiscardableDistractors != null)
            {
                EasilyDiscardableDistractors = exerciseData.EasilyDiscardableDistractors.ToObject<List<string>>();
            }
            else
            {
                EasilyDiscardableDistractors = new List<string>();
            }
        }
        // Convert the object to a JSON string
        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
        // Process the exercise (for fill in the blanks exercises)
        public static ActivityFinalModel ProcessActivity(ActivityFinalModel model)
        {
            string text = ExtractTextFromExercise(model.Assignment);
            if (string.IsNullOrEmpty(text))
            {
                return model;
            }
            List<string> solutions = new List<string>(model.Solutions);

            foreach (string solution in model.Solutions)
            {
                if (!IsSolutionPresentInText(solution, text))
                {
                    solutions.Remove(solution);
                }
            }

            model.Solutions = solutions;
            if (model.Solutions.Count == 0)
            {
                model.Solutions.Add("No solution found in the text");
            }

            return model;
        }
        // Extract the text from the finn in the blanks exercise
        private static string ExtractTextFromExercise(string exercise)
        {
            // Use regular expression to find the text after "Text :"
            Match match = Regex.Match(exercise, @"Text\s*:\s*(.*)", RegexOptions.IgnoreCase);

            if (match.Success)
            {
                // Extract the captured group
                return match.Groups[1].Value.Trim();
            }

            // Return empty string if no match found
            return string.Empty;
        }
        // Check if the solution is present in the text
        private static bool IsSolutionPresentInText(string solution, string text)
        {
            return text.IndexOf(solution, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
