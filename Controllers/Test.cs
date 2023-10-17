// Import necessary namespaces from the ASP.NET Core framework
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

// Declare the namespace for the SummarizerController
namespace SK_API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public partial class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IConfiguration _configuration;

        public TestController(ILogger<TestController> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // Define your Lesson POST action method here
        [HttpPost("testInputFile")]
        public IActionResult TestInput(InputModel input)
        {
            try
            {
                if (string.IsNullOrEmpty(input.PathOrUrl))
                {
                    return BadRequest("Input path or URL is required.");
                }

                // Create a FileOrUrl instance from the provided input.
                FileOrUrl source = new FileOrUrl(input.PathOrUrl);

                // Create an instance of the TextProcessor.
                TextProcessor textProcessor = new TextProcessor();

                // Call the method to extract text.
                string extractedText = textProcessor.ExtractTextFromFileOrUrl(source);

                if (!string.IsNullOrEmpty(extractedText))
                {
                    // You can return the extracted text as a JSON response or in any other desired format.
                    return Ok(new { ExtractedText = extractedText });
                }
                else
                {
                    return NotFound("No text could be extracted from the provided source.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions if something goes wrong during the text extraction.
                _logger.LogError(ex, "Error during text extraction.");
                return StatusCode(500, "Internal Server Error");
            }

        }
    }
}