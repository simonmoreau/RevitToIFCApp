param ($RevitVersion, $TargetName, $ProjectDir, $TargetPath, $TargetDir)
write-host $RevitVersion
write-host $TargetName
write-host $ProjectDir
write-host $TargetPath
write-host $TargetDir

$filePaths = @()

function SignFiles($TargetDir) {
    Get-ChildItem ($TargetDir) -Recurse | Where-Object { $_.extension -in ".exe", ".dll" } |
    Foreach-Object {

        $signature = Get-AuthenticodeSignature -FilePath $_.FullName

        if ($signature.status -ne "Valid") {
            # $filePaths = $filePaths + " " + "`"" + $_.FullName +"`""
            $filePaths += $_.FullName
        }
    }

    Write-Host $filePaths

    azuresigntool sign -du "https://www.eai.fr" `
      -fd sha384 -kvu https://kv-eai-util-fr-signing.vault.azure.net/ `
      -kvm `
      -kvc EAICodeSigningCertificate `
      -tr http://timestamp.digicert.com `
      -td sha384 `
      -v $filePaths
}

SignFiles $TargetDir

# Copy to Addin folder for debug
$addinFolder = ($env:APPDATA + "\Autodesk\REVIT\Addins\" + $RevitVersion)
xcopy /Y /F ($TargetPath) ($addinFolder)
xcopy /Y /F ($ProjectDir + "RevitToIFCBundle.bundle\Contents\RevitToIFCBundle.addin") ($addinFolder)

# Copy to the package folder structure
$BundleFolder = ($ProjectDir + "RevitToIFCBundle.bundle\")
xcopy /Y /F ($TargetPath) ($BundleFolder + "Contents\")


## Zip the package
$ReleasePath = $TargetDir
$ReleaseZip = ($ReleasePath + "\" + $TargetName + $RevitVersion + ".zip")

if (Test-Path $ReleaseZip) { Remove-Item $ReleaseZip }

if ( Test-Path -Path $ReleasePath ) {
  7z a -tzip $ReleaseZip $BundleFolder
}

