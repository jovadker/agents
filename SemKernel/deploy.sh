#bin/bash
set -e
# To run the script use the following command
# az login (MS FDPO tenant)
# az account set --subscription baa70448-593c-4dc7-8a91-c92cf7eaf66e

current_date_time=$(date +%Y%m%d)
postfix=jv001
resource_group_name=AIBot.Automated.RG
location=eastus2
botId=copilotstudiobot$postfix
web_app_name=automatedbot$postfix
openaiservicename=automatedbotopenai$postfix
storageAccountName=searchstorage$postfix
searchServiceName=searchservice$postfix

az group create --name $resource_group_name --location $location

tenant_id=$(az account show --query tenantId --output tsv)

# create user assigned identity
identity_name=automatedbotIdentity
identity=$(az identity create --resource-group $resource_group_name --name $identity_name --location $location)

identity_id=$(echo $identity | jq -r '.id')
client_id=$(echo $identity | jq -r '.clientId')
resource_id=$(echo $identity | jq -r '.id')
echo "Identity ID: $identity_id"
echo "Client ID: $client_id"
echo "Resource ID: $resource_id"
echo "Tenant ID: $tenant_id"

# https://github.com/Azure-Samples/openai/tree/main/End_to_end_Solutions

az deployment group create --resource-group $resource_group_name \
    --template-file ./iac/ai.json \
    --parameters openaiServiceName=$openaiservicename \
      storageAccountName=$storageAccountName \
      searchServicesName=$searchServiceName \
    --verbose

az deployment group create --resource-group $resource_group_name \
    --template-file ./iac/BotWebApp.json \
    --parameters appServiceName=$web_app_name \
      appServicePlanName=automatedbotAppPlan appServicePlanLocation=$location \
      appType=UserAssignedMSI appId=$client_id appSecret="" tenantId=$tenant_id \
      UMSIName=$identity_name UMSIResourceGroupName=$resource_group_name \
      appInsightsName=${web_app_name}AppInsights \
    --verbose

az deployment group create --resource-group $resource_group_name \
    --template-file ./iac/bot.json \
    --parameters appType=UserAssignedMSI botId=$botId \
      msAppId=$client_id tenantId=$tenant_id msiResourceId=$identity_id \
      messagingEndpoint=https://$web_app_name.azurewebsites.net/api/messages \


# building the app
#dotnet build ./src/jovadkerecho/EchoBot.csproj -c Release
#dotnet publish ./src/jovadkerecho/EchoBot.csproj -c Release -o ./publish
# az webapp deploy --resource-group Telephony.Automated.RG --name automatedbot20250106 --src-path ./publish.zip --type zip

#dotnet publish ./src/jovadkerecho/EchoBot.csproj -c Release -p:WebPublishMethod=Package -p:PackageLocation=./publish.zip

#tar -czf publish.tar.gz -C ./jovadkerecho .

#az webapp deploy --resource-group $resource_group_name --name $web_app_name --src-path ./publish

