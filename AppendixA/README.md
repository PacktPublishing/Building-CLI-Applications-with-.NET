About BookmarkrSyncr
===


# What is it?
`BookmarkrSyncr` is a (very) simple service that receives a list of bookmarks, syncs them with the ones stored in its storage account, then returns the synced list as a response.

It is used to illustrate how a CLI application written in .NET using C# can consume an external API. 


# Deploy it
## Deploy the infrastructure
The infrastructure code can be found in the `iac` folder and can be deployed by invoking the `deployToAzure.ps1` PowerShell script.

Before you invoke it, you will need to provide your Azure Subscription ID on line 5. 

The deployed infrastructure will look like this:

![BookmarkrSyncr Azure infrastructure](./images/bookmarkr-syncr-infra.png)

## Deploy the code
You can follow the instructions provided by Benjamin Day [in his video](https://www.youtube.com/watch?v=UcROU2kO4j0).

## Security
A system-assigned managed identity is enabled for the App Service. This Managed Identity has the 'Storage Blob Data Contributor' RBAC role on the storage account. Hence, only it can access the data in the storage account.