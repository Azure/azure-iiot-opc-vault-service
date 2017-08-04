:: Prepare the environment variables used by the application.

:: Some settings are used to connect to an external dependency, e.g. Azure IoT Hub and IoT Hub Manager API
:: Depending on which settings and which dependencies are needed, edit the list of variables

:: see: Shared access policies => key name => Connection string
SETX IOTHUB_CONNSTRING "..."

:: The URL where IoT Hub Manager web service is listening
SETX PCS_IOTHUBMANAGER_WEBSERVICE_URL "http://127.0.0.1:9002/v1"
