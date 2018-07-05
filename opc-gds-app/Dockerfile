FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 56310
EXPOSE 44342

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY GdsVault.App/GdsVault.App.csproj GdsVault.App/
RUN dotnet restore GdsVault.App/GdsVault.App.csproj
COPY . .
WORKDIR /src/GdsVault.App
RUN dotnet build GdsVault.App.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish GdsVault.App.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GdsVault.App.dll"]
