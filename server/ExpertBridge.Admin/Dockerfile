FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /ExpertBridgeAdmin
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet restore
WORKDIR "/src/ExpertBridge.Admin"
RUN dotnet build -c $BUILD_CONFIGURATION -o /ExpertBridgeAdmin/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ExpertBridge.Admin.csproj" -c $BUILD_CONFIGURATION -o /ExpertBridgeAdmin/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /ExpertBridgeAdmin
COPY --from=publish /ExpertBridgeAdmin/publish .
ENTRYPOINT ["dotnet", "ExpertBridge.Admin.dll"]
