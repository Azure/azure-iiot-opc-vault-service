Build container image
=====================

To build the Docker image and publish it to your local repository:

```
project/scripts/docker-build
```

This will generate a new image with name `azureiotpcs/microservice-template-dotnet-ws`.

Deploy with Docker
==================

To run the container

```
docker run -it -p 8080:80 azureiotpcs/microservice-template-dotnet-ws:1.0-SNAPSHOT
```

or

```
project/scripts/docker-run
```

You can test the web service opening your browser at 

> http://localhost:8080/api/values

Publish to Docker Hub
=====================

Our public repository: https://hub.docker.com/u/azureiotpcs

```
docker login -u azureiotpcs
```

Password: *****

```
docker push azureiotpcs/microservice-template-dotnet-ws
```

You can also work with your own repository, for instance if your docker hub account is "foo":

```
docker tag <image id> foo/microservice-template-dotnet-ws
docker login -u foo
docker push foo/microservice-template-dotnet-ws
```
