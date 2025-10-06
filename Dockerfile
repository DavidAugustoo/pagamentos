# Etapa 1: Build com SDK .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FCG.API/FCG.API.csproj", "FCG.API/"]
COPY ["FCG.Application/FCG.Application.csproj", "FCG.Application/"]
COPY ["FCG.Domain/FCG.Domain.csproj", "FCG.Domain/"]
COPY ["FCG.Infra.Data/FCG.Infra.Data.csproj", "FCG.Infra.Data/"]
COPY ["FCG.Infra.Ioc/FCG.Infra.Ioc.csproj", "FCG.Infra.Ioc/"]
COPY ["FCG.Tests/FCG.Tests.csproj", "FCG.Tests/"]

RUN dotnet restore "FCG.API/FCG.API.csproj"

COPY . .

WORKDIR "/src/FCG.API"
RUN dotnet build "FCG.API.csproj" -c Release -o /app/build

# Etapa 2: Publicação
FROM build AS publish
RUN dotnet publish "FCG.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Criar usuário limitado
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

COPY --from=publish /app/publish .

RUN chown -R appuser:appgroup /app
USER appuser

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80;
ENV ASPNETCORE_ENVIRONMENT=Production

HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:80/swagger/index.html || exit 1

ENTRYPOINT ["dotnet", "FCG.API.dll"]
