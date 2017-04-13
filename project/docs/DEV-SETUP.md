Development configuration for developers
========================================

If you plan to work on this project, to contribute new features, bug fixes or development, please
follow the steps below to setup your workstation.

Git setup
=========

The project includes some Git hooks, to automate some checks before accepting a code change.

To setup Git hooks from a Bash terminal:

```
cd PROJECT-FOLDER
./project/scripts/git/setup.sh
```

and from Powershell:

```
cd PROJECT-FOLDER
./project/scripts/git/setup.ps1
```

.NET setup
==========

1. Install [.NET Core](https://dotnet.github.io/)
2. Use your preferred IDE, here's some options:
   * [Visual Studio](https://www.visualstudio.com/)
   * [IntelliJ Rider](https://www.jetbrains.com/rider) 
   * [Visual Studio Code](https://code.visualstudio.com/)
   * [Visual Studio for Mac](https://www.visualstudio.com/vs/visual-studio-mac)
   
Some scripts are included for common steps:

* Build: `./project/scripts/build.sh`
* Run: @TODO
* Create Docker container: @TODO

IoT Hub setup
=============

At some point you will probably want to setup your Azure IoT Hub, for development and integration tests.

The project includes some scripts to help you:

* Create new IoT Hub: `./project/iothub/create-hub.sh`
* List existing hubs: `./project/iothub/list-hubs.sh`
* Show IoT Hub details (e.g. keys): `./project/iothub/show-hub.sh`

and in case you had multiple Azure subscriptions:

* Show subscriptions list: `./project/iothub/list-subscriptions.sh`
* Change current subscription: `./project/iothub/select-subscription.sh`

Other documents
===============

* [Continuous Integration](CI.md)
* [Build and Run](BUILD.md)
