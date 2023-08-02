// Import necessary namespaces from the ASP.NET Core framework
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

// Declare the namespace for the FillTheGapsController
namespace SK_API.Controllers
{
    // Mark the FillTheGapsController as an ApiController to enable API behavior
    [ApiController]
    // Set the base route for all actions within this controller to be '[controller]'
    // This means that the route for this controller will be based on its name 'WeatherForecast'
    [Route("[controller]")]
    public class FillTheGapsController : ControllerBase
    {
        // Declare a private field to hold an instance of the ILogger interface
        // ILogger allows logging of messages within the controller
        private readonly ILogger<FillTheGapsController> _logger;

        // Constructor for the FillTheGapsController, which takes an instance of ILogger as a parameter
        // ASP.NET Core automatically injects the appropriate ILogger instance when creating an instance of this controller
        public FillTheGapsController(ILogger<FillTheGapsController> logger)
        {
            // Store the injected ILogger instance in the private '_logger' field
            _logger = logger;
        }

        public override bool Equals(object? obj)
        {
            return obj is FillTheGapsController controller &&
                   EqualityComparer<ILogger<FillTheGapsController>>.Default.Equals(_logger, controller._logger);
        }

        // Define a new HTTP POST action method for the WeatherForecast controller
        // This action method will be accessible via a POST request to the route defined by the 'Route' attribute (in this case, '[controller]/fillthegaps')
        // The name of this action method is 'FtG', so the full route will be '/WeatherForecast/fillthegaps'
        [HttpPost("fillthegaps")]
        /*
        // This method returns an IActionResult
        // An IActionResult represents an HTTP response
        // This API should take as input the following parameters:
        // - text: the original text
        // - topic: the topic of the text
        // - level: the level of the text
        // - n_o_w: the number of words that the text should have. The actual number can vary from this by +- 30.
        // - n_o_g: the number of gaps that the text must have.
        // - n_o_d: the number of distractors that the text must have.
        // - temperature: the temperature of the prompt
        // - gaps: an array of strings; it contains the words that could be gaps in the text
        // - distractors: an array of strings; it contains the distractors that could be used in the text
        // In this case, the response will be an HTTP 200 OK response with the JSON of the fill the gaps exercise as the response body
        */
        public IActionResult GetFtG([FromBody] FillTheGapsRequestModel requestModel)
        {
            // Create a copy of the original text to work with
            string t_w_g = requestModel.Text;
            //initialize n_o_g and n_o_d
            int n_o_g;
            int n_o_d;
            // Ensure that there are enough candidate gaps & distractors to satisfy the request
            if(requestModel.N_o_g > requestModel.Gaps.Length)
            {
                n_o_g = requestModel.Gaps.Length;
            } else {n_o_g = requestModel.N_o_g;}
            if(requestModel.N_o_d > requestModel.Distractors.Length)
            {
                n_o_d = requestModel.Distractors.Length;
            } else {n_o_d = requestModel.N_o_d;}

            // Randomly choose n_o_g words from the 'gaps' array
            Random random = new();
            List<string> words = new(requestModel.Gaps);
            List<string> distractors = new(requestModel.Distractors);
            for (int i = words.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                string temp = words[i];
                string tempd = distractors[i];
                words[i] = words[j];
                distractors[i] = distractors[j];
                words[j] = temp;
                distractors[j] = tempd;
            }
            // Choose n_o_g words from the 'gaps' array
            words = words.GetRange(0, n_o_g);
            // Choose n_o_d words from the 'distractors' array
            distractors = distractors.GetRange(0, n_o_d);

            // Replace all occurrences of the chosen words with "__________" in the text making sure it replaces it only when the entire word matches perfectly (dolor != dolores)
            foreach (string word in words)
            {
                t_w_g = Regex.Replace(t_w_g, @"\b" + word + @"\b", "__________");
            }

            // Shuffle the final list of words and distractors
            List<string> choices = new(words);
            choices.AddRange(distractors);
            for (int i = choices.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (choices[j], choices[i]) = (choices[i], choices[j]);
            }
            // Make choices an array of strings
            string[] p_choices = choices.ToArray();

            // Create a new instance of the Fill the Gaps class, passing the required parameters
            Fill_the_Gaps ftg = new(
                requestModel.Text,
                t_w_g,
                requestModel.Topic,
                requestModel.Level,
                requestModel.N_o_w,
                requestModel.N_o_g,
                requestModel.N_o_d,
                requestModel.Temperature,
                p_choices
                );

            // Return the JSON of the fill the gaps exercise as the response body
            return Ok(ftg);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
