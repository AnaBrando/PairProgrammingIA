FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY . .

# Restore dependencies
RUN dotnet restore

# ✅ Install EF Core CLI (required for migrations)
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"
# Ensure the 'data' directory exists
RUN mkdir -p /src/data

# 👇 Move into project context — make sure your .csproj is in /src
WORKDIR /src

# ✅ Apply migrations using EF CLI
RUN dotnet ef database update

# ✅ Publish app after migration
RUN dotnet publish -c Release -o /app/publish

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "IAPairProgrammer.dll"]
