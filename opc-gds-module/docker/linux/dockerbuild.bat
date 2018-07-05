dotnet build Microsoft.Azure.IIoT.OpcUa.Services.Gds.Edge.csproj
dotnet publish Microsoft.Azure.IIoT.OpcUa.Services.Gds.Edge.csproj -o ./publish
docker build -t edgegds .
