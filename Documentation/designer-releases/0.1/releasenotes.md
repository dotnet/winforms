Release Notes V0.1
===========

The .NET Core Windows Forms Designer (aka “WinForms Core Designer”) is not yet
integrated in Visual Studio. For an easy deployment, the WinForms Core Designer
is available as a Visual Studio extension ("VSIX"), can be installed in Visual
Studio and completely removed from it at any time.

Prerequisites
-------------

For the WinForms Core Designer to work properly, you need:

-   The latest .NET Core SDK (at least in version 3.0), which you find here:
    <https://github.com/dotnet/core-sdk>.

-   We suggest the latest Visual Studio 2019 Preview (at least Version 16.3.0
    Preview 4) from the Preview channel, which you can download from here:
    <https://visualstudio.microsoft.com/vs/preview/>.

-   The WinForms Core Designer installation package (“VSIX”) package that you
    can download from here: https://aka.ms/winforms-designer.

    (Please note that, due to the very early days for the WinForms Core
    designer, the package is not yet available in the Visual Studio
    Marketplace.)

Installing the WinForms Core Designer
-------------------------------------

Before you install the WinForms Core Designer VSIX, we recommend that you check
for updates of your Visual Studio Public Preview installation. At least, Version
16.3.0 Preview 4 of Visual Studio must be installed for the WinForms Core
Designer to work.

To check for updates,

1.  Find the Visual Studio Installer in the Start menu, or simply type “Visual
    Studio Installer” in the Windows Start menu Search Box, or press Windows key
    and type “Visual Studio Installer”.

2.  Choose “Run as administrator” option in the right-hand side panel.  

    ![01VisualStudioInstaller][01VisualStudioInstaller]

3.  The Installer will prompt you for an update of the Installer itself if
    necessary. Proceed with the update.

4.  Update the Visual Studio 2019 Preview, if updates are indicated through the
    *Update* button.  

    ![02VisualStudioInstaller][02VisualStudioInstaller]

Once you updated your visual Studio, download the WinForms Designer VSIX from
<https://aka.ms/winforms-designer> if you haven’t done so yet, and install the
WinForms Designer VSIX:

1.  To this end, double-click on the downloaded *WinFormsDesigner.Setup* VSIX
    file.

2.  Wait, until the setup discovers available Visual Studio instances.  

  ![04InstallingVSIX][04InstallingVSIX]

3.  In the VSIX Installer dialog, make sure that *Visual Studio 2019 Preview* is
    selected.

4.  Click *Install* to start the installation. This will take a couple of
    minutes to complete.

When the installation is completed, close the VSIX Installer. Now you can start
Visual Studio and the new .NET Core WinForms Designer will be used for your .NET
Core projects automatically. If you’ll open an application targeting .NET
Framework, the classic .NET Framework Designer will be pulled up by Visual
Studio, so you don’t have to worry about “two designers”; Visual Studio will
handle it for you.

**Note:** You can create a new .NET Core WinForms app using a template *Windows
Forms App (.NET Core) for C\#* in *Create a new project* dialog or via dotnet
CLI.

Tip: Even as an experienced WinForms developer, you might want to take a look at
the section *Getting started with WinForms on .NET Core* to get a better feeling
for the Preview and how you could help to improve the WinForms Core Designer
experience.

**Note:** The Visual Basic language is currently not supported for .NET Core
WinForms applications.

What to expect from this Preview Release of the WinForms Core Designer
======================================================================

We are releasing our first bits of the designer so early to support the general
culture of developing the product with our users’ early feedback in mind. We
have gotten the most commonly used controls and base operations working and we
will be adding more in the further releases. You may notice some behavioral
differences between .NET Core and NET Framework Designers in the current
version. By the time of *General Availability* of the WinForms Core Designer, we
intend to achieve functional parity between two designers. See sections
*Supported controls and scenarios* and *Known issues* for more information about
this version.

Supported controls and scenarios, and things you should be testing
------------------------------------------------------------------

-   **Supported Controls:** All the control that are present in the Toolbox
    under the Tab *All Windows Forms (.NET Core)* are currently supported in the
    Designer. That means, you can add those controls to a Form, set their
    properties, wire up there default event via double-click, or wire up any
    other event via the Event tab of the Property Browser.

-   **Edit Controls:** You can move and resize as well as cut, copy and paste
    controls. Said that, copying controls from a classic framework form is no
    supported scenario.

-   **Using the formatting commands:** You can use the formatting commands of
    the WinForms Core Designer to help to precisely position and align controls.
    Also, with those commands you can change the z-order of the controls by
    using *Bring to Font* and *Send to back*.

-   **Anchroing and Docking:** Those controls which are supporting this, can be
    anchored and docked. Keep in mind, though, that the repositioning of
    anchored controls beyond the borders of the Form can be problematic in this
    Preview.

-   **Using the Type Editors:** Many of the Control’s type editors are ready for
    testing: For example, you can use the type editor of the TreeView’s *Nodes*
    property to add and edit TreeView nodes, or use the Type Editor of the
    ListView’s *Items* property of the to add and edit ListView items.

