# How to use
## Setup
This repo depends on preview ASP.NET Core builds at the moment. This can lead to errors such as
```
It was not possible to find any compatible framework version
The specified framework 'Microsoft.AspNetCore.App', version '2.1.0-preview2-30024' was not found.
  - Check application dependencies and target a framework version installed at:
      C:\Users\dougbu\.dotnet\x64\
  - The .NET framework can be installed from:
      https://aka.ms/dotnet-download-runtime
  - The .NET framework and SDK can be installed from:
      http://go.microsoft.com/fwlink/?LinkID=798306&clcid=0x409
```
One way to avoid these issues is to build one of the ASP.NET Core repos locally:
```
> git clone git@github.com:aspnet/WebHooks.git # or whatever
> cd HttpAbstractions
> git checkout release/2.1
> ./build.cmd /t:UpgradeDependencies # optional; to get a more recent set of consistent versions
> ./build.cmd # Ensure the required target framework runtime is installed
> cd ../webhooks-poc/src/SampleApp

> # Edit SampleApp.csproj to match the versions in HttpAbstractions/build/dependencies.props
> # Use $(MicrosoftNETCoreApp21PackageVersion) value for $(RuntimeFrameworkVersion) in SampleApp.csproj

> # Add $env:UserProfile/.dotnet/x64 to $env:Path and remove 'C:\Program Files\dotnet\`
```
The above will not be necessary once ASP.NET Core 2.1 Preview1 is available on NuGet.org and this sample uses those packages.

## Build and Run
The project in `webhooks-poc.sln` should just work. For example,
```
> git clean -dfx
> dotnet run
```

## Browse
Default home pages are http://localhost:5000/ and https://localhost:5001/ when run from the command line.

Use the home page to add some routes - these will be mapped to the 'github' reciever. The set of routes/actions will be updated each time you submit new data.
