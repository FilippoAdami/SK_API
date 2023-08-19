// Import necessary namespaces from the ASP.NET Core framework
using System.Text.RegularExpressions;
using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

// Declare the namespace for the LessonGeneratorController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class LessonGeneratorController : ControllerBase
    {
        private readonly ILogger<LessonGeneratorController> _logger;
        private readonly IConfiguration _configuration;

        public LessonGeneratorController(ILogger<LessonGeneratorController> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // Define your Lesson POST action method here
        [HttpPost("generatelesson")]
        public async Task<IActionResult> GenerateLesson([FromHeader(Name = "ApiKey")] string apiKey, [FromBody] LessonRequestModel requestModel)
        { 
            var secretToken = _configuration["SECRET_TOKEN"];
            if (string.IsNullOrWhiteSpace(secretToken))
            {
                return BadRequest("Required configuration values are missing or empty.");
            }
            if (apiKey != secretToken)
            {
                return Unauthorized();
            }
            var secretKey = _configuration["OPEAPI_SECRET_KEY"];
            var endpoint = _configuration["OPENAPI_ENDPOINT"];
            var model = _configuration["GPT_35_TURBO_DN"];
            
        //setting up the semantic kernel
            if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(model)){
                return BadRequest("Required configuration values are missing or empty.");
            }
            var SKbuilder = new KernelBuilder();
            SKbuilder.WithAzureChatCompletionService(
            model,  // Azure OpenAI Deployment Name
            endpoint, // Azure OpenAI Endpoint
            secretKey); // Azure OpenAI Key
            var kernel = SKbuilder.Build();

            //defining the prompt & generating the semantic function
            string prompt = @"You are a {{$level}} level professor that wants to create a lesson for his students to teach them the main concepts about {{$topic}}.
                            1) Generate a {{$level}} level academic text about {{$topic}}.
                            It has to be written using {{$level}} vocabulary. (we will call this text: 'OriginalText')
                            Output the text

                            The final output of your answer must be in the format:
                            Original text:
                            ...text...";
            var generate = kernel.CreateSemanticFunction(prompt, "generateLesson" ,"Lesson", "generate lesson", 1000 , requestModel.Temperature);
            //setting up the context
            var context = kernel.CreateNewContext();
            context["level"] = requestModel.Level.ToString();
            context["topic"] = requestModel.Topic;
            //generating the output using the LLM
            try
            {
                var result = await generate.InvokeAsync(context);
                var Date = DateOnly.FromDateTime(DateTime.Now);
                return Ok($"Date: {Date}\nTopic: {requestModel.Topic}\nLevel: {requestModel.Level.ToString()}\nTemperature: {requestModel.Temperature}\n{result.ToString()}");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}