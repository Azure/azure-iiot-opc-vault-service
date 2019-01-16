# Azure Industrial IoT Services

### OPC Unified Architecture (OPC UA) Certificate Management Service

The certificate management service for OPC UA facilitates a CA certificate cloud service for OPC UA devices
based on Azure Key Vault and CosmosDB, a ASP.Net Core web application front end and a OPC UA GDS server based on .Net Standard.
The implementation follows the GDS Certificate Management Services as described in the OPC UA specification Part 12.
The CA certificates are stored in a HSM backed Azure Key Vault, which is also used to sign issued certificates. 
A web management application front end and a local OPC UA GDS server allow for easy connection to the services secured by Azure AD.

### This repository contains the following:

* **ASP.Net Core Certificate Management Service** to manage certificates with Azure Key Vault and CosmosDB.
* **ASP.Net Core Sample Application** as user interface for the Certificate Management Service.
* **OPC UA .Net Standard GDS Server** for local OPC UA device connectivity to the cloud Certificate Management Service.

## Prerequisites

### Build and Deploy the Azure service on Windows

This Powershell script provides an easy way to deploy the OPC UA Vault service and the application.<br>

1. Sign up for an [Azure Subscription][azure-free].
1. [Install .NET Core 2.1+][dotnet-install].
2. [Install Docker][docker-url].
4. Install the [Azure Command Line tools for PowerShell][powershell-install].
1. Open a Powershell window at the repo root. 
3. Go to the deploy folder `cd deploy`
5. Start the deployment with `.\deploy.ps1` for interactive installation<br>
or enter a full command line:  
`.\deploy.ps1  -subscriptionName "MySubscriptionName" -resourceGroupLocation "East US" -tenantId <myTenantId> -resourceGroupName <myResourceGroup>`
6. Follow the instructions in the script to login to your subscription and to provide additional information
9. After a successful build and deploy you should see the following message:

```
To access the web client go to:
https://myResourceGroup-app.azurewebsites.net

To access the web service go to:
https://myResourceGroup-service.azurewebsites.net

To start the local docker GDS server:
.\myResourceGroup-dockergds.cmd

To start the local dotnet GDS server:
.\myResourceGroup-gds.cmd
```

10. Give the web app and the web service a few minutes to start up for the first time.
10. Open your favorite browser and open the application page: `https://myResourceGroup-app.azurewebsites.net`
11. To take a look at the Swagger Api open: `https://myResourceGroup-service.azurewebsites.net`
13. To start a local GDS server with dotnet start `.\myResourceGroup-gds.cmd` or with docker start `.\myResourceGroup-dockergds.cmd`.

### Issue the first CA certificate

1. Open your certificate service at `https://myResourceGroup-app.azurewebsites.net` and login.
2. Navigate to the `Certificate Groups` page.
3. There is one `Default` Certificate Group listed. Click on `Edit`.
4. In `Edit Certificate Group Details` you can modify the Subject Name and Lifetime of your CA and application certificates.
5. Enter a valid Subject in the valid, e.g. `CN=My CA Root, O=MyCompany, OU=MyDepartment`.
6. Click on the `Save` button.
7. Click on the `Details` button. The `View Certificate Group Details` should display the updated information.
8. Click on the `Renew CA Certificate` button to issue your first CA certificate. Press `Ok` to proceed.
9. After a few seconds the the `Certificate Details` are shown. Press `Issuer` or `Crl` to download the latest CA certificate and CRL.
10. Now the OPC UA Certificate Management Service is ready to issue certificates for OPC UA applications.

## Build and Run(TODO)

### Building and running the service with Visual Studio or VS Code(TODO)

