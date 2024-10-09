az group delete --name rg-bim42-prod-fr-revittoifcapp
az group create --name rg-bim42-prod-fr-revittoifcapp --location francecentral
az deployment group create --resource-group rg-bim42-prod-fr-revittoifcapp --template-file deploy.bicep --parameters deploy.parameters.json