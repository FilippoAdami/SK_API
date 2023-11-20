using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SemanticFunctions;
using SK_API.Controllers;

public class Summarizer
{
    private readonly IConfiguration _configuration;
    private readonly Auth _auth;

    public Summarizer(IConfiguration configuration, Auth auth)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _auth = auth ?? throw new ArgumentNullException(nameof(auth));
    }

    public async Task<string> Summarize(string apiKey, string lesson, string level, int noW = 150)
    {
        int try_count = 0;
        string error = "";
        int authenticated = _auth.Authenticate(apiKey);
        if (authenticated == 400)
        {
            Console.WriteLine("Required configuration values are missing or empty.");
            return "Required configuration values are missing or empty.";
        }
        else if (authenticated == 403)
        {
            Console.WriteLine("Unauthorized");
            return "Unauthorized";
        }
        else if (authenticated == 200)
        {
            Console.WriteLine("Authenticated successfully");
        }

        var secretKey = _configuration["OPENAPI_SECRET_KEY"];
        var endpoint = _configuration["OPENAPI_ENDPOINT"];
        var model = _configuration["GPT_35_TURBO_DN"];

        // Setting up the semantic kernel
        if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(model))
        {
            return "Required configuration values are missing or empty.";
        }

        var SKbuilder = new KernelBuilder();
        SKbuilder.WithAzureChatCompletionService(model, endpoint, secretKey);
        var kernel = SKbuilder.Build();

        // Defining the prompt & generating the semantic function
        string prompt = @"You are a {{$level}} teacher that wants to do an end of chapter recap.
            Summarize in {{$n_o_w}} words the provided lesson for people that are attending your {{$level}} course keeping every important concept and every eventual formula:
            Lesson:
            {{$lesson}}
            You should return just the summary.";
        var generate = kernel.CreateSemanticFunction(prompt);

        // Setting up the context
        var context = kernel.CreateNewContext();
        context["lesson"] = lesson;
        context["level"] = level;
        context["n_o_w"] = noW.ToString();

        // Generating the output using the LLM
        while (try_count < 3){
            try
            {
                var result = await generate.InvokeAsync(context);
                Console.WriteLine("summary: " + result);
                return $"{result}";
            }
            catch (Exception e)
            {
                error = e.Message;
                try_count++;
            }
        }
        return error;
    }

    public async Task<string> Translate(string apiKey, string text, string language = "English"){
        int authenticated = _auth.Authenticate(apiKey);
        if (authenticated == 400)
        {
            Console.WriteLine("Required configuration values are missing or empty.");
            return "Required configuration values are missing or empty.";
        }
        else if (authenticated == 403)
        {
            Console.WriteLine("Unauthorized");
            return "Unauthorized";
        }
        else if (authenticated == 200)
        {
            Console.WriteLine("Authenticated successfully");
        }
        
        int try_count = 0;
        string error = "";

        var secretKey = _configuration["OPEAPI_SECRET_KEY"];
        var endpoint = _configuration["OPENAPI_ENDPOINT"];
        var model = _configuration["GPT_35_TURBO_DN"];

        // Setting up the semantic kernel
        if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(model))
        {
            return "Required configuration values are missing or empty.";
        }
        Console.WriteLine("input: "+ text);

        var SKbuilder = new KernelBuilder();
        SKbuilder.WithAzureChatCompletionService(model, endpoint, secretKey);
        var kernel = SKbuilder.Build();

        // Defining the prompt & generating the semantic function
        string prompt = @"You are a {{$language}} native speaker that wants to translate a text.
            Translate the following text in {{$language}} maintaining the exact same formatting:
            Text:
            {{$text}}
            You should return just the translated text.";
        var generate = kernel.CreateSemanticFunction(prompt);

        // Setting up the context
        var context = kernel.CreateNewContext();
        context["text"] = text;
        context["language"] = language;

        // Generating the output using the LLM
        while (try_count < 3){
            try
            {
                var result = await generate.InvokeAsync(context);
                Console.WriteLine("translation: " + result);
                return $"{result}";
            }
            catch (Exception e)
            {
                error = e.Message;
                try_count++;
            }
        }
        return error;
    }
}
