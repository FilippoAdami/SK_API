// Import necessary namespaces from the ASP.NET Core framework
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

// Declare the namespace for the SummarizerController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class SummarizerController : ControllerBase
    {
        private readonly ILogger<SummarizerController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public SummarizerController(ILogger<SummarizerController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        // Define your Lesson POST action method here
        [HttpPost("summarise")]
        public async Task<IActionResult> SummarizerInputAsync([FromHeader(Name = "ApiKey")] string token, [FromHeader(Name = "SetupModel")] string setupModel, [FromBody] SummarizerRequestModel input){
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
                
// Get the material source from the input
                TextExtractor textProcessor = new TextExtractor();
                string extractedText = textProcessor.ExtractTextFromFileOrUrl(input.Material);
                //Console.WriteLine("Extracted text: " + extractedText);

// Generate the output
                string material = extractedText;
                int n_o_w = input.NumberOfWords;
                string level = input.Level.ToString().ToLower();
                var InternalFunctions = new InternalFunctions();
                var translation = await InternalFunctions.Summarize(extractedText, level, kernel, n_o_w);
                string output = translation.ToString().Trim();
                Console.WriteLine("Result: " + output);
                return Ok(output);
            }
// Handle exceptions if something goes wrong during the text extraction
            catch (Exception ex){
                _logger.LogError(ex, "Error during material summarization");
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}