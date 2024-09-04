// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

/// <summary>
/// Describes the list of actions that can be performed in the MaskedTextBox control from the
/// Chrome pannel.
/// </summary>
internal class MaskedTextBoxDesignerActionList : DesignerActionList
{
    private readonly MaskedTextBox _maskedTextBox;
    private readonly ITypeDiscoveryService? _discoverySvc;
    private readonly IUIService? _uiSvc;
    private readonly IHelpService? _helpService;

    /// <summary>
    /// Constructor receiving a MaskedTextBox control the action list applies to. The ITypeDiscoveryService
    /// service provider is used to populate the canned mask list control in the MaskDesignerDialog dialog and
    /// the IUIService provider is used to display the MaskDesignerDialog within VS.
    /// </summary>
    public MaskedTextBoxDesignerActionList(MaskedTextBoxDesigner designer)
        : base(designer.Component)
    {
        _maskedTextBox = (MaskedTextBox)designer.Component;
        _discoverySvc = GetService(typeof(ITypeDiscoveryService)) as ITypeDiscoveryService;
        _uiSvc = GetService(typeof(IUIService)) as IUIService;
        _helpService = GetService(typeof(IHelpService)) as IHelpService;

        if (_discoverySvc is null || _uiSvc is null)
        {
            Debug.Fail("could not get either ITypeDiscoveryService or IUIService");
        }
    }

    /// <summary>
    /// Pops up the Mask design dialog for the user to set the control's mask.
    /// </summary>
    public void SetMask()
    {
        string? mask = MaskPropertyEditor.EditMask(_discoverySvc, _uiSvc, _maskedTextBox, _helpService);

        if (mask is null)
        {
            return;
        }

        PropertyDescriptor? maskProperty = TypeDescriptor.GetProperties(_maskedTextBox)["Mask"];
        maskProperty?.SetValue(_maskedTextBox, mask);
    }

    /// <summary>
    /// Returns the control's action list items.
    /// </summary>
    public override DesignerActionItemCollection GetSortedActionItems()
    {
        DesignerActionItemCollection items = [new DesignerActionMethodItem(this, "SetMask", SR.MaskedTextBoxDesignerVerbsSetMaskDesc)];
        return items;
    }
}
