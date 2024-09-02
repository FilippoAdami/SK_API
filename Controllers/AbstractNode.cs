// Import necessary namespaces from the ASP.NET Core framework
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

// Declare the namespace for the AbstractNodeController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class AbstractNodeController : ControllerBase
    {
        private readonly ILogger<AbstractNodeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public AbstractNodeController(ILogger<AbstractNodeController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        // Define your Lesson POST action method here
        [HttpPost("abstractNode")]
        public async Task<IActionResult> LessonPlannerInputAsync([FromHeader(Name = "ApiKey")] string token, [FromHeader(Name = "SetupModel")] string setupModel, [FromBody] AbstractNodeRequestModel input){
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
                string prompt = PlanningPrompts.AbstractNodePrompt;
                var generate = kernel.CreateSemanticFunction(prompt, "abstractPlanner" ,"AbstractPlanner", "generates a tailored lesson plan", null, input.Temperature);
                var context = kernel.CreateNewContext();
                context["language"] = input.Language;
                context["level"] = input.Level.ToString();
                context["macro_subject"] = input.MacroSubject;
                context["title"] = input.Title;
                context["correction"] = input.Correction;

                context["format"] = FormatStrings.LessonPlanFormat;
                context["activities"] = UtilsStrings.ActivitiesListB;
                context["examples"] = ExamplesStrings.CustomPlanExamples;

                // fill the promptB with the input values
                string promptB = prompt;
                promptB = promptB.Replace("{{$level}}", input.Level.ToString());
                promptB = promptB.Replace("{{$marco_subject}}", input.MacroSubject);
                promptB = promptB.Replace("{{$title}}", input.Title);
                promptB = promptB.Replace("{{$language}}", input.Language);
                promptB = promptB.Replace("{{$correction}}", input.Correction);
                promptB = promptB.Replace("{{$activities}}", UtilsStrings.ActivitiesListB);
                promptB = promptB.Replace("{{$examples}}", ExamplesStrings.CustomPlanExamples);
                promptB = promptB.Replace("{{$format}}", FormatStrings.LessonPlanFormat);

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
                        int exercise = (int)Enum.Parse<TypeOfActivity>(node.Details);
                        node.Details = exercise.ToString();
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
                var InternalFunctionsB = new InternalFunctions();
                string jsonplusprompt = InternalFunctionsB.InsertPromptIntoJSON(json, promptB);
                return Ok(jsonplusprompt.ToString());
            }
// Handle exceptions if something goes wrong during the material generation
            catch (Exception ex){
                _logger.LogError(ex, "Error during abstract node generation");
                return StatusCode(500, "Internal Server Error. \n" + output);
            }
        }

    }
}