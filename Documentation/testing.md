# Windows Forms Testing

This document describes our approach to testing.

We are _still working on_ a scalable solution for functional testing. For now, see [Functional Testing](testing.md#functional-testing) and the [issue #183][issue-#183].

## Building tests

Tests are automatically built when running `.\build` since all test projects are referenced in `Winforms.sln` at the repository root.

## Running unit tests

### Unit testing from the command line

To execute unit tests, run `.\build -test`

If all the tests are successful, you should see something like this:

```console
  Running tests: E:\src\repos\github\winforms\artifacts\bin\System.Windows.Forms.Tests\Debug\netcoreapp5.0\System.Windows.Forms.Tests.dll [netcoreapp5.0|x64]
  Tests succeeded: E:\src\repos\github\winforms\artifacts\bin\System.Windows.Forms.Tests\Debug\netcoreapp5.0\System.Windows.Forms.Tests.dll [netcoreapp5.0|x64]

Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### Troubleshooting command-line unit test errors

When testing from the command line, a failed test should look something like this:

```console
Running tests: E:\src\repos\github\winforms\artifacts\bin\System.Windows.Forms.Tests\Debug\netcoreapp5.0\System.Windows.Forms.Tests.dll [netcoreapp5.0|x64]
XUnit : error : Tests failed: E:\src\repos\github\winforms\artifacts\TestResults\Debug\System.Windows.Forms.Tests_netcoreapp5.0_x64.html [netcoreapp5.0|x64] [E:\src\repos\github\winforms\src\System.Windows.Forms\tests\UnitTests\System.Windows.Forms.Tests.csproj]
XUnit : error : Tests failed: E:\src\repos\github\winforms\artifacts\TestResults\Debug\System.Windows.Forms.Tests_netcoreapp5.0_x64.html [netcoreapp5.0|x64] [E:\src\repos\github\winforms\src\System.Windows.Forms\tests\UnitTests\System.Windows.Forms.Tests.csproj]

Build FAILED.
```

* The test summary can be found under artifacts\log
* To see the actual test(s) that failed, along with their error message(s), open the .html file that is displayed in the error message (which is always under `artifacts\TestResults`)

### Unit testing from Visual Studio

To test from Visual Studio, open Winforms.sln in Visual Studio and test how you normally would (using the Test Explorer, for example)

### Troubleshooting Visual Studio unit test errors

* When testing from Visual Studio, test errors show up as normal in the Test Explorer.
* To troubleshoot, debug the selected test and set breakpoints as you normally would.
* For common issues when running tests through Visual Studio, see [Testing in Visual Studio](https://github.com/dotnet/winforms/blob/master/Documentation/testing-in-vs.md)

## Adding new unit tests

Tests are built and executed by file name convention

* Every WinForms binary has its own folder under src in the repository root (src\System.Windows.Forms, for example)
* Each of those folders has a tests folder under it (src\System.Windows.Forms\tests, for example)
* Each tests folder contains an xUnit test project (System.Windows.Forms.Tests.csproj)
  * These test projects automatically build when running `.\build`
  * The tests from these projects automatically execute when running `.\build -test`

### Therefore, you just need to put your tests in the right place in order for them to run

* Browse to the tests folder for the binary you are testing
* There should be one file per class being tested, and the file name should match the class name followed by a "Tests" suffix.
  * For example, if I wanted to test the `Button` class in System.Windows.Forms.dll, I would look for a ButtonTests.cs under src\System.Windows.Forms\tests
* If the file exists, add your tests there. If it doesn't exist, feel free to create it.
  * **Note that you don't have to modify the csproj at all.** Since the project is a Microsoft.NET.Sdk project, all source files next to it are automatically included

### Unit Test best practices

#### Naming

* Test files names should match the class they are testing followed by a "Tests" suffix
  * For example, tests for the `Button` class should be in ButtonTests.cs
* Test class names should match the class they are testing, followed by a "Tests" suffix
  * For example, tests for the `Button` class should in the `ButtonTests` class
* Test names should start with the class they are testing
  * For example, all tests for the `Button` class should start with "Button"
* Test names should end with a description of what the test does - this is very useful when viewing test results, and when browsing in the test explorer. As far as naming conventions are concerned we don't mandate a specific one, as long as a test name clearly communicates its purpose.
  * For example, `Button_AutoSizeModeGetSet` or `MyButton_Click_should_throw_ArgumentNullException`.

#### Decoration

* All tests that deal with UI controls or types that require synchro0nization context must be decorated with `WinFormsFact` or `WinFormsTheory` attributes.
* Other tests can be decorated with `StaFact`, `StaTheory`, `Fact` or `Theory`

#### Dispose created objects

* All tests creating disposable objects (e.g. UI controls) must dispose them. Otherwise it could lead to memory leaks (bad but not terminal) and race conditions that could lead to hung tests (terminal).
  ```cs
    [WinFormsFact]
    public void ButtonBase_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using var control = new SubButtonBase();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }  
  ```
  
### Throw unhandled exceptions

We have been observed unexplainable hanging tests. To get better handle on the situation we have started trapping `ThreadException`s.
Each test class must implement the `IClassFixture<ThreadExceptionFixture>` interface similar to the one below:
  ```cs
    public class ButtonBaseTests : IClassFixture<ThreadExceptionFixture>
    {
    }
  ```

#### Strategy

##### Unit tests should be part of the same PR as code changes

* Unit tests must be added for any change to public APIs. 
* We will accept unit tests for internal/private methods as well. Some non-public API can be accessed directly (e.g. `internal`), some via subclassing (e.g. `virtual`) or via the public surface. However there are plenty of instances where a non-public API can't be easily accessed or arranged for. In this cases it is totally acceptible to use Reflection API to arrange, act and assert. For example: https://gist.github.com/JeremyKuhne/e8c9c33d037ac5e5ed4c477817cae414

##### Code Coverage

* In Visual Studio Test Explorer, select all tests, right click and execute 'Analyze code coverage for selected tests' command. This will run all tests and give a summary of blocks covered in 'Code Coverage Results' window. The summary can be drilled down to method level.
* Any code change accompanied with unit tests is expected to increase code coverage for the code modified.

##### Avoid duplicating tests just for different inputs

* Use `WinFormsTheory`, `StaTheory` or `Theory` attributes for this, followed by either `[InlineData]` or `[MemberData]`. See existing tests for examples on how to use these attributes
* The exception to this is if the code behavior is fundamentally different based on the inputs. For example, if a method throws an `ArgumentException` for invalid inputs, that should be a separate test.

##### One test (or test data) per code path please

* The most common exception to this is when testing a property, most people test get/set together

##### Whenever possible, mock up dependencies to run tests in isolation
  
* For example, if your method accepts an abstraction, use Moq to mock it up
* Search for Mock in the existing tests for examples, and see [Moq][moq] for details on how to use Moq.

## Functional Testing

Currently, there is a single functional test suite in the repository: the **WinformsControlsTest**. There is an xUnit project that executes various commands against this binary.

### Functional testing from the command line

To execute functional tests, run `.\build -integrationTest`

You will see various windows open and close very quickly. If all the tests are successful, you should see something like this:

```console
  Running tests: E:\src\repos\github\winforms\artifacts\bin\System.Windows.Forms.IntegrationTests\Debug\netcoreapp5.0\System.Windows.Forms.IntegrationTests.dll [netcoreapp5.0|x64]
  Tests succeeded: E:\src\repos\github\winforms\artifacts\bin\System.Windows.Forms.IntegrationTests\Debug\netcoreapp5.0\System.Windows.Forms.IntegrationTests.dll [netcoreapp5.0|x64]

