namespace SK_API
{    
    public enum TypeOfExercise
    {
        open_question,
        short_answer_question,
        true_or_false,

        fill_in_the_blanks,
        
        single_choice,
        multiple_choice,

        //matching
        //ordering

        // text comprehension
        debate,
        essay,
        brainstorming,
        knoledge_exposition,

        non_written_material_analysis,
        non_written_material_production
        
        //conceptual maps
        // graphs
    }
    public enum ExerciseCategory
    {
        fill_in_the_blanks,
        question,
        choice,
        conceptual,
        practical
    }
    public enum TypeOfAssignment
    {
        theoretical,
        code,
        problem_resolution,
    }
    public enum TextLevel
    {
        primary_school,
        middle_school,
        high_school,
        college,
        academy
    }
    public enum BloomLevel
    {
        Remembering,
        Understanding,
        Applying,
        Analyzing,
        Evaluating,
        Creating
    }
}