FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Locadora.Api.sln ./
COPY src/Locadora.Api/Locadora.Api.csproj src/Locadora.Api/
COPY src/Locadora.Application/Locadora.Application.csproj src/Locadora.Application/
COPY src/Locadora.Domain/Locadora.Domain.csproj src/Locadora.Domain/
COPY src/Locadora.Infrastructure/Locadora.Infrastructure.csproj src/Locadora.Infrastructure/
COPY src/Locadora.Jobs/Locadora.Jobs.csproj src/Locadora.Jobs/
RUN dotnet restore src/Locadora.Api/Locadora.Api.csproj

COPY . .
RUN dotnet publish src/Locadora.Api/Locadora.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Locadora.Api.dll"]
