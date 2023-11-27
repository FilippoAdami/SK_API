public class Prompt{
    public static string? Type_of_answer { get; set;}
    public static string Type_of_exercise { get; set; }
    public static string Example { get; set; }
    public static string Format { get; set; }
    // Constructor
    public Prompt(string type_of_answer, string type_of_exercise, string example, string format)
    {
        if (type_of_exercise == null || example == null || format == null)
        {
            throw new ArgumentNullException(nameof(type_of_exercise));
        }
        else
        {
            Type_of_answer = type_of_answer ?? "answer";
            Type_of_exercise = type_of_exercise;
            Example = example;
            Format = format;
        }
        //Console.WriteLine("Prompt created "+ Type_of_exercise);
    }

    public override string ToString(){
string personification =
@$"You are a {{{{$level}}}} level professor that just gave a lecture. Now you want to create a {Type_of_exercise} for your students about your lesson.
1) The summary of your lesson is {{{{$text}}}} (we will call this text: 'OriginalText')
Output the text";
string examples =
@$"2) Now consider this {Type_of_exercise} examples to understand the typology of exercise and the format:\n{Example}";
string extraction =
@$"3) Extract from your lesson one important concept and generate one {{{{$level}}}} {Type_of_exercise} about that topic following the examples format.
Output the {Type_of_exercise}";
string words =
@"3) Extract from your lesson all the proper nouns, dates/numbers and all the scientific/specific terminology.  (we will call this list: 'Words')
Output the list Words";
string distractors_list =
@"4) For each word in the list 'Words' generate one similar word that can be proposed as a distractor in a fill in the blanks exercise.  (we will call this list: 'Distractors') 
Output the list Distractors";
string answer =
@$"4)Extract the {Type_of_answer} for the {Type_of_exercise} like in the examples given.
Output the {Type_of_answer}";
string distractors =
@"5) Generate exactly {{$n_o_d}} distractors that are similar to the correct answer in both value and length. 
Keep in mind that if the correct answer contains formulas or specific terms, ensure that the distractors also include similar formulas or specific terms. 
The distractors should closely resemble the correct answer to provide a challenging set of options.
Output these distractors.";
string easily_discardable_distractors =
@"6) Generate {{$nedd}} distractors that are intentionally different from the correct answer. These distractors should be made easily discardable by containing common errors. 
If the correct answer contains formulas or specific terms, ensure that the distractors also include dissimilar formulas or specific terms.
Output these easily discardable distractors.";
string final_format =
@$"The final output of your answer must be in the format:
{Format}";
        if (Type_of_exercise.Equals("fill in the blanks"))
        {
            return $"" + personification + "\n" + words + "\n" + distractors_list + "\n" + final_format + "";
        }
        else if (Type_of_exercise.StartsWith("theoretical") || Type_of_exercise.StartsWith("problem"))
        {
            return $"" + personification + "\n" + examples + "\n" + extraction + "\n" + answer + "\n" + distractors + "\n" + easily_discardable_distractors + "\n" + final_format + "";
        }
        else if (Type_of_exercise.StartsWith("open") || Type_of_exercise.StartsWith("short") || Type_of_exercise.StartsWith("true"))
        {
            return $"" + personification + "\n" + examples + "\n" + extraction + "\n" + answer + "\n"+ final_format + "";
        }
        else return $"" + personification + "\n" + examples + "\n" + extraction + "\n" + answer + "\n" + final_format + "";
    }
}
