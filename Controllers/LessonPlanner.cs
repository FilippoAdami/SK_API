// Import necessary namespaces from the ASP.NET Core framework
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

// Declare the namespace for the LessonPlannerController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class LessonPlannerController : ControllerBase
    {
        private readonly ILogger<LessonPlannerController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public LessonPlannerController(ILogger<LessonPlannerController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        // Define your Lesson POST action method here
        [HttpPost("planLesson")]
        public async Task<IActionResult> LessonPlannerInputAsync([FromHeader(Name = "ApiKey")] string token, [FromHeader(Name = "SetupModel")] string setupModel, [FromBody] LessonPlannerRequestModel input){
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
                string prompt = PlanningPrompts.LessonPlannerprompt;
                var generate = kernel.CreateSemanticFunction(prompt, "lessonPlanner" ,"LessonPlanner", "generates a lesson plan for the given input", null, input.Temperature);
                var context = kernel.CreateNewContext();
                context["language"] = input.Language;
                context["level"] = input.Level.ToString();
                context["macro_subject"] = input.MacroSubject;
                context["title"] = input.Title;

                context["learning_objective"] = input.LearningObjective;
                context["bloom_level"] = input.BloomLevel.ToString();

                context["context"] = input.Context;
                context["format"] = FormatStrings.LessonPlanFormat;
                context["topics"] = string.Join("",input.MainTopics);
                context["activities"] = UtilsStrings.ActivitiesList;
                context["examples"] = ExamplesStrings.LessonPlanExamples;

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
                LessonPlan lessonPlan = new(output);
                foreach (var node in lessonPlan.Nodes){
                    if(node.Type){
                        //map the value into TypeOfActivity enum
                        int Activity = (int)Enum.Parse<TypeOfActivity>(node.Details);
                        node.Details = Activity.ToString();
                    }
                }
                string json = lessonPlan.ToJson();

                if (input.Language.ToLower() != "english"){
                    var InternalFunctions = new InternalFunctions();
                    var translation = await InternalFunctions.Translate(kernel, json, input.Language);
                    json = translation;
                }
                if (json.StartsWith("```json")){
                    json = json[7..];
                    if (json.EndsWith("```")){
                        json = json[..^3];
                    }
                    // remove all nelines and tabulation
                    json = json.Replace("\n", "").Replace("\t", "").Trim();
                }
                return Ok(json.ToString());
            }
// Handle exceptions if something goes wrong during the material generation
            catch (Exception ex){
                _logger.LogError(ex, "Error during lesson plan generation");
                return StatusCode(500, "Internal Server Error. \n" + output);
            }
        }

    }
}