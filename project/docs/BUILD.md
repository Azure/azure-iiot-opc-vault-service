Build and Run
=============

The project workflow is managed via .NET Core. To install it and follow the instructions
available here:

* Windows: https://www.microsoft.com/net/core#windowsvs2017
* Linux: https://www.microsoft.com/net/core#linuxubuntu
* MacOS: https://www.microsoft.com/net/core#macos

Once installed, you should have a new `dotnet` command available in the console. You can
find information about .NET Core CLI 
[here](https://docs.microsoft.com/en-us/dotnet/articles/core/tools/index). 

### Compile

The following script takes care of building all the projects and running the tests:

```
cd PROJECT-FOLDER
cd project/scripts
build
```

otherwise you can do it manually:

```
dotnet restore *.sln
dotnet build *.sln
dotnet test Services.Test/Services.Test.csproj
dotnet test WebService.Test/WebService.Test.csproj
```

### Run

To run the project from the command line:

```
cd WebService
dotnet run
```
