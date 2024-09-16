namespace SK_API{
    public static class ActivityData
    {
        public static List<(ActivityCategory Category, List<(TypeOfActivity Type, List<BloomLevel> Levels)> Activities)> Activities { get; } = new List<(ActivityCategory, List<(TypeOfActivity, List<BloomLevel>)>)>
        {
            (ActivityCategory.fill_in_the_blanks, new List<(TypeOfActivity, List<BloomLevel>)>
            {
                (TypeOfActivity.information_search, new List<BloomLevel>
                {
                    BloomLevel.Remembering,
                    BloomLevel.Understanding,
                })
            }),

            (ActivityCategory.question, new List<(TypeOfActivity, List<BloomLevel>)>
            {
                (TypeOfActivity.open_question, new List<BloomLevel>
                {
                    BloomLevel.Understanding,
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
                (TypeOfActivity.short_answer_question, new List<BloomLevel>
                {
                    BloomLevel.Remembering,
                    BloomLevel.Understanding,
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                }),
                (TypeOfActivity.true_or_false, new List<BloomLevel>
                {
                    BloomLevel.Remembering,
                    BloomLevel.Understanding,
                }),
            }),

            (ActivityCategory.choice, new List<(TypeOfActivity, List<BloomLevel>)>
            {
                (TypeOfActivity.multiple_choice, new List<BloomLevel>
                {
                    BloomLevel.Remembering,
                    BloomLevel.Understanding,
                    BloomLevel.Applying,
                }),
                (TypeOfActivity.multiple_select, new List<BloomLevel>
                {
                    BloomLevel.Remembering,
                    BloomLevel.Understanding,
                    BloomLevel.Applying,
                })
            }),
            
            (ActivityCategory.conceptual, new List<(TypeOfActivity, List<BloomLevel>)>
            {
                (TypeOfActivity.essay, new List<BloomLevel>
                {
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
                (TypeOfActivity.knoledge_exposition, new List<BloomLevel>
                {
                    BloomLevel.Understanding,
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
                (TypeOfActivity.debate, new List<BloomLevel>
                {
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
                (TypeOfActivity.brainstorming, new List<BloomLevel>
                {
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
                (TypeOfActivity.group_discussion, new List<BloomLevel>
                {
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
                (TypeOfActivity.simulation, new List<BloomLevel>
                {
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
                (TypeOfActivity.inquiry_based_learning, new List<BloomLevel>
                {
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
            }),
    
            (ActivityCategory.practical, new List<(TypeOfActivity, List<BloomLevel>)>
            {
                (TypeOfActivity.non_written_material_analysis, new List<BloomLevel>
                {
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                }),
                (TypeOfActivity.non_written_material_production, new List<BloomLevel>
                {
                    BloomLevel.Creating,
                }),
                (TypeOfActivity.case_study_analysis, new List<BloomLevel>
                {
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                }),
                (TypeOfActivity.project_based_learning, new List<BloomLevel>
                {
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
                (TypeOfActivity.problem_solving_activity, new List<BloomLevel>
                {
                    BloomLevel.Applying,
                    BloomLevel.Analyzing,
                    BloomLevel.Evaluating,
                    BloomLevel.Creating,
                }),
            }),
        };

        public static (ActivityCategory Category, List<BloomLevel> Levels) GetCategoryAndLevels(TypeOfActivity TypeOfActivity)
        {
            foreach (var ActivityCategory in Activities)
            {
                foreach (var Activity in ActivityCategory.Activities)
                {
                    if (Activity.Type == TypeOfActivity)
                    {
                        return (ActivityCategory.Category, Activity.Levels);
                    }
                }
            }

            throw new ArgumentException("TypeOfActivity not found in the Activities data.");
        }
    
        public static List<TypeOfActivity> FilterActivitiesByBloomLevel(BloomLevel BloomLevel)
        {
            List<TypeOfActivity> ActivitiesList = new List<TypeOfActivity>();

            foreach (var Activity in Activities)
            {
                foreach (var Pair in Activity.Activities)
                {
                    if (Pair.Levels.Contains(BloomLevel))
                    {
                        ActivitiesList.Add(Pair.Type);
                    }
                }
            }

            return ActivitiesList;
        }

        public static List<TypeOfActivity> FilterActivitiesByCategory(ActivityCategory Category)
        {
            List<TypeOfActivity> ActivitiesList = new List<TypeOfActivity>();

            foreach (var Activity in Activities)
            {
                if (Activity.Category == Category)
                {
                    foreach (var Pair in Activity.Activities)
                    {
                        ActivitiesList.Add(Pair.Type);
                    }
                }
            }

            return ActivitiesList;
        }

    }

    public class TopicAndExplanation
    {
        public string Topic { get; set; }
        public TypeOfAssignment Type { get; set; }
        public string Description { get; set; }

        // Parameterless constructor
        public TopicAndExplanation() { }

        public TopicAndExplanation(string topic, TypeOfAssignment type, string description)
        {
            Topic = topic;
            Type = type;
            Description = description;
        }

        public override string ToString()
        {
            return $"Topic: {Topic}, Explanation: {Description}, Type of Activity suggested: {Type};\n";
        }
    }
}
