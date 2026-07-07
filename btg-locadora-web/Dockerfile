FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Locadora.Web/Locadora.Web.csproj src/Locadora.Web/
COPY src/Locadora.Web.Application/Locadora.Web.Application.csproj src/Locadora.Web.Application/
COPY src/Locadora.Web.Domain/Locadora.Web.Domain.csproj src/Locadora.Web.Domain/
COPY src/Locadora.Web.Infrastructure/Locadora.Web.Infrastructure.csproj src/Locadora.Web.Infrastructure/
RUN dotnet restore src/Locadora.Web/Locadora.Web.csproj

COPY . .
RUN dotnet publish src/Locadora.Web/Locadora.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Locadora.Web.dll"]
