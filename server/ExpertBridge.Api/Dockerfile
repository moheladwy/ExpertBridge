FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /ExpertBridgeApi
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet restore
WORKDIR "/src/ExpertBridge.Api"
RUN dotnet build -c $BUILD_CONFIGURATION -o /ExpertBridgeApi/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ExpertBridge.Api.csproj" -c $BUILD_CONFIGURATION -o /ExpertBridgeApi/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /ExpertBridgeApi
COPY --from=publish /ExpertBridgeApi/publish .
ENTRYPOINT ["dotnet", "ExpertBridge.Api.dll"]
