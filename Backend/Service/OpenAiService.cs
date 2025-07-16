using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using IAPairProgrammer.Data;

namespace IAPairProgrammer.Service;

public class OpenAiService : IOpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IMemoryService _memoryService;

    public OpenAiService(HttpClient httpClient, IConfiguration configuration, IMemoryService memoryService)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _memoryService = memoryService;
    }

    public async Task<RespostaIA> EnviarPromptAsync(string codigo)
    {
        // 🔍 Step 1: Get embedding
        var userEmbedding = await ObterEmbeddingAsync(codigo);

        // 🧠 Step 2: Load memory
        var pastMessages = await _memoryService.BuscarMensagensSimilares(userEmbedding);

        // 🏗️ Step 3: Build prompt
        var messages = new List<object>
        {
            new
            {
                role = "system",
                content = """
                            You are an expert C# developer acting as a helpful and precise pair programmer.

                            Your goal is to assist the user in improving their .NET code by following best practices in readability, performance, SOLID principles, and testability.

                            When a user sends a code snippet, follow these steps:

                            1. Refactor the Code  
                            - Apply .NET and C# best practices (e.g. naming, async usage, clean architecture).
                            - Improve readability, performance, and maintainability.

                            2. Generate Unit Tests  
                            - Use xUnit or NUnit (default to xUnit unless specified).
                            - Cover the key logic paths.

                            3. Add XML Documentation  
                            - Add `///` comments to public methods and classes.
                            - Summarize the purpose, parameters, and return values.

                            4. Explain Your Reasoning  
                            - Briefly explain what you changed and why.
                            - Focus on educational and technical clarity.

                            Always return your output as a well-formatted JSON using the following schema:

                            ```json
                            {
                            "refactoredCode": "C# code string here...",
                            "unitTests": "xUnit code string here...",
                            "documentation": "XML-commented code string here...",
                            "explanation": "Explanation string here..."
                            }
                          """ 
        } 
    };
        foreach (var msg in pastMessages)
    {
        messages.Add(new { role = msg.Role, content = msg.Content });
    }

    messages.Add(new
    {
        role = "user",
        content = $"Analise o código abaixo e retorne um JSON com as seções esperadas.\n\nCódigo:\n\n{codigo}"
    });

    // 🧠 Step 4: Call OpenAI
    var requestBody = new { model = "gpt-4o", messages, temperature = 0.0 };
    var json = JsonSerializer.Serialize(requestBody);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    _httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", ObterApiKey());

    var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
    response.EnsureSuccessStatusCode();
    var responseString = await response.Content.ReadAsStringAsync();

    var contentRaw = JsonDocument.Parse(responseString)
        .RootElement.GetProperty("choices")[0]
        .GetProperty("message")
        .GetProperty("content")
        .GetString();

    var jsonClean = contentRaw
        .Replace("```json", "")
        .Replace("```", "")
        .Trim();

    var resultado = JsonSerializer.Deserialize<RespostaIA>(jsonClean);

    // 💾 Step 5: Save memory
    await _memoryService.SaveMessageAsync("user", codigo, userEmbedding);
    var assistantEmbedding = await ObterEmbeddingAsync(contentRaw);
    await _memoryService.SaveMessageAsync("assistant", contentRaw, assistantEmbedding);

    return resultado!;
}

private string? ObterApiKey()
{
    return Environment.GetEnvironmentVariable("OPENAI_API_KEY");
}

public async Task<float[]> ObterEmbeddingAsync(string texto)
{
    var requestBody = new
    {
        input = texto,
        model = "text-embedding-3-small"
    };

    var json = JsonSerializer.Serialize(requestBody);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    _httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", ObterApiKey());

    var response = await _httpClient.PostAsync("https://api.openai.com/v1/embeddings", content);
    response.EnsureSuccessStatusCode();

    var responseString = await response.Content.ReadAsStringAsync();

    using var doc = JsonDocument.Parse(responseString);
    var embeddingArray = doc.RootElement
        .GetProperty("data")[0]
        .GetProperty("embedding")
        .EnumerateArray()
        .Select(e => e.GetSingle())
        .ToArray();

    return embeddingArray;
 }
}
