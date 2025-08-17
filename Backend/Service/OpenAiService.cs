using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using IAPairProgrammer.Builder;
using IAPairProgrammer.Data;

namespace IAPairProgrammer.Service;

public class OpenAiService : IOpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IMemoryService _memoryService;
    private readonly PromptBuilder _builder;

    public OpenAiService(HttpClient httpClient, IConfiguration configuration, IMemoryService memoryService)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _memoryService = memoryService;
        _builder = new PromptBuilder();
    }

    public async Task<RespostaIA> EnviarPromptAsync(CodigoUploadRequest request)
    {
        
        // 🔍 1. Gerar embedding do código do usuário
        var userEmbedding = await ObterEmbeddingAsync(request.Situacao);

        // 🧠 2. Buscar memórias similares
        var pastMessages = await _memoryService.BuscarMensagensSimilares(userEmbedding);

        // 🏗️ 3. Construir prompt com ajuda do PromptBuilder
        var mensagens = _builder.ConstruirPrompt(pastMessages,request);

        // 🚀 4. Chamada à OpenAI API
        var resultadoIA = await EnviarParaOpenAiAsync(mensagens);

        // 💾 5. Salvar mensagens na memória
        await _memoryService.SaveMessageAsync("user", request.Situacao, userEmbedding);

        var assistantEmbedding = await ObterEmbeddingAsync(resultadoIA.RawContent);
        await _memoryService.SaveMessageAsync("assistant", resultadoIA.RawContent, assistantEmbedding);

        return resultadoIA.Parsed!;
    }

    #region OpenAI Request Helpers

    private async Task<RespostaWrapper> EnviarParaOpenAiAsync(List<object> mensagens)
    {
        var requestBody = new
        {
            model = "gpt-4o",
            messages = mensagens,
            temperature = 0.0
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", ObterApiKey());

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var rawContent = JsonDocument.Parse(responseString)
            .RootElement.GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        var jsonClean = rawContent!
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var parsed = JsonSerializer.Deserialize<RespostaIA>(jsonClean, options);
            return new RespostaWrapper
            {
                RawContent = rawContent!,
                Parsed = parsed
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    private string? ObterApiKey()
    {
        var result = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        return result;
    }

    #endregion

    #region Embeddings

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

        return doc.RootElement
            .GetProperty("data")[0]
            .GetProperty("embedding")
            .EnumerateArray()
            .Select(e => e.GetSingle())
            .ToArray();
    }

    #endregion
}
public class RespostaWrapper
{
    public string RawContent { get; set; }
    public RespostaIA Parsed { get; set; }
}
