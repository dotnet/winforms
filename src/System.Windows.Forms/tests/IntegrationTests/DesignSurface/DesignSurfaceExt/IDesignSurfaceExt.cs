// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace DesignSurfaceExt;

public interface IDesignSurfaceExt
{
    // - perform Cut/Copy/Paste/Delete commands
    void DoAction(string command);

    // - de/activate the TabOrder facility
    void SwitchTabOrder();

    // - select the controls alignment mode
    void UseSnapLines();
    void UseGrid(Size gridSize);
    void UseGridWithoutSnapping(Size gridSize);
    void UseNoGuides();

    // - method useful to create control without the ToolBox facility
    TControl CreateRootComponent<TControl>(Size controlSize)
        where TControl : Control, IComponent;

    TControl CreateControl<TControl>(Size controlSize, Point controlLocation)
        where TControl : Control;

    // - Get the UndoEngineExtended object
    UndoEngineExt GetUndoEngineExt();

    // - Get the IDesignerHost of the .NET 2.0 DesignSurface
    IDesignerHost GetIDesignerHost();

    // - the View of the .NET 2.0 DesignSurface is just a Control
    // - you can manipulate this Control just like any other WinForms Control
    // - (you can dock it and add it to another Control just to display it)
    // - Get the View
    Control GetView();
}
