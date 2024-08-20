az group delete --name rg-bim42-revittoifcapp
az group create --name rg-bim42-revittoifcapp --location francecentral
az deployment group create --resource-group rg-bim42-revittoifcapp --template-file deploy.bicep --parameters appInsightsLocation=francecentral