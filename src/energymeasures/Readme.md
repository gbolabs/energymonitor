# Building

From the `src` Directory.

1. `docker build -f energymeasures/Dockerfile -t crisagocmn.azurecr.io/pr114.energymeter.measuresapi:0.2 .`
2. `az login -t isagonet.ch`
2. `az acr login -n crisagocmn`
3. `docker push crisagocmn.azurecr.io/pr114.energymeter.measuresapi:0.2`