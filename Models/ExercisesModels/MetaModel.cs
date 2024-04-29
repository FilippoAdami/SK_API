namespace SK_API{
    public class MetaModel{
        public TypeOfExercise Type { get; set; }
        public string Examples { get; set;}
        public string Formatting { get; set; }
        public string Personification { get; set; }
        public string Ending { get; set;}
        public AssignmentModel Assignment { get; set; }
        public SolutionModel Solution { get; set; }
        
        // Constructor
        public MetaModel(TypeOfExercise type, ExerciseCategory category, AssignmentModel assignment, SolutionModel solution)
        {
            if ( assignment == null || solution == null)
            {
                throw new ArgumentNullException(nameof(assignment));
            }
            else
            {
                Type = type;
                
                Examples = GetExamples(type);
                Formatting = GetFormatting(category);

                Personification = ExercisesGenerationPrompt.Personification();
                Ending = ExercisesGenerationPrompt.Ending();

                Assignment = assignment;
                Solution = solution;
            }
        }

        public string GetExamples(TypeOfExercise type){
            // Get the correct string from the ExamplesStrings class for each type of exercise
            switch (type)
            {
                case TypeOfExercise.open_question:
                    return ExamplesStrings.OpenQuestion;
                case TypeOfExercise.short_answer_question:
                    return ExamplesStrings.ShortAnswerQuestion;
                case TypeOfExercise.true_or_false:
                    return ExamplesStrings.TrueOrFalse;
                case TypeOfExercise.information_search:
                    return ExamplesStrings.FillInTheBlanks;
                case TypeOfExercise.multiple_choice:
                    return ExamplesStrings.SingleChoice;
                case TypeOfExercise.multiple_select:
                    return ExamplesStrings.MultipleChoice;
                case TypeOfExercise.essay:
                    return ExamplesStrings.Essay;
                case TypeOfExercise.knoledge_exposition:
                    return ExamplesStrings.KnowledgeExposition;
                case TypeOfExercise.non_written_material_analysis:
                    return ExamplesStrings.NonWrittenMaterialAnalysis;
                case TypeOfExercise.non_written_material_production:
                    return ExamplesStrings.NonWrittenMaterialProduction;
                case TypeOfExercise.debate:
                    return ExamplesStrings.Debate;
                case TypeOfExercise.brainstorming:
                    return ExamplesStrings.Brainstorming;
                case TypeOfExercise.group_discussion:   
                    return ExamplesStrings.GroupDiscussion;
                case TypeOfExercise.case_study_analysis:
                    return ExamplesStrings.CaseStudyAnalysis;
                case TypeOfExercise.project_based_learning:
                    return ExamplesStrings.ProjectBasedLearning;
                case TypeOfExercise.problem_solving_activity:
                    return ExamplesStrings.ProblemSolvingActivity;
                case TypeOfExercise.simulation:
                    return ExamplesStrings.Simulation;
                case TypeOfExercise.inquiry_based_learning:
                    return ExamplesStrings.InquiryBasedLearning;
                default:
                    return ExamplesStrings.OpenQuestion;
            }
        }
        public string GetFormatting(ExerciseCategory type){
            switch (type)
            {
                case ExerciseCategory.fill_in_the_blanks:
                    return FormatStrings.MM_FillInTheBlanks;
                case ExerciseCategory.question:
                    return FormatStrings.MM_Question;
                case ExerciseCategory.choice:
                    return FormatStrings.MM_Choice;
                case ExerciseCategory.conceptual:
                    return FormatStrings.MM_Conceptual;
                case ExerciseCategory.practical:
                    return FormatStrings.MM_Practical;
                default:
                    return FormatStrings.MM_Conceptual;
            }
        }

        public string ToString(ExerciseCategory category){
            string result;
            if(category == ExerciseCategory.conceptual || category == ExerciseCategory.practical){
                result = ExercisesGenerationPrompt.Examples(Examples) + '\n' + ExercisesGenerationPrompt.Format(Formatting) + '\n' + Personification + '\n' + Assignment.ToString() + '\n' + Ending;
            }
            else{
                result = ExercisesGenerationPrompt.Examples(Examples) + '\n' + ExercisesGenerationPrompt.Format(Formatting) + '\n' + Personification + '\n' + Assignment.ToString() + '\n' + Solution.ToString(category) + '\n' + Ending;
            }
            return result;
        }
    }
}
