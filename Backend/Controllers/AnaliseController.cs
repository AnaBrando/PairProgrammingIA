using System.Text;
using IAPairProgrammer.Service;
using Microsoft.AspNetCore.Mvc;

namespace IAPairProgrammer.Controllers;

[ApiController]
[Route("[controller]")]
public class AnaliseController : ControllerBase
{
    private readonly IOpenAiService _openAiService;

    public AnaliseController(IOpenAiService openAiService)
    {
        _openAiService = openAiService;
    }

    [HttpPost("analisar-upload")]
    public async Task<IActionResult> AnalisarComArquivo([FromForm] IFormFile file,
        [FromForm] string situacao)
    {
        Console.WriteLine("Requisição recebida!");
        
        if (file == null || file.Length == 0)
            return BadRequest("Arquivo não enviado.");

        using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
        var conteudoArquivo = await reader.ReadToEndAsync();

        var request = new CodigoUploadRequest
        {
            Situacao = situacao,
            Codigo = conteudoArquivo
        };

        var resposta = await _openAiService.EnviarPromptAsync(request);
        return Ok(new
            {
                refactoredCode = $"```csharp\n{resposta.RefactoredCode}\n```",
                unitTests = $"```csharp\n{resposta.UnitTests}\n```",
                documentation = $"```csharp\n{resposta.Documentation}\n```",
                explanation = resposta.Explanation
            });
    }
}