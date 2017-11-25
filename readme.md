# How to use

## Build the webhooks stuff

0. Open VS Developer Command Prompt (sorry)
1. `git submodule update`
2. `cd submodules\WebHooks`
3. `.\build.cmd`
4. `pushd src\Microsoft.AspNetCore.WebHooks.Receivers`
5. `dotnet pack`
6. `popd`
7. `pushd src\Microsoft.AspNetCore.WebHooks.Receivers.Github`
8. `dotnet pack`
9. `popd`

This should put some packages in `submodules\WebHooks\bin\Debug`

## Use the project

Now the project in `webhooks-poc.sln` should just work.

Use the home page to add some routes - these will be mapped to the 'github' reciever. The set of routes/actions will be updated each time you submit new data.