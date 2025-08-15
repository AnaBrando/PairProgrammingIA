namespace IAPairProgrammer.Builder;

public class PromptBuilder
{
    private readonly List<object> _messages = new();

    public PromptBuilder ComPromptSistema()
    {
        _messages.Add(new
        {
            role = "system",
            content = """
                Você é um desenvolvedor especialista em C# atuando como pair programming.

                Seu objetivo é ajudar a melhorar o código .NET do usuário com base em boas práticas, SOLID, performance e testabilidade.

                Quando receber um código, siga os passos:
                1. Refatore com boas práticas
                2. Gere testes unitários (xUnit)
                3. Documente com comentários XML
                4. Explique suas escolhas

                Sempre retorne um JSON estruturado com as seguintes chaves:
                {
                    "refactoredCode": "...",
                    "unitTests": "...",
                    "documentation": "...",
                    "explanation": "..."
                }
            """
        });

        return this;
    }

    public PromptBuilder ComMensagensAnteriores(List<ChatMessage> mensagens)
    {
        if (mensagens == null || mensagens.Count == 0)
            return this;

        foreach (var msg in mensagens)
        {
            if (!string.IsNullOrWhiteSpace(msg.Content) && !string.IsNullOrWhiteSpace(msg.Role))
            {
                _messages.Add(new
                {
                    role = msg.Role,
                    content = msg.Content
                });
            }
        }

        return this;
    }

    public PromptBuilder ComSituacao(string situacao)
    {
        _messages.Add(new
        {
            role = "user",
            content = $"Contexto da situação: {situacao}"
        });
        return this;
    }
    
    public PromptBuilder ComCodigoDoUsuario(string codigo)
    {
        if (!string.IsNullOrWhiteSpace(codigo))
        {
            _messages.Add(new
            {
                role = "user",
                content = $"Analise o código abaixo e retorne um JSON com as seções esperadas.\n\nCódigo:\n\n{codigo}"
            });
        }

        return this;
    }

    public List<object> Build()
    {
        return _messages;
    }
    
    public List<object> ConstruirPrompt(List<ChatMessage> mensagens, string codigo, CodigoUploadRequest request)
    {
        return new PromptBuilder()
            .ComPromptSistema()
            .ComSituacao(request.Situacao)
            .ComMensagensAnteriores(mensagens)
            .ComCodigoDoUsuario(codigo)
            .Build();
    }
}
