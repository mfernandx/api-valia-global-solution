
# ETAPA 1 - BUILD
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ValiaApiWeb/ValiaApiWeb.csproj ValiaApiWeb/
RUN dotnet restore ValiaApiWeb/ValiaApiWeb.csproj

COPY ValiaApiWeb/ ValiaApiWeb/
RUN dotnet publish ValiaApiWeb/ValiaApiWeb.csproj -c Release -o /app/publish --no-restore

# ETAPA 2 - EXECUÇÃO
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=10000

EXPOSE 10000

USER $APP_UID

ENTRYPOINT ["dotnet", "ValiaApiWeb.dll"]
