// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.PropertyGridInternal {
    using System.Runtime.InteropServices;

    using System.Diagnostics;

    using System;
    using System.ComponentModel.Design;
    using System.ComponentModel;
    using System.Windows.Forms.ComponentModel;
    using System.Windows.Forms.Design;
    using System.Collections;
    using Microsoft.Win32;

    /// <include file='doc\PropertiesTab.uex' path='docs/doc[@for="PropertiesTab"]/*' />
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Name="FullTrust")]
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Name="FullTrust")]
    public class PropertiesTab : PropertyTab {


        /// <include file='doc\PropertiesTab.uex' path='docs/doc[@for="PropertiesTab.TabName"]/*' />
        public override string TabName {
            get {
                return SR.PBRSToolTipProperties;
            }
        }
        
        /// <include file='doc\PropertiesTab.uex' path='docs/doc[@for="PropertiesTab.HelpKeyword"]/*' />
        public override string HelpKeyword {
            get {
                return "vs.properties"; // do not localize.
            }
        }

        /// <include file='doc\PropertiesTab.uex' path='docs/doc[@for="PropertiesTab.GetDefaultProperty"]/*' />
        public override PropertyDescriptor GetDefaultProperty(object obj) {
               PropertyDescriptor def = base.GetDefaultProperty(obj);

               if (def == null) {
                   PropertyDescriptorCollection props = GetProperties(obj);
                   if (props != null) {
                       for (int i = 0; i < props.Count; i++) {
                            if ("Name".Equals(props[i].Name)) {
                                def = props[i];
                                break;
                            }
                       }
                   }
               }
               return def;
        }

        /// <include file='doc\PropertiesTab.uex' path='docs/doc[@for="PropertiesTab.GetProperties"]/*' />
        public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes) {
               return GetProperties(null, component, attributes);
        }
        
        /// <include file='doc\PropertiesTab.uex' path='docs/doc[@for="PropertiesTab.GetProperties1"]/*' />
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes) {
            if (attributes == null) {
                attributes = new Attribute[]{BrowsableAttribute.Yes};
            }

            if (context == null) {
                return TypeDescriptor.GetProperties(component, attributes); 
            }
            else {
                TypeConverter tc = (context.PropertyDescriptor == null ? TypeDescriptor.GetConverter(component) : context.PropertyDescriptor.Converter);
                if (tc == null || !tc.GetPropertiesSupported(context)) {
                    return TypeDescriptor.GetProperties(component, attributes);
                }
                else {
                    return tc.GetProperties(context, component, attributes);
                }
            }
        }
    }
}
