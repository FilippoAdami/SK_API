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

        private Quiz GetFG(string language, string result, string level, int nedd, int n_o_d, double temperature, string category, bool type, int noca){
            //parsing the output
            int startIndex = 0;
            int endIndex = 0;
            string solution = "";
            string question = "";
            if(type){
                // Find the start and end of the original resolution
                startIndex = result.ToString().IndexOf("resolution:", StringComparison.OrdinalIgnoreCase) + "Resolution:".Length;
                endIndex = result.ToString().IndexOf("correct answer:", StringComparison.OrdinalIgnoreCase);
                solution = result.ToString()[startIndex..endIndex].Trim();
                Console.WriteLine("original text: \n"+solution);

                //extract the question
                startIndex = result.ToString().IndexOf("problem:", StringComparison.OrdinalIgnoreCase) + "Problem:".Length;
                endIndex = result.ToString().IndexOf("resolution:", StringComparison.OrdinalIgnoreCase);
                question = result.ToString()[startIndex..endIndex].Trim();

            }
            else{
                // Find the start and end of the original text
                startIndex = result.ToString().IndexOf("Original text:", StringComparison.OrdinalIgnoreCase) + "Original text:".Length;
                Console.WriteLine(startIndex);
                endIndex = result.ToString().IndexOf(" question:", StringComparison.OrdinalIgnoreCase);
                Console.WriteLine(endIndex);
                solution = result.ToString()[startIndex..(endIndex-category.Length-1)].Trim();
                Console.WriteLine("Original text: \n"+solution);

                //extract the question
                startIndex = endIndex + "Question:".Length;
                endIndex = result.ToString().IndexOf("Correct answers:", StringComparison.OrdinalIgnoreCase);
                question = result.ToString()[startIndex..endIndex].Trim();
                Console.WriteLine("Question:\n"+question);
            }

            //extract the correct answer
            startIndex = result.ToString().IndexOf("correct answer", StringComparison.OrdinalIgnoreCase) + "Correct answers:".Length;
            endIndex = result.ToString().IndexOf("distractors:", StringComparison.OrdinalIgnoreCase);
            string correct_answers = result.ToString()[startIndex..endIndex].Trim();
            Console.WriteLine("Correct Answer:");
            //split the correct answer into individual items
            string[] correct_answersArray = MyRegex().Split(correct_answers);
            for (int i = 0; i < correct_answersArray.Length; i++)
            {
                correct_answersArray[i] = correct_answersArray[i].Trim();
                Console.WriteLine(i+") "+correct_answersArray[i]);
            }
            correct_answersArray = correct_answersArray.Skip(1).ToArray();
            //if there are more correct answers than requested, remove the last ones
            if (correct_answersArray.Length>noca){
                correct_answersArray = correct_answersArray.Take(noca).ToArray();
            }

            //extract the distractors
            startIndex = endIndex + "Distractors:".Length;
            endIndex = result.ToString().IndexOf("easily discardable distractors:", StringComparison.OrdinalIgnoreCase);
            string distractors = result.ToString()[startIndex..endIndex].Trim();
            Console.WriteLine("Distractors:");
            //split the distractors into individual items
            string[] distractorsArray = MyRegex().Split(distractors);
            for (int i = 0; i < distractorsArray.Length; i++)
            {
                distractorsArray[i] = distractorsArray[i].Trim();
                Console.WriteLine(i+") "+distractorsArray[i]);
            }
            distractorsArray = distractorsArray.Skip(1).ToArray();

            //extract the easily discard distractors
            startIndex = endIndex + "easily discardable distractors:".Length;
            //set the endIdex at the index of the first "\n" character after the last ")" character
            endIndex = result.ToString().LastIndexOf(")");
            if (result.ToString().IndexOf("[end answer]")>-1){
                endIndex = result.ToString().IndexOf("[end answer]");
            } else if(result.ToString().IndexOf("Note:", endIndex)>-1){
                endIndex = result.ToString().IndexOf("Note:", endIndex);
            }else if (result.ToString().IndexOf("\n", endIndex)>-1){
                endIndex = result.ToString().IndexOf("\n", endIndex);    
            }   
            else{
                endIndex = result.ToString().Length;
            }
            string easily_discard_distractors = result.ToString()[startIndex..endIndex].Trim();
            Console.WriteLine("Easily Discard Distractors:");
            //split the easily discard distractors into individual items
            string[] easily_discard_distractorsArray = MyRegex().Split(easily_discard_distractors);
            for (int i = 0; i < easily_discard_distractorsArray.Length; i++)
            {
                easily_discard_distractorsArray[i] = easily_discard_distractorsArray[i].Trim();
                Console.WriteLine(i+") "+easily_discard_distractorsArray[i]);
            }
            easily_discard_distractorsArray = easily_discard_distractorsArray.Skip(1).ToArray();

            //create an array with the correct answer and the distractors and the easily discard distractors
            string serve = "";
            string[] answers = new string[] {serve}.Concat(distractorsArray).Concat(easily_discard_distractorsArray).ToArray();
            //remove the empty string at the beginning of the array
            answers = answers.Skip(1).ToArray();
            //add the correct answers to the answers array
            answers = answers.Concat(correct_answersArray).ToArray();
            //print the answers
            Console.WriteLine("Answers:");
            //shuffle the answers
            Random rnd = new Random();
            answers = answers.OrderBy(x => rnd.Next()).ToArray();  
            for (int i = 0; i < answers.Length; i++)
            {
                Console.WriteLine(i+") "+answers[i]);
            }          
            //print the correct answers indexes into a string
            string correct_answers_string = "";
            for (int i = 0; i < correct_answersArray.Length; i++)
            {
                correct_answers_string += Array.IndexOf(answers, correct_answersArray[i]) + ", ";
            }
            Console.WriteLine("Correct answers indexes: "+correct_answers_string);

            //create the quiz object
            Quiz quiz = new Quiz(language, level, nedd, n_o_d, temperature, category, question, correct_answers_string, answers, solution);
            return quiz;
        }

        // Define your QuizExercise POST action method here
        [HttpPost("generateexercise")]
        public async Task<IActionResult> GenerateQuizExercise([FromHeader(Name = "ApiKey")] string apiKey, [FromBody] QuizExerciseRequestModel requestModel)
        { 
            int try_count = 0;
            string error = "";
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
        
        //extracting and summarizing the text
            // Create a FileOrUrl instance from the provided input.
            string source = requestModel.Text;

            // Create an instance of the TextProcessor.
            TextProcessor textProcessor = new TextProcessor();

            // Create a Summarizer instance
            Summarizer summarizer = new(_configuration, _auth);

            // Call the method to extract text.
            string extractedText = textProcessor.ExtractTextFromFileOrUrl(source);
            
            var finalText = await summarizer.Summarize(apiKey, extractedText, requestModel.Level.ToString());

            //defining the prompt & generating the semantic function
            string prompt;
            string example;
            string type_of_answer;
            string type_of_exercise;
            string format;
            if (requestModel.Type){
                type_of_exercise = "problem with either a numeric or a formula solution";
                type_of_answer = "final answer to the problem, following a step by step approach";
                example = @"[start of answer]
Original text: ...summary of the lesson...
Problem:
A ball is thrown vertically upward from the ground with an initial velocity of 20 m/s. Neglecting air resistance, calculate the time it takes for the ball to reach its maximum height. 
Resolution:
Given: 
Initial velocity, u = 20 m/s
Final velocity, v = 0 m/s (at maximum height)
Acceleration, a = -9.8 m/s^2 (due to gravity)
Time taken, t = ?
We can use the following kinematic equation to solve for time taken:
v = u + at
At maximum height, the final velocity of the ball is 0 m/s. Therefore, we can rearrange the equation to solve for time taken:
t = (v - u) / a
Substituting the given values, we get:
t = (0 - 20) / (-9.8)
t = 2.04 seconds
Therefore, it takes 2.04 seconds for the ball to reach its maximum height.
Correct answer:
@)2.04 seconds
Distractors:
@) 1.96 seconds
@) 2.10 seconds
@) 2.00 seconds
Easily discardeble distractors:
@) 5.00 seconds
[end of answer]";
                format = @"[start of answer]
