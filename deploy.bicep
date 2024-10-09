@description('Specifies the application name.')
param applicationName string

@description('Specifies the application name.')
param applicationContext string

@description('The Id of the subscription.')
param subscriptionId string

@description('Specifies all secrets {"secretName":"","secretValue":""} wrapped in a secure object.')
@secure()
param secretsObject object

@description('Specifies the location for server and database')
param location string = resourceGroup().location

var applicationFullName = '${applicationContext}-${applicationName}'
var apiSiteName = 'app-${toLower(applicationFullName)}'
var appServicePlanName = 'asp-${toLower(applicationFullName)}'
var storageAccountName = 'st${uniqueString(resourceGroup().id)}'
var keyVaultName = 'kv-${toLower(applicationFullName)}'
var applicationInsightsName = 'appi-${toLower(applicationFullName)}'
var userAssignedIdentitiesName = 'id-${toLower(applicationFullName)}'

var staticSiteName = 'stapp-${toLower(applicationFullName)}'
// var hostNameBindingsName = 'hnb-${toLower(applicationFullName)}'
var apiUrl = '${toLower(applicationName)}.azurewebsites.net'
var scmUrl = '${toLower(applicationName)}.scm.azurewebsites.net'
var budgetName = 'bg-${toLower(applicationFullName)}'


resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-07-31-preview' = {
  name: userAssignedIdentitiesName
  location: 'francecentral'
}

// Configured federated identity credentials to allow Github Actions
resource userAssignedIdentities_app_bim42_prod_f_id_a382_name_simonmoreau_RevitToIFCApp_8b5a 'Microsoft.ManagedIdentity/userAssignedIdentities/federatedIdentityCredentials@2023-07-31-preview' = {
  parent: userAssignedIdentity
  name: 'simonmoreau-RevitToIFCApp-8b5a'
  properties: {
    issuer: 'https://token.actions.githubusercontent.com'
    subject: 'repo:simonmoreau/RevitToIFCApp:environment:production'
    audiences: [
      'api://AzureADTokenExchange'
    ]
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
  properties: {
    supportsHttpsTrafficOnly: true
    defaultToOAuthAuthentication: true
  }
}

// Determine our connection string
var storageAccountConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'


resource vault 'Microsoft.KeyVault/vaults@2021-11-01-preview' = {
  name: keyVaultName
  location: location
  properties: {
    accessPolicies:[]
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    tenantId: subscription().tenantId
    sku: {
      name: 'standard'
      family: 'A'
    }
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
  }
}

resource KeyVaultSecretsUserRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(subscription().id, 'bicep-roleassignments', 'KeyVaultSecretsUser')
  scope: vault
  properties: {
    principalId: userAssignedIdentity.properties.principalId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6')
  }
}


resource key 'Microsoft.KeyVault/vaults/secrets@2024-04-01-preview' = [for secret in secretsObject.secrets: {
  name: secret.secretName
  parent: vault
  properties: {
    value: secret.secretValue
  }
}]

resource connectionStringKey 'Microsoft.KeyVault/vaults/secrets@2024-04-01-preview' = {
  name: 'Azure--ConnectionString'
  parent: vault
  properties: {
    value: storageAccountConnectionString
  }
}


resource hostingPlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'F1'
    tier: 'Free'
    size: 'F1'
    family:'F'
    capacity: 0
  }
  properties: {
    perSiteScaling:false
    elasticScaleEnabled:false
    maximumElasticWorkerCount:1
    isSpot:false
    reserved:true
    isXenon:false
    hyperV:false
    targetWorkerCount:0
    targetWorkerSizeId:0
    zoneRedundant:false
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Bluefield'
    Request_Source: 'rest'
    RetentionInDays: 90
  }
}


resource sites_revittoifcapp_api 'Microsoft.Web/sites@2023-12-01' = {
  name: apiSiteName
  location: location
  kind: 'app,linux'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup().name}/providers/Microsoft.ManagedIdentity/userAssignedIdentities/${userAssignedIdentity.name}': {}
    }
  }
  properties: {
    enabled: true
    hostNameSslStates: [
      {
        name: apiUrl
        sslState: 'Disabled'
        hostType: 'Standard'
      }
      {
        name: scmUrl
        sslState: 'Disabled'
        hostType: 'Repository'
      }
    ]
    serverFarmId: hostingPlan.id
    reserved: true
    isXenon: false
    hyperV: false
    dnsConfiguration: {}
    vnetRouteAllEnabled: false
    vnetImagePullEnabled: false
    vnetContentShareEnabled: false
    siteConfig: {
      numberOfWorkers: 1
      linuxFxVersion: 'DOTNETCORE|8.0'
      acrUseManagedIdentityCreds: false
      alwaysOn: false
      http20Enabled: false
      functionAppScaleLimit: 0
      minimumElasticInstanceCount: 1
    }
    scmSiteAlsoStopped: false
    clientAffinityEnabled: true
    clientCertEnabled: false
    clientCertMode: 'Required'
    hostNamesDisabled: false
    vnetBackupRestoreEnabled: false
    customDomainVerificationId: '2F999EC75A4CD61B5FED9FB0E622419CB390238BE602CC038B2E1F3F7DA3F9C7'
    containerSize: 0
    dailyMemoryTimeQuota: 0
    httpsOnly: true
    redundancyMode: 'None'
    publicNetworkAccess: 'Enabled'
    storageAccountRequired: false
    keyVaultReferenceIdentity: userAssignedIdentity.id
  }
}

