Write-Host "This script provisions resources on Azure through the Azure CLI"
Write-Host "Make sure the Azure CLI is installed before running it: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"

az login

# Create resource group
$region = Read-Host "Which region would you like to deploy the application to (e.g. uksouth, westeurope)?"

# Read arm parameters.Template.json
function Get-ScriptDirectory { Split-Path $MyInvocation.ScriptName }
$armParametersTemplatePath = Join-Path (Get-ScriptDirectory) 'AzureResourceManager/parameters.Template.json'
$armParametersTemplate = Get-Content $armParametersTemplatePath -Raw

Write-Host "Please provide the following settings to configure and deploy the application:"

#Replace values for webapp name
$webAppName = Read-Host "The name that will be given to the webapp (e.g. orca). Must be unique as it will form the domain [webAppName].azurewebsites.net:"
$armParametersTemplate = $armParametersTemplate -replace "WEB_APP_NAME", "`"$webAppName`""

#Replace values for sharepoint
$sharepointUrl = Read-Host "Your Sharepoint site url (e.g. https://myinstitution.sharepoint.com/myCustomSite)"
$armParametersTemplate = $armParametersTemplate -replace "SHAREPOINT_URL", "`"$sharepointUrl`""

$clientId = Read-Host "Your Sharepoint Client Id "
$armParametersTemplate = $armParametersTemplate -replace "SHAREPOINT_CLIENT_ID", "`"$clientId`""

$sharepointClientSecret = Read-Host "Your Sharepoint Client Secret "
$armParametersTemplate = $armParametersTemplate -replace "SHAREPOINT_CLIENT_SECRET", "`"$sharepointClientSecret`""

#Replace values for msgraph
$appId = Read-Host "Your Azure App Id with Microsoft Graph Access"
$armParametersTemplate = $armParametersTemplate -replace "MS_GRAPH_APP_ID", "`"$appId`""

$tenantId = Read-Host "Your Azure Tenant Id"
$armParametersTemplate = $armParametersTemplate -replace "TENANT_ID", "`"$tenantId`""

$msGraphClientSecret = Read-Host "Your Azure ClientSecret with Microsoft Graph Access"
$armParametersTemplate = $armParametersTemplate -replace "MS_GRAPH_CLIENT_SECRET", "`"$msGraphClientSecret`""

#Replace values for Caliper settings
$caliperApiKey = Read-Host "The Caliper API Key used by your Moodle server to securely notify ORCA about student interactions (needed when configuring the Caliper plugin for your Moodle server)"
$armParametersTemplate = $armParametersTemplate -replace "CALIPER_API_KEY", "`"$caliperApiKey`""

#Replace values for database
$enableDatabase = Read-Host "Would you like to deploy an Azure PostgreSQL database to enable analytics (this will incur additional charges)?`n([Y]es/[N])o"
$enableDatabase = $enableDatabase.ToUpper() -match "Y" -or $enableDatabase.ToUpper() -match "YES"
If ($enableDatabase) {
    Write-Host "Please provide the following database settings:"

    $databaseName = Read-Host "The name that will be given to the database (e.g. orcadb). Must be unique as it will form the domain [databaseName].postgres.database.azure.com"
    $armParametersTemplate = $armParametersTemplate -replace "DB_NAME", "`"$databaseName`""

    $databasePassword = Read-Host "Database Password (at least 8 characters long, must contain uppercase characters, lowercase lowercase character, and digits number or special characters)"
    $armParametersTemplate = $armParametersTemplate -replace "DB_PASSWORD", "`"$databasePassword`""	
}

# Write to parameters.json
$configuredArmParametersPath = Join-Path (Get-ScriptDirectory) 'AzureResourceManager/parameters.json'
Set-Content -Path $configuredArmParametersPath -Value $armParametersTemplate
Write-Host "Configuration saved to $configuredArmParametersPath"

Write-Host "Beginning deployment to Azure based on generated configuration"

# Create resource group with same name as webapp
az group create --name "$webAppName" --location "$region"

# Deploy resources
$armTemplatePath = Join-Path (Get-ScriptDirectory) 'AzureResourceManager/template.json'
$zippedBinariesPath = Join-Path (Get-ScriptDirectory) 'AzureResourceManager/orca-azure-linux.zip'
az deployment group create --name "${webAppName}Deployment" --resource-group "$webAppName" --template-file "$armTemplatePath" --parameters "$configuredArmParametersPath"
az webapp deployment source config-zip -n "$webAppName" -g "$webAppName" --src "$zippedBinariesPath"

Write-Host "Deployment Complete"

Write-Host "When configuring the Caliper plugin on your Moodle server, make sure to set the following settings:"
Write-Host "'Event Store URL': https://$webAppName.azurewebsites.net/api/events/caliper"
Write-Host "'API key': $caliperApiKey"
Write-Host "'Batch size': 100"

If ($enableDatabase) {
    $powerbiGenerationScriptPath = Join-Path (Get-ScriptDirectory) 'GeneratePowerBiDashboard.ps1'
    $dbHost = "$databaseName.postgres.database.azure.com"
    Write-Host "Configuring Power BI Dashboard for analytics to connect to db host: $databaseName.postgres.database.azure.com and database: $databaseName"
    Write-Host "Make sure to provide the Username 'orca_admin@$databaseName' and previously configured Database Password when opening the Power BI dashboard"
    & $powerbiGenerationScriptPath "$dbHost" "$databaseName"
}