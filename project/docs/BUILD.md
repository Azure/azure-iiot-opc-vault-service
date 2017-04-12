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

To compile the solution

The following bash script takes care of building all the projects and running the tests:

```
./project/scripts/build.sh
```

otherwise you can do it manually:

```
cd WebService
dotnet restore
dotnet build
cd ..

cd WebService.Test
dotnet restore
dotnet build
dotnet vstest ./bin/Debug/netcoreapp1.1/WebService.Test.dll
```

### Run

To run the project from the command line:

```
cd WebService
dotnet run
```
