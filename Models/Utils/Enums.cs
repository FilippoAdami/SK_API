namespace SK_API
{
    public enum TypeOfActivity
    {
        open_question,
        short_answer_question,
        true_or_false,

        information_search,
        //matching
        //ordering
        multiple_choice,
        multiple_select,

        essay,
        knoledge_exposition,
        //text_comprehension,
        debate,
        brainstorming,
        group_discussion,
        simulation,
        inquiry_based_learning,

        non_written_material_analysis,
        non_written_material_production,
        case_study_analysis,
        project_based_learning,
        problem_solving_activity

        //conceptual maps,
        //graphs
    }
    public enum ActivityCategory
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
    public enum TypeOfAssessment
    {
        peer_review,
        self_assessment,
        teacher_assessment,
        automated_assessment
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