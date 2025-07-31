# Dockerfile para ASP.NET Core 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CrossfitLeaderboard.csproj", "./"]
RUN dotnet restore "CrossfitLeaderboard.csproj"
COPY . .
RUN dotnet build "CrossfitLeaderboard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CrossfitLeaderboard.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CrossfitLeaderboard.dll"] 