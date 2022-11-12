param environmentName string = 'Staging'
param name string = 'lostfilmfeed'
param location string = resourceGroup().location

@secure()
param baseLinkId string = ''

@secure()
param baseFeedCookie string = ''

param domainName string = ''

module storageAccountModule './azure_storage.bicep' = {
  name: 'storage_account_deployment'
  params: {
    environmentName: environmentName
    name: name
    location: location
    websiteMainUIAddress: 'https://${domainName}'
    storageAccountCustomDomainName: domainName
  }
}

module functionModule './azure_function.bicep' = {
  name: 'function_deployment'
  params: {
    environmentName: environmentName
    name: name
    location: location
    storageAccountName: storageAccountModule.outputs.storageAccountName
    baseLinkId: baseLinkId
    baseFeedCookie: baseFeedCookie
  }
}
