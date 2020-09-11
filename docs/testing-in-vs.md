# Windows Forms Testing in Visual Studio

Visual Studio executes unit tests differently from the command line. This page describes some of those differences and covers common problems and workarounds.

## Assert.Same() failures

### Problem

`Assert.Same()` calls that pass when running tests from the command line can often fail when running through Visual Studio. This has to do with a quirk in the xUnit Visual Studio test adapter.

### Cause

* When using `[Theory]` combined with `[InlineData]`, `[ClassData]`, or `[MemberData]`, the xUnit VS test adapter does performs "discovery enumeration", before the test is actually executed
    * It does this to populate the Test Explorer window with all the test cases before they run
* The process running this enumeration executes in a **different App domain** than the tests themselves
* This means objects passed in to the test methods have **different references** than expected (when comparing strings, for example)

### Solution(s)

There are two ways to get around this problem:

1. Use a custom *DataAttribute (inheriting from the appropriate *DataAttributeBase class) and set `DisableDiscoveryEnumeration = true` in the constructor
    * For an example of this, see [MauiDataAttribute.cs](https://github.com/dotnet/winforms/blob/afa8a0779f261e46fe7eb1611142bea143622578/src/System.Windows.Forms/tests/IntegrationTests/System.Windows.Forms.Maui.IntegrationTests/MauiDataAttribute.cs#L42)
    * This is not really preferred, and should only be used when your test data can't be resolved at compile-time

    OR

1. Use Assert.Equal() when comparing against objects created by `[InlineData]`, `[ClassData]`, or `[MemberData]`
    * You can still use Assert.Same() in all other cases

## Tests don't execute

### Problem

* When launching tests through Visual Studio, no tests execute
* Click on `View -> Output` and select `Tests` from the `Show output from:` dropdown
* You might see an error that looks like `Testhost process exited with error: It was not possible to find any compatible framework version. The specified framework 'Microsoft.NETCore.App', version 'XXX' was not found`

### Cause (long)

* The .net core SDK and the .net core runtime (Microsoft.NetCore.App) are two different things.
* The SDK targets a specific NetCore.App. This version is automatically downloaded into your local .dotnet folder when you build from command line, and is also installed when you install a dotnet sdk machine-wide.
    * If you don’t specify an explicit NetCore.App version, you will use the one that the SDK targets.
    * If you’re interested in seeing what this version is, look at `<yourRepoRoot>\.dotnet\sdk\<yourSdkVersion>\dotnet.runtimeconfig.json`
* The winforms (runtime) repo DOES specify an explicit NetCore.App version, and we use the one published by the core-setup repo.
    * If you look at [Directory.Build.Targets](https://github.com/dotnet/winforms/blob/ac0426561b158522eb8564de2bedd28f28148f8d/Directory.Build.targets#L35), you can see where this is set 
* `$(MicrosoftNETCoreAppPackageVersion)` is set in [Versions.props](https://github.com/dotnet/winforms/blob/ac0426561b158522eb8564de2bedd28f28148f8d/eng/Versions.props#L19), and gets automatically updated when we ingest dependencies from core-setup. (These PR's are automatically merged in if the build passes)
    * You can see where the dependency comes from by looking at [Version.Details.xml](https://github.com/dotnet/winforms/blob/ac0426561b158522eb8564de2bedd28f28148f8d/eng/Version.Details.xml#L13)
    * The reason we use the version from core-setup instead of the version built-in to the SDK is we don’t want to wait for the runtime to come all the way through the SDK repo to consume it
* Running tests from the command line (.\build -test) works because the build scripts automatically download both the sdk AND the NetCore.App that you need AND add the .dotnet folder to your PATH when running.
    * You can verify this by checking the .dotnet folder in your repo root. The NetCore.App is in the shared folder.
* However, Visual Studio doesn’t care about your local .dotnet folder. It always looks at your machine-wide installs (unless you manually add things to your PATH before launching VS)
    * If you don’t have the required SDK installed machine-wide, your code won’t build (through VS). 
    * If you don’t have the required NetCore.App installed machine-wide, your code won’t run (through VS). 
* **This is why unit tests can fail immediately through VS.**
    * Note that just installing the SDK **IS NOT ENOUGH** if you are using an explicit NetCore.App version.
* Since the command line builds automatically download everything you need, we just have to tell VS where this stuff lives

### Solution(s)

There are two ways to get around this problem:

1. Add your local `<repoRoot>\.dotnet` folder to the front of your PATH variable
    * This is **not preferred** because Visual Studio will always look here first, and can break other things, like if you need to build against a higher version of the SDK

    OR

1. Close VS, run `<repoRoot>\build-local.ps1`, then re-open VS
    * This will do a restore (to download the stuff), do a build, then symlink the stuff you need from your local .dotnet folder to your machine-wide SDK install folder `(C:\Program Files\dotnet\...)`. Then VS will know about it too 😊
