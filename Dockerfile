FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY src/Microsoft.Azure.IIoT.OpcUa.Services.Vault.csproj src/
COPY NuGet.Config NuGet.Config
RUN dotnet restore --configfile NuGet.Config -nowarn:msb3202,nu1503 src/Microsoft.Azure.IIoT.OpcUa.Services.Vault.csproj
COPY src/ src/
WORKDIR /src/src
RUN dotnet build Microsoft.Azure.IIoT.OpcUa.Services.Vault.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Microsoft.Azure.IIoT.OpcUa.Services.Vault.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Microsoft.Azure.IIoT.OpcUa.Services.Vault.dll"]
