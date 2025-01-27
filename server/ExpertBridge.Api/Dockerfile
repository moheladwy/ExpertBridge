FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /ExpertBridge
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet build -c $BUILD_CONFIGURATION -o /ExpertBridge/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /ExpertBridge/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /ExpertBridge
COPY --from=publish /ExpertBridge/publish .
ENTRYPOINT ["dotnet", "ExpertBridge.Api.dll"]