Build succeeded.
    0 Warning(s)
    0 Error(s)
```

[comment]: <> (URI Links)

[issue-#183]: https://github.com/dotnet/winforms/issues/183
[moq]: (https://github.com/Moq/moq4/wiki/Quickstart)

### Troubleshooting command-line functional test errors

Since these run in xUnit, functional test errors can be examined in the same way as unit test failures.

### Functional testing from Visual Studio

To test from Visual Studio, open Winforms.sln in Visual Studio and test how you normally would (using the Test Explorer, for example)

### Troubleshooting Visual Studio functional test errors

* When testing from Visual Studio, test errors show up as normal in the test explorer.
* To troubleshoot, debug the selected test and set breakpoints as you normally would.

## Adding new functional tests

Functional tests are built and executed by file name convention

* Every WinForms binary has its own folder under src in the repository root (src\System.Windows.Forms, for example)
* Each of those folders has a tests folder under it (src\System.Windows.Forms\tests, for example), each of which may contain an IntegrationTests folder
* Each of these folders contains an IntegrationTest xUnit project (System.Windows.Forms.IntegrationTests.csproj)
  * These test projects automatically build when running `.\build`
  * The tests from these projects automatically execute when running `.\build -integrationTest`

### Therefore, you just need to put your tests in the right place in order for them to run

* Browse to the tests folder for the binary you are testing
* There should be one file per class being tested, and the file name should match the class name.
  * For example, if I wanted to test the `Button` class in System.Windows.Forms.dll, I would look for a Button.cs under src\System.Windows.Forms\tests
* If the file exists, add your tests there. If it doesn't exist, feel free to create it.
  * **Note that you don't have to modify the csproj at all.** Since the project is a Microsoft.NET.Sdk project, all source files next to it are automatically included
