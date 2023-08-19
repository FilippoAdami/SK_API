// Import necessary namespaces from the ASP.NET Core framework
using System.Text.RegularExpressions;
using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

// Declare the namespace for the QuestionExerciseController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class QuestionExerciseController : ControllerBase
    {
        private readonly ILogger<QuestionExerciseController> _logger;
        private readonly IConfiguration _configuration;

        public QuestionExerciseController(ILogger<QuestionExerciseController> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        private OpenQuestion GetFG(string result, string topic, string level, double temperature){
            //parsing the output
            // Find the start and end of the original text
            int startIndex = result.ToString().IndexOf("Original text:") + "Original text:".Length;
            int endIndex = result.ToString().IndexOf("Question:");
            string original_text = result.ToString()[startIndex..endIndex].Trim();
            //Console.WriteLine("Original text: \n"+original_text);

            //extract the question
            startIndex = endIndex + "Question:".Length;
            endIndex = result.ToString().IndexOf("Correct answer:");
            string question = result.ToString()[startIndex..endIndex].Trim();
            //Console.WriteLine("Question:\n"+question);

            //extract the correct answer
            startIndex = endIndex + "Correct answer:".Length;
            string correct_answer = result.ToString()[startIndex..].Trim();
            //Console.WriteLine("Correct Answer:\n"+correct_answer);

            //create the question object
            OpenQuestion openQuestion = new OpenQuestion(topic, level, temperature, question, correct_answer);
            return openQuestion;
        }

        // Define your QuestionExcercise POST action method here
        [HttpPost("generateexercise")]
        public async Task<IActionResult> GenerateQuestionExcercise([FromHeader(Name = "ApiKey")] string apiKey, [FromBody] QuestionExcerciseRequestModel requestModel)
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
            string prompt = @"You are a {{$level}} level professor that wants to create a question for his students to see if they have learned the main concepts of {{$topic}}.
                            1) Generate a {{$level}} level informative text about {{$topic}}. 
                            The text must be 100 words long.
                            It has to be written using {{$level}} vocabulary. (we will call this text: 'OriginalText')
                            Output the text
                            2) Using Original text as context, extract from that text one important concept and generate 1 question about that topic.
                            Output the question.
                            3) Generate one possible correct answers for the question.
                            Output the answers

                            The final output of your answer must be in the format:
                            Original text:
                            ...text...
                            Question:
                            ...question...
                            Correct answer:
                            ...correct answer...";
            var generate = kernel.CreateSemanticFunction(prompt);
            //setting up the context
            var context = kernel.CreateNewContext();
            context["level"] = requestModel.Level;
            context["topic"] = requestModel.Topic;
            context["temperature"] = requestModel.Temperature.ToString();
            //generating the output using the LLM
            try
            {
                var result = await generate.InvokeAsync(context);
                var final = GetFG(result.ToString(), requestModel.Topic, requestModel.Level, requestModel.Temperature);
                return Ok(final.ToString());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [GeneratedRegex("\\d+\\)")]
        private static partial Regex MyRegex();
    }
}