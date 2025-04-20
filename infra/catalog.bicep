@description('Cosmos DB account name')
param cosmosName     string = '${uniqueString(resourceGroup().id)}-cosmos'
param location       string = resourceGroup().location

resource cosmos 'Microsoft.DocumentDB/databaseAccounts@2024-12-01-preview' = {
  name:     cosmosName
  location: location
  kind:     'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    enableFreeTier:           true
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
  }
}

resource db 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-12-01-preview' = {
  name:     '${cosmos.name}/campusdb'
  properties: {
    resource: {
      id: 'campusdb'
    }
  }
}

resource container 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-12-01-preview' = {
  name:     '${cosmos.name}/campusdb/knowledge'
  properties: {
    resource: {
      id: 'knowledge'
      partitionKey: {
        paths: ['/pk']
        kind:  'Hash'
      }
    }
  }
}

output cosmosEndpoint string = cosmos.properties.documentEndpoint
output cosmosKey      string = listKeys(cosmos.id, '2024-12-01-preview').primaryMasterKey
