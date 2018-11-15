// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.ComponentModel.Design;    
    using Microsoft.Win32;
    using System.Collections;
    using System.Drawing.Design;
    
    internal class Com2AboutBoxPropertyDescriptor : Com2PropertyDescriptor {
        private TypeConverter converter;
        private UITypeEditor  editor;
    
        public Com2AboutBoxPropertyDescriptor() : base(NativeMethods.ActiveX.DISPID_ABOUTBOX, "About", new Attribute[]{new DispIdAttribute(NativeMethods.ActiveX.DISPID_ABOUTBOX), 
                                                                                      DesignerSerializationVisibilityAttribute.Hidden, 
                                                                                      new DescriptionAttribute(SR.AboutBoxDesc), 
                                                                                      new ParenthesizePropertyNameAttribute(true)}, true, typeof(string), null, false) {
        }
    
        /// <include file='doc\COM2AboutBoxPropertyDescriptor.uex' path='docs/doc[@for="Com2AboutBoxPropertyDescriptor.ComponentType"]/*' />
        /// <devdoc>
        ///     Retrieves the type of the component this PropertyDescriptor is bound to.
        /// </devdoc>
        public override Type ComponentType {
            get {
               return typeof(UnsafeNativeMethods.IDispatch);
            }
        }
        
        
        /// <include file='doc\COM2AboutBoxPropertyDescriptor.uex' path='docs/doc[@for="Com2AboutBoxPropertyDescriptor.Converter"]/*' />
        /// <devdoc>
        ///      Retrieves the type converter for this property.
        /// </devdoc>
        public override TypeConverter Converter {
            get {
                if (converter == null) {
                    converter = new TypeConverter();
                }       
                return converter;
            }
        }
        /// <include file='doc\COM2AboutBoxPropertyDescriptor.uex' path='docs/doc[@for="Com2AboutBoxPropertyDescriptor.IsReadOnly"]/*' />
        /// <devdoc>
        ///     Indicates whether this property is read only.
        /// </devdoc>
        public override bool IsReadOnly { 
            get {
               return true;
            }
        }

        /// <include file='doc\COM2AboutBoxPropertyDescriptor.uex' path='docs/doc[@for="Com2AboutBoxPropertyDescriptor.PropertyType"]/*' />
        /// <devdoc>
        ///     Retrieves the type of the property.
        /// </devdoc>
        public override Type PropertyType {
            get {
               return typeof(string);
            }
        }
        
        /// <include file='doc\COM2AboutBoxPropertyDescriptor.uex' path='docs/doc[@for="Com2AboutBoxPropertyDescriptor.CanResetValue"]/*' />
        /// <devdoc>
        ///     Indicates whether reset will change the value of the component.  If there
        ///     is a DefaultValueAttribute, then this will return true if getValue returns
        ///     something different than the default value.  If there is a reset method and
        ///     a shouldPersist method, this will return what shouldPersist returns.
        ///     If there is just a reset method, this always returns true.  If none of these
        ///     cases apply, this returns false.
        /// </devdoc>
        public override bool CanResetValue(object component) {
            return false;
        }
    
        /// <include file='doc\COM2AboutBoxPropertyDescriptor.uex' path='docs/doc[@for="Com2AboutBoxPropertyDescriptor.GetEditor"]/*' />
        /// <devdoc>
        ///      Retrieves an editor of the requested type.
        /// </devdoc>
        public override object GetEditor(Type editorBaseType) {
            if (editorBaseType == typeof(UITypeEditor)) {
                if (editor == null) {
                    editor = new AboutBoxUITypeEditor();
                }
            }
            
            return editor;
        }

        /// <include file='doc\COM2AboutBoxPropertyDescriptor.uex' path='docs/doc[@for="Com2AboutBoxPropertyDescriptor.GetValue"]/*' />
        /// <devdoc>
        ///     Retrieves the current value of the property on component,
        ///     invoking the getXXX method.  An exception in the getXXX
        ///     method will pass through.
        /// </devdoc>
        public override object GetValue(object component) {
            return "";
        }

        /// <include file='doc\COM2AboutBoxPropertyDescriptor.uex' path='docs/doc[@for="Com2AboutBoxPropertyDescriptor.ResetValue"]/*' />
        /// <devdoc>
        ///     Will reset the default value for this property on the component.  If
        ///     there was a default value passed in as a DefaultValueAttribute, that
        ///     value will be set as the value of the property on the component.  If
        ///     there was no default value passed in, a ResetXXX method will be looked
        ///     for.  If one is found, it will be invoked.  If one is not found, this
        ///     is a nop.
        /// </devdoc>
        public override void ResetValue(object component){
        }

        /// <include file='doc\COM2AboutBoxPropertyDescriptor.uex' path='docs/doc[@for="Com2AboutBoxPropertyDescriptor.SetValue"]/*' />
        /// <devdoc>
        ///     This will set value to be the new value of this property on the
        ///     component by invoking the setXXX method on the component.  If the
        ///     value specified is invalid, the component should throw an exception
        ///     which will be passed up.  The component designer should design the
        ///     property so that getXXX following a setXXX should return the value
        ///     passed in if no exception was thrown in the setXXX call.
        /// </devdoc>        
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public override void SetValue(object component, object value) {
            throw new ArgumentException();
        }

        /// <include file='doc\COM2AboutBoxPropertyDescriptor.uex' path='docs/doc[@for="Com2AboutBoxPropertyDescriptor.ShouldSerializeValue"]/*' />
        /// <devdoc>
        ///     Indicates whether the value of this property needs to be persisted. In
        ///     other words, it indicates whether the state of the property is distinct
        ///     from when the component is first instantiated. If there is a default
        ///     value specified in this PropertyDescriptor, it will be compared against the
        ///     property's current value to determine this.  If there is't, the
        ///     shouldPersistXXX method is looked for and invoked if found.  If both
        ///     these routes fail, true will be returned.
        ///
        ///     If this returns false, a tool should not persist this property's value.
        /// </devdoc>
        public override bool ShouldSerializeValue(object component) {
            return false;
        }
        
        public class AboutBoxUITypeEditor : UITypeEditor {
            /// <include file='doc\COM2AboutBoxPropertyDescriptor.uex' path='docs/doc[@for="Com2AboutBoxPropertyDescriptor.AboutBoxUITypeEditor.EditValue"]/*' />
            /// <devdoc>
            ///      Edits the given object value using the editor style provided by
            ///      GetEditorStyle.  A service provider is provided so that any
            ///      required editing services can be obtained.
            /// </devdoc>
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
                     object component = context.Instance;
                     
                     if (Marshal.IsComObject(component) && component is UnsafeNativeMethods.IDispatch) {
                        UnsafeNativeMethods.IDispatch pDisp = (UnsafeNativeMethods.IDispatch)component;
                        NativeMethods.tagEXCEPINFO pExcepInfo = new NativeMethods.tagEXCEPINFO();
                        Guid g = Guid.Empty;
            
                        int hr = pDisp.Invoke(NativeMethods.ActiveX.DISPID_ABOUTBOX,
                                              ref g,
                                              SafeNativeMethods.GetThreadLCID(),
                                              NativeMethods.DISPATCH_METHOD,
                                              new NativeMethods.tagDISPPARAMS(),
                                              null,
                                              pExcepInfo, null);
                                              
                        Debug.Assert(NativeMethods.Succeeded(hr), "Failed to launch about box.");
                     }
                     return value;
            }
            
            /// <include file='doc\COM2AboutBoxPropertyDescriptor.uex' path='docs/doc[@for="Com2AboutBoxPropertyDescriptor.AboutBoxUITypeEditor.GetEditStyle"]/*' />
            /// <devdoc>
            ///      Retrieves the editing style of the Edit method.  If the method
            ///      is not supported, this will return None.
            /// </devdoc>
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
                     return UITypeEditorEditStyle.Modal;
            }

        }
    }
}
