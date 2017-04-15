FROM microsoft/aspnetcore:1.1

MAINTAINER Devis Lucato (https://github.com/dluc)

EXPOSE 80

ENTRYPOINT ["dotnet", "Microsoft.Azure.IoT.WebService.dll"]

COPY . /app/
WORKDIR /app
