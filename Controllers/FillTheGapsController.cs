// Import necessary namespaces from the ASP.NET Core framework
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Microsoft.SemanticKernel;

// Declare the namespace for the FillTheGapsController
namespace SK_API.Controllers
{
    // Mark the FillTheGapsController as an ApiController to enable API behavior
    [ApiController]
    // Set the base route for all actions within this controller to be '[controller]'
    // This means that the route for this controller will be based on its name 'FillTheGaps'
    [Route("[controller]")]
    public partial class FillTheGapsController : ControllerBase
    {
        private readonly ILogger<FillTheGapsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public FillTheGapsController(ILogger<FillTheGapsController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        public override bool Equals(object? obj)
        {
            return obj is FillTheGapsController controller &&
                   EqualityComparer<ILogger<FillTheGapsController>>.Default.Equals(_logger, controller._logger);
        }

        private Fill_the_Gaps GetFG(string result, int N_o_g, int N_o_d, string level, int n_o_w, double temperature)
        {
            //parsing the output
            // Find the start and end of the original text
            int startIndex = result.ToString().IndexOf("Original text:") + "Original text:".Length;
            int endIndex = result.ToString().IndexOf("Words:");
            string original_text = result.ToString()[startIndex..endIndex].Trim();
            //Console.WriteLine(original_text);
            
            //initialize n_o_g and n_o_d
            int n_o_g;
            int n_o_d;

            // Find the start and end points of the "Words:" list
            int wordsStartIndex = result.IndexOf("Words:") + "Words:".Length;
            int wordsEndIndex = result.IndexOf("Distractors:");
            // Extract the "Words:" list
            string wordsList = result[wordsStartIndex..wordsEndIndex].Trim();
            //Console.WriteLine(wordsList);
            // Split the "Words:" list into individual items
            string[] wordsArray = MyRegex().Split(wordsList);
            for (int i = 0; i < wordsArray.Length; i++)
            {
                wordsArray[i] = wordsArray[i].Trim();
            }
            //remove the first element of the array
            wordsArray = wordsArray.Skip(1).ToArray();
            // Print the items in the array
            //foreach (string item in wordsArray)
            //{
            //    Console.WriteLine(item );
            //}
            //Console.WriteLine(wordsArray.Length);

            // Find the start and end points of the "Distractors:" list
            int distractorsStartIndex = result.IndexOf("Distractors:") + "Distractors:".Length;
            Console.WriteLine(distractorsStartIndex);
            //Console.WriteLine("Distractors:".Length);
            int distractorsEndIndex = result.IndexOf("[END OUTPUT]");
            Console.WriteLine(distractorsEndIndex);
            // Extract the "Distractors:" list
            string distractorsList = result[distractorsStartIndex..distractorsEndIndex].Trim();
            Console.WriteLine(distractorsList);
            // Split the "Distractors:" list into individual items
            string[] distractorsArray = MyRegex().Split(distractorsList);
            for (int i = 0; i < distractorsArray.Length; i++)
            {
                distractorsArray[i] = distractorsArray[i].Trim();
            }
            //remove the first element of the array
            distractorsArray = distractorsArray.Skip(1).ToArray();
            // Print the items in the array
            //foreach (string item in distractorsArray)
            //{
            //    Console.WriteLine(item);
            //}
            //Console.WriteLine(distractorsArray.Length);
            
            // Ensure that there are enough candidate gaps & distractors to satisfy the request
            if(N_o_g > wordsArray.Length)
            {
                n_o_g = wordsArray.Length;
            } else {n_o_g = N_o_g;}
            if(N_o_d > n_o_g)
            {
                n_o_d = n_o_g;
            } else {n_o_d = N_o_d;}
            //Console.WriteLine(n_o_g);
            //Console.WriteLine(n_o_d);

            // Randomly choose n_o_g words from the 'gaps' array
            Random random = new();
            List<string> words = new(wordsArray);
            List<string> distractors = new(distractorsArray);
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
            //print the words
            //foreach (string item in words)
            //{
            //    Console.WriteLine(item);
            //} 

            // Choose n_o_d words from the 'distractors' array
            distractors = distractors.GetRange(0, n_o_d);
            //print the distractors
            //foreach (string item in distractors)
            //{
            //    Console.WriteLine(item);
            //}

            // create a codpy of the original text to work with
            string t_w_g = original_text;
            // Replace all occurrences of the chosen words with "__________" in the text making sure it replaces it only when the entire word matches perfectly (dolor != dolores)
            foreach (string word in words)
            {
                string pattern = @"\b" + Regex.Escape(word) + @"\b";
                RegexOptions options = RegexOptions.IgnoreCase;
                Regex regex = new(pattern, options);
                t_w_g = regex.Replace(t_w_g, "__________");
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
                original_text,
                t_w_g,
                level,
                n_o_w,
                n_o_g,
                n_o_d,
                temperature,
                p_choices
                );

            // Return the JSON of the fill the gaps exercise as the response body
            return ftg;
        }

        // Define a new HTTP POST action method for the FillTheGaps controller
        // This action method will be accessible via a POST request to the route defined by the 'Route' attribute (in this case, '[controller]/prompt')
        // The name of this action method is 'prompt', so the full route will be '/FillTheGaps/generatedexercise'
        [HttpPost("generateexercise")]
        //the following function has to 
        public async Task<IActionResult> GeneratePromptAsync([FromHeader(Name = "ApiKey")] string apiKey, [FromBody] FillTheGapsRequestModel requestModel)
        {   
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

        //extracting and summarizing the text
            // Create a FileOrUrl instance from the provided input.
            FileOrUrl source = new FileOrUrl(requestModel.Text);

            // Create an instance of the TextProcessor.
            TextProcessor textProcessor = new TextProcessor();

            // Create a Summarizer instance
            Summarizer summarizer = new(_configuration, _auth);

            // Call the method to extract text.
            string extractedText = textProcessor.ExtractTextFromFileOrUrl(source);
            
            var finalText = await summarizer.Summarize(apiKey, extractedText, requestModel.N_o_w);

        //defining the prompt & generating the semantic function
            string prompt = @"You are a {{$level}} professor. You have just given a lesson and now you want to create a fill the gaps exercise for your students about the lecture.
                            1) The summart of your lesson is {{$text}}; output it. (we will call this text: 'OriginalText')
                            2) Extract from the text 'OriginalText' all the proper nouns, dates/numbers  and all the scientific/specific terminology).  (we will call this list: 'Words')
                            Output the list 'Words'
                            3) For each word in the list 'Words' generate one similar word that can be proposed as a distractor in a fill the gaps exercise.  (we will call this list: 'Distractors') 
                            Output the list 'Distractors'
                            The format of the output has to be:
                            Original text:
                            text...
                            Words:
                            1)word1
                            2)word2
                            3)...
                            Distractors:
                            1)distractor1
                            2)distractor2
                            3)...
                            [END OUTPUT]

                            [INPUT]
                            {{$level}}
                            {{$type_of_text}}
                            {{$topic}}
                            {{$n_o_w}}
                            {{$n_o_g}}
                            [END INPUT]";

            var generate = kernel.CreateSemanticFunction(prompt, "generateFTG" ,"FillTheGaps", "generate exercise", null , requestModel.Temperature);
            //setting up the context
            var context = kernel.CreateNewContext();
            context["level"] = requestModel.Level.ToString();
            context["text"]  = finalText;
            context["n_o_w"] = requestModel.N_o_w.ToString();
            context["n_o_g"] = requestModel.N_o_g.ToString();
            var result = "";
            //generating the output using the LLM
            try
            {
                _logger.LogInformation("Invoking semantic function...");
                var output = await generate.InvokeAsync(context);
                _logger.LogInformation("Semantic function invoked successfully.");
                result = output.ToString();
                Console.WriteLine(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error invoking semantic function:");
                result = e.ToString();
            }

            _logger.LogInformation("Prompt generation complete.");

            //parse the result to get the final result
            var final = GetFG(result.ToString(), requestModel.N_o_g, requestModel.N_o_d, requestModel.Level.ToString(), requestModel.N_o_w, requestModel.Temperature);

            // Return the JSON of the fill the gaps exercise as the response body
            return Ok(final.ToString());
        }


        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        [GeneratedRegex("\\d+\\)")]
        private static partial Regex MyRegex();
    }
}
