// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.Windows.Forms.Design {
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Windows.Forms;
    using System.ComponentModel.Design;
    using System.Drawing;    
    using Microsoft.Win32;

    /// <include file='doc\WinFormsComponentEditor.uex' path='docs/doc[@for="WindowsFormsComponentEditor"]/*' />
    /// <devdoc>
    ///    <para> Provides a base class for editors that support any type 
    ///       of <see cref='System.ComponentModel.IComponent'/>
    ///       objects.</para>
    /// </devdoc>
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Name="FullTrust")]
    public abstract class WindowsFormsComponentEditor : ComponentEditor {
        
        /// <include file='doc\WinFormsComponentEditor.uex' path='docs/doc[@for="WindowsFormsComponentEditor.EditComponent"]/*' />
        /// <devdoc>
        ///    <para> Activates a UI used to edit the component.</para>
        /// </devdoc>
        public override bool EditComponent(ITypeDescriptorContext context, object component){
            return EditComponent(context, component, null);
        }
    
        /// <include file='doc\WinFormsComponentEditor.uex' path='docs/doc[@for="WindowsFormsComponentEditor.EditComponent1"]/*' />
        /// <devdoc>
        ///    <para> 
        ///       Activates the advanced UI used to edit the component.</para>
        /// </devdoc>
        public bool EditComponent(object component, IWin32Window owner) {
            return EditComponent(null, component, owner);
        }
        
        /// <include file='doc\WinFormsComponentEditor.uex' path='docs/doc[@for="WindowsFormsComponentEditor.EditComponent2"]/*' />
        /// <devdoc>
        ///    <para> 
        ///       Activates the advanced UI used to edit the component.</para>
        /// </devdoc>
        public virtual bool EditComponent(ITypeDescriptorContext context, object component, IWin32Window owner) {
            bool changed = false;
            Type[] pageControlTypes = GetComponentEditorPages();

            if ((pageControlTypes != null) && (pageControlTypes.Length != 0)) {
                ComponentEditorForm form = new ComponentEditorForm(component,
                                                                   pageControlTypes);

                if (form.ShowForm(owner, GetInitialComponentEditorPageIndex()) == DialogResult.OK)
                    changed = true;
            }

            return changed;
        }

        /// <include file='doc\WinFormsComponentEditor.uex' path='docs/doc[@for="WindowsFormsComponentEditor.GetComponentEditorPages"]/*' />
        /// <devdoc>
        /// <para>Gets the set of <see cref='System.Windows.Forms.Design.ComponentEditorPage'/> pages to be used.</para>
        /// </devdoc>
        protected virtual Type[] GetComponentEditorPages() {
            return null;
        }

        /// <include file='doc\WinFormsComponentEditor.uex' path='docs/doc[@for="WindowsFormsComponentEditor.GetInitialComponentEditorPageIndex"]/*' />
        /// <devdoc>
        /// <para>Gets the index of the <see cref='System.Windows.Forms.Design.ComponentEditorPage'/> to be shown by default as the 
        ///    first active page.</para>
        /// </devdoc>
        protected virtual int GetInitialComponentEditorPageIndex() {
            return 0;
        }
    }
}
