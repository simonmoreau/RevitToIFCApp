az group create --name rg-bim42-revittoifcapp --location francecentral
az deployment group create --resource-group MyFeeds --template-file main.bicep --parameters appInsightsLocation=francecentral