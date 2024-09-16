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
                string activities = "";
                //switch the value of activities based on the bloom level
                switch (input.BloomLevel){
                    case BloomLevel.Remembering:
                        activities = UtilsStrings.ActivitiesListRemembering;
                        break;
                    case BloomLevel.Understanding:
                        activities = UtilsStrings.ActivitiesListUnderstanding;
                        break;
                    case BloomLevel.Applying:
                        activities = UtilsStrings.ActivitiesListApplying;
                        break;
                    case BloomLevel.Analyzing:
                        activities = UtilsStrings.ActivitiesListAnalyzing;
                        break;
                    case BloomLevel.Evaluating:
                        activities = UtilsStrings.ActivitiesListEvaluating;
                        break;
                    case BloomLevel.Creating:
                        activities = UtilsStrings.ActivitiesListCreating;
                        break;
                    default:
                        activities = UtilsStrings.ActivitiesList;
                        break;
                }
                context["activities"] = UtilsStrings.ActivitiesList;
                context["examples"] = ExamplesStrings.LessonPlanExamples;

                // fill the promptB with the input values
                string promptB = prompt;
                promptB = promptB.Replace("{{$language}}", input.Language);
                promptB = promptB.Replace("{{$level}}", input.Level.ToString());
                promptB = promptB.Replace("{{$marco_subject}}", input.MacroSubject);
                promptB = promptB.Replace("{{$title}}", input.Title);
                promptB = promptB.Replace("{{$learning_objective}}", input.LearningObjective);
                promptB = promptB.Replace("{{$bloom_level}}", input.BloomLevel.ToString());
                promptB = promptB.Replace("{{$context}}", input.Context);
                promptB = promptB.Replace("{{$format}}", FormatStrings.LessonPlanFormat);
                promptB = promptB.Replace("{{$topics}}", string.Join("",input.MainTopics));
                promptB = promptB.Replace("{{$activities}}", UtilsStrings.ActivitiesList);
                promptB = promptB.Replace("{{$examples}}", ExamplesStrings.LessonPlanExamples);


                var result = await generate.InvokeAsync(context);
                var intf = new InternalFunctions();
                output = intf.CheckResponse(result.ToString());
                Console.WriteLine("Result: "+ output);
                LessonPlan lessonPlan = new(output);
                foreach (var node in lessonPlan.Nodes){
                    if(node.Type){
                        //map the value into TypeOfActivity enum
                        int Activity = (int)Enum.Parse<TypeOfActivity>(node.Details);
                        node.Details = Activity.ToString();
                    }
                }
                string json = lessonPlan.ToJSON();

                if (input.Language.ToLower() != "english"){
                    var translation = await intf.Translate(kernel, json, input.Language);
                    json = intf.CheckResponse(translation);
                }
                string jsonplusprompt = intf.InsertPromptIntoJSON(json, promptB);
                return Ok(jsonplusprompt.ToString());
            }
// Handle exceptions if something goes wrong during the material generation
            catch (Exception ex){
                _logger.LogError(ex, "Error during lesson plan generation");
                return StatusCode(500, "Internal Server Error. \n" + output);
            }
        }

    }
}