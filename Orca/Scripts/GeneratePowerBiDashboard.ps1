param (
[Parameter(Mandatory)]$databaseServerName,
[Parameter(Mandatory)]$databaseName
)

function Get-ScriptDirectory { Split-Path $MyInvocation.ScriptName }

$powerbiTemplatePath = Join-Path (Get-ScriptDirectory) 'PowerBi/dashboard.Template.zip'

$tempConfigDir = Join-Path (Get-ScriptDirectory) 'TempPowerBiConfig'

# clean existing tempConfigDir as we will unzip the powerbi template to change its settings
Remove-Item $tempConfigDir -Force -Recurse -ErrorAction Ignore

try {
    # copy the template to temp dir where it will be modified
    New-Item -Type dir $tempConfigDir | Out-Null
    $generatedDashboardZipPath = Join-Path $tempConfigDir 'OrcaDashboard.zip'
    Copy-Item -Path $powerbiTemplatePath -Destination $generatedDashboardZipPath


    $file = New-Object System.IO.FileStream($generatedDashboardZipPath, [System.IO.FileMode]::Open)
    # update means Read and Write permissions
    $zip = New-Object System.IO.Compression.ZipArchive($file, [System.IO.Compression.ZipArchiveMode]::Update)
    foreach ($entry in $zip.Entries) {
        if ($entry.Name -match "DataModelSchema") {
            # modify the data model schema
            $reader = New-Object System.IO.StreamReader($entry.Open(), [System.Text.Encoding]::Unicode, $False)
            $dataModelSchema = $reader.ReadToEnd()
            $reader.Close()

            $dataModelSchema = $dataModelSchema -replace "ORCA_DB_SERVER_NAME", "$databaseServerName"
            $dataModelSchema = $dataModelSchema -replace "ORCA_DB_NAME", "$databaseName"
            
            $unicodeNoBomEncoding = New-Object System.Text.UnicodeEncoding(! [System.BitConverter]::IsLittleEndian, $False)
            $writer = New-Object System.IO.StreamWriter($entry.Open(), $unicodeNoBomEncoding)
            $writer.WriteLine($dataModelSchema)
            $writer.Close()
        }
    }
    $zip.Dispose()
    $file.Close()
    
    # rename to .pbit
    $generatedPbitFilePath = Join-Path (Get-ScriptDirectory) "OrcaDashboard.pbit"
    Move-Item -Path $generatedDashboardZipPath -Force -Destination $generatedPbitFilePath

    Write-Host "Power BI report file saved to $generatedPbitFilePath"
    Write-Host "Launch the file with Power BI Desktop to publish online"
    
}
finally {
    # delete all possible temp files
    Remove-Item $tempConfigDir -Force -Recurse -ErrorAction Ignore
}