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

    [HttpPost("analisar")]
    public async Task<IActionResult> AnalisarCodigo([FromBody] CodigoRequest request)
    {
        var resposta = await _openAiService.EnviarPromptAsync(request.Codigo);
        return Ok(new { resposta });
    }

    [HttpGet("inline")]
   
    public string AnalisarCodigo([FromQuery] string codigo)
    {
        return CompactarCodigo(codigo);
    }

public static string CompactarCodigo(string codigo)
{
    return codigo
        .Replace("\r\n", " ")  // Para Windows
        .Replace("\n", " ")    // Para Linux/Mac
        .Replace("\t", " ")    // Remove tabulação
        .Replace("\"", "\\\"") // Escapa aspas para JSON
        .Replace("  ", " ")    // Remove espaços duplos (opcional)
        .Trim();
}

}