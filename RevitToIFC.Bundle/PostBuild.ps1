﻿param ($Configuration, $TargetName, $ProjectDir, $TargetPath, $TargetDir)
write-host $Configuration
write-host $TargetName
write-host $ProjectDir
write-host $TargetPath
write-host $TargetDir

# sign the dll
$thumbPrint = "e729567d4e9be8ffca04179e3375b7669bccf272"
$cert=Get-ChildItem -Path Cert:\CurrentUser\My -CodeSigningCert | Where { $_.Thumbprint -eq $thumbPrint}

Set-AuthenticodeSignature -FilePath $TargetPath -Certificate $cert -IncludeChain All -TimestampServer "http://timestamp.comodoca.com/authenticode"

# Copy to Addin folder for debug
$addinFolder = ($env:APPDATA + "\Autodesk\REVIT\Addins\2024")
CopyToFolder $revitVersion $addinFolder

# Copy to the package folder structure
$BundleFolder = ($ProjectDir + "RevitToIFCBundle.bundle\")
xcopy /Y /F ($TargetDir + "*.dll") ($BundleFolder + "Contents\RevitToIFCBundle")
xcopy /Y /F ($ProjectDir + "*.addin") ($BundleFolder + "Contents\")


## Zip the package
$ReleasePath = $ProjectDir
$ReleaseZip = ($ReleasePath + "\" + $TargetName + ".zip")

if (Test-Path $ReleaseZip) { Remove-Item $ReleaseZip }

if ( Test-Path -Path $ReleasePath ) {
  7z a -tzip $ReleaseZip $BundleFolder
}

