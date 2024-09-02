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
        [HttpPost("planCourse")]
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
string ll = "";
                string prompt = PlanningPrompts.CoursePlanPrompt;
                var generate = kernel.CreateSemanticFunction(prompt, "coursePlanner" ,"CoursePlanner", "generates a course plan about the given topic", null, input.Temperature);
                var context = kernel.CreateNewContext();
                context["language"] = input.Language;
                context["level"] = input.Level.ToString();
                context["macro_subject"] = input.MacroSubject;
                context["title"] = input.Title;
                context["topics"] = input.TopicsBasicInfo();
                context["number_of_lessons"] = input.NumberOfLessons.ToString();
                context["lesson_duration"] = input.LessonDuration.ToString();
                if(input.Title != "" && input.Title != null && input.Title != "null" && input.Title != " " && input.Title.ToLower() != "string")
                {
                    Console.WriteLine("Title: " + input.Title);
                    ll = @$"You already taught everything up to '{input.Title}', so you need to continue from there.";
                }
                context["last_lesson"] = ll;
                context["format"] = FormatStrings.CoursePlanFormat;
                context["example"] = ExamplesStrings.CoursePlanExamples;

                // fill the promptB with the input values
                string promptB = prompt;
                promptB = promptB.Replace("{{$language}}", input.Language);
                promptB = promptB.Replace("{{$title}}", input.Title);
                promptB = promptB.Replace("{{$level}}", input.Level.ToString());
                promptB = promptB.Replace("{{$macro_subject}}", input.MacroSubject);
                promptB = promptB.Replace("{{$topics}}", input.TopicsBasicInfo());
                promptB = promptB.Replace("{{$number_of_lessons}}", input.NumberOfLessons.ToString());
                promptB = promptB.Replace("{{$lesson_duration}}", input.LessonDuration.ToString());
                promptB = promptB.Replace("{{$last_lesson}}", ll);
                promptB = promptB.Replace("{{$format}}", FormatStrings.CoursePlanFormat);
                promptB = promptB.Replace("{{$example}}", ExamplesStrings.CoursePlanExamples);

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
                var InternalFunctionsB = new InternalFunctions();
                string jsonplusprompt = InternalFunctionsB.InsertPromptIntoJSON(json, promptB);
                return Ok(jsonplusprompt.ToString());
            }
// Handle exceptions if something goes wrong during the material generation
            catch (Exception ex){
                _logger.LogError(ex, "Error during course plan generation");
                return StatusCode(500, "Internal Server Error. \n" + output);
            }
        }

    }
}