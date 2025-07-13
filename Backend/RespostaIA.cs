using System.Text.Json.Serialization;

public class RespostaIA
{
    [JsonPropertyName("melhoria")]
    public string Melhoria { get; set; }

    [JsonPropertyName("documentacao")]
    public string Documentacao { get; set; }

    [JsonPropertyName("explicacao")]
    public string Explicacao { get; set; }

    [JsonPropertyName("teste_unitario")]
    public string TesteUnitario { get; set; }
}