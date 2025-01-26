
# Define variables
$searchServiceName = "searchservicejv001"
$adminApiKey = "your-admin-api-key"
$dataSourceName = "your-data-source-name"
$storageAccountName = "your-storage-account-name"
$containerName = "your-container-name"
$connectionString = "your-storage-account-connection-string"

# Create the data source JSON
$dataSourceJson = @{
    name = $dataSourceName
    type = "azureblob"
    credentials = @{
        connectionString = $connectionString
    }
    container = @{
        name = $containerName
    }
} | ConvertTo-Json -Depth 4

# Define the REST API endpoint
$endpoint = "https://$searchServiceName.search.windows.net/datasources?api-version=2020-06-30"

# Create the data source
$response = Invoke-RestMethod -Uri $endpoint -Method Post -Headers @{
    "Content-Type" = "application/json"
    "api-key" = $adminApiKey
} -Body $dataSourceJson

# Output the response
$response

