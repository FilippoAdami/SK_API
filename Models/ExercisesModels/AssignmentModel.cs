namespace SK_API{
    public class AssignmentModel{
        public string SolutionType { get; set; }
        public string CorrectAnswersNumber { get; set; }
        public string Description { get; set; }

        // Constructor
        public AssignmentModel(ExerciseCategory category, TypeOfExercise typeOfExercise, int answers)
        {
            // Set the correct number of answers for the type of exercise
            CorrectAnswersNumber = answers.ToString();
            if (typeOfExercise == TypeOfExercise.single_choice || typeOfExercise == TypeOfExercise.true_or_false || typeOfExercise == TypeOfExercise.short_answer_question){
                CorrectAnswersNumber = "one";
            } else if (typeOfExercise == TypeOfExercise.open_question || category == ExerciseCategory.conceptual || category == ExerciseCategory.practical){
                CorrectAnswersNumber = "multiple";
            } else if (typeOfExercise == TypeOfExercise.fill_in_the_blanks){
                CorrectAnswersNumber = "at least " + (answers+2).ToString();
            }

            // Set the correct solution type for the type of exercise
            if (typeOfExercise == TypeOfExercise.fill_in_the_blanks){
            SolutionType = "relevant information (domain specific knowledge / specific terms / concepts / numbers / formulas / code pieces / etc.)";
            }else if (typeOfExercise == TypeOfExercise.open_question){
                SolutionType = "long form answer";
            }else if (typeOfExercise == TypeOfExercise.short_answer_question){
                SolutionType = "short form answer (2 or 3 words max, taxatively no more than 4 words)";
            }else if (typeOfExercise == TypeOfExercise.true_or_false){
                SolutionType = "true/false + explanation";
            }else if (typeOfExercise == TypeOfExercise.single_choice || typeOfExercise == TypeOfExercise.multiple_choice){
                SolutionType = "choice element";
            }else{
                SolutionType = "";
            }

            // Set the correct description for the category
            Description = category switch
            {
                ExerciseCategory.choice => "it should expect choice answers no longer than 4 words",
                ExerciseCategory.question => "the question must be clear and avoid any kind of ambiguity",
                ExerciseCategory.fill_in_the_blanks => "you must provide the original generated text about the topic with no remotion of words. **Ensure that the text is complete; it is very important for post processing purposes that the text do not contain any gap**",
                ExerciseCategory.conceptual => "the assignment should be designed to give a prompt for students to then develop in their on ways",
                ExerciseCategory.practical => "the assignment should be designed to give a prompt for students to then develop in their on ways",
                // question
                _ => "the question must be clear and avoid any kind of ambiguity",
            };
        }
    
        public override string ToString(){
            string result = ExercisesGenerationPrompt.A_Description() + '\n' + ExercisesGenerationPrompt.A_Resolution(Description, SolutionType, CorrectAnswersNumber.ToString());
            return result;
        }
    }
}