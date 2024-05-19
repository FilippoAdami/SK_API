namespace SK_API{
    public class SolutionModel{
        public string SolutionType { get; set; }
        public string Indications { get; set; }
        public string CorrectAnswersNumber { get; set; }

        // Constructor
        public SolutionModel(TypeOfActivity typeOfActivity, TypeOfAssignment typeOfAssignment, ActivityCategory category, int answers){
            
            // Set the correct number of answers for the type of Activity
            CorrectAnswersNumber = "the "+answers.ToString();
            CorrectAnswersNumber = answers.ToString();
            if (typeOfActivity == TypeOfActivity.multiple_choice || typeOfActivity == TypeOfActivity.true_or_false || typeOfActivity == TypeOfActivity.short_answer_question){
                CorrectAnswersNumber = "the one";
            } else if (typeOfActivity == TypeOfActivity.open_question || category == ActivityCategory.conceptual || category == ActivityCategory.practical){
                CorrectAnswersNumber = "exactly one valid";
            } else if (typeOfActivity == TypeOfActivity.information_search){
                CorrectAnswersNumber = "all the";
            }

            // Set the correct format and indications for the Activity
            if (typeOfAssignment == TypeOfAssignment.code){
                Indications = "generate a code that could be used to achieve the goal identified in the question/assignment";
            }else if (typeOfAssignment == TypeOfAssignment.problem_resolution){
                Indications = "generate a correct result to the question/assignment by applying a step by step process to solve the problem";
            }else{
                Indications = "generate, or extract from the material, the answer/answers to the question/assignment";
            }
            switch (category)
                {
                    case ActivityCategory.choice:
                        Indications += $"and extract {CorrectAnswersNumber} correct options from the explanation.";
                        break;
                    case ActivityCategory.question:
                        break;
                    case ActivityCategory.fill_in_the_blanks:
                        if (typeOfAssignment == TypeOfAssignment.code){
                            Indications = "generate a list of all relevant code pieces used in the text.";
                        }else if (typeOfAssignment == TypeOfAssignment.problem_resolution){
                            Indications = "generate a list of all domain specific terms, formulas, numbers and concepts written in the text.";
                        }else{
                            Indications = "generate a list of all relevant information written in the text.";
                        }
                        break;
                    default: // question
                        Indications = "no indications needed";
                        break;
                }
            
            // Set the correct solution type for the type of Activity
            if (typeOfActivity == TypeOfActivity.information_search){
                SolutionType = "relvenant inforation";
            }else if (typeOfActivity == TypeOfActivity.open_question){
                SolutionType = "long form answer";
            }else if (typeOfActivity == TypeOfActivity.short_answer_question){
                SolutionType = "short form answer (4 words max)";
            }else if (typeOfActivity == TypeOfActivity.true_or_false){
                SolutionType = "true/false + explanation";
            }else if (typeOfActivity == TypeOfActivity.multiple_choice || typeOfActivity == TypeOfActivity.multiple_select){
                SolutionType = "choice element";
            }else{
                SolutionType = "";
            }
        }

        public string ToString(ActivityCategory category){
            string result;
            if(category == ActivityCategory.question){
                result = ActivityGenerationPrompt.S_Solution(SolutionType, Indications, CorrectAnswersNumber);
            }
            else{
                result = ActivityGenerationPrompt.S_Solution(SolutionType, Indications, CorrectAnswersNumber) + '\n' + ActivityGenerationPrompt.S_Distractors() + '\n' + ActivityGenerationPrompt.S_EasilyDiscardableDistractors();
            }
            return result;
        }    
    }
}