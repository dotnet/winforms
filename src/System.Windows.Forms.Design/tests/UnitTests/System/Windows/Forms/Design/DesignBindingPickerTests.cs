// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using static System.Windows.Forms.Design.DesignBindingPicker;

namespace System.Windows.Forms.Design.Tests;

public class DesignBindingPickerTests : IDisposable
{
    private readonly DesignBindingPicker _picker;
    private readonly BindingPickerTree _treeViewCtrl;
    private readonly BindingPickerLink _addNewCtrl;
    private readonly Panel _addNewPanel;
    private readonly HelpTextLabel _helpTextCtrl;
    private readonly Panel _helpTextPanel;

    public DesignBindingPickerTests()
    {
        _picker = new DesignBindingPicker();
        _treeViewCtrl = _picker.TestAccessor().Dynamic._treeViewCtrl;
        _addNewCtrl = _picker.TestAccessor().Dynamic._addNewCtrl;
        _addNewPanel = _picker.TestAccessor().Dynamic._addNewPanel;
        _helpTextCtrl = _picker.TestAccessor().Dynamic._helpTextCtrl;
        _helpTextPanel = _picker.TestAccessor().Dynamic._helpTextPanel;
    }

    public void Dispose() => _picker.Dispose();

    [WinFormsFact]
    public void Ctor_InitializesControlsAndProperties()
    {
        // Check control hierarchy.
        var controls = _picker.Controls;

        controls.Contains(_treeViewCtrl).Should().BeTrue();
        controls.Contains(_addNewPanel).Should().BeTrue();
        controls.Contains(_helpTextPanel).Should().BeTrue();

        // Check some property values.
        Assert.Equal(SystemColors.Control, _picker.BackColor);
        Assert.Equal(SR.DesignBindingPickerAccessibleName, _picker.AccessibleName);
        Assert.Equal(_treeViewCtrl, _picker.ActiveControl);
    }

    [WinFormsFact]
    public void AddNewPanel_ContainsExpectedControls()
    {
        _addNewPanel.Controls.Contains(_addNewCtrl).Should().BeTrue();
        ContainsType<PictureBox>(_addNewPanel.Controls).Should().BeTrue();
        ContainsType<Label>(_addNewPanel.Controls).Should().BeTrue();
    }

    [WinFormsFact]
    public void HelpTextPanel_ContainsExpectedControls()
    {
        _helpTextPanel.Controls.Contains(_helpTextCtrl).Should().BeTrue();
        ContainsType<Label>(_helpTextPanel.Controls).Should().BeTrue();
    }

    private static bool ContainsType<T>(Control.ControlCollection collection)
    {
        foreach (var item in collection)
        {
            if (item is T)
                return true;
        }

        return false;
    }
}
