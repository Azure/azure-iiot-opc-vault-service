<#
 .SYNOPSIS
    Deploys to Azure

 .DESCRIPTION
    Deploys an Azure Resource Manager template of choice

 .PARAMETER resourceGroupName
    The resource group where the template will be deployed.

 .PARAMETER webAppName
    The host name prefix of the web application. 

 .PARAMETER webServiceName
    The host name prefix of the web application. 

 .PARAMETER aadConfig
    The AAD configuration the template will be configured with.

 .PARAMETER interactive
    Whether to run in interactive mode

#>

param(
    [Parameter(Mandatory=$True)] [string] $resourceGroupName,
    [string] $webAppName = $null,
    [string] $webServiceName = $null,
    $aadConfig = $null,
    $interactive = $true
)

#******************************************************************************
# Generate a random password
#******************************************************************************
Function CreateRandomPassword() {
    param(
        $length = 15
    ) 
    $punc = 46..46
    $digits = 48..57
    $lcLetters = 65..90
    $ucLetters = 97..122

    $password = `
        [char](Get-Random -Count 1 -InputObject ($lcLetters)) + `
        [char](Get-Random -Count 1 -InputObject ($ucLetters)) + `
        [char](Get-Random -Count 1 -InputObject ($digits)) + `
        [char](Get-Random -Count 1 -InputObject ($punc))
    $password += get-random -Count ($length -4) `
        -InputObject ($punc + $digits + $lcLetters + $ucLetters) |`
         % -begin { $aa = $null } -process {$aa += [char]$_} -end {$aa}

    return $password
}

#******************************************************************************
# Script body
#******************************************************************************
$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path $script:MyInvocation.MyCommand.Path

# Register RPs
Register-AzureRmResourceProvider -ProviderNamespace "microsoft.web" | Out-Null
#Register-AzureRmResourceProvider -ProviderNamespace "microsoft.compute" | Out-Null

# Set admin password
$adminPassword = CreateRandomPassword
$templateParameters = @{ 
    # adminPassword = $adminPassword
}

try {
    # Try set branch name as current branch
    $output = cmd /c "git rev-parse --abbrev-ref HEAD" 2`>`&1
    $branchName = ("{0}" -f $output);
    Write-Host "Deployment will use configuration from '$branchName' branch."
    # $templateParameters.Add("branchName", $branchName)
}
catch {
    # $templateParameters.Add("branchName", "master")
}

# Configure auth
if ($aadConfig) {
    if (![string]::IsNullOrEmpty($aadConfig.Audience)) { 
        $templateParameters.Add("aadAudience", $aadConfig.Audience)
    }
    if (![string]::IsNullOrEmpty($aadConfig.ServiceId)) { 
        $templateParameters.Add("aadServiceId", $aadConfig.ServiceId)
    }
    if (![string]::IsNullOrEmpty($aadConfig.ServiceObjectId)) { 
        $templateParameters.Add("aadServiceObjectId", $aadConfig.ServiceObjectId)
    }
    if (![string]::IsNullOrEmpty($aadConfig.ServiceSecret)) { 
        $templateParameters.Add("aadServiceSecret", $aadConfig.ServiceSecret)
    }
    if (![string]::IsNullOrEmpty($aadConfig.ClientId)) { 
        $templateParameters.Add("aadClientId", $aadConfig.ClientId)
    }
    if (![string]::IsNullOrEmpty($aadConfig.ClientSecret)) { 
        $templateParameters.Add("aadClientSecret", $aadConfig.ClientSecret)
    }
    if (![string]::IsNullOrEmpty($aadConfig.ModuleId)) { 
        $templateParameters.Add("aadModuleId", $aadConfig.ModuleId)
    }
    if (![string]::IsNullOrEmpty($aadConfig.ModuleSecret)) { 
        $templateParameters.Add("aadModuleSecret", $aadConfig.ModuleSecret)
    }
    if (![string]::IsNullOrEmpty($aadConfig.TenantId)) { 
        $templateParameters.Add("aadTenantId", $aadConfig.TenantId)
    }
    if (![string]::IsNullOrEmpty($aadConfig.Instance)) { 
        $templateParameters.Add("aadInstance", $aadConfig.Instance)
    }
    if (![string]::IsNullOrEmpty($aadConfig.UserPrincipalId)) { 
        $templateParameters.Add("aadUserPrincipalId", $aadConfig.UserPrincipalId)
    }
}


# Set web app site name
if ($interactive -and [string]::IsNullOrEmpty($webAppName)) {
    $webAppName = Read-Host "Please specify a web applications site name"
}

if (![string]::IsNullOrEmpty($webAppName)) { 
    $templateParameters.Add("webAppName", $webAppName)
}

# Set web service site name
if ($interactive -and [string]::IsNullOrEmpty($webServiceName)) {
    $webServiceName = Read-Host "Please specify a web service site name"
}

if (![string]::IsNullOrEmpty($webServiceName)) { 
    $templateParameters.Add("webServiceName", $webServiceName)
}


# Start the deployment
$templateFilePath = Join-Path $ScriptDir "template.json"
Write-Host "Starting deployment..."
$deployment = New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName `
    -TemplateFile $templateFilePath -TemplateParameterObject $templateParameters

$webAppPortalUrl = $deployment.Outputs["webAppPortalUrl"].Value
$webAppServiceUrl = $deployment.Outputs["webAppServiceUrl"].Value
$webAppPortalName = $deployment.Outputs["webAppPortalName"].Value
$webAppServiceName = $deployment.Outputs["webAppServiceName"].Value
#$adminUser = $deployment.Outputs["adminUsername"].Value

if ($aadConfig -and $aadConfig.ClientObjectId) {
    # 
    # Update client application to add reply urls required permissions.
    #
    $adClient = Get-AzureADApplication -ObjectId $aadConfig.ClientObjectId 
    Write-Host "Adding ReplyUrls:"
    #$replyUrls = New-Object System.Collections.Generic.List[System.String]
    $replyUrls = $adClient.ReplyUrls
    $replyUrls.Add($webAppPortalUrl + "/signin-oidc")
    $replyUrls.Add($webAppServiceUrl + "/oauth2-redirect.html")
    Write-Host $webAppPortalUrl"/signin-oidc"
    Write-Host $webAppServiceUrl"/oauth2-redirect.html"
    Set-AzureADApplication -ObjectId $aadConfig.ClientObjectId -ReplyUrls $replyUrls -HomePage $webAppPortalUrl
}

if ($aadConfig -and $aadConfig.ClientObjectId) {
    Set-AzureADApplication -ObjectId $aadConfig.ServiceObjectId -HomePage $webServicePortalUrl
}

Write-Host
Write-Host "To access the web client go to:"
Write-Host $webAppPortalUrl
Write-Host
Write-Host "To access the web service go to:"
Write-Host $webAppServiceUrl
Write-Host
#Write-Host "Use the following User and Password to log onto your VM:"
#Write-Host 
#Write-Host $adminUser
#Write-Host $adminPassword
#Write-Host 
