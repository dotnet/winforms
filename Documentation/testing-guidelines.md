# Windows Forms Testing Guidelines

This doc coveres Winfors Forms **unit testing** guidelines. 
We are still working on a scalable solution for functional testing and will update this doc when we have one.

## Building tests
* Tests are automatically built since all test projects are referenced in System.Windows.Forms.sln at the repo root.

## Running tests

### Testing from the command line
* To run unit tests, add -test to your build command
 * For example, ```.\build -test```

If all the tests are successful, you should see something like this:
```
  Running tests: E:\src\repos\github\winforms\artifacts\bin\System.Windows.Forms.Tests\Debug\netcoreapp3.0\System.Windows.Forms.Tests.dll [netcoreapp3.0|x64]
  Tests succeeded: E:\src\repos\github\winforms\artifacts\bin\System.Windows.Forms.Tests\Debug\netcoreapp3.0\System.Windows.Forms.Tests.dll [netcoreapp3.0|x64]

Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### Troubleshooting command-line test errors
* When testing from the command line, a failed test should look something like this:
```
Running tests: E:\src\repos\github\winforms\artifacts\bin\System.Windows.Forms.Tests\Debug\netcoreapp3.0\System.Windows.Forms.Tests.dll [netcoreapp3.0|x64]
XUnit : error : Tests failed: E:\src\repos\github\winforms\artifacts\TestResults\Debug\System.Windows.Forms.Tests_netcoreapp3.0_x64.html [netcoreapp3.0|x64] [E:\src\repos\github\winforms\src\System.Windows.Forms\tests\UnitTests\System.Windows.Forms.Tests.csproj]
XUnit : error : Tests failed: E:\src\repos\github\winforms\artifacts\TestResults\Debug\System.Windows.Forms.Tests_netcoreapp3.0_x64.html [netcoreapp3.0|x64] [E:\src\repos\github\winforms\src\System.Windows.Forms\tests\UnitTests\System.Windows.Forms.Tests.csproj]

Build FAILED.
```
* To see the actual test(s) that failed, along with their error message(s), open the .html file that is displayed in the error message.

### Testing from Visual Studio
* To test from Visual Studio, open System.Windows.Forms.sln in Visual Studio and test how you normally would (using the test explorer, for example)

### Troubleshooting Visual Studio test errors
* When testing from Visual Studio, test errors show up as normal in the test explorer.
* To troubleshoot, debug the selected test and set breakpoints as you normally would.

## Adding new tests
Tests are built and executed by file name convention
* Every winforms binary has its own folder under src in the repo root (src\System.Windows.Forms, for example)
* Each of those folders has a tests folder under it (src\System.Windows.Forms\tests, for example)
* Each tests folder contains an xUnit test project (System.Windows.Forms.Tests.csproj)
 * These test projects automatically build when running .\build
 * The tests from these projects automatically execute when running .\build -test

**Therefore, you just need to put your tests in the right place in order for them to run**
* Browse to the tests folder for the binary you are testing
* There should be one file per class being tested, and the file name should match the class name.
 * For example, if I wanted to test the Button class in System.Windows.Forms.dll, I would look for a Button.cs under src\System.Windows.Forms\tests
* If the file exists, add your tests there. If it doesn't exist, feel free to create it.
 * **Note that you don't have to modify the csproj at all.** Since the project is a Microsoft.NET.Sdk project, all source files next to it are automatically included

### Test best pactices ###
Naming
1. Test files names should match the class they are testing
 * For example, tests for the Button class should be in Button.cs
2. Test class names should match the class they are testing, followed by "Tests"
 * For example, tests for the Button class should in the ButtonTests class
3. Test names should start with the class they are testing
 * For example, all tests for the button class should start with "Button"
4. Test names should end with a description of what the test does
 * For example, Button_AutoSizeModeGetSet
 * This is very useful when viewing test results, and when browsing in the test explorer

Strategy
1. Avoid duplicating tests just for different inputs
 * Use ```[Theory]``` for this, followed by either ```[InlineData]``` or ```[MemberData]```. See existing tests for examples on how to use these attributes
 * The exception to this is if the code behavior is fundamentally different based on the inputs. For example, if a method throws an ArgumentException for invalid inputs, that should be a separate test.
2. One test (or test data) per code path please
 * The most common exception to this is when testing a property, most people test get/set together
3. Whenever possible, mock up dependencies to run tests in isolation
 * For example, if your method accepts an abstraction, use Moq to mock it up
 * Search for Mock in the existing tests for examples, and see [Moq](https://github.com/Moq/moq4/wiki/Quickstart) for details on how to use Moq.