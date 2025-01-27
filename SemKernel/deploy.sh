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

#user_id=$(az ad signed-in-user show --query id --output tsv)
user_id="cd7092e7-9685-4513-8a9d-a4398ed49cdf"
echo "User ID: $user_id"

# https://github.com/Azure-Samples/openai/tree/main/End_to_end_Solutions

az deployment group create --resource-group $resource_group_name \
    --template-file ./iac/AI.json \
    --parameters openaiServiceName=$openaiservicename \
      storageAccountName=$storageAccountName \
      searchServicesName=$searchServiceName \
      userId=$user_id \
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


# get the user id running the script az ad signed-in-user show
#subscription_id=$(az account show --query id --output tsv)

# Check if the role assignment already exists
#role_assignment_exists=$(az role assignment list --assignee $user_id --role "Storage Blob Data Contributor" \
#  --scope "subscriptions/${subscription_id}/resourceGroups/${resource_group_name}/providers/Microsoft.Storage/storageAccounts/${storageAccountName}" --query "[].id" --output tsv)

#if [ -z "$role_assignment_exists" ]; then
  # Assign the Storage Blob Data Contributor role to the identity
#  az role assignment create --assignee-object-id $user_id --role "Storage Blob Data Contributor" \
#    --scope "subscriptions/${subscription_id}/resourceGroups/${resource_group_name}/providers/Microsoft.Storage/storageAccounts/${storageAccountName}"
#else
#  echo "Role assignment already exists."
#fi

# building the app
#dotnet build ./src/jovadkerecho/EchoBot.csproj -c Release
#dotnet publish ./src/jovadkerecho/EchoBot.csproj -c Release -o ./publish
# az webapp deploy --resource-group Telephony.Automated.RG --name automatedbot20250106 --src-path ./publish.zip --type zip

#dotnet publish ./src/jovadkerecho/EchoBot.csproj -c Release -p:WebPublishMethod=Package -p:PackageLocation=./publish.zip

#tar -czf publish.tar.gz -C ./jovadkerecho .

#az webapp deploy --resource-group $resource_group_name --name $web_app_name --src-path ./publish

