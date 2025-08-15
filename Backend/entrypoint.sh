#!/bin/bash
set -e

echo "🔥 ENTRYPOINT STARTED"
echo "🧹 Limpando pastas bin e obj..."

echo "📦 Buildando projeto..."

chmod -R 777 /app/bin
chmod -R 777 /app/obj
dotnet build /app/IAPairProgrammer.csproj

echo "📁 Criando diretório de banco se não existir..."
mkdir -p /app/App_Data
ls -la /app/App_Data

echo "🔧 Instalando dotnet-ef..."
dotnet tool install --global dotnet-ef || echo "✅ dotnet-ef já instalado."

# Atualiza o PATH para encontrar o dotnet-ef
export PATH="$PATH:/root/.dotnet/tools"

echo "🧠 Verificando se existe migration..."
if ! dotnet ef migrations list --project /app/IAPairProgrammer.csproj | grep -q "Create"; then
  echo "📌 Nenhuma migration encontrada. Criando migration inicial..."
  dotnet ef migrations add CreateChatMessages --project /app/IAPairProgrammer.csproj
else
  echo "✅ Migration já existe."
fi

echo "🛠️ Rodando EF Core database update..."
dotnet ef database update --project /app/IAPairProgrammer.csproj

echo "✅ Banco atualizado. Pronto para debug no Rider!"

# Mantém o container vivo para que o Rider possa acessar
tail -f /dev/null
