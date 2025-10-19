FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /ExpertBridgeWorker

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet restore
WORKDIR "/src/ExpertBridge.Worker"
RUN dotnet build -c $BUILD_CONFIGURATION -o /ExpertBridgeWorker/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ExpertBridge.Worker.csproj" -c $BUILD_CONFIGURATION -o /ExpertBridgeWorker/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /ExpertBridgeWorker
COPY --from=publish /ExpertBridgeWorker/publish .
ENTRYPOINT ["dotnet", "ExpertBridge.Worker.dll"]
