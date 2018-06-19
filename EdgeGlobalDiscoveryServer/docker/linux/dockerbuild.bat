dotnet build EdgeGlobalDiscoveryServer.csproj
dotnet publish EdgeGlobalDiscoveryServer.csproj -o ./publish
docker build -t edgegds .
