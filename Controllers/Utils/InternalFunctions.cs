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
        context["lesson"] = lesson;
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

}
