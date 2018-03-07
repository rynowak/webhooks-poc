# How to use
## Setup
This repo depends on the 2.1 Preview1 SDK at the moment. This can lead to errors such as
```
It was not possible to find any compatible framework version
The specified framework 'Microsoft.AspNetCore.App', version '2.1.0-preview1-final' was not found.
  - Check application dependencies and target a framework version installed at:
      C:\Users\dougbu\.dotnet\x64\
  - The .NET framework can be installed from:
      https://aka.ms/dotnet-download-runtime
  - The .NET framework and SDK can be installed from:
      http://go.microsoft.com/fwlink/?LinkID=798306&clcid=0x409
```
To avoid these issues:
1. Download the SDK installer for your platform from https://www.microsoft.com/net/download/dotnet-core/sdk-2.1.300-preview1
2. Install the SDK
3. Ensure `dotnet` is now in your path e.g. `$env:Path` should include `C:\Program Files\dotnet` on Windows

## Build and Run
The project in `webhooks-poc.sln` should just work. For example,
```
> git clean -dfx
> cd src/SampleApp
> dotnet run
```

## Browse
Default home pages are http://localhost:5000/ and https://localhost:5001/ when run from the command line.

Use the home page to add some routes - these will be mapped to the 'github' reciever. The set of routes/actions will be updated each time you submit new data.

The "Dump Routes" action just redirects back to the home page. It's useful when debugging: Set a breakpoint in the action to examine the current routes.
