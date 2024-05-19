// Import necessary namespaces from the ASP.NET Core framework
using iText.Commons.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

// Declare the namespace for the SummarizerController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class MaterialAnalyserController : ControllerBase
    {
        private readonly ILogger<MaterialAnalyserController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public MaterialAnalyserController(ILogger<MaterialAnalyserController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        // Define your Lesson POST action method here
        [HttpPost("analyseMaterial")]
        public async Task<IActionResult> AnaliserInputAsync([FromHeader(Name = "ApiKey")] string token, [FromHeader(Name = "SetupModel")] string setupModel, [FromBody] AnalyserRequestModel input){
            string foutput = "";
            try{
// Authentication with the token
                if (token == null)
                {
                    return Unauthorized("API token is required");
                }
                else if (!_auth.Authenticate(token)){
                    return BadRequest("API token is invalid");
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
                //check if the extracted text is longer than 60000 characters
                Console.WriteLine("Extracted text length: " + extractedText.Length);
                if (extractedText.Length > 60000){
                    return BadRequest("The extracted text is too long. Please provide a shorter text");
                }
                //if the code is longer than 15000 characters, divide it into parts smaller than 15000 characters
                List<string> extractedTexts = new();
                if (extractedText.Length > 30000){
                    Console.WriteLine("Extracted text is too long. Splitting it into smaller parts");
                    extractedTexts = textProcessor.DivideText(extractedText);
                }
                else {
                    extractedTexts.Add(extractedText);
                }
                Console.WriteLine("Extracted text: " + extractedText);

// Define the TextAnalysis Semantic Function
                string prompt = TextAnalyserPrompt.TextAnalysisPrompt;
                var generate = kernel.CreateSemanticFunction(prompt, "materialAnalyser" ,"MaterialAnalyser", "analyses the given material");
                Microsoft.SemanticKernel.Orchestration.SKContext result;
                List<MaterialAnalysis> analysis = new();
                // Run the TextAnalysis function for each part of the extracted text
                foreach (string text in extractedTexts){
                    var context = kernel.CreateNewContext();
                    context["material"] = text;
                    context["examples"] = ExamplesStrings.MaterialAnalysisExamples;
                    context["format"] = FormatStrings.AnalyserFormat;
                    result = await generate.InvokeAsync(context);
                    string output = result.ToString().Trim();
                    Console.WriteLine("Result: " + output);
                    MaterialAnalysis analysisPart = new(output);
                    analysis.Add(analysisPart);
                }
                // Combine the analysis parts into a single analysis
                MaterialAnalysis finalAnalysis = analysis[0];
                for (int i = 1; i < analysis.Count; i++){
                    finalAnalysis.MainTopics.AddRange(analysis[i].MainTopics);
                }

// Generate the output
        // Translate the output to the user's language
                if (finalAnalysis.Language.ToLower() != "english"){
                    Console.WriteLine("Translating the analysis to " + finalAnalysis.Language);
                    var InternalFunctions = new InternalFunctions();
                    var translation = await InternalFunctions.Translate(kernel, finalAnalysis.ToJson().ToString(), finalAnalysis.Language);
                    MaterialAnalysis translatedAnalysis = new(translation);
                    // add the translated main topics to the analysis
                    finalAnalysis.MainTopics.AddRange(translatedAnalysis.MainTopics);
                }
                foutput = finalAnalysis.ToJson();
                return Ok(foutput);
            }
// Handle exceptions if something goes wrong during the text extraction
            catch (Exception ex){
                _logger.LogError(ex, "Error during text analysis");
                return StatusCode(500, "Internal Server Error\n" + foutput);
            }
        }

    }
}