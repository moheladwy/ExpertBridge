FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /ExpertBridgeWorker

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ExpertBridge.Worker/ExpertBridge.Worker.csproj", "ExpertBridge.Worker/"]
RUN dotnet restore "ExpertBridge.Worker/ExpertBridge.Worker.csproj"
COPY . .
WORKDIR "/src/ExpertBridge.Worker"
RUN dotnet build "./ExpertBridge.Worker.csproj" -c $BUILD_CONFIGURATION -o /ExpertBridgeWorker/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ExpertBridge.Worker.csproj" -c $BUILD_CONFIGURATION -o /ExpertBridgeWorker/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /ExpertBridgeWorker
COPY --from=publish /ExpertBridgeWorker/publish .
ENTRYPOINT ["dotnet", "ExpertBridge.Worker.dll"]
