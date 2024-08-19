# Building

- For MacOS need to have the following environment variable set to instruct docker building in a linux-amd format.

  `export DOCKER_DEFAULT_PLATFORM=linux/amd64`

From the `src` Directory.

## PRD
1. `docker build -f energymeasures/Dockerfile -t crisagocmn.azurecr.io/pr114.energymeter.measuresapi:0.2 .`
2. `az login -t isagonet.ch`
3. `az acr login -n crisagocmn`
4. `docker push crisagocmn.azurecr.io/pr114.energymeter.measuresapi:0.2`

## DEV
Registry: `acrisagocmd001.azurecr.io/pr114.energymeter.measuresapi`

# Running

To execute the container, the following environment variables are required, at least.

- `pr114energymeasures` with the Container connection string
- `CosmosDbName` with the name of the container name for the measures