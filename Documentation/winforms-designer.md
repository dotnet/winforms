# Using the .NET Framework WinForms Designer in WinForms Core

At this point, a dedicated WinForms Designer for WinForms .NET Core is not yet available. As a workaround, you can use Visual Studio's option to work with linked files and use its WinForms Designer for the .NET Framework.

Here is, how it is done:

:point_up: **TIP:** During the process, you need to re-nest Form files in the .NET Framework project whenever you add a new `Form` or a `UserControl`. Instead of using a text editor for patching the project file, you can use Mad Kristensen's [File Nesting Extension][file-nesting-extension], which is recommended to be installed beforehand.<br/>
Please close every open instance of Visual Studio before installing this extension.

## Create WinForms .NET Core app

Create a new WinForms application targeting .NET Core from Visual Studio or your favorite command line interface.

### Create in Visual Studio

`VS2019`: 

1. _File > Add > New Project... > Windows Forms App (.NET Core)_, choose C# or Visual Basic
2. Specify project name, e.g. `SimpleWinForms`



### Create from command line

Open your favorite console, create a new folder for your application:

```cmd
md SimpleWinForms
cd SimpleWinForms
dotnet new winforms -n SimpleWinForms
dotnet new sln
dotnet sln add SimpleWinForms
```

:point_up: **TIP:** You can have the folder name different from the project's name. Use the option `-n` (or `-name`) for that when using `dotnet new`.

:point_up: **TIP:** For Visual Basic projects use the option `-lang vb` when using `dotnet new`.

After creating the project, you can run the application:
```cmd
dotnet run
```


## Prepare WinForms .NET Core app for the Designer

There are few options available to help you to design UI for your .NET Core project.


### Option 1

1. Open `SimpleWinForms.sln`

2. Open the `SimpleWinForms` project file by double clicking on it in Solution Explorer. Change the ``TargetFramework`` property from:

    ```diff
    -    <TargetFramework>netcoreapp3.0</TargetFramework>
    +    <TargetFrameworks>net472;netcoreapp3.0</TargetFrameworks>
    ```

3. Add for any and every form file you have in this ``ItemGroup``:

    ```xml
     <ItemGroup Condition="'$(TargetFramework)' == 'net472'">
       <Compile Update="Form1.cs">
         <SubType>Form</SubType>
       </Compile>
       <Compile Update="Form1.Designer.cs">
         <DependentUpon>Form1.cs</DependentUpon>
       </Compile>
     </ItemGroup>
    
     <ItemGroup>
       <EmbeddedResource Update="Form1.resx">
         <DependentUpon>Form1.cs</DependentUpon>
       </EmbeddedResource>
     </ItemGroup>
    ```

    After doing these steps this is what you should end up with:

    ![edit-project-file][edit-project-file]


### Option 2

1. Open `SimpleWinForms.sln`

2. Add a new **Windows Forms App (.NET Framework)** project (VS2017: _Visual C# > Windows Desktop_ / VS2019: _C# : Windows : Desktop_) to the solution (_File > Add > New Project..._)

    ![add-netfx-project][add-netfx-project]

3. Name the new .NET Framework project as the .NET Core project, but add ".Designer" to it (e.g. `SimpleWinForms.Designer`)

4. Go to the .NET Framework project properties and set the default namespace to the .NET Core project's namespace

    ![edit-namespace][edit-namespace]

5. Delete the existing Form files in both projects

6. Add a new Windows Form in the .NET Framework project's context menu _Add > Windows Form..._

    ![add-new-form][add-new-form]

7. In the section list, click on *Windows Forms*, and chose *Windows Form* from the installed templates

    ![add-new-form-dialog][add-new-form-dialog]

8. Give the name and click `[Add]`.

   :exclamation: **IMPORTANT**: You need to trigger a form change event for the Designer to create a `resx` file. You can do it by resizing the form for a couple of pixels or changing the form's `Text` property. Don't forget to save.

9. Now we move the form to the .NET Core project.<br />
In the Solution Explorer click on the form and press <kbd>CTRL</kbd>+<kbd>X</kbd> to cut it; and then paste it in to the .NET Core project (<kbd>CTRL</kbd>+<kbd>V</kbd>). Check that the main form file, the .designer file and the resource file for the form are all present.

10. Then we link the form back into the .NET Framework project back.<br />
Remember: We can only use the Classic Designer, but we want to have only one set of files. So the form files, of course, belong to the .NET Core project but we want to edit them in the context of the .NET Framework project (thus using the .NET Framework Designer).

    * To do this open the context menu on the .NET Framework project in the Solution Explorer, and pick _Add > Existing Item_
    
    * In the File Open Dialog, navigate to the .NET Core project, select the *Form.cs*, *Form.Designer.cs* and *Form.resx* files and choose *Add as Link* option.

    ![add-as-link][add-as-link]

11. Compile the solution to see if the file references were set up correctly

1. As the last but important step, we need to re-nest the linked form files.<br/>
If you installed the [File Nesting Visual Studio Extension][file-nesting-extension] then that is done easily: Select both the *Form.Designer.cs* and *Form.resx* file, and from the context menu click _File Nesting > Nest Items_. In the dialog, pick the main form file (Form.cs), and click OK.

Now, whenever you need to use the Designer on one of the .NET Core Form or UserControl files, simply open the linked files in the .NET Framework project with the Windows Forms Designer.


## More information

If you are porting an existing .NET Framework application to .NET Core you may wish to read the following blog posts:
* [Porting desktop apps to .NET Core](https://devblogs.microsoft.com/dotnet/porting-desktop-apps-to-net-core/)
* [Using the WinForms designer for .NET Core projects](https://devblogs.microsoft.com/dotnet/how-to-port-desktop-applications-to-net-core-3-0/#user-content-using-the-winforms-designer-for-net-core-projects)

[comment]: <> (URI Links)

[file-nesting-extension]: https://marketplace.visualstudio.com/items?itemName=MadsKristensen.FileNesting

[comment]: <> (Images)

[add-netfx-project]: images/add-netfx-project.png
[edit-namespace]: images/edit-namespace.png
[add-new-form]: images/add-new-form.png
[add-new-form-dialog]: images/add-new-form-dialog.png
[add-as-link]: images/add-as-link.png
[edit-project-file]: images/edit-project-file.png
