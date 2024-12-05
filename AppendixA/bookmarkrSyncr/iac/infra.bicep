param appName string = 'bookmarkrSyncr-api'
param storageAccountName string = 'bookmarkrdatastore'
param location string = resourceGroup().location
param runtimeStack string = 'DOTNETCORE|8.0'


// Storage Account
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    cors: [
      {
        allowedOrigins: [
          'https://${appName}.azurewebsites.net' // Allow only the App Service
        ]
        allowedMethods: [
          'GET'
          'POST'
        ]
        maxAgeInSeconds: 3600
      }
    ]
  }
}

// App Service
resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: appName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1' // Enable code publishing model
        }
      ]
      linuxFxVersion: runtimeStack // Specify the runtime stack for Linux apps
    }
  }
}


// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: '${appName}-Plan'
  location: location
  sku: {
    name: 'D1'
    tier: 'Shared'
    capacity: 0
  }
}
