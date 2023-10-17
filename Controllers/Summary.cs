using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public async Task<string> Summarize(string apiKey, string lesson, int noW = 150)
    {
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

        var secretKey = _configuration["OPEAPI_SECRET_KEY"];
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
        string prompt = @"You are a teacher that wants to do an end of chapter recap.
            Summarize in {{$n_o_w}} words the provided lesson keeping every important concept and every eventual formula:
            Lesson:
            {{$lesson}}
            You should return just the summary.";
        var generate = kernel.CreateSemanticFunction(prompt);

        // Setting up the context
        var context = kernel.CreateNewContext();
        context["lesson"] = lesson;
        context["n_o_w"] = noW.ToString();

        // Generating the output using the LLM
        try
        {
            Console.WriteLine("lesson: " + lesson, "n_o_w: " + noW);
            var result = await generate.InvokeAsync(context);
            return $"{result}";
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }
}
