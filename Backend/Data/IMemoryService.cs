namespace IAPairProgrammer.Data;

public interface IMemoryService
{
    Task SaveMessageAsync(string role, string content, float[] embedding);
    Task<List<ChatMessage>> BuscarMensagensSimilares(float[] targetVector, int topN = 3);
}
