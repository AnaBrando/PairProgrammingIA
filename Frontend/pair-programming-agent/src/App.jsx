import React, { useState } from 'react';
import ReactMarkdown from 'react-markdown';
import { Prism as SyntaxHighlighter } from 'react-syntax-highlighter';
import { oneDark } from 'react-syntax-highlighter/dist/esm/styles/prism';
import './App.css';

function App() {
  const [open, setOpen] = useState(false);
  const [messages, setMessages] = useState([]);
  const [inputText, setInputText] = useState('');
  const [file, setFile] = useState(null);
  const [loading, setLoading] = useState(false);

  const handleSend = async () => {
    if (!inputText.trim() || !file) return;

    const userMessage = { from: 'user', text: inputText };
    setMessages(prev => [...prev, userMessage]);
    setLoading(true);

    const formData = new FormData();
    formData.append('situacao', inputText);
    formData.append('file', file);

    try {
      const res = await fetch('http://localhost/Analise/analisar-upload', {
        method: 'POST',
        body: formData,
      });

      if (!res.ok) {
        const errText = await res.text();
        throw new Error(`Erro na requisição: ${errText}`);
      }

      const data = await res.json();
      const markdownResult = `
### 📘 Código Refatorado
\`\`\`csharp
${data.refactoredCode.replace(/^```csharp|```$/g, '')}
\`\`\`

### ✅ Testes Unitários
\`\`\`csharp
${data.unitTests.replace(/^```csharp|```$/g, '')}
\`\`\`

### 🧾 Documentação
\`\`\`csharp
${data.documentation.replace(/^```csharp|```$/g, '')}
\`\`\`

### 📖 Explicação
${data.explanation}

---
🧙‍♂️ Posso ajudar com mais alguma coisa? Só enviar outro código ou dúvida!`;

      setMessages(prev => [
        ...prev.filter(m => m.from !== 'user'),
        { from: 'bot', text: markdownResult }
      ]);
    } catch (err) {
      console.error('🚨 Erro ao enviar dados:', err);
      setMessages(prev => [
        ...prev,
        { from: 'bot', text: '❌ Erro ao processar sua solicitação.' }
      ]);
    }

    setLoading(false);
    setInputText('');
    setFile(null);
  };

  return (
    <div className="app-container">
      {!open && (
        <img
          src="/4712100.png"
          alt="Abrir assistente"
          className="bot-icon"
          onClick={() => setOpen(true)}
        />
      )}

      {open && (
        <div className="chat-box">
          <p className="welcome">
            Olá! Sou seu assistente de pair programming em <strong>C# 🧙‍♂️⚡</strong> — preparado para conjurar código limpo!
          </p>

          <div className="chat-messages">
            {messages.map((msg, i) => (
              <div key={i} className={`msg ${msg.from}`}>
                {msg.from === 'bot' ? (
                  <ReactMarkdown
                    components={{
                      code({ node, inline, className, children, ...props }) {
                        const match = /language-(\w+)/.exec(className || '');
                        return !inline && match ? (
                          <SyntaxHighlighter
                            style={oneDark}
                            language={match[1]}
                            PreTag="div"
                            {...props}
                          >
                            {String(children).replace(/\n$/, '')}
                          </SyntaxHighlighter>
                        ) : (
                          <code className={className} {...props}>
                            {children}
                          </code>
                        );
                      },
                    }}
                  >
                    {msg.text}
                  </ReactMarkdown>
                ) : (
                  <div className="user-bubble">
                    <span>{msg.text}</span>
                  </div>
                )}
              </div>
            ))}
            {loading && <div className="loading">⏳ Pensando<span className="dots"></span></div>}
          </div>

          <form className="chat-input" onSubmit={(e) => { e.preventDefault(); handleSend(); }}>
            <input
              type="text"
              placeholder="🧙‍♂️ Digite seu feitiço de código..."
              value={inputText}
              onChange={(e) => setInputText(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === 'Enter') {
                  e.preventDefault();
                  handleSend();
                }
              }}
            />

            <input
              type="file"
              onChange={(e) => setFile(e.target.files[0])}
            />
            <button type="submit">Enviar</button>
            <button type="button" onClick={() => setOpen(false)}>×</button>
          </form>
        </div>
      )}
    </div>
  );
}

export default App;
