namespace SK_API{
    public static class ExerciseData
    {
        public static List<(ExerciseCategory Category, List<(TypeOfExercise Type, List<BloomLevel> Levels)> Exercises)> Exercises { get; } = new List<(ExerciseCategory, List<(TypeOfExercise, List<BloomLevel>)>)>
        {
            (ExerciseCategory.fill_in_the_blanks, new List<(TypeOfExercise, List<BloomLevel>)>
            {
                (TypeOfExercise.fill_in_the_blanks, new List<BloomLevel>
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
                (TypeOfExercise.single_choice, new List<BloomLevel>
                {
                    BloomLevel.Remembering,
                    BloomLevel.Understanding,
                    BloomLevel.Applying,
                }),
                (TypeOfExercise.multiple_choice, new List<BloomLevel>
                {
                    BloomLevel.Remembering,
                    BloomLevel.Understanding,
                    BloomLevel.Applying,
                })
            }),
            
            (ExerciseCategory.conceptual, new List<(TypeOfExercise, List<BloomLevel>)>
            {
                (TypeOfExercise.debate, new List<BloomLevel>
                {
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
                (TypeOfExercise.essay, new List<BloomLevel>
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
                (TypeOfExercise.knoledge_exposition, new List<BloomLevel>
                {
                    BloomLevel.Remembering,
                    BloomLevel.Understanding,
                    BloomLevel.Applying,
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

        public TopicAndExplanation(string topic, TypeOfAssignment type, string explanation)
        {
            Topic = topic;
            Type = type;
            Explanation = explanation;
        }
    }
}
