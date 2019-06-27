# Porting a Winforms Maui test to xUnit

## Summary

This document details how to port an existing WinForms Maui test to xUnit so it can be run as part of our continuous integration builds. 

If you run into any problems, please contact Adam Yoblick.

## Cloning and building source code

1. If you have not already done so, clone or fork the repo at https://github.com/dotnet/winforms

1. Checkout a new branch for your work with the following command:
        
        git checkout -b <your-new-branch-name>

1. Run a build and execute integration tests with the following command:

        .\build -integrationTest /m:1

1. If the above step fails, **STOP**.

    See https://github.com/dotnet/winforms/tree/master/Documentation for more verbose instructions on how to build this repo.

## Porting the maui test

See the pull request at https://github.com/dotnet/winforms/pull/1240 for an example of how to do this.

1. Browse to .\src\System.Windows.Forms\tests\IntegrationTests\MauiTests. All Maui tests live under this folder, each in their own folder.
	
1. Create a copy of the existing MauiButtonTest folder in the same directory

    1. Rename the new folder to something more appropriate for the test you are porting (`MauiComboBoxTest`, for example)
    1. Browse into the new folder and rename the .csproj to the name you chose in step 3 (`MauiComboBoxTest.csproj`, for example)
    1. Delete the existing MauiButtonTest.cs file

1. Find the source code for the test you are porting and copy it into the new folder you created in the previous step

1. Clean up the Maui test source:

    1. Rename the file appropriately (`MauiComboBoxTest.cs`, for example)
    1. Add `using System.Windows.Forms.IntegrationTests.Common;` to the using directives.
    1. Change the namespace to `System.Windows.Forms.IntegrationTests.MauiTests`
    1. Rename the class appropriately (`MauiComboBoxTest`, for example)
    1. Add the following line as the first command in the constructor:
    
            this.BringToForeground();

    1. Add the following line as the first command in Main():
    
            Thread.CurrentThread.SetCulture("en-US");

    1. Make sure each scenario method has the `[Scenario(true)]` attribute
    1. Remove the entire `[Scenarios]` section at the bottom of the file
    1. Make sure each scenario method’s name describes what the test is actually doing. 
    
        For example, the button scenario methods are named as follows:
            
            Click_Fires_OnClick()
            Hotkey_Fires_OnClick()
            Hotkey_DoesNotFire_OnClick()

    1. Clean up the code style, following visual studio suggestions (we will catch these in a PR, but it is good to clean up beforehand). For example:
        1. Use implicit types (var) whenever possible
        1. All fields should have access modifiers
        1. Prefer built-in types instead of classes (“string” is preferred to “String”)
        1. Only use “this” when necessary
        1. Etc...

## Update the WinForms solution

1. Open `.\Winforms.sln` in Visual Studio
1. In the solution explorer, browse to `tests\integration\maui`
1. Right-click on the maui folder and click on `Add -> Existing Project...`
1. Add the csproj you created in the previous section

## Run the maui test manually

1. Open a command prompt and browse to the root of your repo.
1. Build the source code (including the test you just ported):

        .\build-local.ps1
	
1. Browse to `.\artifacts\bin\<your-test-name>\Debug\netcoreapp3.0`
1. Double click on `<your-test-name>.exe`
1. Check the results.log in the same directory to make sure everything passed
1. If anything shows as a failure in the log, **STOP**. 

    Figure out why the scenario  is failing and fix it before moving on.

## Run the maui test through the local build

1. Browse to `.\src\System.Windows.Forms\tests\IntegrationTests\System.Windows.Forms.Maui.IntegrationTests`
1. Copy `WinformsMauiButtonTest.cs` to a new file in the same directory
1. Rename the new file according to the Maui test you added in the earlier section (`WinformsMauiComboBoxTest.cs`, for example)
1. Rename the class to the same thing (`WinformsMauiComboBoxTest`, for example)
1. Change the ProjectName string to match the name of the Maui test project you added in the earlier section (`MauiComboBoxTest`, for example)

    **Note that this must EXACTLY MATCH the project name for the test runner to find your Maui test executable**
	
1. Change the name of the single test method to match the name of your project (`MauiComboBoxTest`, for example)
1. Open a command prompt and browse to the root of your repo.
1. Run the following command:

        .\build -integrationTest /m:1
	
1. Verify all the integration tests pass. If there is a failure, the following files may help:
    - xUnit test results summary can be found at `.\artifacts\TestResults\Debug\System.Windows.Forms.Maui.IntegrationTests_netcoreapp3.0_x64.html`
    - xUnit test outputs can be found at `.\artifacts\log\Debug\System.Windows.Forms.Maui.IntegrationTests_netcoreapp3.0_x64.log`
    - The results.log for your individual Maui test can be found at `.\artifacts\bin\<your-test-name>\Debug\netcoreapp3.0\results.log`

## Run the maui test through the CI build

1. Create a pull request from your new branch (or fork) into winforms master at https://github.com/dotnet/winforms/compare
1. Once the PR is created (and every time new files are pushed to the PR), the build will kick off
1. You can see the result of the build by scrolling down to the checks section and clicking on Show All Checks
1. If the build is not successful, try to investigate the failure
    1. Click on Details next to the dotnet-winforms CI check
    1. Examine the build output. You can also click on the Summary tab and download build artifacts, which will include the same logs you examined locally
