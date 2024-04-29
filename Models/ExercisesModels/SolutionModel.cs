namespace SK_API{
    public class SolutionModel{
        public string SolutionType { get; set; }
        public string Indications { get; set; }
        public string CorrectAnswersNumber { get; set; }

        // Constructor
        public SolutionModel(TypeOfExercise typeOfExercise, TypeOfAssignment typeOfAssignment, ExerciseCategory category, int answers){
            
            // Set the correct number of answers for the type of exercise
            CorrectAnswersNumber = "the "+answers.ToString();
            CorrectAnswersNumber = answers.ToString();
            if (typeOfExercise == TypeOfExercise.multiple_choice || typeOfExercise == TypeOfExercise.true_or_false || typeOfExercise == TypeOfExercise.short_answer_question){
                CorrectAnswersNumber = "the one";
            } else if (typeOfExercise == TypeOfExercise.open_question || category == ExerciseCategory.conceptual || category == ExerciseCategory.practical){
                CorrectAnswersNumber = "exactly one valid";
            } else if (typeOfExercise == TypeOfExercise.information_search){
                CorrectAnswersNumber = "all the";
            }

            // Set the correct format and indications for the exercise
            if (typeOfAssignment == TypeOfAssignment.code){
                Indications = "generate a code that could be used to achieve the goal identified in the question/assignment";
            }else if (typeOfAssignment == TypeOfAssignment.problem_resolution){
                Indications = "generate a correct result to the question/assignment by applying a step by step process to solve the problem";
            }else{
                Indications = "generate or extract from the material the answer/answers to the question/assignment";
            }
            switch (category)
                {
                    case ExerciseCategory.choice:
                        Indications += $"and extract {CorrectAnswersNumber} correct options from the explanation.";
                        break;
                    case ExerciseCategory.question:
                        break;
                    case ExerciseCategory.fill_in_the_blanks:
                        if (typeOfAssignment == TypeOfAssignment.code){
                            Indications = "generate a list of all relevant code pieces used in the text. ";
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
            
            // Set the correct solution type for the type of exercise
            if (typeOfExercise == TypeOfExercise.information_search){
                SolutionType = "relvenant inforation";
            }else if (typeOfExercise == TypeOfExercise.open_question){
                SolutionType = "long form answer";
            }else if (typeOfExercise == TypeOfExercise.short_answer_question){
                SolutionType = "short form answer (4 words max)";
            }else if (typeOfExercise == TypeOfExercise.true_or_false){
                SolutionType = "true/false + explanation";
            }else if (typeOfExercise == TypeOfExercise.multiple_choice || typeOfExercise == TypeOfExercise.multiple_select){
                SolutionType = "choice element";
            }else{
                SolutionType = "";
            }
        }

        public string ToString(ExerciseCategory category){
            string result;
            if(category == ExerciseCategory.question){
                result = ExercisesGenerationPrompt.S_Solution(SolutionType, Indications, CorrectAnswersNumber);
            }
            else{
                result = ExercisesGenerationPrompt.S_Solution(SolutionType, Indications, CorrectAnswersNumber) + '\n' + ExercisesGenerationPrompt.S_Distractors() + '\n' + ExercisesGenerationPrompt.S_EasilyDiscardableDistractors();
            }
            return result;
        }    
    }
}