using Microsoft.SemanticKernel;
using SK_API;

public class InternalFunctions
{
    public async Task<string> GenerateMaterial(string topic, string info, string level, IKernel kernel, int noW = 150)
    {
        // Defining the prompt & generating the semantic function
        string prompt = InternalPrompts.MaterialGenerationPrompt;
        var generate = kernel.CreateSemanticFunction(prompt);

        // Setting up the context
        var context = kernel.CreateNewContext();
        context["topic"] = topic;
        context["learning_objective"] = info;
        context["level"] = level;
        context["number_of_words"] = noW.ToString();

        // Generating the output using the LLM
        try
        {
            var result = await generate.InvokeAsync(context);
            return $"{result}";
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    public async Task<string> Summarize(string lesson, string level, IKernel kernel, int noW = 150)
    {
        // Defining the prompt & generating the semantic function
        string prompt = InternalPrompts.TextSummarizationPrompt;
        var generate = kernel.CreateSemanticFunction(prompt);
        // Setting up the context
        var context = kernel.CreateNewContext();
        context["material"] = lesson;
        context["level"] = level;
        context["n_o_w"] = noW.ToString();

        // Generating the output using the LLM
        try
        {
            var result = await generate.InvokeAsync(context);
            return $"{result}";
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    public async Task<string> Translate(IKernel kernel, string text, string language = "english"){

        // Defining the prompt & generating the semantic function
        string prompt = InternalPrompts.TextTranslationPrompt;
        var generate = kernel.CreateSemanticFunction(prompt);

        // Setting up the context
        var context = kernel.CreateNewContext();
        context["json"] = text;
        context["language"] = language;

        // Generating the output using the LLM
        try
        {
            var result = await generate.InvokeAsync(context);
            //Console.WriteLine("translation: " + result);
            return $"{result}";
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    public string InsertPromptIntoJSON(string json, string prompt){
        // inject a new 'Prompt' string element as first element of the JSON string
        //string newJson = json.Insert(0, $"***{prompt}***");
        return json;
    }

public string CheckResponse(string final)
{
    Console.WriteLine("Response: " + final);
    string edit = final;

    // Remove eventual `json at the beginning and ` at the end of the string
    if (edit.StartsWith("```json", StringComparison.Ordinal))
    {
        edit = edit.Substring(7).TrimStart();  // Remove ```json and trim any leading whitespace
    }
    else if (edit.StartsWith("``json", StringComparison.Ordinal))
    {
        edit = edit.Substring(6).TrimStart();  // Remove ``json and trim any leading whitespace
    }

    if (edit.EndsWith("```", StringComparison.Ordinal))
    {
        edit = edit.Substring(0, edit.Length - 3).TrimEnd();  // Remove ``` and trim any trailing whitespace
    }
    else if (edit.EndsWith("``", StringComparison.Ordinal))
    {
        edit = edit.Substring(0, edit.Length - 2).TrimEnd();  // Remove `` and trim any trailing whitespace
    }

    // Remove any new lines at the beginning or end
    edit = edit.Trim('\n', '\r');

    // If the result is not enclosed in curly brackets, add them
    if (!edit.StartsWith("{", StringComparison.Ordinal) && !edit.StartsWith("[", StringComparison.Ordinal))
    {
        edit = "{" + edit;
    }
    if (!edit.EndsWith("}", StringComparison.Ordinal) && !edit.EndsWith("]", StringComparison.Ordinal))
    {
        edit = edit + "}";
    }

    Console.WriteLine("FinalCheckResponse: " + edit);
    return edit;
}
}
