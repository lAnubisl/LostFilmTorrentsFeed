param storageAccountName string = ''

// Storage table to store episodes
resource storageAccountEpisodesTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2022-05-01' = {
  name: '${storageAccountName}/default/episodes'
  properties: { }
}

// Storage table to store series
resource storageAccountSeriesTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2022-05-01' = {
  name: '${storageAccountName}/default/series'
  properties: { }
}

// Storage table to store subscriptions
resource storageAccountSubscriptionTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2022-05-01' = {
  name: '${storageAccountName}/default/subscription'
  properties: { }
}

// Storage table to store users
resource storageAccountUsersTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2022-05-01' = {
  name: '${storageAccountName}/default/users'
  properties: { }
}
