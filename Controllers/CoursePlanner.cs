// Import necessary namespaces from the ASP.NET Core framework
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

// Declare the namespace for the CoursePlannerController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class CoursePlannerController : ControllerBase
    {
        private readonly ILogger<CoursePlannerController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public CoursePlannerController(ILogger<CoursePlannerController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        // Define your Lesson POST action method here
        [HttpPost("plancourse")]
        public async Task<IActionResult> CoursePlannerInputAsync([FromHeader(Name = "ApiKey")] string token, [FromHeader(Name = "SetupModel")] string setupModel, [FromBody] CoursePlanRequestModel input){
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
                string prompt = PlanningPrompts.CoursePlanPrompt;
                var generate = kernel.CreateSemanticFunction(prompt, "coursePlanner" ,"CoursePlanner", "generates a course plan about the given topic", null, input.Temperature);
                var context = kernel.CreateNewContext();
                context["language"] = input.Language;
                context["level"] = input.Level.ToString();
                context["macro_subject"] = input.MacroSubject;
                context["topic"] = input.Topic;
                context["number_of_lessons"] = input.NumberOfLessons.ToString();
                context["lesson_duration"] = input.LessonDuration.ToString();
                context["last_lesson"] = "";
                if(input.Title != "" && input.Title != null && input.Title != "null" && input.Title != " " && input.Title.ToLower() != "string")
                {
                    Console.WriteLine("Title: " + input.Title);
                    string ll = @$"You already taught everything up to '{input.Title}', so you need to continue from there.";
                    context["last_lesson"] = ll;
                }
                context["format"] = FormatStrings.CoursePlanFormat;
                context["example"] = ExamplesStrings.CoursePlanExamples;

                var result = await generate.InvokeAsync(context);
                output = result.ToString();
                if (output.StartsWith("```json")){
                    output = output[7..];
                    if (output.EndsWith("```")){
                        output = output[..^3];
                    }
                    output = output.TrimStart('\n').TrimEnd('\n');
                    output = output.TrimStart(' ').TrimEnd(' ');
                }
                Console.WriteLine("Result: "+ output);
                CoursePlan coursePlan = new(output);
                string json = coursePlan.ToJson();
                return Ok(json.ToString());
            }
// Handle exceptions if something goes wrong during the material generation
            catch (Exception ex){
                _logger.LogError(ex, "Error during course plan generation");
                return StatusCode(500, "Internal Server Error. \n" + output);
            }
        }

    }
}