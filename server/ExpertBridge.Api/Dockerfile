FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /ExpertBridge
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ExpertBridge.Api/ExpertBridge.Api.csproj", "ExpertBridge.Api/"]
COPY ["ExpertBridge.Core/ExpertBridge.Core.csproj", "ExpertBridge.Core/"]
COPY ["ExpertBridge.Data/ExpertBridge.Data.csproj", "ExpertBridge.Data/"]
COPY ["ExpertBridge.GroqLibrary/ExpertBridge.GroqLibrary.csproj", "ExpertBridge.GroqLibrary/"]
RUN dotnet restore "ExpertBridge.Api/ExpertBridge.Api.csproj"
COPY . .
WORKDIR "/src/ExpertBridge.Api"
RUN dotnet build "./ExpertBridge.Api.csproj" -c $BUILD_CONFIGURATION -o /ExpertBridge/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ExpertBridge.Api.csproj" -c $BUILD_CONFIGURATION -o /ExpertBridge/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /ExpertBridge
COPY --from=publish /ExpertBridge/publish .
ENTRYPOINT ["dotnet", "ExpertBridge.Api.dll"]