resource WebsiteContributorRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(subscription().id, 'bicep-roleassignments', 'WebsiteContributor')
  scope: sites_revittoifcapp_api
  properties: {
    principalId: userAssignedIdentity.properties.principalId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', 'de139f84-1756-47ae-9be6-808fbbe84772')
  }
}

resource sites_revittoifcapp_name_ftp 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2023-12-01' = {
  parent: sites_revittoifcapp_api
  name: 'ftp'
  properties: {
    allow: false
  }
}

resource sites_revittoifcapp_name_scm 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2023-12-01' = {
  parent: sites_revittoifcapp_api
  name: 'scm'
  properties: {
    allow: false
  }
}

resource sites_revittoifcapp_name_static_site 'Microsoft.Web/staticSites@2022-09-01' = {
  name: staticSiteName
  location: 'westeurope'
  sku:{
    name: 'Free'
    tier: 'Free'
  }
  properties: {
    allowConfigFileUpdates: true
    stagingEnvironmentPolicy:'Enabled'
    provider: 'None'
    enterpriseGradeCdnStatus:'Disabled'
  }
}

resource sites_revittoifcapp_name_web 'Microsoft.Web/sites/config@2023-12-01' = {
  parent: sites_revittoifcapp_api
  name: 'web'
  properties: {
    appSettings: [
      {
        name: 'ManagedIdentityClientId'
        value: userAssignedIdentity.properties.clientId
      }
      {
        name: 'KeyVaultName'
        value: keyVaultName
      }
    ]
    numberOfWorkers: 1
    linuxFxVersion: 'DOTNETCORE|8.0'
    defaultDocuments: [
      'Default.htm'
      'Default.html'
      'Default.asp'
      'index.htm'
      'index.html'
      'iisstart.htm'
      'default.aspx'
      'index.php'
      'hostingstart.html'
    ]
    requestTracingEnabled: false
    remoteDebuggingEnabled: false
    remoteDebuggingVersion: 'VS2022'
    httpLoggingEnabled: false
    acrUseManagedIdentityCreds: false
    logsDirectorySizeLimit: 35
    detailedErrorLoggingEnabled: false
    publishingUsername: 'REDACTED'
    scmType: 'None'
    use32BitWorkerProcess: true
    webSocketsEnabled: false
    alwaysOn: false
    appCommandLine: 'dotnet WebApp.dll'
    managedPipelineMode: 'Integrated'
    virtualApplications: [
      {
        virtualPath: '/'
        physicalPath: 'site\\wwwroot'
        preloadEnabled: false
      }
    ]
    loadBalancing: 'LeastRequests'
    experiments: {
      rampUpRules: []
    }
    autoHealEnabled: false
    vnetRouteAllEnabled: false
    vnetPrivatePortsCount: 0
    publicNetworkAccess: 'Enabled'
    localMySqlEnabled: false
    managedServiceIdentityId: 6577
    ipSecurityRestrictions: [
      {
        ipAddress: 'Any'
        action: 'Allow'
        priority: 2147483647
        name: 'Allow all'
        description: 'Allow all access'
      }
    ]
    scmIpSecurityRestrictions: [
      {
        ipAddress: 'Any'
        action: 'Allow'
        priority: 2147483647
        name: 'Allow all'
        description: 'Allow all access'
      }
    ]
    scmIpSecurityRestrictionsUseMain: false
    http20Enabled: false
    minTlsVersion: '1.2'
    scmMinTlsVersion: '1.2'
    ftpsState: 'FtpsOnly'
    preWarmedInstanceCount: 0
    elasticWebAppScaleLimit: 0
    functionsRuntimeScaleMonitoringEnabled: false
    minimumElasticInstanceCount: 1
    azureStorageAccounts: {}
  }
}





@description('The total amount of cost or usage to track with the budget')
param amount int = 15

@description('The time covered by a budget. Tracking of the amount will be reset based on the time grain.')
@allowed([
  'Monthly'
  'Quarterly'
  'Annually'
])
param timeGrain string = 'Monthly'
param startDate string = utcNow('2024-10-01')

var firstThreshold = 90
var secondThreshold = 110
var contactEmails = ['simon@bim42.com']

resource budget 'Microsoft.Consumption/budgets@2023-11-01' = {
  name: budgetName
  properties: {
    timePeriod: {
      startDate: startDate
    }
    timeGrain: timeGrain
    amount: amount
    category: 'Cost'
    notifications: {
      NotificationForExceededBudget1: {
        enabled: true
        operator: 'GreaterThan'
        threshold: firstThreshold
        contactEmails: contactEmails
      }
      NotificationForExceededBudget2: {
        enabled: true
        operator: 'GreaterThan'
        threshold: secondThreshold
        contactEmails: contactEmails
      }
    }
  }
}


output AZUREAPPSERVICE_CLIENTID string = userAssignedIdentity.properties.clientId
