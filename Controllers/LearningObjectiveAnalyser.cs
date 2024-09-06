// Import necessary namespaces from the ASP.NET Core framework
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

// Declare the namespace for the SummarizerController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class LearningObjectiveAnalyserController : ControllerBase
    {
        private readonly ILogger<LearningObjectiveAnalyserController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public LearningObjectiveAnalyserController(ILogger<LearningObjectiveAnalyserController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        // Define your Lesson POST action method here
        [HttpPost("analyseLearningObjective")]
        public async Task<IActionResult> LOAnaliserInputAsync([FromHeader(Name = "ApiKey")] string token, [FromHeader(Name = "SetupModel")] string setupModel, [FromBody] LOAnalyserRequestModel input){
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
                string prompt = LearningObjectivePrompts.LearningObjectiveAnalysis;
                var generate = kernel.CreateSemanticFunction(prompt, "LearningObjectiveAnalyser" ,"LOAnalyser", "analyses the given learning objective");
                var context = kernel.CreateNewContext();
                context["learningObjective"] = input.LearningObjective;
                context["examples"] = ExamplesStrings.LearningObjective;
                Console.WriteLine("Learning Objective: " + input.LearningObjective);

                // fill the promptB with the input values
                string promptB = prompt;
                promptB = promptB.Replace("{{$learningObjective}}", input.LearningObjective);
                promptB = promptB.Replace("{{$examples}}", ExamplesStrings.LearningObjective);

// Generate the output
                var result = await generate.InvokeAsync(context);
                var intf = new InternalFunctions();
                output = intf.CheckResponse(result.ToString());
                LOAnalysis analysis = new(output);
                output = analysis.ToJSON();
                string json = output;
                string jsonplusprompt = intf.InsertPromptIntoJSON(json, promptB);
                return Ok(jsonplusprompt.ToString());
            }
// Handle exceptions if something goes wrong during the text extraction
            catch (Exception ex){
                _logger.LogError(ex, "Error during learning objective analysis");
                return StatusCode(500, "Internal Server Error\n"+ output);
            }
        }
    }
}