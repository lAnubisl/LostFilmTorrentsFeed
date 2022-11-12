param environmentName string = 'Staging'
param name string = 'lostfilmfeed'
param location string = resourceGroup().location
param websiteMainUIAddress string = 'https://staging.lostfilmfeed.petproject.by'
param storageAccountCustomDomainName string = 'staging.lostfilmfeed.petproject.by'

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  kind: 'StorageV2'
  location: location
  name: 'st${name}'
  properties: {
    accessTier: 'Hot'  
    allowBlobPublicAccess: true
    allowCrossTenantReplication: true 
    allowSharedKeyAccess: true
    customDomain: {
      name: storageAccountCustomDomainName
    }
    defaultToOAuthAuthentication: false
    encryption: {
      identity: {}
      keySource: 'Microsoft.Storage'
      requireInfrastructureEncryption: true
      services: {
        blob: {
          enabled: true
          keyType: 'Account'    
        }
        file: {
          enabled: true
          keyType: 'Account'
        }
      }
    }
    networkAcls: {
      bypass: 'AzureServices'
      virtualNetworkRules: []
      ipRules: []
      defaultAction: 'Allow'
    }
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  }
  sku: {
    name: 'Standard_GRS'
  }
  tags: {
    Service: 'LostFilmFeed'
    Environment: environmentName
  }
}

resource storageAccountProperties 'Microsoft.Storage/storageAccounts/blobServices@2022-05-01' = {
  name: '${storageAccount.name}/default'
  properties: {
    changeFeed: {
      enabled: false
    }
    containerDeleteRetentionPolicy: {
      days: 7
      enabled: true
    }
    cors: {
      corsRules: [
        {
          allowedHeaders: [
           '*' 
          ]
          allowedMethods: [
            'GET', 'OPTIONS'
          ]
          allowedOrigins: [
            websiteMainUIAddress
          ]
          exposedHeaders: [
            ''
          ]
          maxAgeInSeconds: 0
        }
      ]
    }
    deleteRetentionPolicy: {
      allowPermanentDelete: false
      enabled: false
    }
    isVersioningEnabled: false
    restorePolicy: {
      enabled: false
    }
  }
}

module containers './azure_storage_containers.bicep' = {
  name: 'storage_account_containers'
  params: {
    storageAccountName: storageAccount.name
  }
}

module tables './azure_storage_tables.bicep' = {
  name: 'storage_account_tables'
  params: {
    storageAccountName: storageAccount.name
  }
}

output storageAccountName string = storageAccount.name
