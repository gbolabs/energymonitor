# Build
In `src/MyStromUploader` directory

On MacOs, instruct docker to build in a linux-amd64 format.
- `export DOCKER_DEFAULT_PLATFORM=linux/amd64`
or
- 'docker build .  --platform linux/amd64 -t crisagocmn.azurecr.io/pr114/mystromuploader:1.1-beta'

- `docker build . -t crisagocmn.azurecr.io/pr114/mystromuploader:1.1-beta`
- `az login -t {tenant}`
- `az acr login -n crisagocmn`
- `docker push crisagocmn.azurecr.io/pr114/mystromuploader:1.1-beta`

# Operate
On a docker-enabled system (linux-based)

- establish a `.docker/config.json` with the login credentials (admin-login must be enabled on Azure Container Registry)
- `docker login crisagocmn.azurecr.io`
- To minimize down-time pull at first, `docker pull crisagocmn.azurecr.io/pr114/mystromuploader:1.1-beta`
- Remove existing instance, `docker container rm mystromupload-1 -f`
- Start new instance, `docker run --name mystromupload-1 -d -e APPINSIGHTS_INSTRUMENTATIONKEY={key} -e CloudIngressConfig__ApiKey={ingress-key}`
- Live monitor the logs, `docker logs -f mystromupload-1`