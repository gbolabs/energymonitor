#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ENV DOTNET_EnableWriteXorExecute=0
WORKDIR /src
COPY . .
WORKDIR "/src/energymeasures"
RUN dotnet restore "energymeasures.csproj"
RUN dotnet build "energymeasures.csproj" -c Release --no-restore
RUN dotnet publish "energymeasures.csproj" -c Release -o /app/publish --no-build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "energymeasures.dll"]
