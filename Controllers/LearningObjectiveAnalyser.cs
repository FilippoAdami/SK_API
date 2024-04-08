// Import necessary namespaces from the ASP.NET Core framework
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

// Declare the namespace for the SummarizerController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class LOAnalyserController : ControllerBase
    {
        private readonly ILogger<LOAnalyserController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public LOAnalyserController(ILogger<LOAnalyserController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        // Define your Lesson POST action method here
        [HttpPost("analyselearningobjective")]
        public async Task<IActionResult> LOAnaliserInputAsync([FromHeader(Name = "ApiKey")] string token, [FromHeader(Name = "SetupModel")] LLM_SetupModel setupModel, [FromBody] LOAnalyserRequestModel input){
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
                LLM_SetupModel LLM = setupModel ?? throw new ArgumentNullException(nameof(setupModel));
                IKernel kernel = LLM.Validate();

// Define the TextAnalysis Semantic Function
                string prompt = LearningObjectivePrompts.LearningObjectiveAnalysis;
                var generate = kernel.CreateSemanticFunction(prompt, "LearningObjectiveAnalyser" ,"LOAnalyser", "analyses the given learning objective");
                var context = kernel.CreateNewContext();
                context["learningObjective"] = input.LearningObjective;
                context["examples"] = ExamplesStrings.LearningObjective;
                Console.WriteLine("Learning Objective: " + input.LearningObjective);

// Generate the output
                var result = await generate.InvokeAsync(context);
                Console.WriteLine("Result: " + result.ToString());
                LOAnalysis analysis = new(result.ToString());
                return Ok(analysis.ToJSON());
            }
// Handle exceptions if something goes wrong during the text extraction
            catch (Exception ex){
                _logger.LogError(ex, "Error during learning objective analysis");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}