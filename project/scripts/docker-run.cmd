@ECHO off

echo Starting web service at: http://localhost:8080/api/values

docker run -it -p 8080:80 azureiotpcs/microservice-template-dotnet-ws:1.0-SNAPSHOT
