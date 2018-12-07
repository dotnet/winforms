# Windows Forms Unit Testing

This document describes our approach to unit testing.

We are _still working on_ a scalable solution for functional testing. For now, see [Functional Testing](testing.md#functional-testing) and the [issue #183](https://github.com/dotnet/winforms/issues/183).

## Building tests

Tests are automatically built when running `.\build` since all test projects are referenced in `Winforms.sln` at the repo root.

## Running tests

### Testing from the command line

To execute unit tests, run `.\build -test`

If all the tests are successful, you should see something like this:

```console
  Running tests: E:\src\repos\github\winforms\artifacts\bin\System.Windows.Forms.Tests\Debug\netcoreapp3.0\System.Windows.Forms.Tests.dll [netcoreapp3.0|x64]
  Tests succeeded: E:\src\repos\github\winforms\artifacts\bin\System.Windows.Forms.Tests\Debug\netcoreapp3.0\System.Windows.Forms.Tests.dll [netcoreapp3.0|x64]

Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### Troubleshooting command-line test errors

When testing from the command line, a failed test should look something like this:

```console
Running tests: E:\src\repos\github\winforms\artifacts\bin\System.Windows.Forms.Tests\Debug\netcoreapp3.0\System.Windows.Forms.Tests.dll [netcoreapp3.0|x64]
XUnit : error : Tests failed: E:\src\repos\github\winforms\artifacts\TestResults\Debug\System.Windows.Forms.Tests_netcoreapp3.0_x64.html [netcoreapp3.0|x64] [E:\src\repos\github\winforms\src\System.Windows.Forms\tests\UnitTests\System.Windows.Forms.Tests.csproj]
XUnit : error : Tests failed: E:\src\repos\github\winforms\artifacts\TestResults\Debug\System.Windows.Forms.Tests_netcoreapp3.0_x64.html [netcoreapp3.0|x64] [E:\src\repos\github\winforms\src\System.Windows.Forms\tests\UnitTests\System.Windows.Forms.Tests.csproj]

Build FAILED.
```

* The test summary can be found under artifacts\log
* To see the actual test(s) that failed, along with their error message(s), open the .html file that is displayed in the error message (which is always under `artifacts\TestResults`)

### Testing from Visual Studio

To test from Visual Studio, open Winforms.sln in Visual Studio and test how you normally would (using the Test Explorer, for example)

### Troubleshooting Visual Studio test errors

* When testing from Visual Studio, test errors show up as normal in the test explorer.
* To troubleshoot, debug the selected test and set breakpoints as you normally would.

## Unit Testing

### Adding new unit tests

Tests are built and executed by file name convention

* Every WinForms binary has its own folder under src in the repo root (src\System.Windows.Forms, for example)
* Each of those folders has a tests folder under it (src\System.Windows.Forms\tests, for example)
* Each tests folder contains an xUnit test project (System.Windows.Forms.Tests.csproj)
  * These test projects automatically build when running .\build
  * The tests from these projects automatically execute when running .\build -test

#### Therefore, you just need to put your tests in the right place in order for them to run

* Browse to the tests folder for the binary you are testing
* There should be one file per class being tested, and the file name should match the class name.
  * For example, if I wanted to test the Button class in System.Windows.Forms.dll, I would look for a Button.cs under src\System.Windows.Forms\tests
* If the file exists, add your tests there. If it doesn't exist, feel free to create it.
  * **Note that you don't have to modify the csproj at all.** Since the project is a Microsoft.NET.Sdk project, all source files next to it are automatically included

### Unit Test best practices

#### Naming

* Test files names should match the class they are testing
  * For example, tests for the Button class should be in Button.cs
* Test class names should match the class they are testing, followed by "Tests"
  * For example, tests for the Button class should in the ButtonTests class
* Test names should start with the class they are testing
  * For example, all tests for the button class should start with "Button"
* Test names should end with a description of what the test does
  * For example, Button_AutoSizeModeGetSet
  * This is very useful when viewing test results, and when browsing in the test explorer

#### Strategy

* **Unit tests should be part of the same PR as code changes**
  * Unit tests must be added for any change to public APIs. We will accept unit tests for internal methods as well. 
* **Code Coverage**
  * In Visual Studio Test Explorer, select all tests, right click and execute 'Analyze code coverage for selected tests' command. This will run all tests and give a summary of blocks covered in 'Code Coverage Results' window. The summary can be drilled down to method level.   
  * Any code change accompanied with unit tests is expected to increase code coverage for the code modified. 
* Avoid duplicating tests just for different inputs
  * Use `[Theory]` for this, followed by either `[InlineData]` or `[MemberData]`. See existing tests for examples on how to use these attributes
  * The exception to this is if the code behavior is fundamentally different based on the inputs. For example, if a method throws an ArgumentException for invalid inputs, that should be a separate test.
* One test (or test data) per code path please
  * The most common exception to this is when testing a property, most people test get/set together
* Whenever possible, mock up dependencies to run tests in isolation
  * For example, if your method accepts an abstraction, use Moq to mock it up
  * Search for Mock in the existing tests for examples, and see [Moq](https://github.com/Moq/moq4/wiki/Quickstart) for details on how to use Moq.

## Functional Testing

Currently, there is a single functional test in the repository: the WinformsControlsTest

### Running the application

In the console, run the following command from the base of the repository:

```cmd
.\.dotnet\dotnet.exe .\artifacts\bin\WinformsControlsTest\Debug\netcoreapp3.0\WinformsControlsTest.dll
```

**Note:** that this will fail if the WinformsControlsTest is not built. See [Build](testing.md) for more information on how to build from source.

### The test runner

This runner will open the application and all single-depth dialogs with the SendKeys API; reporting whether or not it was possible to do so.

#### Running

The runner batch (`run_individual_exe.bat`) and accompanying powershell (`run_individual_exe.ps1`) script can be found in:

`...\winforms\src\System.Windows.Forms\tests\FuncTests\Runner_WinformsControlsTest`

To run them, execute the following command the Runner_WinformsControlsTest directory:

`run_individual_exe.bat`

#### Interpreting results

The execution of that command will return a 0 if all tests passed and a -1 if even one test failed. To look at _slightly_ more in-depth results, see the `results.log` file generated alongside the .bat and .ps1 files. An example set of entries in that file follows:

```txt
12/6/2018 9:41 AM: ***************************
12/6/2018 9:41 AM: Overall Form Open passed.
12/6/2018 9:41 AM: Buttons passed.
12/6/2018 9:41 AM: Calendar passed.
12/6/2018 9:41 AM: TreeView, ImageList failed.
12/6/2018 9:41 AM: Content alignment passed.
12/6/2018 9:41 AM: Multiple controls passed.
12/6/2018 9:41 AM: DataGridView passed.
12/6/2018 9:41 AM: Menus failed.
12/6/2018 9:41 AM: Panels passed.
12/6/2018 9:42 AM: Splitter passed.
12/6/2018 9:42 AM: ComboBoxes passed.
12/6/2018 9:42 AM: MDI Parent failed.
12/6/2018 9:42 AM: Property Grid passed.
12/6/2018 9:42 AM: ListView failed.
12/6/2018 9:42 AM: DateTimePickerButton passed.
12/6/2018 9:42 AM: FolderBrowserDialogButton passed.
```
