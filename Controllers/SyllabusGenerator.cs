// Import necessary namespaces from the ASP.NET Core framework
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

// Declare the namespace
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class SyllabusGeneratorController : ControllerBase
    {
        private readonly ILogger<SyllabusGeneratorController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public SyllabusGeneratorController(ILogger<SyllabusGeneratorController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        // Define your Lesson POST action method here
        [HttpPost("generateSyllabus")]
        public async Task<IActionResult> LOAnaliserInputAsync([FromHeader(Name = "ApiKey")] string token, [FromHeader(Name = "SetupModel")] string setupModel, [FromBody] SyllabusRM input){
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
                string prompt = SyllabusPrompts.SyllabusGenerator;
                var generate = kernel.CreateSemanticFunction(prompt, "SyllabusGenerator" ,"SyllabusGenerator", "generates a syllabus starting from a given 'analysis'");
                var context = kernel.CreateNewContext();
                context["macro_subject"] = input.Analysis.MacroSubject;
                context["title"] = input.Analysis.Title;
                context["level"] = input.Analysis.PerceivedDifficulty.ToString();
                context["topics"] = input.Analysis.BasicTopicsInfo() ?? "you need to generate the main topics for this course";
                context["format"] = FormatStrings.SyllabusFormat;
                context["examples"] = ExamplesStrings.SyllabusExamples;

                // fill the promptB with the input values
                string promptB = prompt;
                promptB = promptB.Replace("{{macro_subject}}", input.Analysis.MacroSubject);
                promptB = promptB.Replace("{{title}}", input.Analysis.Title);
                promptB = promptB.Replace("{{level}}", input.Analysis.PerceivedDifficulty.ToString());
                promptB = promptB.Replace("{{topics}}", input.Analysis.BasicTopicsInfo());
                promptB = promptB.Replace("{{format}}", FormatStrings.SyllabusFormat);
                promptB = promptB.Replace("{{examples}}", ExamplesStrings.SyllabusExamples);

// Generate the output
                var result = await generate.InvokeAsync(context);
                string final = result.ToString().Trim();
                final = final.Substring(1, final.Length - 2);
               Console.WriteLine("Result: " + final);
               // remove eventual ``json at the beginning and `` at the end of the string
                final = final.Replace("``json", "");
                final = final.Replace("``", "");
                final = final.Trim();
                Syllabus syllabus = new(final);
                output = syllabus.ToJson();
                string json = output;
                var InternalFunctionsB = new InternalFunctions();
                string jsonplusprompt = InternalFunctionsB.InsertPromptIntoJSON(json, promptB);
                return Ok(jsonplusprompt.ToString());
            }
// Handle exceptions if something goes wrong during the text extraction
            catch (Exception ex){
                _logger.LogError(ex, "Error during learning objective generation");
                return StatusCode(500, "Internal Server Error\n" + output);
            }
        }
    }
}