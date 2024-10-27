#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["mef.db.plf.edition.api/mef.db.plf.edition.api.csproj", "mef.db.plf.edition.api/"]
COPY ["mef.db.plf.repositories/mef.db.plf.repositories.csproj", "mef.db.plf.repositories/"]
COPY ["mef.db.plf.models/mef.db.plf.models.csproj", "mef.db.plf.models/"]
COPY ["mef.db.plf.shared/mef.db.plf.shared.csproj", "mef.db.plf.shared/"]
COPY ["mef.db.plf.edition.data/mef.db.plf.edition.generation.csproj", "mef.db.plf.edition.data/"]
RUN dotnet restore "mef.db.plf.edition.api/mef.db.plf.edition.api.csproj"
COPY . .
WORKDIR "/src/mef.db.plf.edition.api"
RUN dotnet build "mef.db.plf.edition.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "mef.db.plf.edition.api.csproj" -c Release -o /app/publish
COPY ["mef.db.plf.edition.api/License", "/app/publish/License"]

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "mef.db.plf.edition.api.dll"]