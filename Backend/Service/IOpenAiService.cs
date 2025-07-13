namespace IAPairProgrammer.Service;

public interface IOpenAiService
{
    Task<RespostaIA> EnviarPromptAsync(string prompt);
}