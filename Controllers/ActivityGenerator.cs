//Import necessary namespaces from the ASP.NET Core framework
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;
using SK_API;

// Declare the namespace for the SummarizerController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class ActivityGeneratorController : ControllerBase
    {
        private readonly ILogger<ActivityGeneratorController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public ActivityGeneratorController(ILogger<ActivityGeneratorController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        // Define your Lesson POST action method here
        [HttpPost("generateActivity")]
        public async Task<IActionResult> ActivitysInputAsync([FromHeader(Name = "ApiKey")] string token, [FromHeader(Name = "SetupModel")] string setupModel, [FromBody] ActivityInputModel input){
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
                
// Get the variables from the input
                string language = input.Language.ToLower();
                double temperature = input.Temperature;
                if (temperature < 0.0){
                    temperature = 0.0;
                }
                else if (temperature > 0.5){
                    temperature = 0.5;
                }

// Get the material source from the input
                TextExtractor textProcessor = new TextExtractor();
                string extractedText = textProcessor.ExtractTextFromFileOrUrl(input.Material);
                //Console.WriteLine("Extracted text: " + extractedText);

// Retrieve the variables for the Activity to be generated
                // difficulty level
                TextLevel difficulty_level = input.Level;
                // set the presencePenalty and the frequencyPenalty based on the difficulty level
                double presencePenalty;
                double frequencyPenalty;
                switch (difficulty_level){
                    case TextLevel.primary_school:
                        presencePenalty = 0.1; // Encourage variety with a slight penalty
                        frequencyPenalty = 0.9; // Allow common words for simplicity
                        break;
                    case TextLevel.middle_school:
                        presencePenalty = 0.2; // Slightly more variety
                        frequencyPenalty = 0.7; // Introduce a small penalty for common words
                        break;
                    case TextLevel.high_school:
                        presencePenalty = 0.3; // Encourage even more variety
                        frequencyPenalty = 0.5; // Increase penalty for common words
                        break;
                    case TextLevel.college:
                        presencePenalty = 0.4; // Further encourage variety
                        frequencyPenalty = 0.3; // Allow for some uncommon terms
                        break;
                    case TextLevel.academy:
                        presencePenalty = 0.5; // Maximize variety
                        frequencyPenalty = 0.1; // Encourage use of specialized language
                        break;
                    default:
                        presencePenalty = 0.0;
                        frequencyPenalty = 0.0;
                        break;
                }

                // domain of expertise
                string domain_of_expertise = input.MacroSubject;
                // title 
                string title = input.Title;
                // tipe of Activity
                TypeOfActivity type_of_Activity = input.TypeOfActivity;
                // learning objective
                string learning_objective = input.LearningObjective;

                // number of solutions
                int number_of_solutions = input.CorrectAnswersNumber;
                // number of distractors
                int number_of_distractors = input.DistractorsNumber;
                // number of easy distractors
                int number_of_easy_distractors = input.EasilyDiscardableDistractorsNumber;

                // topic
                string final_topic = input.Topic;
                // type of assignment
                TypeOfAssignment final_type = input.AssignmentType;          

                // category and bloom level
                var (category, levels) = ActivityData.GetCategoryAndLevels(input.TypeOfActivity);
                BloomLevel bloom_level = input.BloomLevel;
                    // if the bloom_level is not contained in levels, set it as the closest level contained in levels
                    if (!levels.Contains(bloom_level)){
                        BloomLevel closest = levels.Aggregate((x, y) => Math.Abs((int)x - (int)bloom_level) < Math.Abs((int)y - (int)bloom_level) ? x : y);
                        bloom_level = closest;
                    }
                
// Construct the Meta Model
                AssignmentModel assignmentModel = new(category, type_of_Activity, number_of_solutions);
                SolutionModel solutionModel = new(type_of_Activity, final_type, category, number_of_solutions);
                MetaModel metaModel = new(type_of_Activity, category, assignmentModel, solutionModel);
                
// Define the ActivitysGenerator Semantic Function
                string prompt = metaModel.ToString(category);
                var generate = kernel.CreateSemanticFunction(prompt, "ActivitysGenerator" ,"ActivitysGenerator", "generates Activitys for the given material", null, temperature, 0, presencePenalty, frequencyPenalty);
                Console.WriteLine("Prompt: " + prompt);
                var context = kernel.CreateNewContext();

                context["difficulty_level"] = difficulty_level.ToString();
                context["domain_of_expertise"] = domain_of_expertise;
                context["lesson_title"] = title;
                context["material"] = extractedText;

                context["type_of_Activity"] = type_of_Activity.ToString();
                context["learning_objective"] = learning_objective;

                context["type_of_assignment"] = final_type.ToString();
                context["bloom_level"] = bloom_level.ToString();
                context["topic"] = final_topic.ToString();

                context["number_of_distractors"] = number_of_distractors.ToString();
                context["number_of_easily_discardable_distractors"] = number_of_easy_distractors.ToString();


// Generate the output
                var result = await generate.InvokeAsync(context);
                string resultString = result.ToString();
                Console.WriteLine(result.ToString());
// Translate the result
                if (language != "english"){
                    var InternalFunctions = new InternalFunctions();
                    var translation = await InternalFunctions.Translate(kernel, resultString, language);
                    resultString = translation;
                }
                Console.WriteLine(resultString);
// Map the result to the ActivitysOutputModel
                ActivityFinalModel Activity = new(resultString);
                if (type_of_Activity == TypeOfActivity.information_search){
                    Activity = ActivityFinalModel.ProcessActivity(Activity);
                }
                output = Activity.ToJSON();
                return Ok(output);
            }
// Handle exceptions if something goes wrong during the Activitys generation
            catch (Exception ex){
                _logger.LogError(ex, "Error during Activitys generation");
                return StatusCode(500, "Internal Server Error\n" + output);
            }
        }
    }
}