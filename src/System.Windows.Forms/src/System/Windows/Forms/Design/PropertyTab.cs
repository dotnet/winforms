// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design {
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Drawing;    
    using System.Windows.Forms;
    using Microsoft.Win32;
    using System.Runtime.Versioning;

    /// <include file='doc\PropertyTab.uex' path='docs/doc[@for="PropertyTab"]/*' />
    /// <devdoc>
    ///    <para>Provides a base class for property tabs.</para>
    /// </devdoc>
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Name="FullTrust")]
    public abstract class PropertyTab : IExtenderProvider {

        private Object[] components; 
        private Bitmap   bitmap;
        private bool     checkedBmp;

        /// <include file='doc\PropertyTab.uex' path='docs/doc[@for="PropertyTab.Finalize"]/*' />
        ~PropertyTab() {
            Dispose(false);
        }

        // don't override this. Just put a 16x16 bitmap in a file with the same name as your class in your resources.
        /// <include file='doc\PropertyTab.uex' path='docs/doc[@for="PropertyTab.Bitmap"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets a bitmap to display in the property tab.</para>
        /// </devdoc>
        public virtual Bitmap Bitmap {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get {
                if (!checkedBmp && bitmap == null) {
                    string bmpName = GetType().Name + ".bmp";
                    try
                    {
                        bitmap = new Bitmap(GetType(), bmpName);
                    }
                    catch (Exception ex)
                    {
                        Debug.Fail("Failed to find bitmap '" + bmpName + "' for class " + GetType().FullName, ex.ToString());
                    }
                    checkedBmp = true;
                }
                return bitmap;
            }
        }

        // don't override this either.
        /// <include file='doc\PropertyTab.uex' path='docs/doc[@for="PropertyTab.Components"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the array of components the property tab is associated with.</para>
        /// </devdoc>
        public virtual Object[] Components {
            get {
                return components;
            }
            set {
                this.components = value;
            }
        }

        // okay.  Override this to give a good TabName.
        /// <include file='doc\PropertyTab.uex' path='docs/doc[@for="PropertyTab.TabName"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the name for the property tab.</para>
        /// </devdoc>
        public abstract string TabName {
            get;
        }
        
        /// <include file='doc\PropertyTab.uex' path='docs/doc[@for="PropertyTab.HelpKeyword"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the help keyword that is to be associated with this tab. This defaults
        ///       to the tab name.</para>
        /// </devdoc>
        public virtual string HelpKeyword {
            get {
                return TabName;
            }
        }

        // override this to reject components you don't want to support.
        /// <include file='doc\PropertyTab.uex' path='docs/doc[@for="PropertyTab.CanExtend"]/*' />
        /// <devdoc>
        ///    <para>Gets a value indicating whether the specified object be can extended.</para>
        /// </devdoc>
        public virtual bool CanExtend(Object extendee) {
            return true;
        }

        /// <include file='doc\PropertyTab.uex' path='docs/doc[@for="PropertyTab.Dispose"]/*' />
        public virtual void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <include file='doc\PropertyTab.uex' path='docs/doc[@for="PropertyTab.Dispose2"]/*' />
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (bitmap != null) {
                    bitmap.Dispose();
                    bitmap = null;
                }
            }
        }

        // return the default property item
        /// <include file='doc\PropertyTab.uex' path='docs/doc[@for="PropertyTab.GetDefaultProperty"]/*' />
        /// <devdoc>
        ///    <para>Gets the default property of the specified component.</para>
        /// </devdoc>
        public virtual PropertyDescriptor GetDefaultProperty(Object component) {
            return TypeDescriptor.GetDefaultProperty(component);
        }

        // okay, override this to return whatever you want to return... All properties must apply to component.
        /// <include file='doc\PropertyTab.uex' path='docs/doc[@for="PropertyTab.GetProperties"]/*' />
        /// <devdoc>
        ///    <para>Gets the properties of the specified component.</para>
        /// </devdoc>
        public virtual PropertyDescriptorCollection GetProperties(Object component) {
            return GetProperties(component, null);
        }

        /// <include file='doc\PropertyTab.uex' path='docs/doc[@for="PropertyTab.GetProperties1"]/*' />
        /// <devdoc>
        ///    <para>Gets the properties of the specified component which match the specified 
        ///       attributes.</para>
        /// </devdoc>
        public abstract PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes);

        /// <include file='doc\PropertyTab.uex' path='docs/doc[@for="PropertyTab.GetProperties2"]/*' />
        /// <devdoc>
        ///    <para>Gets the properties of the specified component...</para>
        /// </devdoc>
        public virtual PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes) {
             return GetProperties(component, attributes);
        }
    }
}

