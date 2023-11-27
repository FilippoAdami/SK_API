// Import necessary namespaces from the ASP.NET Core framework
using System.Text.RegularExpressions;
using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

// Declare the namespace for the QuestionExerciseController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class QuestionExerciseController : ControllerBase
    {
        private readonly ILogger<QuestionExerciseController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Auth _auth;

        public QuestionExerciseController(ILogger<QuestionExerciseController> logger, IConfiguration configuration, Auth auth)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _auth = auth;
        }

        private OpenQuestion GetFG(string language, string result, string type, string level, string category, double temperature){
            //parsing the output
            // Find the start and end of the original text
            int startIndex = result.ToString().IndexOf("Original text:") + "Original text:".Length;
            int endIndex = result.ToString().IndexOf("Question:");
            string original_text = result.ToString()[startIndex..(endIndex-category.Length)].Trim();
            Console.WriteLine("Original text: \n"+original_text);

            //extract the question
            startIndex = endIndex + "Question:".Length;
            endIndex = result.ToString().IndexOf("Correct answer:");
            string question = result.ToString()[startIndex..endIndex].Trim();
            //remove eventual \n from the question
            question = question.Replace("\n", " ");
            Console.WriteLine("Question:\n"+question);

            //extract the correct answer
            startIndex = endIndex + "Correct answer:".Length;
            string correct_answer = result.ToString()[startIndex..].Trim();
            Console.WriteLine("Correct Answer:\n"+correct_answer);
            //remove eventual \n from the question
            question = question.Replace("\n", " ");
            if(type == "ShortAnswer" && correct_answer.Length>20 ){
                type="Open";
            }
            //create the question object
            OpenQuestion openQuestion = new OpenQuestion(language, type, level, category, temperature, question, correct_answer);

            return openQuestion;
        }

        // Define your QuestionExcercise POST action method here
        [HttpPost("generateexercise")]
        public async Task<IActionResult> GenerateQuestionExcercise([FromHeader(Name = "ApiKey")] string apiKey, [FromBody] QuestionExcerciseRequestModel requestModel)
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

        //extracting and summarizing the text
            // Create a FileOrUrl instance from the provided input.
            string source = requestModel.Text;

            // Create an instance of the TextProcessor.
            TextProcessor textProcessor = new TextProcessor();

            // Create a Summarizer instance
            Summarizer summarizer = new(_configuration, _auth);

            // Call the method to extract text.
            string extractedText = textProcessor.ExtractTextFromFileOrUrl(source);
            
            var finalText = await summarizer.Summarize(apiKey, extractedText, requestModel.Level.ToString());

            //defining the prompts & generating the semantic function
            /*
            string prompt1 = @"You are a {{$level}} level professor that just gave a lecture. Now you want to create a question for your students about your lesson.
                            1) The summary of your lesson is {{$text}} (we will call this text: 'OriginalText')
                            Output the text
                            2) Now you have to consider these possible types of Q/A examples:
                            -Factual Knowledge: Question: Which year did World War II end? a) 1940 b) 1945 c) 1939 d) 1950 Correct Answer: b) 1945 
                            -Understanding of Concepts: Question: Which of the following best describes the impact of World War II on global geopolitics? a) It led to the emergence of the United States and the Soviet Union as superpowers. b) It resulted in the dominance of colonial empires. c) It had no significant impact on global geopolitics. d) It led to the unification of Europe. Correct Answer: a) It led to the emergence of the United States and the Soviet Union as superpowers. 
                            -Application of Skills: Question: If you were a historian researching the social impact of World War II, which of the following sources would be most relevant? a) Military strategy documents b) Letters from soldiers to their families c) Modern movies about World War II d) Fiction books set during World War II Correct Answer: b) Letters from soldiers to their families 
                            -Analysis and Evaluation: Question: Which of the following arguments is best supported by historical evidence? a) World War II had no significant impact on women\'s roles in society. b) World War II was solely caused by Germany. c) World War II led to significant advancements in military technology. d) World War II was avoidable. Correct Answer: c) World War II led to significant advancements in military technology.
                            3) Using Original text as context, extract from that text one important concept and generate one {{$type}} question about that topic.
                            Output the one question.
                            4) Generate one possible correct answers for the question.
                            Output the answers

                            The final output of your answer must be in the format:
                            Original text:
                            ...text...
                            {{$type}}Question:
                            ...question...
                            Correct answer:
                            ...correct answer...";
            string prompt2 = @"You are a {{$level}} level professor that just gave a lecture. Now you want to create a question for your students about your lesson. 
                            1) The summary of your lesson is {{$text}} (we will call this text: 'OriginalText')
                            Output the text
                            2) Now consider this short answer question examples to understand the format:
                            - Which year did World War II end? Correct Answer: 1945 
                            - What's the capital of Italy? Correct Answer: Rome
                            - What is the periodic symbol for Aluminium? Correct Answer: Al
                            - Given the equation  3x + 2 = 0, find x. Correct Answer: -2/3
                            3) Using Original text, extract from that text one important concept and generate one short answer question about that topic.
                            Output the one question.
                            4) Generate the correct short answers for the question like in the examples given (maximum two words).
                            Output the answers
                            The final output of your answer must be in the format:
                            Original text:
                            Question:
                            Correct answer:";
            string prompt3 = @"You are a {{$level}} level professor that just gave a lecture. Now you want to create a question for your students about your lesson.
                            1) The summary of your lesson is {{$text}} (we will call this text: 'OriginalText')
                            Output the text
                            2) Now you have to consider this true or false exercises examples:
                            There are three rivers in Saudi Arabia.
                            Correct answer: False, Saudi Arabia is one of 17 countries in the world with no rivers flowing through them.
                            The Great Wall of China is visible from space.
                            Correct answer: False, This is a myth. the Wall is even difficult to see from a low search orbit without artificial magnification. 
                            The Atlantic Ocean is the warmest in the world.
                            Correct answer: False, While the Atlantic Ocean is the warmest it has been in 3000 years, it still isn’t as warm as the Indian Ocean.
                            When the Eiffel Tower was unveiled, the Parisian art community hated it.
                            Correct answer: True, Some say the author Guy de Maupassant hated it so much that he had his lunch in it every day just as not to see the monstrous landmark while eating.
                            There are more ancient pyramids in Sudan than in Egypt.
                            Correct answer: True, Egypt has roughly 100 ancient pyramids, while Sudan has roughly 250.
                            3) Using Original text, extract from that text one important concept and generate one true or false statement about that topic following the provided format.
                            Output the one statement and its solution
                            The final output of your answer must be in the format:
                            Original text:
                            Question:
                            Correct answer: ture/false, solution...";
            */
            string prompt;
            string example;
            string type_of_answer;
            string type_of_exercise;
            string format;
            Console.WriteLine(requestModel.Type.ToString());
            if (requestModel.Type == TypeOfQuestion.Open){
                type_of_exercise = "open question";
                type_of_answer = "answer";
                example = @"-Factual Knowledge: Question: Which year did World War II end? Correct Answer: World War II ended in 1945. The war in Europe concluded with the unconditional surrender of Nazi Germany on May 7, 1945, which was officially ratified on May 8, 1945, known as Victory in Europe (VE) Day. The war in the Pacific ended later, after the United States dropped atomic bombs on the Japanese cities of Hiroshima on August 6, 1945, and Nagasaki on August 9, 1945. The Japanese government announced its surrender on August 15, 1945, and the formal signing of the Instrument of Surrender took place on September 2, 1945, aboard the USS Missouri, officially marking the end of World War II.
                            -Understanding of Concepts: Question: What impact did World War II on global geopolitics? Correct Answer: World War II had a profound and far-reaching impact on global geopolitics, reshaping the political, economic, and social landscape in numerous ways such as superpower emergence: the U.S. and Soviet Union became superpowers, defining post-war geopolitics; cold war division: ideological differences led to the Cold War, influencing global politics; decolonization and Global Shifts: economic changes, decolonization, and new alliances reshaped the geopolitical landscape.
                            -Application of Skills: Question: If you were a historian researching the social impact of World War II, what source would be most relevant? Correct Answer: The personal diaries of civilians and soldiers from diverse backgrounds offer invaluable insights into the social impact of World War II. These primary sources provide raw, unfiltered perspectives on daily life, emotions, and societal changes, offering a nuanced understanding of the war's profound effects on individuals and communities.
                            -Analysis and Evaluation: Question: How did the global economic and political landscape transform in the aftermath of World War II, and to what extent did these changes contribute to shaping the post-war world order? Correct Answer: The aftermath of World War II reshaped the global order. The rise of superpowers, decolonization, and the establishment of international institutions were pivotal. This transformative era laid the foundation for Cold War dynamics and defined the geopolitical landscape of the second half of the 20th century.";
                format = @"Original text:
                            ...text...
                            {{$type}} Question:
                            ...question...
                            Correct answer:
                            ...correct answer...";
            }
            else if (requestModel.Type == TypeOfQuestion.ShortAnswer){
                type_of_exercise = "short answer question";
                type_of_answer = "short answer (no more than 18 chars)";
                example = @"- Which year did World War II end? Correct Answer: 1945 
- What's the capital of Italy? Correct Answer: Rome
- What is the periodic symbol for Aluminium? Correct Answer: Al
- Given the equation  3x + 2 = 0, find x. Correct Answer: -2/3
- What is the largest planet in our solar system? Correct Answer: Jupiter
- Who wrote the play 'Romeo and Juliet'? Correct Answer: William Shakespeare
- In which year did the first moon landing occur? Correct Answer: 1969
- What is the chemical symbol for gold? Correct Answer: Au
- What is the square root of 64? Correct Answer: 8
- Which gas do plants primarily absorb during photosynthesis? Correct Answer: Carbon dioxide (CO2)
- Who is the author of the Harry Potter book series? Correct Answer: J.K. Rowling
- What is the capital of Japan? Correct Answer: Tokyo
- What is the boiling point of water in Celsius? Correct Answer: 100 degrees Celsius
- Who developed the theory of relativity? Correct Answer: Albert Einstein";
                format = @"Original text:
Question:
Correct answer:";
            }
            else if(requestModel.Type == TypeOfQuestion.TrueFalse){
                type_of_exercise = "true or false statement";
                type_of_answer = "true or false, plus the solution";
                example = @"-There are three rivers in Saudi Arabia.
                            Correct answer: False, Saudi Arabia is one of 17 countries in the world with no rivers flowing through them.
                            -The Great Wall of China is visible from space.
                            Correct answer: False, This is a myth. the Wall is even difficult to see from a low search orbit without artificial magnification. 
                            -The Atlantic Ocean is the warmest in the world.
                            Correct answer: False, While the Atlantic Ocean is the warmest it has been in 3000 years, it still isn’t as warm as the Indian Ocean.
                            -When the Eiffel Tower was unveiled, the Parisian art community hated it.
                            Correct answer: True, Some say the author Guy de Maupassant hated it so much that he had his lunch in it every day just as not to see the monstrous landmark while eating.
                            -There are more ancient pyramids in Sudan than in Egypt.
                            Correct answer: True, Egypt has roughly 100 ancient pyramids, while Sudan has roughly 250.";
                format = @"Original text:
                           Question:
                           Correct answer: ture/false, solution...";
            }
            else {
                //default case
                type_of_exercise = "open question";
                type_of_answer = "answer";
                example = @"-Factual Knowledge: Question: Which year did World War II end? a) 1940 b) 1945 c) 1939 d) 1950 Correct Answer: b) 1945 
                            -Understanding of Concepts: Question: Which of the following best describes the impact of World War II on global geopolitics? a) It led to the emergence of the United States and the Soviet Union as superpowers. b) It resulted in the dominance of colonial empires. c) It had no significant impact on global geopolitics. d) It led to the unification of Europe. Correct Answer: a) It led to the emergence of the United States and the Soviet Union as superpowers. 
                            -Application of Skills: Question: If you were a historian researching the social impact of World War II, which of the following sources would be most relevant? a) Military strategy documents b) Letters from soldiers to their families c) Modern movies about World War II d) Fiction books set during World War II Correct Answer: b) Letters from soldiers to their families 
                            -Analysis and Evaluation: Question: Which of the following arguments is best supported by historical evidence? a) World War II had no significant impact on women\'s roles in society. b) World War II was solely caused by Germany. c) World War II led to significant advancements in military technology. d) World War II was avoidable. Correct Answer: c) World War II led to significant advancements in military technology.";
                format = @"Original text:
                            ...text...
                            {{$type}} Question:
                            ...question...
                            Correct answer:
                            ...correct answer...";
            }
            prompt = new Prompt(type_of_answer, type_of_exercise, example, format).ToString();
            Console.WriteLine(prompt);
            var generate = kernel.CreateSemanticFunction(prompt, "generateQuestion" ,"Question", "generate exercise", null , requestModel.Temperature);
            //setting up the context
            var context = kernel.CreateNewContext();
            context["level"] = requestModel.Level.ToString();
            context["category"] = requestModel.Category.ToString();
            context["text"] = finalText;
            context["type"] = requestModel.Category.ToString();
            //generating the output using the LLM
            while (try_count<3){
            try
            {
                var result = await generate.InvokeAsync(context);
                Console.WriteLine(result);
                var final = GetFG(requestModel.Language, result.ToString(), requestModel.Type.ToString(), requestModel.Level.ToString(), requestModel.Category.ToString(), requestModel.Temperature);
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
                try_count++;
                error = e.Message;
                Console.WriteLine("error number :"+try_count);
            }
            }
            return BadRequest(error);
        }
        [GeneratedRegex("\\d+\\)")]
        private static partial Regex MyRegex();
    }
}