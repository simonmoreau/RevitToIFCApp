@description('The name of the function app that you wish to create.')
param appName string = 'revittoifcapp'

@description('Storage Account type')
@allowed([
  'Standard_LRS'
  'Standard_GRS'
  'Standard_RAGRS'
])
param storageAccountType string = 'Standard_LRS'

@description('Location for all resources.')
param location string = resourceGroup().location

@description('Location for Application Insights')
param appInsightsLocation string

@description('The language worker runtime to load in the function app.')
@allowed([
  'node'
  'dotnet'
  'java'
])
param runtime string = 'dotnet'

var hostingPlanName = appName
var applicationInsightsName = appName
var storageAccountName = '${uniqueString(resourceGroup().id)}azfunctions'
var sites_revittoifcapp_name_param = appName

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
    reserved:false
    isXenon:false
    hyperV:false
    targetWorkerCount:0
    targetWorkerSizeId:0
    zoneRedundant:false
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: appInsightsLocation
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Bluefield'
    Request_Source: 'rest'
    RetentionInDays: 90
  }
}


resource sites_revittoifcapp_name 'Microsoft.Web/sites@2023-12-01' = {
  name: sites_revittoifcapp_name_param
  location: location
  kind: 'app'
  properties: {
    enabled: true
    hostNameSslStates: [
      {
        name: '${sites_revittoifcapp_name_param}-htfdghcthschdken.francecentral-01.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Standard'
      }
      {
        name: '${sites_revittoifcapp_name_param}-htfdghcthschdken.scm.francecentral-01.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Repository'
      }
    ]
    serverFarmId: hostingPlan.id
    reserved: false
    isXenon: false
    hyperV: false
    dnsConfiguration: {}
    vnetRouteAllEnabled: false
    vnetImagePullEnabled: false
    vnetContentShareEnabled: false
    siteConfig: {
      numberOfWorkers: 1
      acrUseManagedIdentityCreds: false
      alwaysOn: false
      http20Enabled: false
      functionAppScaleLimit: 0
      minimumElasticInstanceCount: 0
    }
    scmSiteAlsoStopped: false
    clientAffinityEnabled: true
    clientCertEnabled: false
    clientCertMode: 'Required'
    hostNamesDisabled: false
    vnetBackupRestoreEnabled: false
    customDomainVerificationId: 'B8C6D4F97ABE756F9775858B8F74F7BCAF29557E90CF8ECF3C16F191D5188A2D'
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
  name: '${sites_revittoifcapp_name_param}/ftp'
  location: location
  properties: {
    allow: false
  }
  dependsOn: [
    sites_revittoifcapp_name
  ]
}

resource sites_revittoifcapp_name_scm 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2023-12-01' = {
  name: '${sites_revittoifcapp_name_param}/scm'
  location: location
  properties: {
    allow: false
  }
  dependsOn: [
    sites_revittoifcapp_name
  ]
}


resource sites_revittoifcapp_name_web 'Microsoft.Web/sites/config@2023-12-01' = {
  name: '${sites_revittoifcapp_name_param}/web'
  location: location
  properties: {
    numberOfWorkers: 1
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
    netFrameworkVersion: 'v8.0'
    requestTracingEnabled: false
    remoteDebuggingEnabled: false
    httpLoggingEnabled: false
    acrUseManagedIdentityCreds: false
    logsDirectorySizeLimit: 35
    detailedErrorLoggingEnabled: false
    publishingUsername: 'REDACTED'
    scmType: 'None'
    use32BitWorkerProcess: true
    webSocketsEnabled: false
    alwaysOn: false
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
    minimumElasticInstanceCount: 0
    azureStorageAccounts: {}
  }
  dependsOn: [
    sites_revittoifcapp_name
  ]
}


