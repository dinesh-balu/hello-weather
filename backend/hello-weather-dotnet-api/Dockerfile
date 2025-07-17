FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HelloWeatherApi.csproj", "."]
RUN dotnet restore "HelloWeatherApi.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "HelloWeatherApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HelloWeatherApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HelloWeatherApi.dll"]
