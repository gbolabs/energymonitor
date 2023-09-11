# How to connect github action to Azure

Command `az ad sp create-for-rbac` outputs:

_parameters: [MS Learn, az-ad-sp-create-for-rbac](https://learn.microsoft.com/en-us/cli/azure/ad/sp?view=azure-cli-latest#az-ad-sp-create-for-rbac)_

```
{
  "appId": "54864645********-fdf6ecb17008",
  "displayName": "MyApp",
  "password": "************",
  "tenant": "as--a-sd"
}
```

## Reset credentials
After expiration, being lost or leaked.

`az ad sp credential reset --id {app/client_id} --years 2`

outputs: 

```
{
  "appId": "314e9301-****",
  "password": "******",
  "tenant": "1d4783f4-******"
}
```

Transforms to ealier format understood by the `azure/login@v1` GitHub action.