FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /Admin
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Admin/Admin.csproj", "Admin/"]
RUN dotnet restore "Admin/Admin.csproj"
COPY . .
WORKDIR "/src/Admin"
RUN dotnet build "./Admin.csproj" -c $BUILD_CONFIGURATION -o /Admin/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Admin.csproj" -c $BUILD_CONFIGURATION -o /Admin/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /Admin
COPY --from=publish /Admin/publish .
ENTRYPOINT ["dotnet", "Admin.dll"]
