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

    public string CheckResponse(string final){
        Console.WriteLine("CheckResponse: " + final);
        string edit = final;
        // remove eventual ``json at the beginning and `` at the end of the string
        if (final.StartsWith("```json"))
        {
            edit = final.Replace("```json", "");
        } else if (final.StartsWith("``json"))
        {
            edit = final.Replace("``json", "");
        }
        final = edit;
        if (final.EndsWith("```"))
        {
            edit = final.Replace("```", "");
        } else if (final.EndsWith("``"))
        {
            edit = final.Replace("``", "");
        }
        edit = edit.Trim();
        // if the result is not enclosed in curly brackets, add them
        if (!edit.StartsWith("{"))
        {
            edit = "{" + edit;
        }
        if (!edit.EndsWith("}"))
        {
            edit = edit + "}";
        }
        
        return edit;
    }
}
