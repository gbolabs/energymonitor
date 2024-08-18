# fly auth login
# fly app create energymeasures-api
fly secrets set -a energymeasures-api pr114energymeasures="AccountEndpoint=https://cdb-em-pr114-dev-01.documents.azure.com:443/;AccountKey=Qq20h6o8kbtFJHL0qUWbqaQgl1dDZzpEOy0qBJPTbPpWz2bkytD6YGOtRA7ns83XbEfIr1NeHWPEACDbymCC5w==;"
fly secrets set -a energymeasures-api CosmosDbName=powerMeter_Measures
