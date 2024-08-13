#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ENV DOTNET_EnableWriteXorExecute=0
WORKDIR /src
COPY ["energymeasures/energymeasures.csproj", "energymeasures/"]
RUN dotnet restore "energymeasures/energymeasures.csproj"
COPY . .
WORKDIR "/src/energymeasures"
RUN dotnet build "energymeasures.csproj" -c Release -o /app/build -r linux-x64

FROM build AS publish
RUN dotnet publish "energymeasures.csproj" -c Release -o /app/publish -r linux-x64

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "energymeasures.dll"]
