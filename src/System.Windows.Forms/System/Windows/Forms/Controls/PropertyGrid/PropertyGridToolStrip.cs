// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Represents the PropertyGrid inner ToolStrip control.
///  Is used starting with Accessibility Improvements of level 3.
/// </summary>
internal partial class PropertyGridToolStrip : ToolStrip
{
    private readonly PropertyGrid _parentPropertyGrid;

    /// <summary>
    ///  Initializes new instance of PropertyGridToolStrip control.
    /// </summary>
    /// <param name="parentPropertyGrid">The parent PropertyGrid control.</param>
    public PropertyGridToolStrip(PropertyGrid parentPropertyGrid)
    {
        _parentPropertyGrid = parentPropertyGrid;
    }

    /// <summary>
    ///  Indicates whether or not the control supports UIA Providers via
    ///  IRawElementProviderFragment/IRawElementProviderFragmentRoot interfaces.
    /// </summary>
    internal override bool SupportsUiaProviders => true;

    /// <summary>
    ///  Constructs the new instance of the accessibility object for this control.
    /// </summary>
    /// <returns>The accessibility object for this control.</returns>
    protected override AccessibleObject CreateAccessibilityInstance()
        => new PropertyGridToolStripAccessibleObject(this, _parentPropertyGrid);

    /// <summary>
    ///  Overrides WndProc to avoid keeping this inner ToolStrip as the active ToolStrip
    ///  in ModalMenuFilter when it receives focus.
    /// </summary>
    /// <param name="m">The window message to process.</param>
    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);

        if (m.MsgInternal == PInvokeCore.WM_SETFOCUS)
        {
            bool exitMenuMode = ToolStripManager.ModalMenuFilter.GetActiveToolStrip() == this;

            ToolStripManager.ModalMenuFilter.RemoveActiveToolStrip(this);

            if (exitMenuMode && ToolStripManager.ModalMenuFilter.GetActiveToolStrip() is null)
            {
                ToolStripManager.ModalMenuFilter.ExitMenuMode();
            }
        }
    }
}
