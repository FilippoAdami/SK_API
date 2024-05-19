// Import necessary namespaces from the ASP.NET Core framework
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

// Declare the namespace for the SummarizerController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class LearningObjectiveGeneratorController : ControllerBase
    {
        private readonly ILogger<LearningObjectiveGeneratorController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public LearningObjectiveGeneratorController(ILogger<LearningObjectiveGeneratorController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        // Define your Lesson POST action method here
        [HttpPost("generateLearningObjective")]
        public async Task<IActionResult> LOAnaliserInputAsync([FromHeader(Name = "ApiKey")] string token, [FromHeader(Name = "SetupModel")] string setupModel, [FromBody] LORM input){
            string output = "";
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
                string prompt = LearningObjectivePrompts.LearningObjectiveGenerator;
                var generate = kernel.CreateSemanticFunction(prompt, "LearningObjectiveGenerator" ,"LOGenerator", "generates a learning objective starting from a given 'analysis'");
                var context = kernel.CreateNewContext();
                context["level"] = input.Level.ToString();
                context["topic"] = input.Topic;
                if(input.Context == null || input.Context == "" || input.Context == " " || input.Context == "null" || input.Context == "NULL"  || input.Context == "Null" || input.Context == "string"){
                    input.Context = "";
                } else{ context["context"] = "Consider that : '"+input.Context+"'.";}
                context["format"] = FormatStrings.LO_Format;
                context["examples"] = ExamplesStrings.LearningObjectives;
// Generate the output
                var result = await generate.InvokeAsync(context);
                string final = result.ToString().Trim();
                final = final.Substring(1, final.Length - 2);
                Console.WriteLine("Result: " + final);
                LOFM learningObjective = new(final);
                output = learningObjective.ToJSON();
                return Ok(output);
            }
// Handle exceptions if something goes wrong during the text extraction
            catch (Exception ex){
                _logger.LogError(ex, "Error during learning objective generation");
                return StatusCode(500, "Internal Server Error\n" + output);
            }
        }
    }
}