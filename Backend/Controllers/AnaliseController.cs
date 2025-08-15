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
    public async Task<IActionResult> AnalisarComArquivo([FromForm] IFormFile file, [FromForm] string codigo,
        [FromForm] string situacao)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        var conteudoArquivo = await reader.ReadToEndAsync();

        var codigoCompleto = string.IsNullOrWhiteSpace(codigo)
            ? conteudoArquivo
            : $"{situacao}\n\n{conteudoArquivo}\n\n{codigo}";

        var resposta = await _openAiService.EnviarPromptAsync(new CodigoUploadRequest()
            { Codigo = codigo, Situacao = situacao, File = file });
        return Ok(new { resposta });
    }
}