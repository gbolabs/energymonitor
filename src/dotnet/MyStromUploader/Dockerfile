FROM --platform=amd64 mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Avoid infinte dotnet restore
ENV DOTNET_EnableWriteXorExecute=0

WORKDIR /src

# Copy csproj and restore as distinct layers
COPY MyStromUploader/*.csproj ./MyStromUploader/
COPY Directory.Packages.props .

# Restore as distinct layers
RUN dotnet restore ./MyStromUploader/

# Copy everything else and build
COPY MyStromUploader/. ./MyStromUploader/

# Build and publish a release
RUN dotnet publish -c Release -o /publish ./MyStromUploader/MyStromUploader.csproj

# Build runtime image
FROM --platform=amd64 mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /publish .
ENTRYPOINT ["dotnet", "MyStromUploader.dll"]
