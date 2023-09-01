// Import necessary namespaces from the ASP.NET Core framework
using System.Text.RegularExpressions;
using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

// Declare the namespace for the SummarizerController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class SummarizerController : ControllerBase
    {
        private readonly ILogger<SummarizerController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public SummarizerController(ILogger<SummarizerController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        // Define your Lesson POST action method here
        [HttpPost("summarizelesson")]
        public async Task<IActionResult> SummarizeLesson([FromHeader(Name = "ApiKey")] string apiKey, [FromBody] SummarizerRequestModel requestModel)
        { 
            int authenticated = _auth.Authenticate(apiKey);
            if (authenticated == 400)
            {
                return BadRequest("Required configuration values are missing or empty.");
            }
            else if (authenticated == 403)
            {
                return Unauthorized();
            }
            else if(authenticated==200){
                Console.WriteLine("Authenticated successfully");
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
            string prompt = @"You are a teacher that wants to do an end of chapter recap.
            Summarize in 3 sentences the provided lesson without removing formulas and important concepts:
            Lesson:
            {{$lesson}}";
            var generate = kernel.CreateSemanticFunction(prompt);
            //setting up the context
            var context = kernel.CreateNewContext();
            context["lesson"] = requestModel.Lesson;
            //generating the output using the LLM
            try
            {
                var result = await generate.InvokeAsync(context);
                var Date = DateOnly.FromDateTime(DateTime.Now);
                return Ok($"Date: {Date}\nSummary: {result.ToString()}");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}