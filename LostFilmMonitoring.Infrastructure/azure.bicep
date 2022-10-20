param location string = resourceGroup().location
param functionstorageAccountName string = 'stfunclostfilmfeed'
param storageAccountName string = 'stlostfilmfeed'
param functionAppName string = 'funclostfilmfeed'
param functionAppServicePlanName string = 'planlostfilmfeed'
param environmentName string = 'Staging'
param websiteMainUIAddress string = 'https://staging.lostfilmfeed.petproject.by'
param storageAccountCustomDomainName string = 'staging.lostfilmfeed.petproject.by'

//https://stackoverflow.com/questions/61637124/azure-devops-pipeline-error-tenant-id-application-id-principal-id-and-scope
@allowed([
  'new'
  'existing'
])
param newOrExisting string = 'existing'

var roleAssignmentName = guid(resourceGroup().id, 'contributor')
var contributorRoleDefinitionId = resourceId('Microsoft.Authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c')


resource storageAccount 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  kind: 'StorageV2'
  location: location
  name: storageAccountName
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

resource storageAccountWebsiteContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-05-01' = {
  name: '${storageAccount.name}/default/$web'
  properties: {
    immutableStorageWithVersioning: {
      enabled: false
    }
    defaultEncryptionScope: '$account-encryption-key'
    denyEncryptionScopeOverride: false
    publicAccess: 'None'
  }
}

resource storageAccountBaseTorrentsContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-05-01' = {
  name: '${storageAccount.name}/default/basetorrents'
  properties: {
    immutableStorageWithVersioning: {
      enabled: false
    }
    defaultEncryptionScope: '$account-encryption-key'
    denyEncryptionScopeOverride: false
    publicAccess: 'None'
  }
}

resource storageAccountImagesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-05-01' = {
  name: '${storageAccount.name}/default/images'
  properties: {
    immutableStorageWithVersioning: {
      enabled: false
    }
    defaultEncryptionScope: '$account-encryption-key'
    denyEncryptionScopeOverride: false
    publicAccess: 'None'
  }
}

resource storageAccountImagesModelsContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-05-01' = {
  name: '${storageAccount.name}/default/models'
  properties: {
    immutableStorageWithVersioning: {
      enabled: false
    }
    defaultEncryptionScope: '$account-encryption-key'
    denyEncryptionScopeOverride: false
    publicAccess: 'None'
  }
}

resource storageAccountRssFeedsContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-05-01' = {
  name: '${storageAccount.name}/default/rssfeeds'
  properties: {
    immutableStorageWithVersioning: {
      enabled: false
    }
    defaultEncryptionScope: '$account-encryption-key'
    denyEncryptionScopeOverride: false
    publicAccess: 'None'
  }
}

resource storageAccountUserTorrentsContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-05-01' = {
  name: '${storageAccount.name}/default/usertorrents'
  properties: {
    immutableStorageWithVersioning: {
      enabled: false
    }
    defaultEncryptionScope: '$account-encryption-key'
    denyEncryptionScopeOverride: false
    publicAccess: 'None'
  }
}

resource storageAccountEpisodesTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2022-05-01' = {
  name: '${storageAccount.name}/default/episodes'
  properties: { }
}

resource storageAccountSeriesTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2022-05-01' = {
  name: '${storageAccount.name}/default/series'
  properties: { }
}

resource storageAccountSubscriptionTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2022-05-01' = {
  name: '${storageAccount.name}/default/subscription'
  properties: { }
}

resource storageAccountUsersTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2022-05-01' = {
  name: '${storageAccount.name}/default/users'
  properties: { }
}

resource uami 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: 'BicepScript${environmentName}'
  location: location
}

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = if(newOrExisting == 'new') {
  name: roleAssignmentName
  properties: {
    roleDefinitionId: contributorRoleDefinitionId
    principalId: uami.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

resource scriptEnableStaticWebsite 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  dependsOn: [
    roleAssignment
  ]
  kind: 'AzureCLI'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${uami.id}': {}
    }
  }
  location: location
  name: 'enableStaticWebsite'
  properties: {
    azCliVersion: '2.40.0'
    cleanupPreference: 'Always'
    retentionInterval: 'P1D'
    scriptContent: 'az storage blob service-properties update --account-name ${storageAccount.name} --static-website --index-document index.html'
    timeout: 'PT5M'
  }
}




resource functionStorageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' = {
  name: functionstorageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
}

resource functionAppServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  kind: 'functionapp'
  location: location
  name: functionAppServicePlanName
  properties: {
    elasticScaleEnabled: false
    hyperV: false
    maximumElasticWorkerCount: 0
    isSpot: false
    isXenon: false
    perSiteScaling: false
    reserved: true
    targetWorkerCount: 0
    targetWorkerSizeId: 0
    zoneRedundant: false
  }
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
}

resource functionApp 'Microsoft.Web/sites@2022-03-01' = {
  kind: 'functionapp,linux'
  location: location
  name: functionAppName
  properties: {
    enabled: true
    serverFarmId: functionAppServicePlan.id
    siteConfig: {
        appSettings: [
          {
            name: 'AzureWebJobsDashboard'
            value: 'DefaultEndpointsProtocol=https;AccountName=${functionStorageAccount.name};AccountKey=${functionStorageAccount.listKeys().keys[0].value}'
          }
          {
            name: 'AzureWebJobsStorage'
            value: 'DefaultEndpointsProtocol=https;AccountName=${functionStorageAccount.name};AccountKey=${functionStorageAccount.listKeys().keys[0].value}'
          }
          {
            name: 'FUNCTIONS_EXTENSION_VERSION'
            value: '~4'
          }
          {
            name: 'FUNCTIONS_WORKER_RUNTIME'
            value: 'dotnet-isolated'
          }
          {
            name: 'TORRENTTRACKERS'
            value: 'http://bt.tracktor.in/tracker.php/{0}/announce,http://bt99.tracktor.in/tracker.php/{0}/announce,http://bt0.tracktor.in/tracker.php/{0}/announce,http://user5.newtrack.info/tracker.php/{0}/announce,http://user1.newtrack.info/tracker.php/{0}/announce'
          }
          {
            name: 'TORRENTSDIRECTORY'
            value: 'torrentfiles'
          }
          {
            name: 'IMAGESDIRECTORY'
            value: 'images'
          }
        ]
        numberOfWorkers: 1
        linuxFxVersion: 'DOTNET-ISOLATED|6.0'
        ftpsState: 'FtpsOnly'
        minTlsVersion: '1.2'
    }
  }
}
