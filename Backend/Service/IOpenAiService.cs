namespace IAPairProgrammer.Service;

public interface IOpenAiService
{
    Task<RespostaIA> EnviarPromptAsync(CodigoUploadRequest request);
}