using Azure.Search.Documents;
using Azure;
using ChatbotBackend.Model;
using Microsoft.SemanticKernel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.AspNetCore.Http.HttpResults;
using OpenAI.Chat;

namespace ChatbotBackend.Services
{
    public class AzureSearchService
    {
        private readonly SearchClient _searchClient;
        private Kernel _kernel;
        public AzureSearchService(IConfiguration configuration)
        {
            var serviceName = configuration["AzureSearch:ServiceName"];
            var apiKey = configuration["AzureSearch:ApiKey"];
            var indexName = configuration["AzureSearch:IndexName"];

            string endpoint = $"https://{serviceName}.search.windows.net";
            var credential = new AzureKeyCredential(apiKey);

            _searchClient = new SearchClient(new Uri(endpoint), indexName, credential);

            // Semantic Kernel setup (OpenAI Chat Completion)
            string openAiApiKey = configuration["AzureOpenAI:ApiKey"];
            string openAiEndpoint = configuration["AzureOpenAI:Endpoint"];
            string deploymentName = configuration["AzureOpenAI:DeploymentName"];

            // Initialize the Semantic Kernel
         
            _kernel = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(deploymentName, openAiEndpoint, openAiApiKey)
            .Build();

          

        }
        public async Task<string> Test()
        {
            var chat = _kernel.GetRequiredService<IChatCompletionService>();

            // Create a sample chat history
            var history = new ChatHistory();
            history.AddSystemMessage("You are a helpful assistant.");
            history.AddUserMessage("Who won the world cup in 2022?");
            history.AddAssistantMessage("Argentina won in 2022.");
            history.AddUserMessage("Where was it played? and who was the best player?");

            // run the prompt
            var result = await chat.GetChatMessageContentsAsync(history);
            return result[^1].Content;
            //Console.WriteLine(result[^1].Content);
        }
        public async Task<List<Book>> SearchBooksAsync(string query)
        {
            var options = new SearchOptions
            {
                Filter = null,
                Size = 10
            };

            var searchResults = await _searchClient.SearchAsync<Book>(query, options);

            return searchResults.Value.GetResults().Select(r => r.Document).ToList();
        }
        public async Task<string> AskQuestionAsync(string prompt)
        {
            var chat = _kernel.GetRequiredService<IChatCompletionService>();

            // Create a chat history object, similar to Test method
            var history = new ChatHistory();
            history.AddSystemMessage("You are a helpful assistant.");
            history.AddUserMessage(prompt);

            // Get the assistant's response
            var result = await chat.GetChatMessageContentsAsync(history);

            // Return the last message in the chat history (the assistant's response)
           return result[^1].Content;
            
        }

        //public async Task<List<string>> AskQuestionAsync(string prompt)
        //{
        //    var chat = _kernel.GetService<IChatCompletion>();

        //    // Create chat history for AI to generate a response
        //    var history = new ChatHistory();
        //    history.AddSystemMessage("You are a helpful assistant.");
        //    history.AddUserMessage(prompt);

        //    // Get the assistant's response
        //    var result = await chat.GenerateMessageAsync(history);

        //    // Return the entire history as a list of strings
        //    var fullHistory = history.Messages.Select(m => m.Content).ToList();
        //    fullHistory.Add(result.Content); // Add the assistant's latest response
        //    return fullHistory;
        //}


    }
}
