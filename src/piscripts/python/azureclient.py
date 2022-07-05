import requests

def post_to_azure_read_info(jsonData):
    print(jsonData)
    url = 'https://func-priv114-em-ingress.azurewebsites.net/api/priv114_em_ingress?code=CdV2Ikqaoglj8jM2YDMxIZy6sZgodWcQVULJJmOvTvxh/lMRS/EWXw=='
    headers = {'Content-type': 'application/json'}
    response = requests.post(url, data=jsonData, headers=headers)
    if response.status_code != 200:
        print('Unable to post request to azure')
    else:
        print('Data posted to Azure')
