using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

// Constructs prompt messages
class PromptMessage {
  public string Role { get; set; } = "user";
  public string Content { get; set; } = "";
}

namespace TestOpenAIReq
{
    class Program
    {
        static void Main()
        {
          // Prompt the user for input
          Console.WriteLine("Enter Prompt:");
          string prompt = Console.ReadLine() ?? throw new Exception("You have to ask me something");

          // Create a new HttpClient and set params
          using var client = new HttpClient();
          string OPENAI_API_KEY = Environment.GetEnvironmentVariable("OPENAI_API_KEY") 
            ?? throw new Exception("OPENAI_API_KEY is not set");
          client.BaseAddress = new Uri("https://api.openai.com/v1/chat/completions");
          client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
          client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OPENAI_API_KEY);
          
          // Make a POST request to the OpenAI API
          var response = client.PostAsJsonAsync(client.BaseAddress.ToString(), new { 
            messages = new List<PromptMessage> {
              new() { Role = "system", Content = "You are a helpful assistant."},
              new() { Role = "user", Content = prompt }
            },
            model = "gpt-3.5-turbo",
            max_tokens = 500,
            temperature = 0.5,
          }).Result;

          // Check if the request was successful
          if (response.IsSuccessStatusCode) {
            string answer = response.Content.ReadAsStringAsync().Result 
              ?? throw new Exception("No response from OpenAI");
            JsonNode parsed = JsonNode.Parse(answer) 
              ?? throw new Exception("Failed to parse response");
            Console.WriteLine(parsed["choices"]?[0]?["message"]?["content"]);
          } else {
            Console.WriteLine($"Failed on request: {response.ReasonPhrase}");
          }
        }
    }
}
