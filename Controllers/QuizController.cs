// Import necessary namespaces from the ASP.NET Core framework
using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

// Declare the namespace for the QuizExerciseController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public class QuizExerciseController : ControllerBase
    {
        private readonly ILogger<QuizExerciseController> _logger;
        private readonly IConfiguration _configuration;

        public QuizExerciseController(ILogger<QuizExerciseController> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // Define your QuizExercise POST action method here
        [HttpPost("generateexercise")]
        public async Task<IActionResult> GenerateQuizExercise([FromHeader(Name = "ApiKey")] string apiKey, [FromBody] QuizExerciseRequestModel requestModel)
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
            string prompt = @"You are a {{$level}} level professor that wants to create a quiz exercise for his students.
                            1) Generate a {{$level}} level informative text about {{$topic}}. 
                            The text must be 100 words long.
                            It has to be written using {{$level}} vocabulary. (we will call this text: 'OriginalText')
                            Output the text
                            2) Using Original text as context, extract from that text one important concept and generate 1 question about that topic.
                            Output the question.
                            3) Generate one correct answers for the question.
                            Output the answers
                            4) Generate {{$n_o_d}} distractors that are similar and in the same format and length as the correct one
                            NB: if the correct answer contains formulas or specific terms also the distractors must contains formulas or specific terms
                            Output these distractors
                            5) Generate {{$nedd}} distractor that could be easily discarded by students
                            Output the distractors

                            The final output of your answer must be in the format:

                            Original text:
                            ...text...
                            Question:
                            ...question...
                            Correct answer:
                            ...correct answer...
                            Distractors:
                            1)...list of {{$n_o_d}} distractors...
                            Easily discard distractors:
                            1)...list of {{$nedd}} easily discard distractors...";
            var generate = kernel.CreateSemanticFunction(prompt);
            //setting up the context
            var context = kernel.CreateNewContext();
            context["level"] = requestModel.Level;
            context["topic"] = requestModel.Topic;
            context["nedd"] = requestModel.Nedd.ToString();
            context["n_o_d"] = requestModel.N_o_d.ToString();
            context["temperature"] = "0.0";
            //generating the output using the LLM
            try
            {
                var result = await generate.InvokeAsync(context);
                return Ok(result.ToString());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}