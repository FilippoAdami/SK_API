namespace SK_API{
public class ExercisesInputModel{
    public string MacroSubject { get; set; }
    public string Title { get; set; }
    public TextLevel Level { get; set; }
    public TypeOfActivity TypeofActivity { get; set; } 
    public string LearningObjective { get; set; }
    public BloomLevel BloomLevel { get; set; }
    public string Language { get; set; }
    public string Material { get; set; }

    public int CorrectAnswersNumber { get; set; }
    public int DistractorsNumber { get; set; }
    public int EasilyDiscardableDistractorsNumber { get; set; }

    public TypeOfAssignment AssignmentType { get; set; }
    public string Topic { get; set; }

    public double Temperature { get; set; }
    
    public ExercisesInputModel(string macroSubject, string title, TextLevel level, TypeOfActivity typeofActivity, string learningObjective, BloomLevel bloomLevel, string material, TypeOfAssignment assignmentType, string topic, string language = "english", int correctAnswersNumber = 1, int distractorsNumber = 1, int easilyDiscardableDistractorsNumber = 1, double temperature = 0.1){
        MacroSubject = macroSubject;
        Title = title;
        Level = level;
        TypeofActivity = typeofActivity;
        LearningObjective = learningObjective;
        BloomLevel = bloomLevel;
        Language = language ?? "english";
        Material = material;
        CorrectAnswersNumber = correctAnswersNumber;
        DistractorsNumber = distractorsNumber;
        EasilyDiscardableDistractorsNumber = easilyDiscardableDistractorsNumber;
        AssignmentType = assignmentType;
        Topic = topic;
        Temperature = temperature;
    }

    public ActivityCategory CategoryFromType(TypeOfActivity type)
    {
        return type switch
        {
            TypeOfActivity.multiple_select => ActivityCategory.choice,
            //TypeOfActivity.matching => ActivityCategory.choice,
            TypeOfActivity.multiple_choice => ActivityCategory.choice,
            //TypeOfActivity.ordering => ActivityCategory.choice,

            TypeOfActivity.true_or_false => ActivityCategory.question,
            TypeOfActivity.short_answer_question => ActivityCategory.question,
            TypeOfActivity.open_question => ActivityCategory.question,

            TypeOfActivity.information_search => ActivityCategory.fill_in_the_blanks,

            TypeOfActivity.essay => ActivityCategory.conceptual,
            //TypeOfActivity.text_comprehension => ActivityCategory.conceptual,
            TypeOfActivity.knoledge_exposition => ActivityCategory.conceptual,
            TypeOfActivity.debate => ActivityCategory.conceptual,
            TypeOfActivity.brainstorming => ActivityCategory.conceptual,
            TypeOfActivity.group_discussion => ActivityCategory.conceptual,
            TypeOfActivity.simulation => ActivityCategory.conceptual,
            TypeOfActivity.inquiry_based_learning => ActivityCategory.conceptual,

            TypeOfActivity.non_written_material_analysis => ActivityCategory.practical,
            TypeOfActivity.non_written_material_production => ActivityCategory.practical,
            TypeOfActivity.case_study_analysis => ActivityCategory.practical,
            TypeOfActivity.project_based_learning => ActivityCategory.practical,
            TypeOfActivity.problem_solving_activity => ActivityCategory.practical,
            
            _ => ActivityCategory.question,
        };
    }
}
}
