# Building

From the `src` Directory.

# PRD
1. `docker build -f energymeasures/Dockerfile -t crisagocmn.azurecr.io/pr114.energymeter.measuresapi:0.2 .`
2. `az login -t isagonet.ch`
3. `az acr login -n crisagocmn`
4. `docker push crisagocmn.azurecr.io/pr114.energymeter.measuresapi:0.2`

# DEV
Registry: `acrisagocmd001.azurecr.io/pr114.energymeter.measuresapi`