FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WeatherArchive/WeatherArchive.csproj", "WeatherArchive/"]
COPY ["WeatherArchive.DAL/WeatherArchive.DAL.csproj", "WeatherArchive.DAL/"]
COPY ["WeatherArchive.Domain/WeatherArchive.Domain.csproj", "WeatherArchive.Domain/"]
COPY ["WeatherArchive.Service/WeatherArchive.Service.csproj", "WeatherArchive.Service/"]
RUN dotnet restore "WeatherArchive/WeatherArchive.csproj"
COPY . .
WORKDIR "/src/WeatherArchive"
RUN dotnet build "WeatherArchive.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WeatherArchive.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
RUN mkdir -p /app/wwwroot/uploads
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WeatherArchive.dll"]