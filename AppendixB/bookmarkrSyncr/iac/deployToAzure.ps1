# Log in to Azure account
az login --tenant "<your-tenant-id>"

# Set the subscription ID
az account set --subscription "<your-subscription-id>"

# Create a resource group
az group create --name "bookmarkr-rg" --location "canada central"

# Deploy the Bicep script
az deployment group create --resource-group "bookmarkr-rg" --template-file "infra.bicep"

# Create a blob container named 'bookmarks' with blob access
az storage container create --name bookmarks --account-name "bookmarkrdatastore" --account-key "<your-account-key"
