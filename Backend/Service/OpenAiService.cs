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
                content = "Você é um engenheiro especialista em C#/.NET. Sempre responda com um JSON válido..."
            }
        };

        foreach (var msg in pastMessages)
        {
            messages.Add(new { role = msg.Role, content = msg.Content });
        }

        messages.Add(new
        {
            role = "user",
            content = $"Analise o código abaixo e retorne um JSON com...\n\nCódigo:\n\n{codigo}"
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

        var jsonClean = contentRaw.Replace("```json", "").Replace("```", "").Trim();
        var resultado = JsonSerializer.Deserialize<RespostaIA>(jsonClean);

        // 💾 Step 5: Save memory
        await _memoryService.SaveMessageAsync("user", codigo, userEmbedding);
        var assistantEmbedding = await ObterEmbeddingAsync(contentRaw);
        await _memoryService.SaveMessageAsync("assistant", contentRaw, assistantEmbedding);

        return resultado!;
    }

    private string? ObterApiKey()
    {
        var text = File.ReadAllText(@"C:\Projeto\text.txt");
        return text;
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

    public async Task DoEmbedding(string userInput)
    {
        // STEP 1 - Get embedding for the user’s message
        var vector = await ObterEmbeddingAsync(userInput);

        // STEP 2 - Search your SQLite messages (e.g., top 3 similar)
        var pastMessages = await _memoryService.BuscarMensagensSimilares(vector);

        // STEP 3 - Build the chat history
        var messages = new List<object>
        {
            new { role = "system", content = "You are a .NET pair programmer..." }
        };

        foreach (var msg in pastMessages)
        {
            messages.Add(new { role = msg.Role, content = msg.Content });
        }

        // Add the new user message
        messages.Add(new { role = "user", content = userInput });
    }
}