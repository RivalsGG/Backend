FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RivalsGG.API/appsettings.Docker.json", "RivalsGG.API/"]
COPY ["RivalsGG.API/RivalsGG.API.csproj", "RivalsGG.API/"]
COPY ["RivalsGG.Core/RivalsGG.Core.csproj", "RivalsGG.Core/"]
COPY ["RivalsGG.BLL/RivalsGG.BLL.csproj", "RivalsGG.BLL/"]
COPY ["RivalsGG.DAL/RivalsGG.DAL.csproj", "RivalsGG.DAL/"]
RUN dotnet restore "RivalsGG.API/RivalsGG.API.csproj"
COPY . .
WORKDIR "/src/RivalsGG.API"
RUN dotnet build "RivalsGG.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RivalsGG.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RivalsGG.API.dll"]