:: Prepare the environment variables used by the application.

:: The port where this project's web service is listening
:: See https://github.com/Azure/azure-iot-pcs-team/wiki/Architecture-draft
SETX PCS_PROJECTNAMEHERE_WEBSERVICE_PORT "900X"

:: Some settings are used to connect to an external dependency, e.g. Azure IoT Hub and IoT Hub Manager API
:: Depending on which settings and which dependencies are needed, edit the list of variables

:: see: Shared access policies => key name => Connection string
SETX IOTHUB_CONN_STRING "..."

:: The URL where IoT Hub Manager web service is listening
SETX PCS_IOTHUBMANAGER_WEBSERVICE_URL "http://127.0.0.1:9002/v1"
