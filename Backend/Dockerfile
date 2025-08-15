# Usa a imagem oficial com SDK do .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0

# Define diretório de trabalho
WORKDIR /app

# Copia tudo (incluindo seu .csproj e migrations)
COPY . .

# Instala o dotnet-ef (compatível com .NET 8)
RUN dotnet tool install --global dotnet-ef \
    && export PATH="$PATH:/root/.dotnet/tools" \
    && echo 'export PATH="$PATH:/root/.dotnet/tools"' >> ~/.bashrc

# Aponta variável de ambiente para que o ef funcione
ENV PATH="${PATH}:/root/.dotnet/tools"

# Executa as migrations apenas (sem rodar app)
CMD ["dotnet", "ef", "database", "update", "--project", "./IAPairProgrammer.csproj"]
