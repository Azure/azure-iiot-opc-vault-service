@ECHO off

SET DOCKER_IMAGE="azureiotpcs/PROJECT-NAME-HERE-dotnet:0.1-SNAPSHOT"

echo Starting web service at: http://localhost:8080

docker run -it -p 8080:8080 -e PCS_IOTHUB_CONN_STRING=%PCS_IOTHUB_CONN_STRING% %DOCKER_IMAGE%
