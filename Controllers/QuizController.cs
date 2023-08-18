// Import necessary namespaces from the ASP.NET Core framework
using System.Text.RegularExpressions;
using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

// Declare the namespace for the QuizExerciseController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class QuizExerciseController : ControllerBase
    {
        private readonly ILogger<QuizExerciseController> _logger;
        private readonly IConfiguration _configuration;

        public QuizExerciseController(ILogger<QuizExerciseController> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        private Quiz GetFG(string result, string topic, string level, int nedd, int n_o_d, double temperature){
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
            endIndex = result.ToString().IndexOf("Distractors:");
            string correct_answer = result.ToString()[startIndex..endIndex].Trim();
            //Console.WriteLine("Correct Answer:\n"+correct_answer);

            //extract the distractors
            startIndex = endIndex + "Distractors:".Length;
            endIndex = result.ToString().IndexOf("Easily discard distractors:");
            string distractors = result.ToString()[startIndex..endIndex].Trim();
            //Console.WriteLine("Distractors:\n");
            //split the distractors into individual items
            string[] distractorsArray = MyRegex().Split(distractors);
            for (int i = 0; i < distractorsArray.Length; i++)
            {
                distractorsArray[i] = distractorsArray[i].Trim();
                //Console.WriteLine(i+") "+distractorsArray[i]);
            }
            distractorsArray = distractorsArray.Skip(1).ToArray();


            //extract the easily discard distractors
            startIndex = endIndex + "Easily discard distractors:".Length;
            string easily_discard_distractors = result.ToString()[startIndex..].Trim();
            //Console.WriteLine("Easily Discard Distractors:\n");
            //split the easily discard distractors into individual items
            string[] easily_discard_distractorsArray = MyRegex().Split(easily_discard_distractors);
            for (int i = 0; i < easily_discard_distractorsArray.Length; i++)
            {
                easily_discard_distractorsArray[i] = easily_discard_distractorsArray[i].Trim();
                //Console.WriteLine(i+") "+easily_discard_distractorsArray[i]);
            }
            easily_discard_distractorsArray = easily_discard_distractorsArray.Skip(1).ToArray();

            //create an array with the correct answer and the distractors and the easily discard distractors
            string[] answers = new string[] {correct_answer}.Concat(distractorsArray).Concat(easily_discard_distractorsArray).ToArray();
            //shuffle the answers
            Random rnd = new Random();
            answers = answers.OrderBy(x => rnd.Next()).ToArray();            
            //Console.WriteLine("Correct Answer Index: "+Array.IndexOf(answers, correct_answer));

            //create the quiz object
            Quiz quiz = new Quiz(topic, level, nedd, n_o_d, temperature, question, Array.IndexOf(answers, correct_answer), answers);
            return quiz;
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
                var final = GetFG(result.ToString(), requestModel.Topic, requestModel.Level, requestModel.Nedd, requestModel.N_o_d, requestModel.Temperature);
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