Original text:
Problem:
Reolution:
...step by step answer extraction...
Correct answer:
@)...exactly one answer that can be either a number or a formula...
Distractors:
@)...list of {{$n_o_d}} distractors...
Easily Discardable Distractors:
@)...list of {{$nedd}} easily discard distractors...
[end of answer]";            
            }
            else{
                type_of_exercise = "theoretical "+requestModel.Category.ToString()+" quiz that has exactly "+requestModel.N_o_ca.ToString()+" correct answers";
                type_of_answer = requestModel.N_o_ca.ToString()+" "+requestModel.Category.ToString()+" theoretical quiz answer";
                example = @"-Factual Knowledge type: 
                            Question: Which countries were not involved in World War II? 
                            Correct Answers: 
                            @)India 
                            @)Quatar
                            Distractors: 
                            @)Russia 
                            @)Brazil
                            Easily Discardable Distractors: 
                            @)Germany

                            -Understanding of Concepts type: 
                            Question: Which of the following statements about free fall are true?  
                            Correct Answers: 
                            @)Considering air resistance null, a feather and a rock would fall at the same speed. 
                            @)If you jump on the Moon, you will stay in the air longer rather than jumping  on Earth.
                            Distractors: 
                            @)An object on the ground is not affected by the force of gravity as it does not move.
                            Easily Discardable Distractors: 
                            @)In space, if you throw an object, its speed will decelerate and the object will eventually stop.

                            -Application of Skills type: 
                            Question: If you were a historian researching the social impact of World War II, which of the following sources would be most relevant?  
                            Correct Answers: 
                            @)Letters from soldiers to their families
                            Distractors: 
                            @)Military strategy documents
                            Easily Discardable Distractors: 
                            @)Modern movies about World War II 
                            @)Fiction books set during World War II 

                            -Analysis and Evaluation type: 
                            Question: Which of the following formulas represent a paraboloid ? 
                            Correct Answers:
                            @)z=x^2+y^2 
                            @)x^2/4+4y^2=z 
                            @)x^2+3y^2-z=0 
                            Distractors:
                            @)
                            Easily Discardable distractors: 
                            @)z=4x^3+3";
                format = @"[start of answer]
                            Original text:
                            ..text...
                            {{$type}} Question:
                            ...question?...
                            Correct answers:
                            @)...list of {{$n_o_ca}} correct answers...
                            Distractors:
                            @)...list of  {{$n_o_d}} distractors...
                            Easily Discardable Distractors:
                            @)...list of {{$nedd}} easily discard distractors...
                            [end of answer]";                           
            }
            prompt = new Prompt(type_of_answer, type_of_exercise, example, format).ToString();
            Console.WriteLine(prompt);
            //setting up the context
            var context = kernel.CreateNewContext();
            context["level"] = requestModel.Level.ToString();
            context["text"] = finalText;
            context["n_o_ca"] = requestModel.N_o_ca.ToString();
            context["nedd"] = requestModel.Nedd.ToString();
            context["n_o_d"] = requestModel.N_o_d.ToString();
            context["type"] = requestModel.Category.ToString();
            var generate = kernel.CreateSemanticFunction(prompt, "generateQuiz" ,"Quiz", "generate exercise", null , requestModel.Temperature);
            //generating the output using the LLM
            while (try_count<3){
                try
                {
                    var result = await generate.InvokeAsync(context);
                    Console.WriteLine(result.ToString());
                    var final = GetFG(requestModel.Language, result.ToString(), requestModel.Level.ToString(), requestModel.Nedd, requestModel.N_o_d, requestModel.Temperature, requestModel.Category.ToString(), requestModel.Type, requestModel.N_o_ca);
                    if (requestModel.Language=="English"){
                        return Ok(final.ToString());
                    }
                    else{
                        var translated = await summarizer.Translate(apiKey, final.ToString(), requestModel.Language);
                        return Ok(translated);
                    }
                }
                catch (Exception e)
                {
                    try_count++;
                    error = e.Message;
                }
            }
            return BadRequest(error);
        }
        [GeneratedRegex("\\@+\\)")]
        private static partial Regex MyRegex();
    }
}