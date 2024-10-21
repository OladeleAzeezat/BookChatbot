using ChatbotBackend.Services;
using Microsoft.AspNetCore.Mvc;
using OpenAI;

namespace ChatbotBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly AzureSearchService _azureSearchService;

        public ChatbotController(AzureSearchService azureSearchService)
        {
            _azureSearchService = azureSearchService;
        }

        // Endpoint for book search
        [HttpGet("{query}")]
        public async Task<IActionResult> Search(string query)
        {
            var results = await _azureSearchService.SearchBooksAsync(query);
            return Ok(results); // This returns a 200 status code with the search results.
        }

        // Endpoint to ask questions with AI
        [HttpPost("ask")]
        public async Task<IActionResult> AskQuestion([FromBody] string prompt)
        {
            var answer = await _azureSearchService.AskQuestionAsync(prompt);
            return Ok(answer);
        }

        [HttpGet("test")]
        public async Task<IActionResult> TestApi()
        {
            var result = await _azureSearchService.Test();
            return Ok(result);
        }
       
    }
}
    public class ChatRequest
    {
        public string BookTitle { get; set; }
        public string Query { get; set; }
    }
