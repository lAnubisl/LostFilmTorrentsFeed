
function signingIn() {
    Write-Host "Sign In ..."
    az login
    Write-Host "Sign In completed."
}

function setSubscription($subscriptionId) {
    Write-Host "Set subscription ..."
    az account set --subscription $subscriptionId
    Write-Host "Set subscription completed."
}

function createResourceGroup($location, $resourceGroupName) {
    Write-Host "Creating 'Resource Group' ..."
    az group create `
        -l $location `
        -n $resourceGroupName
    Write-Host "Creating 'Resource Group' completed."
}

function createResources($location, $resourceGroupName, $name, $baseLinkId, $baseFeedCookie, $environment, $domainName) {
    Write-Host "Creating all resources in bicep template ..."
    az deployment group create `
        --resource-group $resourceGroupName `
        --template-file azure.bicep `
        --parameters `
            environmentName=$environment `
            location=$location `
            baseLinkId=$baseLinkId `
            baseFeedCookie=$baseFeedCookie `
            name=$name `
            domainName=$domainName
    Write-Host "Creating all resources in bicep template completed."
}

function enableStorageAccountWebsite($storageAccountName) {
    Write-Host "Configuring storage account static website ..."
    az storage blob service-properties update `
        --account-name $storageAccountName `
        --static-website $true `
        --index-document index.html
    Write-Host "Configuring storage account static website completed."
}