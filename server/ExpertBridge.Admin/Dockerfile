FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /ExpertBridge.Admin
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ExpertBridge.Admin/ExpertBridge.Admin.csproj", "ExpertBridge.Admin/"]
RUN dotnet restore "ExpertBridge.Admin/ExpertBridge.Admin.csproj"
COPY . .
WORKDIR "/src/ExpertBridge.Admin"
RUN dotnet build "./ExpertBridge.Admin.csproj" -c $BUILD_CONFIGURATION -o /ExpertBridge.Admin/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ExpertBridge.Admin.csproj" -c $BUILD_CONFIGURATION -o /ExpertBridge.Admin/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /ExpertBridge.Admin
COPY --from=publish /ExpertBridge.Admin/publish .
ENTRYPOINT ["dotnet", "ExpertBridge.Admin.dll"]
