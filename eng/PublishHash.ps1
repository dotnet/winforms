Install-PackageProvider -Name NuGet -MinimumVersion 2.8.5.201 -Force -Scope CurrentUser
Install-Module -Name Az -AllowClobber -Scope CurrentUser
Write-Host "Finished installing modules"

Connect-AzAccount

$location = "westus"

# put resource group in a variable so you can use the same group name going forward,
# without hardcoding it repeatedly
$resourceGroup = "storage-quickstart-resource-group"
New-AzResourceGroup -Name $resourceGroup -Location $location 

$storageAccount = New-AzStorageAccount -ResourceGroupName $resourceGroup `
  -Name "storagequickstart" `
  -Location $location `
  -SkuName Standard_LRS `
  -Kind StorageV2 

$ctx = $storageAccount.Context
$containerName = "quickstartblobs"
new-azurestoragecontainer -Name $containerName -Context $ctx -Permission blob

set-azurestorageblobcontent -File "E:\winforms\eng\PublishHash.ps1"
  -Container $containerName `
  -Blob "PublishHash.ps1" `
  -Context $ctx 