Known Issues
------------

If you took the time to read through *Getting started with WinForms on .NET
Core*, then you might agree that neither were these the fastest steps to get
this small sample app done, nor did we actually need all of that to make that
result happen. But that was also not the point. This is a first very early
Preview of the WinForms Core Designer, it is to check if we’re on the right
track, and to note that a lot of things are still in development.

-   **Components, Containers, Menu and ToolStripItems and Data:** All the
    components, containers, tool and menu strip and data related controls are
    not yet added to the toolbox. For all the supported controls, see the
    correlating section below. The rule of thumb is: The controls you see in the
    toolbox are supported. Said that, there might be a few properties of those
    controls, which rely on other features, that have not implemented yet – like
    the *Image* property of the PicturteBox control, which needs the Resources
    feature to be working.

-   **Resource Files:** Resource files are not yet supported, so you cannot use
    for example any property for a control which uses Images, since this would
    require the Resources feature to be working. This in turn means, you cannot
    set the *Image* property for a PictureBox control or the *BackgroundImage*
    property of a Form or a Button in the current preview.

-   **User Controls, Inherited Forms/Controls:** User controls are not supported
    in the current preview. Also, *Inherited Forms* and *Inherited UserControls* are
    not supported in the current Preview. The Form is currently the only top
    level container which has designer support.

-   **Visual Editors of the Property Grid:** Some of the visual Editors of the
    controls, which are already supported, do not render correctly – when you
    picked the value for *TextAlign* for example in the *Getting started*
    document, you probably expected a graphical representation of the possible
    alignment settings in the Properties window rather than a simple list of
    text entries to pick the value from.

-   **Unsupported Control Properties:** Some of the properties in the Properties
    windows are disabled because the functionality has not been implemented yet.

-   **Localization of UI:** Non of the WinForms Core Designer dialogs are yet
    localized for other languages/cultures.

-   **Right-to-left functionality:** The Core Designer does not yet support
    Right-to-left functionality.

-   **Databinding:** Databinding and the visual tools for setting up DataBinding
    are not supported in this Preview.

For a more detailed list of the known issues in the current Preview, please
refer to the [knownissues][knownissues] document.

### Migration of existing WinForms framework apps

When you convert a WinForms App from the Framework designer to the Core
Designer, it may contain definitions of controls, user controls, components or
menus and toolbars, which the Preview version of the Core Designer does not yet
support.

The Designer will try its best, to bring up the form for editing and testing,
anyway. Said that, unsupported controls are rendered by the Designer with a red
X. Those controls are locked in place, their properties cannot be changed, and
they cannot be resized or repositioned.

![10UnsupportedControls][10UnsupportedControls]

**WARNING:** In migrated WinForms projects, which contain user control or custom
controls, those controls might crash the Core designer and unsaved work could be
lost, or the definition of those controls in Forms, which use user controls or
custom controls might be unexpectedly removed!

**If you test migrated forms in the WinForms Core Designer, make sure you
perform your testing with a copy of the original project to avoid unexpected
data loss!**

The Output tool window
----------------------

You might have already discovered a new output pane in the Output tool window of
Visual Studio, which says “Show the output from Windows Forms”.

![09WinFormsOutputPane][09WinFormsOutputPane]

Here you can see status messages of the new Designer’s engine, which is
developed from scratch. In contrast to the classic designer, this engine is the
connection between Visual Studio, which runs under the classic .NET Framework,
and every Core Form you see in the designer. As you might know, .NET Core
binaries cannot be loaded into a .NET Framework process, and that does also
apply to Visual Studio (classic framework) and a WinForms Core Form which you
want to design. There is now a new process, which is called the “Surface
Process” (*WinFormsSurface.exe*), it is developed in .NET Core Framework and is
responsible for rendering the Form you see. This engine sends the commands from
the Designer in Visual Studio to this Surface Process, so it can render all the
Forms, controls and later also handle components, menus and toolbars, and brings
back the data, Visual Studio needs – for example to display them in the
Properties window.

Providing feedback
==================

Here are some things we’d love to hear from you:

-   What is missing, but should be there, because we did not yet include it in
    our known issues list (see below).

-   What is not working properly: Can you select and move controls around the
    same way as it was the case in the .NET Framework designer? Can controls be
    resized and aligned with the Format commands of Visual Studio? Do edit
    functions like *Bring to Front* or *Send to Back* work as expected,
    especially when controls are docked or anchored?

-   What seems to be working, but is too slow or sluggish?

The best way to send us feedback is through the Visual Studio Feedback tool.
Just click the correlating icon in the upper right corner of the Visual Studio
UI, and chose, if you want to suggest an additional feature or if you want to
report a bug:

![11SendFeedback][11SendFeedback]

For further instructions on how to use Visual Studio to send feedback, see this

helpful page. By using the feedback tool you will be able to send critical
diagnostic information directly to Microsoft so that we are able to more quickly
address the problem.

[knownissues]: knownissues.md

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
