@ECHO off
setlocal enableextensions enabledelayedexpansion

IF "%PCS_PROJECTNAMEHERE_WEBSERVICE_PORT%" == "" (
    echo Error: the PCS_PROJECTNAMEHERE_WEBSERVICE_PORT environment variable is not defined.
    exit /B 1
)

:: Some settings are used to connect to an external dependency, e.g. Azure IoT Hub and IoT Hub Manager API
:: Depending on which settings and which dependencies are needed, edit the list of variables checked

IF "%PCS_IOTHUB_CONN_STRING%" == "" (
    echo Error: the PCS_IOTHUB_CONN_STRING environment variable is not defined.
    exit /B 1
)

IF "%PCS_IOTHUBMANAGER_WEBSERVICE_HOST%" == "" (
    echo Error: the PCS_IOTHUBMANAGER_WEBSERVICE_HOST environment variable is not defined.
    exit /B 1
)

IF "%PCS_IOTHUBMANAGER_WEBSERVICE_PORT%" == "" (
    echo Error: the PCS_IOTHUBMANAGER_WEBSERVICE_PORT environment variable is not defined.
    exit /B 1
)

endlocal
