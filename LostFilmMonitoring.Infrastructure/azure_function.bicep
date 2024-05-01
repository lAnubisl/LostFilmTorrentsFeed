param name string = 'lostfilmfeed'
param location string = resourceGroup().location
param environmentName string = 'Staging'

@secure()
param baseLinkId string = ''

@secure()
param baseFeedCookie string = ''

param storageAccountName string = ''

var functionStorageAccountName = 'stfunc${name}'
var functionAppName = 'func${name}'
var functionAppServicePlanName = 'plan${name}'

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' existing = {
  name: storageAccountName
}

resource functionStorageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' = {
  name: functionStorageAccountName
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
  tags: {
    Service: 'LostFilmFeed'
    Environment: environmentName
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
          {
            name: 'BASEFEEDCOOKIE'
            value: baseFeedCookie
          }
          {
            name: 'BASELINKUID'
            value: baseLinkId
          }
          {
            name: 'BASEURL'
            value: '${storageAccount.properties.primaryEndpoints.blob}usertorrents'
          }
          {
            name: 'StorageAccountConnectionString'
            value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
          }
        ]
        numberOfWorkers: 1
        linuxFxVersion: 'DOTNET-ISOLATED|8.0'
        ftpsState: 'FtpsOnly'
        minTlsVersion: '1.2'
    }
  }
  tags: {
    Service: 'LostFilmFeed'
    Environment: environmentName
  }
}
