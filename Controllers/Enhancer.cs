// Import necessary namespaces from the ASP.NET Core framework
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

// Declare the namespace for the SummarizerController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class EnhancerController : ControllerBase
    {
        private readonly ILogger<EnhancerController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public EnhancerController(ILogger<EnhancerController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        // Define your Lesson POST action method here
        [HttpPost("enhance")]
        public async Task<IActionResult> AnaliserInputAsync([FromHeader(Name = "ApiKey")] string token, [FromHeader(Name = "SetupModel")] string setupModel, [FromBody] EnhancerRM input){
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

// Validate the request
                string response = input.Json_response;
                string request = input.Request;
                string language = input.Language;
                double temperature = input.Temperature;
                var intf = new InternalFunctions();
                string prompt = EnhancerPrompts.RequestValidationPrompt;
                var generate = kernel.CreateSemanticFunction(prompt, "RequestValidation" ,"RequestEnhancer", "enhance a request", null, temperature);
                var context = kernel.CreateNewContext();
                context["json"] = response;
                context["request"] = request;
                context["language"] = language;

                // fill the promptB with the input values
                string promptB = prompt;
                promptB = promptB.Replace("{{$json}}", response);
                promptB = promptB.Replace("{{$request}}", request);
                promptB = promptB.Replace("{{$language}}", language);

                var result = await generate.InvokeAsync(context);
                string validated = intf.CheckResponse(result.ToString());
                SimpleText validatedRequest = new SimpleText(validated);
                if (validatedRequest.Text.Contains("ERROR")){
                    return BadRequest(validatedRequest.Text);
                }

// Enhance the response
                var prompt2 = EnhancerPrompts.EnhancerPrompt;
                var generate2 = kernel.CreateSemanticFunction(prompt2, "ResponseEnhancer" ,"ResponseEnhancer", "enhance a response", null, temperature);
                var context2 = kernel.CreateNewContext();
                context2["json"] = response;
                context2["request"] = validatedRequest.Text;
                context2["language"] = language;
                var response2 = await generate2.InvokeAsync(context2);
                output = response2.ToString();
                output = intf.CheckResponse(response2.ToString());
                string json = "";
                dynamic typeOfModel;
                switch (input.Type){
                    case TypeOfResponse.MaterialAnalysis:
                        typeOfModel = new MaterialAnalysis(output);
                        break;
                    case TypeOfResponse.LessonPlan:
                        typeOfModel = new LessonPlan(output);
                        break;
                    case TypeOfResponse.CoursePlan:
                        typeOfModel = new CoursePlan(output);
                        break;
                    case TypeOfResponse.ActivityFinalModel:
                        typeOfModel = new ActivityFinalModel(output);
                        break;
                    case TypeOfResponse.CorrectedAnswer:
                        typeOfModel = new CorrectedAnswer(output);
                        break;
                    case TypeOfResponse.Syllabus:
                        typeOfModel = new Syllabus(output);
                        break;
                    case TypeOfResponse.LOFM:
                        typeOfModel = new LOFM(output);
                        break;
                    case TypeOfResponse.SimpleText:
                        typeOfModel = new SimpleText(output);
                        break;
                    default:
                        typeOfModel = new SimpleText(output);
                        break;
                }
                json = typeOfModel.ToJSON();
                string jsonplusprompt = intf.InsertPromptIntoJSON(json, promptB);
                return Ok(jsonplusprompt.ToString());
            }
// Handle exceptions if something goes wrong during the text extraction
            catch (Exception ex){
                _logger.LogError(ex, "Error during json translation");
                return StatusCode(500, "Internal Server Error\n" + output);
            }
        }

    }
}