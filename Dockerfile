FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Timezone ayarı - Türkiye saati
ENV TZ=Europe/Istanbul
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/CryptoAlarmSystem.Api/CryptoAlarmSystem.Api.csproj", "CryptoAlarmSystem.Api/"]
COPY ["src/CryptoAlarmSystem.Application/CryptoAlarmSystem.Application.csproj", "CryptoAlarmSystem.Application/"]
COPY ["src/CryptoAlarmSystem.Domain/CryptoAlarmSystem.Domain.csproj", "CryptoAlarmSystem.Domain/"]
COPY ["src/CryptoAlarmSystem.Infrastructure/CryptoAlarmSystem.Infrastructure.csproj", "CryptoAlarmSystem.Infrastructure/"]
RUN dotnet restore "CryptoAlarmSystem.Api/CryptoAlarmSystem.Api.csproj"
COPY src/ .
WORKDIR "/src/CryptoAlarmSystem.Api"
RUN dotnet build "CryptoAlarmSystem.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CryptoAlarmSystem.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "CryptoAlarmSystem.Api.dll"]
