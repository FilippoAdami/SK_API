// Import necessary namespaces from the ASP.NET Core framework
using System.Text.RegularExpressions;
using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

// Declare the namespace for the AnswerCorrectorController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class AnswerCorrectorController : ControllerBase
    {
        private readonly ILogger<AnswerCorrectorController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public AnswerCorrectorController(ILogger<AnswerCorrectorController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        private CorrectedAnswer GetFG(string language, string result, string topic, string level, double temperature){
            //parsing the output
            // Find the accuracy
            int startIndex = result.ToString().IndexOf("Accuracy:") + "Accuracy:".Length;
            int endIndex = result.ToString().IndexOf("Correct answer:");
            string acc = result.ToString()[startIndex..endIndex].Trim();
            double accuracy = Convert.ToDouble(acc);
            if (accuracy == 0.2){
                accuracy = 0.0;
            } else if (accuracy == 0.4 || accuracy == 0.6){
                accuracy = 0.5;
            } else if (accuracy == 0.8){
                accuracy = 1.0;
            }
            //Console.WriteLine("Original text: \n"+accuracy);

            //extract the correct answer
            startIndex = endIndex + "Correct answer:".Length;
            endIndex = result.ToString().IndexOf("What was wrong and why:");
            string correct_answer = result.ToString()[startIndex..endIndex].Trim();
            //Console.WriteLine("Question:\n"+correct_answer);

            //extract the correct answer
            startIndex = endIndex + "What was wrong and why:".Length;
            string correction = result.ToString()[startIndex..].Trim();
            //Console.WriteLine("Correct Answer:\n"+correction);

            //create the question object
            CorrectedAnswer correctedAnswer = new CorrectedAnswer(language, accuracy, correct_answer, correction, temperature);
            return correctedAnswer;
        }

        // Define your QuestionExcercise POST action method here
        [HttpPost("correctanswer")]
        public async Task<IActionResult> GenerateQuestionExcercise([FromHeader(Name = "ApiKey")] string apiKey, [FromBody] CorrectorRequestModel requestModel)
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

            //defining the prompt & generating the semantic function
            string prompt = @"You are a teacher and you need to evaluate a test. Given the following question and answer, you need to evaluate the accuracy of the answer and, eventually, correct it

                            The output should fit the format:
                            Accuracy: from 0 (if completely wrong) to 1 (if completely correct) with 0.2 intervals
                            Correct answer: null if accuracy < 0.8
                            What was wrong in the answer and why: null if accuracy <0.8
                            
                            Here are some examples:
                            Question: What is quantum entanglement, and how does it impact the state description of particles?
                            Answer: Quantum entanglement is a quantum mechanical phenomenon where particles, even when separated by distance, become interdependent in a manner that the state of one particle is inseparable from the state of another. 
                            Accuracy: 0.8 
                            Correct answer: null 
                            What was wrong in the answer and why: null

                            Question: According to the laws of thermodynamics, which principle states that energy cannot be created or destroyed, only changed from one form to another?
                            Answer: The first law of thermodynamics: 'the law of conservation of energy'.
                            Accuracy: 1.0
                            Correct answer: null
                            What was wrong and why: null

                            Question: What is quantum entanglement, and how does it impact the state description of particles?
                            Answer: Quantum entanglement is a rare occurrence that only applies to particles in laboratories.
                            Accuracy: 0.0
                            Correct answer: Quantum entanglement is a phenomenon that occurs when a group of particles are generated, interact, or share spatial proximity in a way such that the quantum state of each particle of the group cannot be described independently of the state of the others, including when the particles are separated by a large distance. When two particles, such as a pair of photons or electrons, become entangled, they remain connected even when separated by vast distances. Measurements of physical properties such as position, momentum, spin, and polarization performed on entangled particles can, in some cases, be found to be perfectly correlated.

                            Question: What is quantum entanglement, and how does it impact the state description of particles?
                            Answer: Quantum entanglement is a simple interaction between particles that occurs at a distance, without any influence on their individual states.
                            Accuracy: 0.4 
                            Correct answer: Quantum entanglement is a phenomenon that occurs when a group of particles are generated, interact, or share spatial proximity in a way such that the quantum state of each particle of the group cannot be described independently of the state of the others, including when the particles are separated by a large distance1. When two particles, such as a pair of photons or electrons, become entangled, they remain connected even when separated by vast distances2. Measurements of physical properties such as position, momentum, spin, and polarization performed on entangled particles can, in some cases, be found to be perfectly correlated1. 
                            What was wrong in the answer and why: The answer provided is incorrect because it oversimplifies the concept of quantum entanglement and misrepresents its impact on the state description of particles. Quantum entanglement is not a simple interaction between particles at a distance; it’s a complex phenomenon where the quantum states of two or more particles become interconnected. Once these particles are entangled, the state of one particle cannot be described independently of the other(s), no matter how far apart they are21. This means that a change to one particle’s state will instantaneously affect the state of the other particle(s), regardless of the distance between them21. This is a fundamental aspect of quantum mechanics and has significant implications for our understanding of nature at its most fundamental level.

                            Question: {{$question}}
                            Answer: {{$answer}}
                            Accuracy:
                            Correct answer:
                            What was wrong and why:";
            var generate = kernel.CreateSemanticFunction(prompt, "correctAnswer" ,"Corrector", "correct answer to open question", null , requestModel.Temperature);
            //setting up the context
            var context = kernel.CreateNewContext();
            context["question"] = requestModel.Question;
            context["answer"] = requestModel.Answer;
            //instanciating the summarizer
            var summarizer = new Summarizer(_configuration, _auth);
            //generating the output using the LLM
            while(try_count < 3){
                try
                {
                    var result = await generate.InvokeAsync(context);
                    var final = GetFG(requestModel.Language, result.ToString(), requestModel.Question, requestModel.Answer, requestModel.Temperature);
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
                    error = e.Message;
                    try_count++;
                }
            }
            return BadRequest(error);
        }
        [GeneratedRegex("\\d+\\)")]
        private static partial Regex MyRegex();
    }
}