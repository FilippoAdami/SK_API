// Import necessary namespaces from the ASP.NET Core framework
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

// Declare the namespace for the MaterialGeneratorController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class MaterialGeneratorController : ControllerBase
    {
        private readonly ILogger<MaterialGeneratorController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public MaterialGeneratorController(ILogger<MaterialGeneratorController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        // Define your Lesson POST action method here
        [HttpPost("generateMaterial")]
        public async Task<IActionResult> SummarizerInputAsync([FromHeader(Name = "ApiKey")] string token, [FromHeader(Name = "SetupModel")] string setupModel, [FromBody] MaterialGeneratorRequestModel input){
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

// Generate the output
                string topic = input.Topic;
                int n_o_w = input.NumberOfWords;
                string level = input.Level.ToString().ToLower();
                string info = input.LearningObjective;
                var InternalFunctions = new InternalFunctions();
                var translation = await InternalFunctions.GenerateMaterial(topic, info, level, kernel, n_o_w);
                output = translation.ToString().Trim();
                Console.WriteLine("Result: " + output);
                return Ok(output);
            }
// Handle exceptions if something goes wrong during the material generation
            catch (Exception ex){
                _logger.LogError(ex, "Error during material generation");
                return StatusCode(500, "Internal Server Error\n" + output);
            }
        }

    }
}