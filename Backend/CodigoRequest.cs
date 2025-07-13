namespace IAPairProgrammer;

public class CodigoRequest
{
    public string Codigo { get; set; }

}
public class ChatSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public List<ChatMessage> Messages { get; set; }
}

public class ChatMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SessionId { get; set; }
    public string Role { get; set; } // "user" or "assistant"
    public string Content { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public string? EmbeddingJson { get; set; }
}