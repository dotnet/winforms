// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
    /// <summary>
    /// Design time editing class for the Mask property of the MaskedTextBox control.   
    /// </summary>
    internal class MaskPropertyEditor : UITypeEditor
    {
        /// <summary>
        /// Gets the mask property value fromt the MaskDesignerDialog.  
        /// The IUIService is used to show the mask designer dialog within VS so it doesn't get blocked if focus 
        /// is moved to anoter app.
        /// </summary>
        internal static string EditMask(ITypeDiscoveryService discoverySvc, IUIService uiSvc, MaskedTextBox instance, IHelpService helpService)
        {
            Debug.Assert(instance != null, "Null masked text box.");
            string mask = null;

            // Launching modal dialog in System aware mode.
            MaskDesignerDialog dlg = DpiHelper.CreateInstanceInSystemAwareContext(() => new MaskDesignerDialog(instance, helpService));
            try
            {
                dlg.DiscoverMaskDescriptors(discoverySvc);  // fine if service is null.

                // Show dialog from VS.
                // Debug.Assert( uiSvc != null, "Expected IUIService, defaulting to an intrusive way to show the dialog..." );
                DialogResult dlgResult = uiSvc != null ? uiSvc.ShowDialog(dlg) : dlg.ShowDialog();
                if (dlgResult == DialogResult.OK)
                {
                    mask = dlg.Mask;

                    // ValidatingType is not browsable so we don't need to set the property through the designer.
                    if (dlg.ValidatingType != instance.ValidatingType)
                    {
                        instance.ValidatingType = dlg.ValidatingType;
                    }
                }
            }
            finally
            {
                dlg.Dispose();
            }

            // Will return null if dlgResult != OK.
            return mask;
        }

        /// <summary>
        /// Edits the Mask property of the MaskedTextBox control from the PropertyGrid.
        /// </summary>
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context == null || provider == null)
            {
                return value;
            }

            ITypeDiscoveryService discoverySvc = (ITypeDiscoveryService)provider.GetService(typeof(ITypeDiscoveryService));  // fine if service is not found.
            IUIService uiSvc = (IUIService)provider.GetService(typeof(IUIService));
            IHelpService helpService = (IHelpService)provider.GetService(typeof(IHelpService));
            string mask = MaskPropertyEditor.EditMask(discoverySvc, uiSvc, context.Instance as MaskedTextBox, helpService);

            if (mask != null)
            {
                return mask;
            }

            return value;
        }

        /// <summary>
        /// Painting a representation of the Mask value is not supported.
        /// </summary>
        public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
            => false;

        /// <summary>
        /// Gets the edit style of the type editor.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
            => UITypeEditorEditStyle.Modal;

    }
}
