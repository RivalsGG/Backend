FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG MARVEL_API_KEY
WORKDIR /src

COPY ["RivalsGG.API/RivalsGG.API.csproj", "RivalsGG.API/"]
COPY ["RivalsGG.Core/RivalsGG.Core.csproj", "RivalsGG.Core/"]
COPY ["RivalsGG.BLL/RivalsGG.BLL.csproj", "RivalsGG.BLL/"]
COPY ["RivalsGG.DAL/RivalsGG.DAL.csproj", "RivalsGG.DAL/"]

RUN dotnet restore "RivalsGG.API/RivalsGG.API.csproj"

COPY . .
COPY ["RivalsGG.API/appsettings.Docker.json", "RivalsGG.API/"]

WORKDIR "/src/RivalsGG.API"
RUN dotnet build "RivalsGG.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RivalsGG.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore

FROM base AS final
ARG MARVEL_API_KEY
WORKDIR /app
COPY --from=publish /app/publish .

ENV MarvelRivalsApi__ApiKey=$MARVEL_API_KEY

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1
  
ENV DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "RivalsGG.API.dll"]