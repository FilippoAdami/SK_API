namespace SK_API{
    public class MetaModel{
        public TypeOfActivity Type { get; set; }
        public string Examples { get; set;}
        public string Formatting { get; set; }
        public string Personification { get; set; }
        public string Ending { get; set;}
        public AssignmentModel Assignment { get; set; }
        public SolutionModel Solution { get; set; }
        
        // Constructor
        public MetaModel(TypeOfActivity type, ActivityCategory category, AssignmentModel assignment, SolutionModel solution)
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

                Personification = ActivityGenerationPrompt.Personification();
                Ending = ActivityGenerationPrompt.Ending();

                Assignment = assignment;
                Solution = solution;
            }
        }

        public string GetExamples(TypeOfActivity type){
            // Get the correct string from the ExamplesStrings class for each type of Activity
            switch (type)
            {
                case TypeOfActivity.open_question:
                    return ExamplesStrings.OpenQuestion;
                case TypeOfActivity.short_answer_question:
                    return ExamplesStrings.ShortAnswerQuestion;
                case TypeOfActivity.true_or_false:
                    return ExamplesStrings.TrueOrFalse;
                case TypeOfActivity.information_search:
                    return ExamplesStrings.FillInTheBlanks;
                case TypeOfActivity.multiple_choice:
                    return ExamplesStrings.SingleChoice;
                case TypeOfActivity.multiple_select:
                    return ExamplesStrings.MultipleChoice;
                case TypeOfActivity.essay:
                    return ExamplesStrings.Essay;
                case TypeOfActivity.knoledge_exposition:
                    return ExamplesStrings.KnowledgeExposition;
                case TypeOfActivity.non_written_material_analysis:
                    return ExamplesStrings.NonWrittenMaterialAnalysis;
                case TypeOfActivity.non_written_material_production:
                    return ExamplesStrings.NonWrittenMaterialProduction;
                case TypeOfActivity.debate:
                    return ExamplesStrings.Debate;
                case TypeOfActivity.brainstorming:
                    return ExamplesStrings.Brainstorming;
                case TypeOfActivity.group_discussion:   
                    return ExamplesStrings.GroupDiscussion;
                case TypeOfActivity.case_study_analysis:
                    return ExamplesStrings.CaseStudyAnalysis;
                case TypeOfActivity.project_based_learning:
                    return ExamplesStrings.ProjectBasedLearning;
                case TypeOfActivity.problem_solving_activity:
                    return ExamplesStrings.ProblemSolvingActivity;
                case TypeOfActivity.simulation:
                    return ExamplesStrings.Simulation;
                case TypeOfActivity.inquiry_based_learning:
                    return ExamplesStrings.InquiryBasedLearning;
                default:
                    return ExamplesStrings.OpenQuestion;
            }
        }
        public string GetFormatting(ActivityCategory type){
            switch (type)
            {
                case ActivityCategory.fill_in_the_blanks:
                    return FormatStrings.MM_FillInTheBlanks;
                case ActivityCategory.question:
                    return FormatStrings.MM_Question;
                case ActivityCategory.choice:
                    return FormatStrings.MM_Choice;
                case ActivityCategory.conceptual:
                    return FormatStrings.MM_Conceptual;
                case ActivityCategory.practical:
                    return FormatStrings.MM_Practical;
                default:
                    return FormatStrings.MM_Conceptual;
            }
        }

        public string ToString(ActivityCategory category){
            string result;
            if(category == ActivityCategory.conceptual || category == ActivityCategory.practical){
                result = ActivityGenerationPrompt.Examples(Examples) + '\n' + ActivityGenerationPrompt.Format(Formatting) + '\n' + Personification + '\n' + Assignment.ToString() + '\n' + Ending;
            }
            else{
                result = ActivityGenerationPrompt.Examples(Examples) + '\n' + ActivityGenerationPrompt.Format(Formatting) + '\n' + Personification + '\n' + Assignment.ToString() + '\n' + Solution.ToString(category) + '\n' + Ending;
            }
            return result;
        }
    }
}
