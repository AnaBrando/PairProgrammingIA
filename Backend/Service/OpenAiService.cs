using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace IAPairProgrammer.Service;

public class OpenAiService : IOpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OpenAiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }


    public async Task<RespostaIA> EnviarPromptAsync(string codigo)
    {
        var requestBody = new
        {
            model = "gpt-4o",
            messages = new[]
            {
                new
                {
                    role = "system",
                    content =
                        "Você é um engenheiro especialista em C#/.NET. Sempre responda com um JSON válido contendo as chaves: melhoria, documentacao, explicacao, teste_unitario. Não escreva nada fora do JSON."
                },
                new
                {
                    role = "user",
                    content = $"Analise o código abaixo e retorne um JSON contendo:\n\n" +
                              "- melhoria: como melhorar o código\n" +
                              "- documentacao: XML doc do método\n" +
                              "- explicacao: explicação técnica\n" +
                              "- teste_unitario: exemplo de teste unitário\n\n" +
                              $"Código:\n\n{codigo}"
                }
            },
            temperature = 0.0
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", ObterApiKey());

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(responseString);
        var contentRaw = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        // Remove as crases e identação Markdown
        var jsonClean = contentRaw
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();

        var resultado = JsonSerializer.Deserialize<RespostaIA>(jsonClean);

        return resultado;
    }

    private string? ObterApiKey()
    {
        var text  = File.ReadAllText(@"C:\Projeto\text.txt");
        return text;
    }
}