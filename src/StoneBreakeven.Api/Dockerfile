FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/StoneBreakeven.Api/StoneBreakeven.Api.csproj", "src/StoneBreakeven.Api/"]
COPY ["src/ExternalServices/StoneBreakeven.ExampleService/StoneBreakeven.ExampleService.csproj", "src/ExternalServices/StoneBreakeven.ExampleService/"]
RUN dotnet restore "src/StoneBreakeven.Api/StoneBreakeven.Api.csproj"
COPY . .
WORKDIR "/src/src/StoneBreakeven.Api"
RUN dotnet build "StoneBreakeven.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StoneBreakeven.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StoneBreakeven.Api.dll"]