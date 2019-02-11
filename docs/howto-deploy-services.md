# Build and Deploy the Azure Industrial IoT OPC UA Certificate Management Service and dependencies

This article explains how to deploy the OPC UA Certificate Management Service in Azure.

## Prerequisites

### Install required software

Currently the build and deploy operation is limited to Windows.
The samples are all written for .NetStandard, which is needed to build the service and samples for deployment.
All the tools you need for .Net Standard come with the .Net Core tools. See [here](https://docs.microsoft.com/en-us/dotnet/articles/core/getting-started) for what you need.

1. [Install .NET Core 2.1+][dotnet-install].
2. [Install Docker][docker-url] (optional, only if the local docker build is required).
4. Install the [Azure Command Line tools for PowerShell][powershell-install].
5. Sign up for an [Azure Subscription][azure-free].

### Clone the repository

If you have not done so yet, clone this Github repository.  Open a command prompt or terminal and run:

```bash
git clone https://github.com/Azure/azure-iiot-opc-vault-service
cd azure-iiot-opc-vault-service 
```

or clone the repo directly in Visual Studio 2017.

### Build and Deploy the Azure service on Windows

A Powershell script provides an easy way to deploy the OPC Vault microservice and the application.<br>

1. Open a Powershell window at the repo root. 
3. Go to the deploy folder `cd deploy`
3. Choose a name for `myResourceGroup` which is unlikely to cause a conflict with other deployed web pages. See [below](#Website-name-already-in-use) how web page names are chosen based on the name of the resource group.
5. Start the deployment with `.\deploy.ps1` for interactive installation<br>
or enter a full command line:  
`.\deploy.ps1  -subscriptionName "MySubscriptionName" -resourceGroupLocation "East US" -tenantId "myTenantId" -resourceGroupName "myResourceGroup"`
7. If you plan to develop with this deployment, add `-development 1` to enable the Swagger UI and to deploy debug builds.
6. Follow the instructions in the script to login to your subscription and to provide additional information.
9. After a successful build and deploy operation you should see the following message:

```
To access the web client go to:
https://myResourceGroup.azurewebsites.net

To access the web service go to:
https://myResourceGroup-service.azurewebsites.net

To start the local docker GDS server:
.\myResourceGroup-dockergds.cmd

To start the local dotnet GDS server:
.\myResourceGroup-gds.cmd
```
In case you run into issues please follow the steps [below](#Troubleshooting-deployment-failures).

8. Open your favorite browser and open the application page: `https://myResourceGroup-app.azurewebsites.net`
8. Give the web app and the OPC Vault microservice a few minutes to warm up after deployment. The web home page may hang on first use for up to a minute until you get the first responses.
11. To take a look at the Swagger Api open: `https://myResourceGroup-service.azurewebsites.net`
13. To start a local GDS server with dotnet start `.\myResourceGroup-gds.cmd` or with docker start `.\myResourceGroup-dockergds.cmd`.

As a sidenote, it is possible to redeploy a build with exactly the same settings. Be aware that such an operation renews all application secrets and may reset some settings in the AAD application registrations.

## [How to use the Certificate Management Service](howto-use-cert-services.md)

Please find an article describing how to use the Certificate Management Service [here](howto-use-cert-services.md).

## Delete the Certificate management services from the subscription

1. Sign in to the Azure portal: `https://portal.azure.com`.
2. Go to the resource group in which the service was deployed.
3. Select `Delete resource group` and confirm.
4. After a short while all deployed servcie components are deleted.
5. Now go to `Azure Active Directory/App registrations`.
6. There should be 3 registrations listed for each deployed resource group with the following names:
`resourcegroup-client`, `resourcegroup-module`, `resourcegroup-service`.
Each registration needs to be deleted separately.
7. Now all deployed components are removed.

## Troubleshooting deployment failures

### Resource group name

Ensure you use a short and simple resource group name.  The name is used also to name resources and the service url prefix and as such, it must comply with resource naming requirements.  

### Website name already in use

It is possible that the name of the website is already in use.  If you run into this error, you need to use a different resource group name. The hostnames in use by the deployment script are: https://resourcegroupname.azurewebsites.net and https://resourgroupname-service.azurewebsites.net.
Other names of services are built by the combination of short name hashes and are unlikely to conflict with other services.

### Azure Active Directory (AAD) Registration 

The deployment script tries to register 3 AAD applications in Azure Active Directory.  
Depending on your rights to the selected AAD tenant, this operation might fail.   There are 2 options:

1. If you chose a AAD tenant from a list of tenants, restart the script and choose a different one from the list.
2. Alternatively, deploy a private AAD tenant in another subscription, restart the script and select to use it.

## Deployment script options

The script takes the following parameters:


```
-resourceGroupName
```

Can be the name of an existing or a new resource group.

```
-subscriptionId
```


Optional, the subscription id where resources will be deployed.

```
-subscriptionName
```


Or alternatively the subscription name.

```
-resourceGroupLocation
```


Optional, a resource group location. If specified, will try to create a new resource group in this location.


```
-tenantId
```


AAD tenant to use. 

```
-development 0|1
```

Optional, to deploy for development. Use debug build and set the ASP.Net Environment to Development. Create .publishsettings for import in Visual Studio 2017 to allow to deploy the app and the service directly.

```
-onlyBuild 0|1
```

Optional, to rebuild and to redeploy only the web apps and to rebuild the docker containers.

[azure-free]:https://azure.microsoft.com/en-us/free/
[powershell-install]:https://azure.microsoft.com/en-us/downloads/#PowerShell
[docker-url]: https://www.docker.com/
[dotnet-install]: https://www.microsoft.com/net/learn/get-started

