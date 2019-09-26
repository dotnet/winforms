Getting started with WinForms on .NETCore
==========================================

Create your first WinForms .NET Core “Hello World” application
--------------------------------------------------------------

The way you create your .NET Core WinForms application is very similar to
creating .NET Framework application:

-   In the *Open recent/Get started* Dialog of Visual Studio 2019, click on
    *Create a new project*.

-   Visual Studio shows the *Create a new project* dialog. In the project filter
    textbox, enter “winforms” to filter all the templates to WinForms projects,
    and pick *Windows Forms App (.NET Core) for C\#*.  

    ![05CreateNewProject][05CreateNewProject]

    **Note:** The Visual Basic language is currently not supported for .NET Core
    WinForms applications.

-   In the Dialog *Configure your new project*, enter a *Project name*, for
    example “HelloWinFormsCore”. Chose a file *Location* for the project files
    and click *Create* to create the Visual Studio Solution with the WinForms
    Core Project.  

    **Note:** After you’ve created a new WinForms project, it might take Visual
    Studio a few seconds to properly load all files before you can open the
    designer by double-clicking on the Form1.cs.  

    ![06TheCoreDesigner][06TheCoreDesigner]

-   For the first “Hello world” we’re going to add two controls from the Toolbox
    to the Form:  
    A Button and a Label.  

    *Please note that there are issues with Preview 1, and we appreciate all
    reports of bugs and odd behaviors that you might find, and most of all any
    additional feedback concerning performance, crashes or ideas you can
    provide. For example, Preview 1 does not support any container controls.
    Also, Resource Files are not supported at this point, which means you will
    not be able to set background images for certain controls or assign images
    to PictureBox controls. The component tray is also not enabled in the
    current preview. That's why it is important that you explore the tasks you
    want to do with the designer in as many different ways as possible and try
    them out to make sure that the designer will work the way you expect in all
    the different scenarios in the future. Should the Designer not work as you
    expect it, and what you discovered is not in the list of Known Issues (see
    KnownIssues.md) – providing VS feedback is the most helpful cause of
    action!*  

    So, to insert the first control – the Label – just double-click on the
    *Label* in the toolbox to insert it onto the design surface.

-   Now, grab the *Label* control, and drag it somewhat centered in the upper
    third of the Form.

-   Find the *AutoSize* property in the properties window and double-click to
    disable it.

-   Find the *Text* property in the properties window, open its context menu,
    and click *Reset* to clear the text field.

-   Resize the control, so it becomes double its original size, and place it
    somewhat in the middle of the Form.

-   Find the *TextAlign* property and set it to *MiddleCenter*.

-   Find the *Anchor* property and set it to None.

-   Find the *Font* property in the properties window and set it to *12pt
    Semibold or Bold*.

-   Finally, find the *Name* property, and name the Label “helloWorldLabel”.

Now, let’s add a Button to the Form, but let’s do it in another way.

-   Click the Button in the toolbox to select it.

-   “Draw” the Button with the mouse on the Form.

-   Drag the Button and place it beneath the Label.

-   Find the *Name* property and name the Button “helloWorldButton”.

-   Find the *Text* property and set the text to “Greet the world!”.

-   Set the *Anchor* property of the Button to *None*.  

    ![07HelloWorldCoreForm][07HelloWorldCoreForm]

-   Switch to the Event tab in the property browser.

-   Double-click the *Click* event in the event list to switch to the code
    editor, so you can add the code line, which assigns the string “Hello
    World!” to the *Label* control:  

    ![08HelloWorldCodeEditor][08HelloWorldCodeEditor]

-   Start the app by clicking *Debug* from the pulldown menu, click on *Start
    Debugging* (or alternatively press F5) and check, if your first designed
    WinForms Core App acts as expected.

**IMPORTANT:** Close the open Form in the Designer before you close the project
or quit Visual Studio.

[01VisualStudioInstaller]: screenshots/01VisualStudioInstaller.png
[02VisualStudioInstaller]: screenshots/02VisualStudioInstaller.png
[03InitializingVSIX]: screenshots/03InitializingVSIX.png
[04InstallingVSIX]: screenshots/04InstallingVSIX.png
[05CreateNewProject]: screenshots/05CreateNewProject.png
[06TheCoreDesigner]: screenshots/06TheCoreDesigner.png
[07HelloWorldCoreForm]: screenshots/07HelloWorldCoreForm.png
[08HelloWorldCodeEditor]: screenshots/08HelloWorldCodeEditor.png
[09WinFormsOutputPane]: screenshots/09WinFormsOutputPane.png
[10UnsupportedControls]: screenshots/10UnsupportedControls.png
[11SendFeedback]: screenshots/11SendFeedback.png
[12PLM_EmptyForm]: screenshots/12PLM_EmptyForm.png
[13EnabledPLM]: screenshots/13EnabledPLM.png
