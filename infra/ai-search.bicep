@description('Name of the Azure Cognitive Search service')
param searchName string = '${uniqueString(resourceGroup().id)}-search'

@minLength(2)
@maxLength(48)
param location string = resourceGroup().location

resource search 'Microsoft.Search/searchServices@2025-02-01-preview' = {
  name: searchName
  location: location
  sku: {
    name: 'free'
  }
  properties: {
    hostingMode: 'default'
    replicaCount: 1
    partitionCount: 1
    publicNetworkAccess: 'enabled'
  }
}

output searchEndpoint string = 'https://${search.name}.search.windows.net'
output adminKey string = listAdminKeys(search.id, '2025-02-01-preview').primaryKey
