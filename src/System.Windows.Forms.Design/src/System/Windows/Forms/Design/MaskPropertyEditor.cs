// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  Design time editing class for the Mask property of the <see cref="MaskedTextBox"/> control.
/// </summary>
internal class MaskPropertyEditor : UITypeEditor
{
    /// <summary>
    ///  Gets the mask property value from the MaskDesignerDialog.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The <see cref="IUIService"/> is used to show the mask designer dialog within VS so it doesn't get
    ///   blocked if focus is moved to another app.
    ///  </para>
    /// </remarks>
    internal static string? EditMask(
        ITypeDiscoveryService? discoveryService,
        IUIService? uiService,
        MaskedTextBox instance,
        IHelpService? helpService)
    {
        Debug.Assert(instance is not null, "Null masked text box.");
        string? mask = null;

        // Launching modal dialog in System aware mode.
        using MaskDesignerDialog dialog = ScaleHelper.InvokeInSystemAwareContext(
            () => new MaskDesignerDialog(instance, helpService));

        dialog.DiscoverMaskDescriptors(discoveryService);  // fine if service is null.

        // Show dialog from VS.
        DialogResult result = uiService is not null ? uiService.ShowDialog(dialog) : dialog.ShowDialog();
        if (result == DialogResult.OK)
        {
            mask = dialog.Mask;

            // ValidatingType is not browsable so we don't need to set the property through the designer.
            if (dialog.ValidatingType != instance.ValidatingType)
            {
                instance.ValidatingType = dialog.ValidatingType;
            }
        }

        // Will return null if dlgResult != OK.
        return mask;
    }

    /// <summary>
    ///  Edits the Mask property of the MaskedTextBox control from the PropertyGrid.
    /// </summary>
    public override object? EditValue(System.ComponentModel.ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (context is null || provider is null)
        {
            return value;
        }

        string? mask = EditMask(
            provider.GetService<ITypeDiscoveryService>(),
            provider.GetService<IUIService>(),
            (context.Instance as MaskedTextBox)!,
            provider.GetService<IHelpService>());

        return mask ?? value;
    }

    /// <summary>
    ///  Painting a representation of the Mask value is not supported.
    /// </summary>
    public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext? context)
        => false;

    /// <inheritdoc />
    public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext? context)
        => UITypeEditorEditStyle.Modal;
}
