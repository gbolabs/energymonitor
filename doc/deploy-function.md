# How to deploy the Azure Function

Here we got, at least for now, on a less IaC track.

1. Have an Azure Function ready within an Azure subscription where you've got an identity able to login
2. Install locally the Az Cli (I went for Linux/WSL/Ubuntu)
3. Install the Azure Function command line tools `sudo apt-get install azure-functions-core-tools-4`
4. Open a terminal within the function folder and run `func azure functionapp publish <func-name> --csharp`