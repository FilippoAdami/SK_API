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
            if (typeOfExercise == TypeOfExercise.multiple_choice || typeOfExercise == TypeOfExercise.true_or_false || typeOfExercise == TypeOfExercise.short_answer_question){
                CorrectAnswersNumber = "one";
            } else if (typeOfExercise == TypeOfExercise.open_question || category == ExerciseCategory.conceptual || category == ExerciseCategory.practical){
                CorrectAnswersNumber = "multiple";
            } else if (typeOfExercise == TypeOfExercise.information_search){
                CorrectAnswersNumber = "at least " + (answers+2).ToString();
            }

            // Set the correct solution type for the type of exercise
            if (typeOfExercise == TypeOfExercise.information_search){
            SolutionType = "relevant information (domain specific knowledge / specific terms / concepts / numbers / formulas / code pieces / etc.)";
            }else if (typeOfExercise == TypeOfExercise.open_question){
                SolutionType = "long form answer";
            }else if (typeOfExercise == TypeOfExercise.short_answer_question){
                SolutionType = "short form answer (2 or 3 words max, taxatively no more than 4 words)";
            }else if (typeOfExercise == TypeOfExercise.true_or_false){
                SolutionType = "true/false + explanation";
            }else if (typeOfExercise == TypeOfExercise.multiple_choice || typeOfExercise == TypeOfExercise.multiple_select){
                SolutionType = "choice element";
            }else{
                SolutionType = "";
            }

            // Set the correct description for the category
            Description = category switch
            {
                ExerciseCategory.choice => "it expects concise and consistent choice answers",
                ExerciseCategory.question => "the question must be clear and avoid any kind of ambiguity",
                ExerciseCategory.fill_in_the_blanks => "you must generate a text about the topic like in the examples",
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