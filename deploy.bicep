@description('The name of the function app that you wish to create.')
param appName string

@description('Specifies all secrets {"secretName":"","secretValue":""} wrapped in a secure object.')
@secure()
param secretsObject object

@description('Location for all resources.')
param location string = resourceGroup().location

var hostingPlanName = appName
var applicationInsightsName = appName
var storageAccountName = uniqueString(resourceGroup().id)
var keyVaultName = '${appName}Vault'
var identitiesName = '${appName}Identities'
var apiSiteName = '${appName}API'
var apiUrl = '${toLower(apiSiteName)}.azurewebsites.net'
var scmUrl = '${toLower(apiSiteName)}.scm.azurewebsites.net'
var frontEndSiteName = '${appName}Site'

var skuName = 'standard'
var storageAccountType = 'Standard_LRS'



resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: storageAccountType
  }
  kind: 'Storage'
  properties: {
    supportsHttpsTrafficOnly: true
    defaultToOAuthAuthentication: true
  }
}

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
      name: skuName
      family: 'A'
    }
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
  }
}

resource key 'Microsoft.KeyVault/vaults/secrets@2024-04-01-preview' = [for secret in secretsObject.secrets: {
  name: secret.secretName
  parent: vault
  properties: {
    value: secret.secretValue
  }
}]

resource hostingPlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: hostingPlanName
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
    type: 'SystemAssigned'
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
    keyVaultReferenceIdentity: 'SystemAssigned'
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
  name: frontEndSiteName
  location: 'westeurope'
  sku:{
    name: 'Free'
    tier: 'Free'
  }
  properties: {
    allowConfigFileUpdates: true
    provider: 'GitHub'
    repositoryUrl: 'https://github.com/simonmoreau/RevitToIFCApp'
    branch: 'master'
  }
}

resource sites_revittoifcapp_name_web 'Microsoft.Web/sites/config@2023-12-01' = {
  parent: sites_revittoifcapp_api
  name: 'web'
  properties: {
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
    scmType: 'GitHubAction'
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

resource revittoifcapp_revittoifcapp_azurewebsites_net 'Microsoft.Web/sites/hostNameBindings@2023-12-01' = {
  parent: sites_revittoifcapp_api
  name: apiUrl
  properties: {
    siteName: toLower(apiSiteName)
    hostNameType: 'Verified'
  }
}

resource revittoifcapp_identities 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-07-31-preview' = {
  name: identitiesName
  location: location
}

resource revittoifcapp_id_88be_simonmoreau_RevitToIFCApp_b00a 'Microsoft.ManagedIdentity/userAssignedIdentities/federatedIdentityCredentials@2023-07-31-preview' = {
  parent: revittoifcapp_identities
  name: 'simonmoreau-RevitToIFCApp-b00a'
  properties: {
    issuer: 'https://token.actions.githubusercontent.com'
    subject: 'repo:simonmoreau/RevitToIFCApp:environment:production'
    audiences: [
      'api://AzureADTokenExchange'
    ]
  }
}

