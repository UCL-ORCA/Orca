Write-Host "Configuring appsettings.json"

# Read appsettings.json
function Get-ScriptDirectory { Split-Path $MyInvocation.ScriptName }
$appsettingsPath = Join-Path (Get-ScriptDirectory) 'appsettings.Template.json'
$appsettings = Get-Content $appsettingsPath -Raw

#Replace values for sharepoint
Write-Host "Please provide the following settings to configure the application:"

$sharepointUrl = Read-Host "Your Sharepoint site url (e.g. https://myinstitution.sharepoint.com/myCustomSite)"
$appsettings = $appsettings -replace "//`"SharepointUrl`": `"`"", "`"SharepointUrl`": `"$sharepointUrl`""

$clientId = Read-Host "Your Sharepoint Client Id "
$appsettings = $appsettings -replace "//`"ClientId`": `"`"", "`"ClientId`": `"$clientId`""

$sharepointClientSecret = Read-Host "Your Sharepoint Client Secret "
$appsettings = $appsettings -replace "//`"ClientSecret`": `"YOUR_SHAREPOINT_CLIENT_SECRET`"", "`"ClientSecret`": `"$sharepointClientSecret`""

#Replace values for msgraph
$appId = Read-Host "Your Azure App Id with Microsoft Graph Access"
$appsettings = $appsettings -replace "//`"AppId`": `"`"", "`"AppId`": `"$appId`""

$tenantId = Read-Host "Your Azure Tenant Id"
$appsettings = $appsettings -replace "//`"TenantId`": `"`"", "`"TenantId`": `"$tenantId`""

$msGraphClientSecret = Read-Host "Your Azure ClientSecret with Microsoft Graph Access"
$appsettings = $appsettings -replace "//`"ClientSecret`": `"YOUR_MS_GRAPH_CLIENT_SECRET`"", "`"ClientSecret`": `"$msGraphClientSecret`""

$domain = Read-Host "Your public host URL through which this application is exposed (e.g. https://myorcadeployment.azurewebsites.net)"
$appsettings = $appsettings -replace "//`"Domain`": `"`"", "`"Domain`": `"$domain`""

#Replace values for Caliper settings
$caliperApiKey = Read-Host "The Caliper API Key used by your Moodle server to securely notify ORCA about student interactions (needed when configuring the Caliper plugin for your Moodle server)"
$appsettings = $appsettings -replace "//`"ApiKey`": `"`"", "`"ApiKey`": `"$caliperApiKey`""

#Replace values for database
$enableDatabase = Read-Host "Would you like to connect ORCA to a PostgreSQL database to enable analytics?`n([Y]es/[N])o"
$enableDatabase = $enableDatabase.ToUpper() -match "Y" -or $enableDatabase.ToUpper() -match "YES"
If ($enableDatabase) {
    $serverName = Read-Host "Database Server Name"
    $appsettings = $appsettings -replace "//`"Servername`": `"`"", "`"Servername`": `"$serverName`""
	
    $username = Read-Host "Database Username"
    $appsettings = $appsettings -replace "//`"Username`": `"`"", "`"Username`": `"$username`""

    $Password = Read-Host "Database Password"
    $appsettings = $appsettings -replace "//`"Password`": `"`"", "`"Password`": `"$Password`""	
	
    $Database = Read-Host "Database name"
    $appsettings = $appsettings -replace "//`"Database`": `"`"", "`"Database`": `"$Database`""	
	
}

# Write to appsettings.json
$configuredAppsettingsPath = Join-Path (Get-ScriptDirectory) '../appsettings.json'
Set-Content -Path $configuredAppsettingsPath -Value $appsettings
Write-Host "Configuration saved to $configuredAppsettingsPath"

Write-Host "When configuring the Caliper plugin on your Moodle server, make sure to set the following settings:"
Write-Host "'Event Store URL': https://$domain/api/events/caliper"
Write-Host "'API key': the Caliper API Key you provided"
Write-Host "'Batch size': 100"

If ($enableDatabase) {
    $powerbiGenerationScriptPath = Join-Path (Get-ScriptDirectory) 'GeneratePowerBiDashboard.ps1'
    Write-Host "Configuring Power BI Dashboard for analytics to connect to db host: $serverName and database: $Database"
    Write-Host "Make sure to provide the Username '$username@$Database' and previously configured Database Password when opening the Power BI dashboard"
    & $powerbiGenerationScriptPath "$serverName" "$Database"
}