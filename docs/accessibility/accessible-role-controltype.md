# Consistency of AccessibleRole and "ControlType" for accessible objects

This document describes a behavior and relations of `AccessibleRole` property of 
[controls](https://docs.microsoft.com/dotnet/api/system.windows.forms.control.accessiblerole) and 
[ToolStrip items](https://docs.microsoft.com/dotnet/api/system.windows.forms.toolstripitem.accessiblerole) 
 and `"ControlType"` accessibility property.

* [Overview](#Overview)
* [AccessibleRole-"ControlType" mapping](#AccessibleRole-"ControlType"-mapping)
* [Default "ControlType" values](#Default-"ControlType"-values)
* [Exceptional cases and additional info](#Exceptional-cases-and-additional-info)
    

# Overview

All accessible objects have `"ControlType"` (a part of modern [UIA](https://docs.microsoft.com/dotnet/framework/ui-automation/ui-automation-overview)) and "[LegacyIAccessible](https://docs.microsoft.com/windows/win32/winauto/uiauto-implementinglegacyiaccessible)`.Role"` 
(further `"Role"` - a part of outdated [MSAA](https://docs.microsoft.com/windows/win32/winauto/microsoft-active-accessibility)) accessibility properties. 

![inspect-accessibility-properties][inspect-accessibility-properties]

Accessibility tools ([Inspect](https://docs.microsoft.com/windows/win32/winauto/inspect-objects), 
[Narrator](https://support.microsoft.com/windows/complete-guide-to-narrator-e4397a0d-ef4f-b386-d8ae-c172f109bdb1),
 etc.) request [Role](https://docs.microsoft.com/dotnet/api/system.windows.forms.accessibleobject.role) property of accessible objects to 
get `"Role"` accessibility property value. `Role` property value of 
[ControlAccessibleObject](https://docs.microsoft.com/dotnet/api/system.windows.forms.control.controlaccessibleobject), 
[ToolStripItemAccessibleObject](https://docs.microsoft.com/dotnet/api/system.windows.forms.toolstripitem.toolstripitemaccessibleobject), 
and their descendants can be set by the developer using public `AccessibleRole` property.

![accessibleobject-role-implementation][accessibleobject-role-implementation]


Accessibility tools call internal `GetPropertyValue` method with `ControlTypePropertyId` 
argument to get `"ControlType"` value.

![accessibleobject-getpropertyvalue-implementation][accessibleobject-getpropertyvalue-implementation]


**.NET Framework 4.7.2** implements the following `AccessibleRole` property and 
`"ControlType"` accessibility property relations:
when `Control`'s or `ToolStripItem`'s `AccessibleRole` property is set to a non-default 
value by the developer, the native accessibility proxy implementation would 
set `"ControlType"` property to a matching value.</br>
**.NET Framework 4.8**, **.NET Core 3.0** and **3.1**, **.NET 5.0** lost 
this behavior for all controls that implement **UIA providers** because 
they provide custom implementation of UIA properties and no longer rely 
on UIA proxy to match roles to control types. **.NET 6.0** restores the **.NET Framework 4.7.2** behavior.


# AccessibleRole-"ControlType" mapping

When a developer changes `AccessibleRole` property value of a control 
or a `ToolStrip` item (for example: `control.AccessibleRole = AccessibleRole.List;`), `"ControlType"` accessibility 
property value of the corresponding accessible object will 
be changed according to the following list (except some cases described below):

| AccessibleRole | ControlType |
| ------------- | ------------- |
|Alert|Pane|
|Animation|Pane|
|Application|Window|
|Border|Pane|
|ButtonDropDown|SplitButton|
|ButtonDropDownGrid|Button|
|ButtonMenu|MenuItem|
|Caret|Pane|
|Cell|DataItem|
|Character|Pane|
|Chart|Pane|
|CheckButton|CheckBox|
|Client|Pane|
|Clock|Button|
|Column|Pane|
|ColumnHeader|Header|
|ComboBox|ComboBox|
|Cursor|Pane|
|Default|Pane|
|Diagram|Pane|
|Dial|Pane|
|Dialog|Pane|
|Document|Document|
|DropList|Pane|
|Equation|Pane|
|Graphic|Image|
|Grip|Thumb|
|Grouping|Group|
|HelpBalloon|Pane|
|HotkeyField|Pane|
|Indicator|Thumb|
|IpAddress|Pane|
|Link|Hyperlink|
|List|List|
|ListItem|ListItem|
|MenuBar|MenuBar|
|MenuItem|MenuItem|
|MenuPopup|Menu|
|None|Pane|
|Outline|Tree|
|OutlineButton|Pane|
|OutlineItem|TreeItem|
|PageTab|TabItem|
|PageTabList|Tab|
|Pane|Pane|
|ProgressBar|ProgressBar|
|PropertyPage|Pane|
|PushButton|Button|
|RadioButton|RadioButton|
|Row|Pane|
|RowHeader|Header|
|ScrollBar|ScrollBar|
|Separator|Separator|
|Slider|Slider|
|Sound|Pane|
|SpinButton|Spinner|
|SplitButton|SplitButton|
|StaticText|Text|
|StatusBar|StatusBar|
|Table|Table|
|Text|Edit|
|TitleBar|TitleBar|
|ToolBar|ToolBar|
|ToolTip|ToolTip|
|WhiteSpace|Pane|
|Window|Window|


# Default "ControlType" values

Here is the list of common controls and `ToolStrip` items with 
their accessible objects `"ControlType"` accessibility property values 
by default - when `AccessibleRole` property value of these controls 
and `ToolStrip` items is not set:
```cs
Control control = new Control();
// control.AccessibleRole = AccessibleRole.List; - Role is not set
```
or equals [AccessibleRole](https://docs.microsoft.com/dotnet/api/system.windows.forms.accessiblerole)`.Default`:
```cs
Control control = new Control();
control.AccessibleRole = AccessibleRole.Default; // Role is set to the default value
```

| Control or item | Default ControlType |
| ------------- | ------------- |
|Button|Button |
|CheckBox |CheckBox |
|CheckedListBox |List |
|ComboBox |ComboBox |
|ContextMenuStrip<br/>└ ToolStripMenuItem<br/>└┴ ToolStripMenuItem<br/>└┴ ToolStripComboBox<br/>└┴ ToolStripTextBox<br/>└┴ ToolStripSeparator<br/>└ ToolStripComboBox<br/>└ ToolStripTextBox<br/>└ ToolStripSeparator|Menu<br/>MenuItem<br/>Pane<br/>ComboBox<br/>Edit<br/>Separator<br/>ComboBox<br/>Edit<br/>Separator|
|DataGridView <br/>└ DataGridViewEditingPanel |Table <br/>Pane|
|DateTimePicker |ComboBox|
|DomainUpDown<br/>└ UpDownButtons<br/>└ UpDownEdit|Spinner<br/>Spinner<br/>Edit|
|FlowLayoutPanel|Pane|
|Form|Window|
|GroupBox|Group|
|HScrollBar|ScrollBar|
|Label|Text|
|LinkLabel|Text|
|ListBox|List|
|ListView|List|
|MenuStrip<br/>└ ToolStripMenuItem<br/>└┴ ToolStripMenuItem<br/>└┴ ToolStripComboBox<br/>└┴ ToolStripTextBox<br/>└┴ ToolStripSeparator<br/>└ ToolStripComboBox<br/>└ ToolStripTextBox|MenuBar<br/>MenuItem<br/>MenuItem<br/>ComboBox<br/>Edit<br/>Separator<br/>ComboBox<br/>Edit|
|MaskedTextBox|Edit|
|MonthCalendar|Calendar/Table|
|NumericUpDown<br/>└ UpDownButtons<br/>└ UpDownEdit|Spinner<br/>Spinner<br/>Edit|
|PictureBox|Pane|
|ProgressBar|ProgressBar|
|PropertyGrid|Pane|
|RadioButton|RadioButton|
|RichTextBox|Document|
|ScrollBar|ScrollBar|
|SplitContainer|Pane|
|Splitter|Pane|
|StatusStrip<br/>└ ToolStripStatusLabel<br/>└ ToolStripProgressBar<br/>└ ToolStripSplitButton<br/>└ ToolStripDropDownButton<br/>└┴ ToolStripMenuItem<br/>└┴ ToolStripComboBox<br/>└┴ ToolStripTextBox<br/>└┴ ToolStripSeparator|StatusBar<br/>Text/Link<br/>ProgressBar<br/>Button<br/>Button<br/>MenuItem<br/>ComboBox<br/>Edit<br/>Separator|
|TabControl|Tab|
|TableLayoutPanel|Pane|
|TabPage|Pane|
|TextBox|Edit|
|TextBoxBase|Edit|
|ToolStrip<br/>└ ToolStripItem<br/>└ ToolStripComboBox<br/>└ ToolStripDropDown<br/>└ ToolStripNumericUpDown<br/>└ ToolStripOverflow<br/>└ ToolStripProgressBar<br/>└ ToolStripTextBox<br/>└ ToolStripButton<br/>└ ToolStripLabel<br/>└ ToolStripSplitButton<br/>└ ToolStripDropDownButton<br/>└ ToolStripSeparator<br/>└ ToolStripOverflowButton<br/>|ToolBar<br/>Button<br/>ComboBox<br/>Menu<br/>Spinner<br/>Menu<br/>ProgressBar<br/>Edit<br/>Button<br/>Text/Link<br/>Button<br/>Button<br/>Separator<br/>MenuItem|
|TrackBar|Slider|
|TreeView|Tree|
|VScrollBar|ScrollBar|
|TrackBar|Slider|
|TreeView|Tree|
|VScrollBar|ScrollBar|

# Exceptional cases and additional info
- Here is the list of controls and `ToolStrip` items that didn't change 
their accessible objects `"ControlType"` values depending on `Role` in **.NET Framework 4.7.2** 
but in **in .NET 6.0** they can do it correctly:
  1. **`ListView`** - `"ControlType"` was always `List`
  2. **`ToolStripMenuItem`** - `"ControlType"` was always `Menu item`
  3. **`ToolStripSplitButton`** - `"ControlType"` was always `Button`
  4. **`ToolStripDropDownButton`** - `"ControlType"` was always `Button`

- **`RichTextBox`**: at this moment, we don't provide a managed accessible object for it. 
It doesn't support setting a custom `AccessibleRole` and 
changing `"ControlType"` respectively (in **.NET Framework 4.7.2** too). 
It always has `"Role"` as `Document` and `"ControlType"` as `Document`.
- **`TreeView`**: at this moment, we don't provide a managed accessible object for it. 
It doesn't support setting a custom `AccessibleRole` and 
changing `"ControlType"` respectively (in **.NET Framework 4.7.2** too). 
It always has `"Role"` value as `Outline` and `"ControlType"` value as `Tree`.
- **`DateTimePicker`**: its accessible object has non-standard 
`"LocalizedControlType"` accessibility property value by default. 
But it changes if to change `AccessibleRole` property value of the control.
- **`Form`**: default `"Role"` value is `Client` but default `"ControlType"` value
is `Window`. This is an exception and does not correspond to the list above.
In the rest, `Form` with a custom `AccessibleRole` value works as expected.
- **`MonthCalendar`**: if `AccessibleName` property value is null or empty 
default `"ControlType"` of its accessible object is `Calendar`, 
if `AccessibleName` is a some text - default `"ControlType"` is `Table`.
- **`ToolStripLabel`**: if `IsLink` property value equals `false` 
its accessible object default `"ControlType"` value is `Text`, 
and `"ControlType"` is `Hyperlink` if `IsLink` is `true`.
- **`ToolStripStatusLabel`**: if `IsLink` property value equals `false` 
its accessible object default `"ControlType"` value is `Text`, 
and `"ControlType"` is `Hyperlink` if `IsLink` is `true`.
- **`ToolStripNumericUpDown`**: this item is used in [PrintPreviewDialog](https://docs.microsoft.com/dotnet/api/system.windows.forms.printpreviewdialog) control.
- **`ToolStripOverflowButton`**:  needs to use [ToolStrip.OverflowButton](https://docs.microsoft.com/dotnet/api/system.windows.forms.toolstrip.overflowbutton) 
property to get this item and set a custom `AccessibleRole` value.
- **`DataGridViewEditingPanel`**: default `"Role"` is `Client`. 
Needs to use [DataGridView.EditingPanel](https://docs.microsoft.com/dotnet/api/system.windows.forms.datagridview.editingpanel) to get the control and set 
a custom value for its `AccessibleRole`.

[inspect-accessibility-properties]: ../images/inspect-accessibility-properties.png
[accessibleobject-role-implementation]: ../images/accessibleobject-role-implementation.png
[accessibleobject-getpropertyvalue-implementation]: ../images/accessibleobject-getpropertyvalue-implementation.png