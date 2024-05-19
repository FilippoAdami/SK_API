// Import necessary namespaces from the ASP.NET Core framework
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class CorrectorController : ControllerBase
    {
        private readonly ILogger<CorrectorController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public CorrectorController(ILogger<CorrectorController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        // Define your Lesson POST action method here
        [HttpPost("evaluate")]
        public async Task<IActionResult> LOAnaliserInputAsync([FromHeader(Name = "ApiKey")] string token, [FromHeader(Name = "SetupModel")] string setupModel, [FromBody] CorrectorRequestModel input){
            string final = "";
            try{
// Authentication with the token
                if (token == null)
                {
                    return Unauthorized("API token is required");
                }
                else if (!_auth.Authenticate(token)){
                    return BadRequest("API token is required");
                }
                else{
                    Console.WriteLine("Authenticated successfully");
                }

// Validate the setup model
                var LLMsetupModel = JsonConvert.DeserializeObject<LLM_SetupModel>(setupModel);
                LLM_SetupModel LLM = LLMsetupModel ?? throw new ArgumentNullException(nameof(setupModel));
                IKernel kernel = LLM.Validate();

// Define the TextAnalysis Semantic Function
                double temperature = input.Temperature;
                string prompt = ExerciseCorrectorPrompt.ExerciseCorrector;
                var generate = kernel.CreateSemanticFunction(prompt, "ExerciseCorrector" ,"ExerciseCorrector", "corrects an exercise", null, temperature);
                var context = kernel.CreateNewContext();
                context["question"] = input.Question;
                context["answer"] = input.Answer;
                context["expected_answer"] = input.ExpectedAnswer;
                context["format"] = FormatStrings.ExerciseCorrectionsFormat;
                context["examples"] = ExamplesStrings.ExerciseCorrections;
// Generate the output
                var result = await generate.InvokeAsync(context);
                final = result.ToString().Trim();
                Console.WriteLine("Result: " + final);
                CorrectedAnswer correctedAnswer = new(final);
                return Ok(correctedAnswer.ToJSON());
            }
// Handle exceptions if something goes wrong during the text extraction
            catch (Exception ex){
                _logger.LogError(ex, "Error during exercise correction");
                return StatusCode(500, "Internal Server Error\n"+ final);
            }
        }
    }
}            
            