[![Build][build-badge]][build-url]
[![Issues][issues-badge]][issues-url]
[![Gitter][gitter-badge]][gitter-url]

PROJECT NAME HERE
=================

... project description ...

Overview
========

... quick demonstrative examples ...

How to use it
=============

... steps to use this project ...
... deployment notes ...

## Running the template with Visual Studio

1. Open the solution using the `project-name-here.sln` file.
1. When the solution is loaded, right click on the `WebService` project,
   select `Properties` and go to the `Debug` section.
1. Add a new environment variable with name
   `PCS_PROJECTNAMEHERE_WEBSERVICE_PORT` and value `9001`.
1. In the same section set the `App URL` to
   `http://localhost:9001/v1/status`
1. Save the settings and Run the WebService project, the web service status
   should appear in your browser, in JSON format.

## Running the template with IntelliJ Rider

1. Open the solution using the `project-name-here.sln` file.
1. When the solution is loaded, got to `Run -> Edit Configurations` and
   create a new `.NET Project` configuration.
1. In the configuration select the WebService project
1. Add a new environment variable with name
   `PCS_PROJECTNAMEHERE_WEBSERVICE_PORT` and value `9001`.
1. Save the settings and run the configuration just created, from the IDE
   toolbar.
1. You should see the service bootstrap messages in IntelliJ Run window,
   with details such as the URL where the web service is running, plus
   the service logs.

Configuration
=============

... how to customize the service, settings, etc. ...

Special notes
=============

... technical details ...

Edge cases
==========

... things to take into consideration ...
... known limitations ...

Other documents
===============

* [Contributing and Development setup](CONTRIBUTING.md)
* [Development setup, scripts and tools](DEVELOPMENT.md)

[build-badge]: https://img.shields.io/travis/Azure/PROJECT-ID-HERE-dotnet.svg
[build-url]: https://travis-ci.org/Azure/PROJECT-ID-HERE-dotnet
[issues-badge]: https://img.shields.io/github/issues/azure/PROJECT-ID-HERE-dotnet.svg
[issues-url]: https://github.com/azure/PROJECT-ID-HERE-dotnet/issues
[gitter-badge]: https://img.shields.io/gitter/room/azure/iot-pcs.js.svg
[gitter-url]: https://gitter.im/azure/iot-pcs
