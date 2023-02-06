
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CsvImportSite/CsvImportSite.csproj", "CsvImportSite/"]
RUN dotnet restore "CsvImportSite/CsvImportSite.csproj"
COPY . .
WORKDIR "/src/CsvImportSite"
RUN dotnet build "CsvImportSite.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CsvImportSite.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CsvImportSite.dll"]