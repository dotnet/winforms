# Intellisense XML Incorporation into Ref-Pack

Intellisense XML's are produced in the `dotnet/dotnet-api-docs` repo. They now produce a nuget package on an internal feed (https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet5-transport/nuget/v3/index.json) that we can consume normally.

To update the version of docs included in the WinForms package, change the `MicrosoftPrivateIntellisenseVersion` in `Versions.props`. The version number is usually provided to us by the docs team.

If you have a link to the api-docs build and you don't know the package version, here's how you find it:

1. Browse to the build (https://apidrop.visualstudio.com/Content%20CI/_build/results?buildId=167131&view=results, for example)
1. Click on the `Phase 1` job to bring up all the steps and logs.
1. Click on the `NuGet push` step
1. Look for a line like this: `Pushing Microsoft.Private.Intellisense.5.0.0-preview-20201009.2.nupkg to ...`
   * In this case, the version would be `5.0.0-preview-20201009.2`