1. Make sure the [Prerequisites](#Prerequisites) are set up.
1. [Install .NET Core 2.1+][dotnet-install]
1. Install any recent edition of Visual Studio (Windows/MacOS) or Visual Studio Code (Windows/MacOS/Linux).
   * If you already have Visual Studio installed, then ensure you have [.NET Core Tools for Visual Studio 2017][dotnetcore-tools-url] installed (Windows only).
   * If you already have VS Code installed, then ensure you have the [C# for Visual Studio Code (powered by OmniSharp)][omnisharp-url] extension installed.
1. Open the solution in Visual Studio or VS Code
1. Start the `Microsoft.Azure.IIoT.OpcUa.Services.Twin` project (e.g. press F5).
1. Open a browser to `http://localhost:9041/` and test the service using the services' Swagger UI or the [OPC Twin CLI](https://github.com/Azure/azure-iiot-opc-twin-api).

### Building and running the service on the command line(TODO)

1. Make sure the [Prerequisites](#Prerequisites) are set up.
1. [Install .NET Core 2.1+][dotnet-install]
1. Open a terminal window or command line window at the repo root. 
1. Set the [required environment variables](#Setup-Environment-variables) as explained [here](#Configuration-And-Environment-Variables)
1. Run the following command:
    ```bash
    cd src
    dotnet run
    ```
1. Open a browser to `http://localhost:9041/` and test the service using the services' Swagger UI or the [OPC Twin CLI](https://github.com/Azure/azure-iiot-opc-twin-api).

### Building and running the service using Docker(TODO)

1. Make sure [Docker][docker-url] is installed.
1. Make sure the [Prerequisites](#prerequisites) are set up.
1. Set the [required environment variables](#Setup-Environment-variables) as explained [here](#Configuration-And-Environment-Variables)
1. Change into the repo root and build the docker image using `docker build -t azure-iiot-opc-twin-service .`
1. To run the image run `docker run -p 9041:9041 -e _HUB_CS=$PCS_IOTHUB_CONNSTRING -it azure-iiot-opc-twin-service` (or `docker run -p 9041:9041 -e _HUB_CS=%PCS_IOTHUB_CONNSTRING% -it azure-iiot-opc-twin-service` on Windows).
1. Open a browser to `http://localhost:9041/` and test the service using the services' Swagger UI or the [OPC Twin CLI](https://github.com/Azure/azure-iiot-opc-twin-api).

### Configuration and Environment variables(TODO)

The service can be configured in its [appsettings.json](src/appsettings.json) file.  Alternatively, all configuration can be overridden on the command line, or through environment variables.  If you have deployed the dependent services using the [pcs local][deploy-local] command, make sure the environment variables shown at the end of deployment are all set in your environment.

* [This page][windows-envvars-howto-url] describes how to setup env vars in Windows.
* For Linux and MacOS, we suggest to create a shell script to set up the environment variables each time before starting the service host (e.g. VS Code or docker). Depending on OS and terminal, there are ways to persist values globally, for more information [this](https://stackoverflow.com/questions/13046624/how-to-permanently-export-a-variable-in-linux), [this](https://help.ubuntu.com/community/EnvironmentVariables), or [this](https://stackoverflow.com/questions/135688/setting-environment-variables-in-os-x) page should help.

> Make sure to restart your editor or IDE after setting your environment variables to ensure they are picked up.

## Other Industrial IoT Solution Accelerator components(TODO)

* OPC GDS Vault service (Coming soon)
* [OPC Twin Registry service](https://github.com/Azure/azure-iiot-opc-twin-registry)
* [OPC Twin Onboarding service](https://github.com/Azure/azure-iiot-opc-twin-onboarding)
* OPC Twin common business logic (Coming soon)
* [OPC Twin IoT Edge module](https://github.com/Azure/azure-iiot-opc-twin-module)
* [OPC Publisher IoT Edge module](https://github.com/Azure/iot-edge-opc-publisher)
* [OPC Twin API](https://github.com/Azure/azure-iiot-opc-twin-api)

# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

### Give Feedback

Please enter issues, bugs, or suggestions for any of the components and services as GitHub Issues [here](https://github.com/Azure/azure-iiot-opcvault/issues).

### Contribute

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct).  For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

If you want/plan to contribute, we ask you to sign a [CLA](https://cla.microsoft.com/) (Contribution License Agreement) and follow the project 's [code submission guidelines](docs/contributing.md). A friendly bot will remind you about it when you submit a pull-request. ​ 

## License

Copyright (c) Microsoft Corporation. All rights reserved.
Licensed under the [MIT](LICENSE) License.  

[azure-free]:https://azure.microsoft.com/en-us/free/
[powershell-install]:https://azure.microsoft.com/en-us/downloads/#PowerShell

[run-with-docker-url]: https://docs.microsoft.com/azure/iot-suite/iot-suite-remote-monitoring-deploy-local#run-the-microservices-in-docker
[rm-arch-url]: https://docs.microsoft.com/azure/iot-suite/iot-suite-remote-monitoring-sample-walkthrough
[postman-url]: https://www.getpostman.com
[iotedge-url]: https://github.com/Azure/iotedge
[iothub-docs-url]: https://docs.microsoft.com/azure/iot-hub/
[docker-url]: https://www.docker.com/
[dotnet-install]: https://www.microsoft.com/net/learn/get-started
[vs-install-url]: https://www.visualstudio.com/downloads
[dotnetcore-tools-url]: https://www.microsoft.com/net/core#windowsvs2017
[omnisharp-url]: https://github.com/OmniSharp/omnisharp-vscode
[windows-envvars-howto-url]: https://superuser.com/questions/949560/how-do-i-set-system-environment-variables-in-windows-10
[iothub-connstring-blog]: https://blogs.msdn.microsoft.com/iotdev/2017/05/09/understand-different-connection-strings-in-azure-iot-hub/
[deploy-rm]: https://docs.microsoft.com/azure/iot-suite/iot-suite-remote-monitoring-deploy
[deploy-local]: https://docs.microsoft.com/azure/iot-suite/iot-suite-remote-monitoring-deploy-local#deploy-the-azure-services
[disable-auth]: https://github.com/Azure/azure-iot-pcs-remote-monitoring-dotnet/wiki/Developer-Reference-Guide#disable-authentication


