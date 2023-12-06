// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides an editor that can be used to visually select and configure the link area of a link label.
/// </summary>
internal partial class LinkAreaEditor : UITypeEditor
{
    private LinkAreaUI? _linkAreaUI;

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (!provider.TryGetService(out IWindowsFormsEditorService? editorService))
        {
            return value;
        }

        if (_linkAreaUI is null)
        {
            IHelpService? helpService = provider.GetService<IHelpService>();

            // Child modal dialog -launching in SystemAware mode.
            _linkAreaUI = ScaleHelper.InvokeInSystemAwareContext(() => new LinkAreaUI(helpService));
        }

        string? text = string.Empty;
        PropertyDescriptor? property = null;

        if (context?.Instance is not null)
        {
            property = TypeDescriptor.GetProperties(context.Instance)["Text"];
            if (property?.PropertyType == typeof(string))
            {
                text = (string?)property.GetValue(context.Instance);
            }
        }

        string? originalText = text;
        _linkAreaUI.SampleText = text;
        _linkAreaUI.Start(value);

        if (editorService.ShowDialog(_linkAreaUI) == DialogResult.OK)
        {
            value = _linkAreaUI.Value;

            text = _linkAreaUI.SampleText;
            if (!text.Equals(originalText) && property?.PropertyType == typeof(string))
            {
                property.SetValue(context!.Instance, text);
            }
        }

        _linkAreaUI.End();

        return value;
    }

    /// <inheritdoc />
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        => UITypeEditorEditStyle.Modal;
}
