namespace SK_API
{
    public enum TextLevel
    {
        primary_school,
        middle_school,
        high_school,
        college,
        academy
    }

    public enum TextType
    {
        narrative,
        expository,
        argumentative,
        informative, 
        research_paper, 
        descriptive, 
        scientific, 
        scientific_with_formulas_and_theorems
    }

    public enum QuestionCategory
    {
        FactualKnowledge,
        Understanding_of_concepts,
        Application_of_skills,
        Analysys_and_evaluation
    }

    public enum TypeOfQuestion
    {
        Open,
        ShortAnswer,
        TrueFalse
    }

    public enum ExperienceLevel
    {
        Beginner,
        Intermediate,
        Advanced
    }

    public enum Dimension
    {
        Small,
        Medium,
        Large
    }

    public enum BloomLevel
    {
        Remember,
        Understand,
        Apply,
        Analyze,
        Evaluate,
        Create
    }
}