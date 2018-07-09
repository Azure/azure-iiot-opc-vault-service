rem start docker with mapped logs
rem push image: docker push mregen/edgegds:latest
docker run -it -p 58850-58852:58850-58852 -e 58850-58852 -h edgegds -v "/c/GDS:/root/.local/share/Microsoft/GDS" cosmosgds:latest --g http://iopgds.azurewebsites.net/ -c https://iopgds.documents.azure.com -k <add connection string here>"
