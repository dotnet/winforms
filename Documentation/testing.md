# Windows Forms Testing

This document describes our approach to testing.

* [Building tests](#building-tests)
* [Unit Tests](#unit-tests)
    * [Running unit tests](#running-unit-tests)
        - [Unit testing from the command line](#unit-testing-from-the-command-line)
        - [Troubleshooting command-line unit test errors](#troubleshooting-command-line-unit-test-errors)
        - [Unit testing from Visual Studio](#unit-testing-from-visual-studio)
        - [Troubleshooting Visual Studio unit test errors](#troubleshooting-visual-studio-unit-test-errors)
    * [Adding new unit tests](#adding-new-unit-tests)
        - [Test placement](#therefore-you-just-need-to-put-your-tests-in-the-right-place-in-order-for-them-to-run)
        - [Unit Test best practices](#unit-test-best-practices)
            - [Naming](#naming)
            - [Decoration](#decoration)
            - [Disposal](#dispose-created-objects)
            - [Theory tests](#theory-tests#theory-tests)
        - [Throw unhandled exceptions](#throw-unhandled-exceptions)
* [Functional Tests](#functional-tests)
    * [Running functional tests](#running-functional-tests)
        - [Functional testing from the command line](#functional-testing-from-the-command-line)
        - [Troubleshooting command-line functional test errors](#troubleshooting-command-line-functional-test-errors)
        - [Functional testing from Visual Studio](#functional-testing-from-visual-studio)
        - [Troubleshooting Visual Studio functional test errors](#troubleshooting-visual-studio-functional-test-errors)
    * [Adding new functional tests](#adding-new-functional-tests)
        - [Test placement](#therefore-you-just-need-to-put-your-tests-in-the-right-place-in-order-for-them-to-run-1)
 * [Testing for Accessibility](#testing-for-accessibility)
    

# Building tests

Tests are automatically built when running `.\build` since all test projects are referenced in `Winforms.sln` at the repository root.

# Unit Tests

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

To test from Visual Studio, start Visual Studio via `.\start-vs.cmd`, and test how you normally would (using the Test Explorer, for example)

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
  * The tests from these projects automatically execute when running `.\build -test`.<br/>
    It is also possible to execute individual tests via `dotnet test --filter <filter expression>` command by switching to the desired test project directory first.
  

### Therefore, you just need to put your tests in the right place in order for them to run

* Browse to the tests folder for the binary you are testing.
* There should be one file per class being tested, and the file name should match the class name followed by a "**Tests**" suffix.
  * For example, if I wanted to test the `Button` class in System.Windows.Forms.dll, I would look for a **ButtonTests.cs** under src\System.Windows.Forms\tests.
  * For example, if I wanted to test the `Button.ButtonAccessibleObject` class in System.Windows.Forms.dll, I would look for a **Button.ButtonAccessibleObjectTests.cs** under src\System.Windows.Forms\tests.
* If the file exists, add your tests there. If it doesn't exist, feel free to create it.
  * **Note that you don't have to modify the csproj at all.** Since the project is a Microsoft.NET.Sdk project, all source files next to it are automatically included.

### Unit Test best practices

#### Naming

* Test files names should match the class they are testing followed by a "**Tests**" suffix.
  * For example, tests for the `Button` class should be in ButtonTests.cs.
  * For example, tests for the `Button.ButtonAccessibleObject` class should be in **Button.ButtonAccessibleObjectTests.cs**.
* Test class names should match the class they are testing, followed by a "**Tests**" suffix.
  * For example, tests for the `Button` class should in the `ButtonTests` class.
  * For example, tests for the `Button.ButtonAccessibleObject` class should in the `Button_ButtonAccessibleObjectTests` class.
* Test names should start with the class they are testing.
  * For example, all tests for the `Button` class should start with "Button".
* Test names should end with a description of what the test does - this is very useful when viewing test results, and when browsing in the test explorer. As far as naming conventions are concerned we don't mandate a specific one, as long as a test name clearly communicates its purpose.
  * For example, `Button_AutoSizeModeGetSet` or `MyButton_Click_should_throw_ArgumentNullException`.

#### Decoration

* All tests that deal with UI controls or types that require synchronization context must be decorated with `WinFormsFact` or `WinFormsTheory` attributes.
* Other tests can be decorated with `StaFact`, `StaTheory`, `Fact` or `Theory`

#### Dispose created objects

* All tests creating disposable objects (e.g. UI controls) must dispose them. Otherwise it could lead to memory leaks (bad but not terminal) and race conditions that could lead to deadlocked tests (terminal).
  ```cs
    [WinFormsFact]
    public void ButtonBase_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using var control = new SubButtonBase();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }  
  ```
  
#### Theory tests

Quite often there may be multiple tests that test exectly the same functionality but with different input parameters (e.g. `null`, `""`, `" "` for a `string` argument). 
In such cases instead of creating multiple tests it is preferred to have a theory test, which is in another words a data-driven test.

When writing theories note the following:

1. theory test must be [correctly decorated](#Decoration)

2. theories must avoid creating UI controls. E.g. instead of writing:
    ```cs
    public static IEnumerable<object[]> GetButton_TestData()
    {
        yield return new object[] { new Button() };
        yield return new object[] { new Button() { Text = "bla" } };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetButton_TestData))]
    public void Ctor_Control_DataGridViewCellStyle(Button button) { ... }
    ```
    prefer the following:
    ```cs
    public static IEnumerable<object[]> GetButton_TestData()
    {
        yield return new object[] { string.Empty };
        yield return new object[] { "bla" };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetButton_TestData))]
    public void Ctor_Control_DataGridViewCellStyle(string buttonText) 
    { 
        using var button = new Button() { Text = buttonText };
        ...
    }    
    ```
  
3. theories must not reuse disposable components.<br />
  In situations where following the above recommentation could be impractical, it is maybe acceptable to create disposable controls for each theory data, e.g.:
    ```cs
    public static IEnumerable<object[]> GetButton_TestData()
    {
        yield return new object[] { new Button(), new DataGridViewCellStyle() };
        yield return new object[] { new Button() { Text = "bla" }, new DataGridViewCellStyle() };
    }
    ```
    xUnit tries its best to [dispose](https://github.com/xunit/xunit/blob/56476a9691bb061661190b183733c5d8a2c6ef4d/src/xunit.execution/Sdk/Frameworks/TestMethodTestCase.cs#L191-L197) of disposable objects.
    However objects must not be shared across theories, as it could lead to unknown state, e.g.
    ```cs
    // ** DO NOT DO THIS! **
    public static IEnumerable<object[]> GetButton_TestData()
    {
        var button = new Button();
        yield return new object[] { button, new DataGridViewCellStyle() };
        yield return new object[] { button { Text = "bla" }, new DataGridViewCellStyle() }; // the button could already be disposed by the time this theory runs
    }
    ```


Also be beware and be mindful of VS-specific behaviours: https://xunit.net/faq/theory-data-stability-in-vs
  
### Throw unhandled exceptions

We have been observing unexplainable deadlocked tests. To get better handle on the situation we have started trapping `ThreadException`s.
Each test class must implement the `IClassFixture<ThreadExceptionFixture>` interface similar to the one below:
  ```cs
    public class ButtonBaseTests : IClassFixture<ThreadExceptionFixture>
    {
    }
  ```

#### Strategy

##### Unit tests should be part of the same PR as code changes

* Unit tests must be added for any change to public APIs. 
* We will accept unit tests for internal/private methods as well. Some non-public API can be accessed directly (e.g. `internal`), some via subclassing (e.g. `virtual`) or via the public surface. However there are plenty of instances where a non-public API can't be easily accessed or arranged for. In this cases it is totally acceptable to use Reflection API to arrange, act and assert. For example: https://gist.github.com/JeremyKuhne/e8c9c33d037ac5e5ed4c477817cae414

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
* Search for Mock in the existing tests for examples, and see [Moq](https://github.com/Moq/moq4/wiki/Quickstart) for details on how to use Moq.




# Functional Tests

Currently, there is a single functional test suite in the repository: the **WinformsControlsTest**. There is an xUnit project that executes various commands against this binary.

## Running functional tests

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
 
  
# Testing for Accessibility

Our goal is to make writing accessible WinForms applications easy. Specifically, all default property values should yield accessible experience. To test that controls are accessible, find or add the changed control to [AccessibilityTests application](https://github.com/dotnet/winforms/tree/master/src/System.Windows.Forms/tests/AccessibilityTests) and run [Accessibility Insights for Windows](https://accessibilityinsights.io/docs/en/windows/overview) on it. 
