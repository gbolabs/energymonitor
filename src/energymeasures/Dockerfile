#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ENV DOTNET_EnableWriteXorExecute=0
WORKDIR /src
COPY energymeasures/energymeasures.csproj energymeasures/
COPY Directory.Packages.props Directory.Packages.props
COPY EnergyMonitor.sln .
COPY common/common.csproj common/
RUN dotnet restore energymeasures/energymeasures.csproj
COPY . .
RUN dotnet publish energymeasures/energymeasures.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "energymeasures.dll"]
