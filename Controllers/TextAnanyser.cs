// Import necessary namespaces from the ASP.NET Core framework
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

// Declare the namespace for the SummarizerController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class AnalyserController : ControllerBase
    {
        private readonly ILogger<AnalyserController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public AnalyserController(ILogger<AnalyserController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        // Define your Lesson POST action method here
        [HttpPost("analyseMaterial")]
        public async Task<IActionResult> AnaliserInputAsync([FromHeader(Name = "ApiKey")] string token, [FromHeader(Name = "SetupModel")] LLM_SetupModel setupModel, [FromBody] AnalyserRequestModel input){
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

// Get the material source from the input
                TextExtractor textProcessor = new TextExtractor();
                string extractedText = textProcessor.ExtractTextFromFileOrUrl(input.Material);
                //Console.WriteLine("Extracted text: " + extractedText);

// Define the TextAnalysis Semantic Function
                string prompt = TextAnalyserPrompt.TextAnalysisPrompt;
                var generate = kernel.CreateSemanticFunction(prompt, "materialAnalyser" ,"MaterialAnalyser", "analyses the given material");
                var context = kernel.CreateNewContext();
                context["material"] = extractedText;
                context["examples"] = ExamplesStrings.MaterialAnalysisExamples;
                context["format"] = FormatStrings.AnalyserFormat;

// Generate the output
                var result = await generate.InvokeAsync(context);
                string output = result.ToString().Trim();
                Console.WriteLine("Result: " + output);
                MaterialAnalysis analysis = new(output);
        // Translate the output to the user's language
                if (analysis.Language.ToLower() != "english"){
                    Console.WriteLine("Translating the analysis to " + analysis.Language);
                    var InternalFunctions = new InternalFunctions();
                    var translation = await InternalFunctions.Translate(kernel, analysis.ToJson().ToString(), analysis.Language);
                    MaterialAnalysis translatedAnalysis = new(translation);
                    // add the translated main topics to the analysis
                    analysis.MainTopics.AddRange(translatedAnalysis.MainTopics);
                }
                string json = analysis.ToJson();
                return Ok(json.ToString());
            }
// Handle exceptions if something goes wrong during the text extraction
            catch (Exception ex){
                _logger.LogError(ex, "Error during text analysis");
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}