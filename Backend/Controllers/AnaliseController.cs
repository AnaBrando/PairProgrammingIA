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

    [HttpPost("analisar")]
    public async Task<IActionResult> AnalisarCodigo([FromBody] CodigoRequest request)
    {
        var resposta = await _openAiService.EnviarPromptAsync(request.Codigo);
        return Ok(new { resposta });
    }


}