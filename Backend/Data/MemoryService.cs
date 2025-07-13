using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace IAPairProgrammer.Data;

public class MemoryService : IMemoryService
{
    private readonly AppDbContext _db;

    public MemoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task SaveMessageAsync(string role, string content, float[] embedding)
    {
        var entity = new ChatMessage
        {
            Role = role,
            Content = content,
            EmbeddingJson = JsonSerializer.Serialize(embedding)
        };

        _db.ChatMessages.Add(entity);
        await _db.SaveChangesAsync();
    }

    public async Task<List<ChatMessage>> BuscarMensagensSimilares(float[] targetVector, int topN = 3)
    {
        var all = await _db.ChatMessages.ToListAsync();

        var ranked = all
            .Select(m => new
            {
                Message = m,
                Similarity = CalcularSimilaridadeCoseno(targetVector, JsonSerializer.Deserialize<float[]>(m.EmbeddingJson ?? "[]") ?? new float[0])
            })
            .OrderByDescending(x => x.Similarity)
            .Take(topN)
            .Select(x => x.Message)
            .ToList();

        return ranked;
    }

    private float CalcularSimilaridadeCoseno(float[] a, float[] b)
    {
        if (a.Length != b.Length || a.Length == 0) return 0;

        float dot = 0, normA = 0, normB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }

        return (float)(dot / (Math.Sqrt(normA) * Math.Sqrt(normB)));
    }
}
