Known Issues
===============

Below are the known issues that impact WinForms .NET Core Designer for the
Preview 1 release. Given that this is an early preview with many features still
to be implemented be sure to consult the ReleaseNotes.md file for comprehensive
information around what to expect in this release. Known issues in the
implemented features are as follows:

PropertyBrowser
---------------

-   The *TextAlign* property does now show the expected visual editor, instead
    it shows a list of the possible values.

-   A preview image is not displayed for properties of type
    *System.Drawing.Image* and the user is unable to set any property of type
    *System.Drawing.Image.*

Controls
--------

-   When a user right-clicks to bring up the context menu on a control which is
    not yet selected, the context menu is shown for the Form, because the
    control does not get selected.

-   When the user wants to copy one control, this control needs to be selected
    first. When the user pastes the control after it was copied, the original
    selected control remains selected. If this happens and the user moves or
    sets a property on the pasted control without de-selecting the first
    control, any properties change would apply to both controls because both
    remain selected. To work around this issue, click the form to deselect both
    controls and then select the control for which you want to set properties.

-   Visual Studio may crash when a user clicks the drop-down button of
    *PictureBox* control’s *LoadCompleted* and *LoadProgressChanged* events in
    Event tab of the property window. This is because .NET Core 3.0 does not yet
    have support for these events. Other event handlers can be created as
    normal.

-   When moving a control, which is anchored on all sides, half way out of its
    container, its size is changed and the position is not where the user had
    intended to place the control.

-   A user cannot use Ctrl+Arrow keys on the keyboard to move a control when
    there is only one control in the form. When multiple controls are present
    and selected the commands work as expected.

-   Renaming a control in the *Designer.cs* file may result in an error
    indicating “The designer loader did not provide a root component but has not
    provided a reason why.” It is recommended that renaming of controls be done
    in the property browser to avoid this error.

-   When setting the *AutoCompleteCustomSource* property of the *TextBox*
    control an error will appear indicating “Property DataSource does not exist”
    rather than displaying the String Collection Editor as expected. Data
    related features will be enabled in a later Preview release of the WinForms
    Core designer.

-   When the *DateTimePicker* control is added to a form and has its format set
    to the long date format, the default width is too narrow to display the
    calendar icon next to the drop down arrow. This is due to a new default font
    in WinForms .NET Core 3.0 that is a small amount larger than in .NET
    Framework.

-   When a *TextBox* control’s *Text* property is set to a string with more than
    200 characters, an unhandled exception occurs. This is due to resources
    being created for the form when the text property’s content exceeds 200
    characters and resources not being supported in Preview 1.

-   When a new control is drawn on the form, the mouse pointer appears to be
    missing the crosshairs that indicate the control can be drawn. In actuality,
    the crosshairs are painted in white and thus the mouse cursor not easily
    visible to the user. The control can be drawn as usual.

Forms
-----

-   When a Form's *AutoScroll* property is set to *true* and the user drags and
    drops a control towards the borders of the Form, the Form’s scrollbars do
    not appear. The user is unable to scroll to see the controls they dropped.

-   The *RightToLeft* property of the form is read-only, and can currently not be
    changed in design mode.

-   When a Form’s *ShowIcon* property is set to *false*, the designer does not
    honor the property and instead displays the icon in the designer.

-   Double-clicking the *ImeMode* property of the form does not switch to other
    values. The user can click the property’s *DropDown* button to change the
    value.

-   The “Tab Order” command found on the *View* menu of the main Visual Studio
    menu bar is used to visually control the tab order on a form, but is not yet
    supported. The user can use the *TabIndex* property on each of the controls
    to set the tab order for the controls on the form.

-   When placing controls outside the width of the form, the form does not
    resize to accommodate the new controls.

-   When resizing controls using Ctrl+Shift+Arrow key on the keyboard, the
    snap lines, which show the distance to the edge of the Form, are not shown
    and the control is resized beyond the form borders.

-   *Select Form\** menu items are not added to the context menu for all
    controls.

-   When adding multiple controls to a form from the tool box with the
    double-click gesture, the new controls are not offset from one another. As a
    result, they are created directly on top of one another making it more
    difficult to see that controls were successfully added. The controls can be
    moved and resized as usual.

-   When scrollbars are present on a form, mouse-clicks to move up/down or
    left/right on the scroll bar are not registered. As a result, the user is
    not able to scroll to non-visible portions of the form to design it.

-   Cycling through controls with the TAB key: If a control on a Form is
    selected, it is currently not possible to select the next control in the
    Tab-Order (or, if no *TabIndex* property does apply for the remaining controls
    like for the *PictureBox* control, the Z-Order) with the Tab key of the
    keyboard.

General
-------

-   There is no support for User Controls in this preview.

-   *UserControl* and *InheritedForm* item templates are not available in this
    preview.

-   The form designer is unreliable on a vertical monitor or on a secondary
    monitor. The selection of controls in the designer does not work.

-   Pressing F1 for any property in Visual Studio properties window goes to a
    blank webpage.

-   Features in the Document Outline Window are not yet supported. Specifically,
    the *Move Up* and *Move Down* buttons are not enabled yet.

-   The Designer Options in the *Tools/Options* dialog are not yet enabled for
    the .NET Core WinForms Designer. As a result, the various options around
    snap lines, Snap-To-Grid and other Features use defaults and are not yet able
    to be set.

-   Undo or Ctrl+Z on the keyboard may need to be invoked several times before
    the action is actually registered and the undo occurs.

-   Horizontal and vertical scroll bars of the Designer window are missing when
    the form is resized to be larger than the available space in the Designer
    window.

-   The toolbox is re-initialized on switching between designer windows. This
    causes a visible flicker, but the toolbox can be used as usual.

-   The Resource Picker Dialog, used to select images from a *.resx* file for
    example, is not yet implemented.

Known Issue: A previously left open Form appears to be empty on reloading the project
-------------------------------------------------------------------------------------

A small placeholder form window gets shown when the WinForms Core Designer is
launched after a fresh start of Visual Studio, and the Form was not closed in
the previous Designer session. This issue will also repro if the Designer was
pinned to the editor during previous Visual Studio session.

![12PLM_EmptyForm][12PLM_EmptyForm]

Closing and opening the designer again should fix the issue. However, it’s
annoying.

Cause of the issue: The loading of CPS projects happens on a background thread.
Due to this reason some of the project references required by the WinForms
designer are not yet resolved at the time the Designer load starts. This causes
the designer to show a placeholder form instead of the actual one. Closing and
reopening this Form gives Visual Studio time to resolve those required
references.

Workaround: Enable *Partial Load Mode (PLM)* in Visual Studio. The steps to
enable PLM are as follows:

1. Download this [zip file](https://webpifeed.blob.core.windows.net/webpifeed/Partners/PLM.zip) and unzip its
contents to a safe location.

2. For the Visual Studio instance that needs to be patched, launch its developer
command prompt in admin mode.

3. Close all Visual Studio instances, if open.

4. From the command prompt, browse to the folder where contents of zip file are
present.

5. Execute the script: *PartialLoad-Enable.cmd*.

6. Launch Visual Studio and open the Designer by double-clicking on a Form in
the Solution Explorer.

7. The editor will now show the following message until everything finished
loading/referencing in Visual Studio. After that WinForms Core Designer will
open up automatically.

![13EnabledPLM][13EnabledPLM]

To disable PLM feature, open admin mode developer command prompt and execute the
script: *PartialLoad-Disable.cmd*. To restore default settings for PLM, execute
the script: *PartialLoad-Default.cmd* in the admin mode developer command
prompt.

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
