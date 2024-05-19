namespace SK_API{
    public class AssignmentModel{
        public string SolutionType { get; set; }
        public string CorrectAnswersNumber { get; set; }
        public string Description { get; set; }

        // Constructor
        public AssignmentModel(ActivityCategory category, TypeOfActivity typeOfActivity, int answers)
        {
            // Set the correct number of answers for the type of Activity
            CorrectAnswersNumber = answers.ToString();
            if (typeOfActivity == TypeOfActivity.multiple_choice || typeOfActivity == TypeOfActivity.true_or_false || typeOfActivity == TypeOfActivity.short_answer_question){
                CorrectAnswersNumber = "one";
            } else if (typeOfActivity == TypeOfActivity.open_question || category == ActivityCategory.conceptual || category == ActivityCategory.practical){
                CorrectAnswersNumber = "multiple";
            } else if (typeOfActivity == TypeOfActivity.information_search){
                CorrectAnswersNumber = "at least " + (answers+2).ToString();
            }

            // Set the correct solution type for the type of Activity
            if (typeOfActivity == TypeOfActivity.information_search){
            SolutionType = "relevant information (domain specific knowledge / specific terms / concepts / numbers / formulas / code pieces / etc.)";
            }else if (typeOfActivity == TypeOfActivity.open_question){
                SolutionType = "long form answer";
            }else if (typeOfActivity == TypeOfActivity.short_answer_question){
                SolutionType = "short form answer (2 or 3 words max, taxatively no more than 4 words)";
            }else if (typeOfActivity == TypeOfActivity.true_or_false){
                SolutionType = "true/false + explanation";
            }else if (typeOfActivity == TypeOfActivity.multiple_choice || typeOfActivity == TypeOfActivity.multiple_select){
                SolutionType = "choice element";
            }else{
                SolutionType = "";
            }

            // Set the correct description for the category
            Description = category switch
            {
                ActivityCategory.choice => "it expects concise and consistent choice answers",
                ActivityCategory.question => "the question must be clear and avoid any kind of ambiguity",
                ActivityCategory.fill_in_the_blanks => "you must generate a text about the topic like in the examples",
                ActivityCategory.conceptual => "the assignment should be designed to give a prompt for students to then develop in their on ways",
                ActivityCategory.practical => "the assignment should be designed to give a prompt for students to then develop in their on ways",
                // question
                _ => "the question must be clear and avoid any kind of ambiguity",
            };
        }
    
        public override string ToString(){
            string result = ActivityGenerationPrompt.A_Description() + '\n' + ActivityGenerationPrompt.A_Resolution(Description, SolutionType, CorrectAnswersNumber.ToString());
            return result;
        }
    }
}