# How to connect github action to Azure

Command `az ad sp create-for-rbac -n TestRBAC001 --years 2 --sdk-auth --role Contributor --scopes /subscriptions/**/resourceGroups/**` outputs:

_parameters: [MS Learn, az-ad-sp-create-for-rbac](https://learn.microsoft.com/en-us/cli/azure/ad/sp?view=azure-cli-latest#az-ad-sp-create-for-rbac)_

```
{
  "clientId": "**********",
  "clientSecret": "**********",
  "subscriptionId": "*****",
  "tenantId": "************",
  "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
  "resourceManagerEndpointUrl": "https://management.azure.com/",
  "activeDirectoryGraphResourceId": "https://graph.windows.net/",
  "sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
  "galleryEndpointUrl": "https://gallery.azure.com/",
  "managementEndpointUrl": "https://management.core.windows.net/"
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

Transforms to ealier format understood by the `azure/login@v1` GitHub action, `appId`==`clientId`.