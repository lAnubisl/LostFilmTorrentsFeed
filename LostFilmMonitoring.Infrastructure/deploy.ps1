param (
    [Parameter(Mandatory)]
    [string] $subscriptionId,

    [Parameter(Mandatory)]
    [string] $resourceGroupName,

    [Parameter(Mandatory)]
    [string] $name,

    [Parameter(Mandatory)]
    [string] $baseLinkId,

    [Parameter(Mandatory)]
    [string] $baseFeedCookie,

    [Parameter(Mandatory)]
    [ValidateSet("Staging","Production")]
    [string] $environmentName,

    [Parameter(Mandatory)]
    [string] $fqdn
)

$currentDir = (Get-Item -Path ".\").FullName
Import-Module -Name ($currentDir + "\Functions.ps1") -Force

$location = 'northeurope'

signingIn
setSubscription $subscriptionId
createResourceGroup $location $resourceGroupName
createResources $location $resourceGroupName $name $baseLinkId $baseFeedCookie $environmentName $fqdn
enableStorageAccountWebsite "st$name"
Write-Host "Deployment completed."