using System.Text.Json.Serialization;

public class RespostaIA
{
    [JsonPropertyName("refactoredCode")]
    public string RefactoredCode { get; set; }

    [JsonPropertyName("unitTests")]
    public string UnitTests { get; set; }

    [JsonPropertyName("documentation")]
    public string Documentation { get; set; }

    [JsonPropertyName("explanation")]
    public string Explanation { get; set; }
}