namespace SK_API{
public class ExercisesInputModel{
    public string MacroSubject { get; set; }
    public string Title { get; set; }
    public TextLevel Level { get; set; }
    public TypeOfExercise TypeOfExercise { get; set; } 
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
    public ExercisesInputModel(string macroSubject, string title, TextLevel level, TypeOfExercise typeOfExercise, string learningObjective, BloomLevel bloomLevel, string material, TypeOfAssignment assignmentType, string topic, string language = "english", int correctAnswersNumber = 1, int distractorsNumber = 1, int easilyDiscardableDistractorsNumber = 1, double temperature = 0.1){
        MacroSubject = macroSubject;
        Title = title;
        Level = level;
        TypeOfExercise = typeOfExercise;
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

    public ExerciseCategory CategoryFromType(TypeOfExercise type)
    {
        return type switch
        {
            TypeOfExercise.multiple_choice => ExerciseCategory.choice,
            //TypeOfExercise.matching => ExerciseCategory.choice,
            TypeOfExercise.single_choice => ExerciseCategory.choice,
            //TypeOfExercise.ordering => ExerciseCategory.choice,

            TypeOfExercise.true_or_false => ExerciseCategory.question,
            TypeOfExercise.short_answer_question => ExerciseCategory.question,
            TypeOfExercise.open_question => ExerciseCategory.question,

            TypeOfExercise.fill_in_the_blanks => ExerciseCategory.fill_in_the_blanks,

            TypeOfExercise.essay => ExerciseCategory.conceptual,
            TypeOfExercise.debate => ExerciseCategory.conceptual,
            //TypeOfExercise.text_comprehension => ExerciseCategory.conceptual,
            TypeOfExercise.knoledge_exposition => ExerciseCategory.conceptual,
            TypeOfExercise.brainstorming => ExerciseCategory.conceptual,

            TypeOfExercise.non_written_material_analysis => ExerciseCategory.practical,
            TypeOfExercise.non_written_material_production => ExerciseCategory.practical,
            
            _ => ExerciseCategory.question,
        };
    }
}
}
