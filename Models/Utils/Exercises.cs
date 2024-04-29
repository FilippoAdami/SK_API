namespace SK_API{
    public static class ExerciseData
    {
        public static List<(ExerciseCategory Category, List<(TypeOfExercise Type, List<BloomLevel> Levels)> Exercises)> Exercises { get; } = new List<(ExerciseCategory, List<(TypeOfExercise, List<BloomLevel>)>)>
        {
            (ExerciseCategory.fill_in_the_blanks, new List<(TypeOfExercise, List<BloomLevel>)>
            {
                (TypeOfExercise.information_search, new List<BloomLevel>
                {
                    BloomLevel.Remembering,
                    BloomLevel.Understanding,
                })
            }),

            (ExerciseCategory.question, new List<(TypeOfExercise, List<BloomLevel>)>
            {
                (TypeOfExercise.open_question, new List<BloomLevel>
                {
                    BloomLevel.Remembering,
                    BloomLevel.Understanding,
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
                (TypeOfExercise.short_answer_question, new List<BloomLevel>
                {
                    BloomLevel.Remembering,
                    BloomLevel.Understanding,
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                }),
                (TypeOfExercise.true_or_false, new List<BloomLevel>
                {
                    BloomLevel.Remembering,
                    BloomLevel.Understanding,
                }),
            }),

            (ExerciseCategory.choice, new List<(TypeOfExercise, List<BloomLevel>)>
            {
                (TypeOfExercise.multiple_choice, new List<BloomLevel>
                {
                    BloomLevel.Remembering,
                    BloomLevel.Understanding,
                    BloomLevel.Applying,
                }),
                (TypeOfExercise.multiple_select, new List<BloomLevel>
                {
                    BloomLevel.Remembering,
                    BloomLevel.Understanding,
                    BloomLevel.Applying,
                })
            }),
            
            (ExerciseCategory.conceptual, new List<(TypeOfExercise, List<BloomLevel>)>
            {
                (TypeOfExercise.essay, new List<BloomLevel>
                {
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
                (TypeOfExercise.knoledge_exposition, new List<BloomLevel>
                {
                    BloomLevel.Remembering,
                    BloomLevel.Understanding,
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
                (TypeOfExercise.debate, new List<BloomLevel>
                {
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
                (TypeOfExercise.brainstorming, new List<BloomLevel>
                {
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
                (TypeOfExercise.group_discussion, new List<BloomLevel>
                {
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
                (TypeOfExercise.simulation, new List<BloomLevel>
                {
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
                (TypeOfExercise.inquiry_based_learning, new List<BloomLevel>
                {
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
            }),
    
            (ExerciseCategory.practical, new List<(TypeOfExercise, List<BloomLevel>)>
            {
                (TypeOfExercise.non_written_material_analysis, new List<BloomLevel>
                {
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                }),
                (TypeOfExercise.non_written_material_production, new List<BloomLevel>
                {
                    BloomLevel.Creating,
                }),
                (TypeOfExercise.case_study_analysis, new List<BloomLevel>
                {
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                }),
                (TypeOfExercise.project_based_learning, new List<BloomLevel>
                {
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
                (TypeOfExercise.problem_solving_activity, new List<BloomLevel>
                {
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
            }),
        };

        public static (ExerciseCategory Category, List<BloomLevel> Levels) GetCategoryAndLevels(TypeOfExercise typeOfExercise)
        {
            foreach (var exerciseCategory in Exercises)
            {
                foreach (var exercise in exerciseCategory.Exercises)
                {
                    if (exercise.Type == typeOfExercise)
                    {
                        return (exerciseCategory.Category, exercise.Levels);
                    }
                }
            }

            throw new ArgumentException("TypeOfExercise not found in the Exercises data.");
        }
    
    }

    public class TopicAndExplanation
    {
        public string Topic { get; set; }
        public TypeOfAssignment Type { get; set; }
        public string Explanation { get; set; }

        // Parameterless constructor
        public TopicAndExplanation() { }

        public TopicAndExplanation(string topic, TypeOfAssignment type, string explanation)
        {
            Topic = topic;
            Type = type;
            Explanation = explanation;
        }

        public override string ToString()
        {
            return $"Topic: {Topic}, Explanation: {Explanation}, Type of exercise suggested: {Type};\n";
        }
    }
}
