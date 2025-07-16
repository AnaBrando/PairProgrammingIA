# STAGE 1 - BUILD
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia todos os arquivos do projeto
COPY . .

# Restaura dependências
RUN dotnet restore IAPairProgrammer.csproj

# Instala dotnet-ef para aplicar migrations
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

# Publica a aplicação
RUN dotnet publish IAPairProgrammer.csproj -c Release -o /app/publish

# STAGE 2 - FINAL
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app

# Copia o código-fonte completo (para rodar migrations)
COPY --from=build /src /app/src

# Copia a aplicação publicada
COPY --from=build /app/publish /app/publish

# Copia o entrypoint
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

RUN mkdir -p /app/data

ENTRYPOINT ["/entrypoint.sh"]

EXPOSE 80
