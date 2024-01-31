// Import necessary namespaces from the ASP.NET Core framework
using System.Text.RegularExpressions;
using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

// Declare the namespace for the LOGeneratorController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class LOGeneratorController : ControllerBase
    {
        private readonly ILogger<LOGeneratorController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public LOGeneratorController(ILogger<LOGeneratorController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        // Define your Lesson POST action method here
        [HttpPost("generatelearningobjective")]
        public async Task<IActionResult> GenerateLesson([FromHeader(Name = "ApiKey")] string apiKey, [FromBody] LORequestModel requestModel)
        { 
            int try_count = 0;
            string error = "";
            int authenticated = _auth.Authenticate(apiKey);
            if (authenticated == 400)
            {
                return BadRequest("Required configuration values are missing or empty.");
            }
            else if (authenticated == 403)
            {
                return Unauthorized();
            }
            else if(authenticated==200){
                Console.WriteLine("Authenticated successfully");
            }
            var secretKey = _configuration["OPENAPI_SECRET_KEY"];
            var endpoint = _configuration["OPENAPI_ENDPOINT"];
            var model = _configuration["GPT_35_TURBO_DN"];
            
        //setting up the semantic kernel
            if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(model)){
                return BadRequest("Required configuration values are missing or empty.");
            }
            var SKbuilder = new KernelBuilder();
            SKbuilder.WithAzureChatCompletionService(
            model,  // Azure OpenAI Deployment Name
            endpoint, // Azure OpenAI Endpoint
            secretKey); // Azure OpenAI Key
            var kernel = SKbuilder.Build();

            //defining the prompt & generating the semantic function
            string prompt = @"You are an {{$educatorExperience}} level teacher of a {{$educationContext}}, {{$learnerExperience}} level, course on {{$learningContext}} for 
                            students with {{$dimension}} classroom size. 
                            You want to teach the students the skill of: {{$skills}} at a Bloom Taxonomy {{$bloomLevel}} level.
                            To do so, you need to define exactly one concrete, specific and very descriptive learning objective using at least one of the verbs {{$verbs}}.
                            Do not mention the Bloom Taxonomy level in the learning objective, just focus on exactly how you intend to teach the students the skill you want.
                            Return only the learning objective.";
            var generate = kernel.CreateSemanticFunction(prompt, "generateLearingObjective" ,"LO", "generate LO", null , requestModel.Temperature);
            //setting up the context
            var context = kernel.CreateNewContext();
            context["learningContext"] = requestModel.LearningContext;
            context["educationContext"] = requestModel.EducationContext.ToString();
            context["dimension"] = requestModel.Dimension.ToString();
            context["skills"] = requestModel.Skills;
            context["bloomLevel"] = requestModel.BloomLevel.ToString();
            context["educatorExperience"] = requestModel.EducatorExperience.ToString();
            context["learnerExperience"] = requestModel.LearnerExperience.ToString();

            //setting up the verbs
            string verbs = "";
            foreach (string verb in requestModel.Verbs)
            {
                verbs += verb + ", ";
            }
            context["verbs"] = verbs;

            //instanciating the summarizer
            var summarizer = new Summarizer(_configuration, _auth);
            //generating the output using the LLM
            while (try_count < 3){
                try
                {
                    var result = await generate.InvokeAsync(context);
                    var Date = DateOnly.FromDateTime(DateTime.Now);
                    var final = $"Date: {Date}\nLearningContext: {requestModel.LearningContext}\nEducationContext: {requestModel.EducationContext}\nDimension: {requestModel.Dimension}\nSkills: {requestModel.Skills}\nBloomLevel: {requestModel.BloomLevel}\nEducatorExperience: {requestModel.EducatorExperience}\nLearnerExperience: {requestModel.LearnerExperience}\nVerbs: {verbs}\nTemperature: {requestModel.Temperature}\n{result.ToString()}";
                    if (requestModel.Language=="English"){
                        return Ok(final.ToString());
                    }
                    else{
                        var translated = await summarizer.Translate(apiKey, final.ToString(), requestModel.Language);
                        return Ok(translated);
                    }
                }
                catch (Exception e)
                {
                    error = e.Message;
                    try_count++;                    
                }
            }
            return BadRequest(error);
        }
    }
}