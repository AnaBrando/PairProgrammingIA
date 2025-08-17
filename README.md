# IA Pair Programmer for .NET 🤖

This is an experimental project that integrates the OpenAI API with an ASP.NET Core application to act as a pair programmer focused on C# code.

The AI is capable of:
- ✅ Analyzing provided C# code
- 💡 Suggesting improvements based on best practices
- 🧾 Generating XML documentation comments
- 🧠 Explaining code logic in a technical way
- 🧪 Proposing unit test examples automatically

---

## 💻 Technologies Used

- .NET 8 (Web API)
- OpenAI GPT-4o API
- System.Text.Json
- HttpClient
- C# 12

---

## 🚀 How to Run Locally

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/ia-pair-programmer-dotnet.git
   cd ia-pair-programmer-dotnet

1. Clone o repositório:
   ```bash
   git clone https://github.com/seu-usuario/ia-pair-programmer-dotnet.git
   cd ia-pair-programmer-dotnet
   ```
2. Add your OpenAI API key in a .txt file stored securely on your virtual machine, for example::
   ```bash
   Exemple: C:\Projeto\text.txt
   ```  
3. Run the project
  ```bash
    dotnet run
  ```
4. Make a POST request to /analise with a JSON body like:
 ```bash
   git clone https://github.com/seu-usuario/ia-pair-programmer-dotnet.git
   cd ia-pair-programmer-dotnet
```
---

## 📦 Output example
```
{
  "melhoria": "Use a switch expression instead of multiple if/else statements...",
  "documentacao": "/// <summary>...",
  "explicacao": "The code performs basic arithmetic operations...",
  "teste_unitario": "[Fact] public void ShouldAddCorrectly()..."
}
```

## 📌 Project Goal
This project was created for educational purposes to explore how generative AI can improve productivity in backend software development using C# and .NET.

## Future Ideas

 Continuous chat with reasoning memory

 Web interface to paste code and view suggestions

 Automatic PR comments generation for GitHub repos

 Multi-method refactoring suggestions

## 🧑‍💻 Developed by
Ana Paula Chaves
Senior Software Engineer in the Investment Division at Itaú Bank

🔗 [LinkedIn](https://www.linkedin.com/in/anachavesdev/) • [GitHub](https://github.com/AnaBrando)

