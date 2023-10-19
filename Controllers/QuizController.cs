// Import necessary namespaces from the ASP.NET Core framework
using System.Text.RegularExpressions;
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
        private readonly Auth _auth;

        public QuizExerciseController(ILogger<QuizExerciseController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        private Quiz GetFG(string result, string level, int nedd, int n_o_d, double temperature, bool type){
            //parsing the output
            int startIndex = 0;
            int endIndex = 0;
            string solution = "";
            string question = "";
            if(type){
                // Find the start and end of the original resolution
                startIndex = result.ToString().IndexOf("Resolution:") + "Resolution:".Length;
                endIndex = result.ToString().IndexOf("Correct answer:");
                solution = result.ToString()[startIndex..endIndex].Trim();
                //Console.WriteLine("Original text: \n"+solution);

                //extract the question
                startIndex = result.ToString().IndexOf("Problem:") + "Problem:".Length;
                endIndex = result.ToString().IndexOf("Resolution:");
                question = result.ToString()[startIndex..endIndex].Trim();

            }
            else{
                // Find the start and end of the original text
                startIndex = result.ToString().IndexOf("Original text:") + "Original text:".Length;
                endIndex = result.ToString().IndexOf("Question:");
                solution = result.ToString()[startIndex..endIndex].Trim();
                //Console.WriteLine("Original text: \n"+solution);

                //extract the question
                startIndex = endIndex + "Question:".Length;
                endIndex = result.ToString().IndexOf("Correct answer:");
                question = result.ToString()[startIndex..endIndex].Trim();
                //Console.WriteLine("Question:\n"+question);
            }

            //extract the correct answer
            startIndex = result.ToString().IndexOf("Correct answer:") + "Correct answer:".Length;
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
            Quiz quiz = new Quiz(level, nedd, n_o_d, temperature, question, Array.IndexOf(answers, correct_answer), answers, solution);
            return quiz;
        }

        // Define your QuizExercise POST action method here
        [HttpPost("generateexercise")]
        public async Task<IActionResult> GenerateQuizExercise([FromHeader(Name = "ApiKey")] string apiKey, [FromBody] QuizExerciseRequestModel requestModel)
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
            var secretKey = _configuration["OPENAPI_SECRET_KEY"];
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
        
        //extracting and summarizing the text
            // Create a FileOrUrl instance from the provided input.
            FileOrUrl source = new FileOrUrl(requestModel.Text);

            // Create an instance of the TextProcessor.
            TextProcessor textProcessor = new TextProcessor();

            // Create a Summarizer instance
            Summarizer summarizer = new(_configuration, _auth);

            // Call the method to extract text.
            string extractedText = textProcessor.ExtractTextFromFileOrUrl(source);
            
            var finalText = await summarizer.Summarize(apiKey, extractedText);

            //defining the prompt & generating the semantic function
            string prompt = @"You are a {{$level}} level professor that just gave a lesson. You now want to create a theoretical quiz exercise for your students.
                            1) The summary of your lesson is {{$lesson}} (we will call this text: 'OriginalText')
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
            string promptB = @"You are a {{$level}} level professor that just gave the followiong lesson:{{$lesson}}. You now want to create a practical quiz exercise for your students.
                            1) Generate a {{$level}} level problem with either a numeric or a formula solution where your students can apply what you just taught.
                            Output the problem.
                            2) Solve the problem following a step by step approach.
                            It has to be written using {{$level}} vocabulary and formulas. (we will call this text: 'Resolution')
                            Output the step by step resolution
                            3) Extract the numeric or formula solution from the resolution.
                            Output the solution
                            4) Generate {{$n_o_d}} distractors that are similar and in the same format and length as the correct answer. (If possible, avoid using multiples of 5).
                            NB: if the correct answer contains formulas or specific terms also the distractors must contains formulas or specific terms.
                            Output these distractors
                            5) Generate {{$nedd}} distractor that could be easily discarded by students
                            Output the distractors

                            The final output of your answer must be in the format:

                            Problem:
                            ...text...
                            Reolution:
                            ...step by step resolution...
                            Correct answer:
                            ...number/formula...
                            Distractors:
                            1)...list of {{$n_o_d}} distractors...
                            Easily discard distractors:
                            1)...list of {{$nedd}} easily discard distractors...";
            
            var generate = kernel.CreateSemanticFunction(prompt, "generateQuiz" ,"Quiz", "generate exercise", null , requestModel.Temperature);
            if (requestModel.Type){
                generate = kernel.CreateSemanticFunction(promptB, "generateQuizB" ,"QuizB", "generate exercise", null , requestModel.Temperature);
            }
            //setting up the context
            var context = kernel.CreateNewContext();
            context["level"] = requestModel.Level.ToString();
            context["lesson"] = finalText;
            context["nedd"] = requestModel.Nedd.ToString();
            context["n_o_d"] = requestModel.N_o_d.ToString();
            //generating the output using the LLM
            try
            {
                var result = await generate.InvokeAsync(context);
                var final = GetFG(result.ToString(), requestModel.Level.ToString(), requestModel.Nedd, requestModel.N_o_d, requestModel.Temperature, requestModel.Type);
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