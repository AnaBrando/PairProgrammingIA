#!/bin/bash
set -e

export PATH="$PATH:/root/.dotnet/tools"

echo "🔥 ENTRYPOINT STARTED"
echo "📁 Criando diretório de banco se não existir..."
mkdir -p /app/data
ls -la /app/data

echo "Installing dotnet-ef..."
dotnet tool install --global dotnet-ef

echo "Running EF Core migrations..."
dotnet ef database update --project /app/src/IAPairProgrammer.csproj

echo "Starting app..."
exec dotnet /app/publish/IAPairProgrammer.dll
