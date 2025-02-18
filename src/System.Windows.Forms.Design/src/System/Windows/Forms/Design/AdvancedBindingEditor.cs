// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

/// <include file='doc\AdvancedBindingEditor.uex' path='docs/doc[@for="AdvancedBindingEditor"]/*' />
/// <devdoc>
///    <para>Provides an editor to edit advanced binding objects.</para>
/// </devdoc>
internal class AdvancedBindingEditor : UITypeEditor
{
    private BindingFormattingDialog bindingFormattingDialog;

    /// <include file='doc\AdvancedBindingEditor.uex' path='docs/doc[@for="AdvancedBindingEditor.EditValue"]/*' />
    /// <devdoc>
    ///    <para>Edits the specified value using the specified provider 
    ///       within the specified context.</para>
    /// </devdoc>
    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
        if (provider is not null)
        {
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            IDesignerHost host = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;

            if (edSvc is not null && host is not null)
            {
                using (DpiAwareness.EnterDpiScope(DpiAwarenessContext.SystemAware))
                {
                    if (bindingFormattingDialog is null)
                    {
                        bindingFormattingDialog = new BindingFormattingDialog();
                    }

                    bindingFormattingDialog.Context = context;
                    bindingFormattingDialog.Bindings = (ControlBindingsCollection)value;
                    bindingFormattingDialog.Host = host;

                    using (DesignerTransaction t = host.CreateTransaction())
                    {
                        edSvc.ShowDialog(bindingFormattingDialog);

                        if (bindingFormattingDialog.Dirty)
                        {
                            // since the bindings may have changed, the properties listed in the properties window
                            // need to be refreshed
                            System.Diagnostics.Debug.Assert(context.Instance is ControlBindingsCollection);
                            TypeDescriptor.Refresh(((ControlBindingsCollection)context.Instance).BindableComponent);

                            if (t is not null)
                            {
                                t.Commit();
                            }
                        }
                        else
                        {
                            t.Cancel();
                        }
                    }
                }
            }
        }

        return value;
    }

    /// <include file='doc\AdvancedBindingEditor.uex' path='docs/doc[@for="AdvancedBindingEditor.GetEditStyle"]/*' />
    /// <devdoc>
    ///    <para>Gets the edit style from the current context.</para>
    /// </devdoc>
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
        return UITypeEditorEditStyle.Modal;
    }
}
