# 1️⃣ Базовий образ для запуску .NET API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000  

# 2️⃣ Базовий образ для збірки застосунку
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Minigame.csproj", "./"]
RUN dotnet restore  # Відновлення залежностей

COPY . .
RUN dotnet publish -c Release -o /app/publish 

# 3️⃣ Запуск API
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Minigame.dll"]  