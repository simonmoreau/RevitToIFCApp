
try {
    $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
 }
 catch {
    $scriptDir = 'C:\Users\Simon\Github\Forge\RevitToIFCApp\deploy'
 }

$settingsFile  = Join-Path -Path $scriptDir -ChildPath ..\api\local.settings.json
$bundlePath = Join-Path -Path $scriptDir -ChildPath RevitToIFCBundle.zip

$settings = Get-Content -Raw -Path $settingsFile | ConvertFrom-Json

$env:FORGE_CLIENT_ID = $settings.Values.FORGE_CLIENT_ID
$env:FORGE_CLIENT_SECRET = $settings.Values.FORGE_CLIENT_SECRET

forge-dm list-buckets

## Create the Forge bucket to store the converted files
forge-dm create-bucket -r persistent $settings.Values.ifcStorageKey
forge-dm create-bucket -r persistent $settings.Values.rvtStorageKey

## Create an array with every version of the engine
$engineVersions = '2018','2019', '2020', '2021', '2022'
# $appName = 'RevitToIFC'
$appName = 'RevitToIFCDev'

# forge-da list-engines
# forge-da list-appbundles
# forge-da list-activities

function DeployApplication($revitVersion,$bundle) {
   
   $nickName = $appName
   $appbundle_file = $bundle
   $appbundle_name = [string]::Format('{0}Bundle{1}',$appName,$revitVersion) # Your own appbundle name here
   $appbundle_alias = $revitVersion # Your own alias name here
   $appbundle_engine = [string]::Format('Autodesk.Revit+{0}',$revitVersion) # Your own appbundle engine here
   $appDescription = [string]::Format('Export Revit {0} files to IFC',$engineVersion)

   $activity_name = [string]::Format('{0}Activity{1}',$appName,$revitVersion) # Your own activity name here
   $activity_alias = $revitVersion # Your own activity alias here

   # Create or update an appbundle
   Write-Host "Creating an appbundle $appbundle_name with zip file $appbundle_file"
   $result = forge-da list-appbundles --short
   $result = $result | Select-String -Pattern $appbundle_name | Measure-Object -Line
   if ($result.Lines -eq 0) {
      Write-Host "Creating new appbundle"
      forge-da create-appbundle $appbundle_name $appbundle_file $appbundle_engine $appDescription
   } else {
      Write-Host "Updating existing appbundle"
      forge-da update-appbundle $appbundle_name $appbundle_file $appbundle_engine $appDescription
   }

   # Create or update an appbundle alias
   Write-Host "Creating an appbundle alias $appbundle_alias"
   $result = forge-da list-appbundle-versions $appbundle_name --short
   $appbundle_version = $result | Select-Object -Last 1
   Write-Host "Last appbundle version: $appbundle_version"
   $result = forge-da list-appbundle-aliases $appbundle_name --short
   $result = $result | Select-String -Pattern $appbundle_alias | Measure-Object -Line
   if ($result.Lines -eq 0) {
      Write-Host "Creating new appbundle alias"
      forge-da create-appbundle-alias $appbundle_alias $appbundle_name $appbundle_version
   } else {
      Write-Host "Updating existing appbundle alias"
      forge-da update-appbundle-alias $appbundle_alias $appbundle_name $appbundle_version
   }

   # Create or update an activity
   Write-Host "Creating an activity $activity_name"
   Write-Host $activity_name $appbundle_name $appbundle_alias $appbundle_engine
   $result = forge-da list-activities --short
   $result = $result | Select-String -Pattern $activity_name | Measure-Object -Line
   if ($result.Lines -eq 0) {
      Write-Host "Creating new activity"
      forge-da create-activity $activity_name $appbundle_name $appbundle_alias $appbundle_engine --update --nickname $nickName `
      --input rvtFile --input-verb get --input-zip false --input-required true --input-description "Input Revit model" --input-local-name input.rvt `
      --output result --output-verb put --output-zip false --output-required true --output-description "Results" --output-local-name output.ifc
   } else {
      Write-Host "Updating existing activity"
      forge-da update-activity $activity_name $appbundle_name $appbundle_alias $appbundle_engine --update --nickname $nickName `
      --input rvtFile --input-verb get --input-zip false --input-required true --input-description "Input Revit model" --input-local-name input.rvt `
      --output result --output-verb put --output-zip false --output-required true --output-description "Results" --output-local-name output.ifc
   }

   # Create or update an activity alias
   Write-Host "Creating an activity alias $activity_alias"
   $result = forge-da list-activity-versions $activity_name --short
   $activity_version = $result | Select-Object -Last 1
   Write-Host "Last activity version: $activity_version"
   $result = forge-da list-activity-aliases $activity_name --short
   $result = $result | Select-String -Pattern $activity_alias | Measure-Object -Line
   if ($result.Lines -eq 0) {
      Write-Host "Creating new activity alias"
      forge-da create-activity-alias $activity_alias $activity_name $activity_version
   } else {
      Write-Host "Updating existing activity alias"
      forge-da update-activity-alias $activity_alias $activity_name $activity_version
   }
}

## Loop on each version
foreach ($engineVersion in $engineVersions) {
   DeployApplication -revitVersion $engineVersion -bundle $bundlePath
}

# # Delete all AppBundle
# foreach ($engineVersion in $engineVersions) {
#    $appBundleId =  [string]::Format('{0}Bundle{1}',$appName,$engineVersion)
#    Write-Host "Deleting the existing appbundle $appBundleId"
#    forge-da delete-appbundle $appBundleId
# }

# # Delete all activity
# foreach ($engineVersion in $engineVersions) {
#    $appActivityId = [string]::Format('{0}Activity{1}',$appName,$engineVersion)
#    Write-Host "Deleting the activity $appActivityId"
#    forge-da delete-activity $appActivityId
# }


