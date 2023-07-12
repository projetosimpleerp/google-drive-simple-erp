# Imagem base para compilação
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Diretório de trabalho
WORKDIR /app

# Copiar os arquivos do projeto para o contêiner
COPY . .

# Restaurar as dependências e compilar o projeto
RUN dotnet restore
RUN dotnet build --configuration Release --no-restore

# Publicar a aplicação
RUN dotnet publish -c Release -o /app/publish --no-restore

# Imagem de tempo de execução
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final

# Diretório de trabalho
WORKDIR /app

# Copiar a aplicação publicada
COPY --from=build /app/publish .

# Definir as variáveis de ambiente, se necessário
ENV ASPNETCORE_ENVIRONMENT=Production

# Expor as portas do contêiner, se necessário
EXPOSE 80
EXPOSE 9002

# Executar a aplicação (API)
CMD ["dotnet", "SimpleERP.GoogleDriveIntegration.dll"]