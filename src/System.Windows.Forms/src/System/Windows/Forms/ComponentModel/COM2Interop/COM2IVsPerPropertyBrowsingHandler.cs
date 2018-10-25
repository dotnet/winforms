// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop {
    using System;    
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using Microsoft.Win32;

    /// <include file='doc\COM2IVsPerPropertyBrowsingHandler.uex' path='docs/doc[@for="Com2IVsPerPropertyBrowsingHandler"]/*' />
    /// <devdoc>
    /// This is the base class for handlers for Com2 extended browsing interface
    /// such as IPerPropertyBrowsing, etc.
    ///
    /// These handlers should be stateless.  That is, they should keep no refs to object
    /// and should only work on a give object and dispid.  That way all objects that
    /// support a give interface can share a handler.
    ///
    /// See Com2Properties for the array of handler classes to interface classes
    /// where handlers should be registered.
    /// </devdoc>
    [System.Security.SuppressUnmanagedCodeSecurityAttribute()]
    internal class Com2IVsPerPropertyBrowsingHandler: Com2ExtendedBrowsingHandler {

         /// <include file='doc\COM2IVsPerPropertyBrowsingHandler.uex' path='docs/doc[@for="Com2IVsPerPropertyBrowsingHandler.Interface"]/*' />
         /// <devdoc>
         /// The interface that this handler managers
         /// such as IPerPropertyBrowsing, IProvidePropertyBuilder, etc.
         /// </devdoc>
         public override Type Interface {
            get {
               return typeof(NativeMethods.IVsPerPropertyBrowsing);
            }
         }
         
         public static bool AllowChildProperties(Com2PropertyDescriptor propDesc) {
            if (propDesc.TargetObject is NativeMethods.IVsPerPropertyBrowsing) {
               bool pfHide = false;
               int hr = ((NativeMethods.IVsPerPropertyBrowsing)propDesc.TargetObject).DisplayChildProperties(propDesc.DISPID, ref pfHide);
               return (hr == NativeMethods.S_OK && pfHide);
            }
            return false;
         }

         /// <include file='doc\COM2IVsPerPropertyBrowsingHandler.uex' path='docs/doc[@for="Com2IVsPerPropertyBrowsingHandler.SetupPropertyHandlers"]/*' />
         /// <devdoc>
         /// Called to setup the property handlers on a given properties
         /// In this method, the handler will add listeners to the events that
         /// the Com2PropertyDescriptor surfaces that it cares about.
         /// </devdoc>
         public override void SetupPropertyHandlers(Com2PropertyDescriptor[] propDesc){
               if (propDesc == null){
                  return;
               }
               for (int i = 0; i < propDesc.Length; i++){
                  propDesc[i].QueryGetDynamicAttributes += new GetAttributesEventHandler(this.OnGetDynamicAttributes);
                  propDesc[i].QueryGetBaseAttributes += new GetAttributesEventHandler(this.OnGetBaseAttributes);
                  propDesc[i].QueryGetDisplayName += new GetNameItemEventHandler(this.OnGetDisplayName);
                  propDesc[i].QueryGetIsReadOnly += new GetBoolValueEventHandler(this.OnGetIsReadOnly);
                  
                  propDesc[i].QueryShouldSerializeValue += new GetBoolValueEventHandler(this.OnShouldSerializeValue);
                  propDesc[i].QueryCanResetValue += new GetBoolValueEventHandler(this.OnCanResetPropertyValue);
                  propDesc[i].QueryResetValue += new Com2EventHandler(this.OnResetPropertyValue);
                  
                  propDesc[i].QueryGetTypeConverterAndTypeEditor += new GetTypeConverterAndTypeEditorEventHandler(this.OnGetTypeConverterAndTypeEditor);
                  
               }
         }

         private void OnGetBaseAttributes(Com2PropertyDescriptor sender, GetAttributesEvent attrEvent){
             NativeMethods.IVsPerPropertyBrowsing vsObj = sender.TargetObject as NativeMethods.IVsPerPropertyBrowsing;

             if (vsObj == null) {
                 return;
             }

             // should we localize this?
             string[] pHelpString = new string[1];
             int hr = vsObj.GetLocalizedPropertyInfo(sender.DISPID, CultureInfo.CurrentCulture.LCID, null, pHelpString);
             if (hr == NativeMethods.S_OK && pHelpString[0] != null){
                attrEvent.Add(new DescriptionAttribute(pHelpString[0]));
             }
         }


         /// <include file='doc\COM2IVsPerPropertyBrowsingHandler.uex' path='docs/doc[@for="Com2IVsPerPropertyBrowsingHandler.OnGetAttributes"]/*' />
         /// <devdoc>
         /// Here is where we handle IVsPerPropertyBrowsing.GetLocalizedPropertyInfo and IVsPerPropertyBrowsing.   HideProperty
         /// such as IPerPropertyBrowsing, IProvidePropertyBuilder, etc.
         /// </devdoc>
         private void OnGetDynamicAttributes(Com2PropertyDescriptor sender, GetAttributesEvent attrEvent){

               if (sender.TargetObject is NativeMethods.IVsPerPropertyBrowsing){
                  NativeMethods.IVsPerPropertyBrowsing vsObj = (NativeMethods.IVsPerPropertyBrowsing)sender.TargetObject;

                  int hr = NativeMethods.S_OK;
                  
                  // we want to avoid allowing clients to force a bad property to be browsable,
                  // so we don't allow things that are marked as non browsable to become browsable,
                  // only the other way around.
                  //
                  if (sender.CanShow) {
                    // should we hide this?
                    bool pfHide = sender.Attributes[typeof(BrowsableAttribute)].Equals(BrowsableAttribute.No);
                  
                    hr = vsObj.HideProperty(sender.DISPID, ref pfHide);
                    if (hr == NativeMethods.S_OK){
                         attrEvent.Add(pfHide ? BrowsableAttribute.No : BrowsableAttribute.Yes);
                    }
                  } 

                  // should we show this
                  if (typeof(UnsafeNativeMethods.IDispatch).IsAssignableFrom(sender.PropertyType) && sender.CanShow){
                     bool pfDisplay = false;
                     hr = vsObj.DisplayChildProperties(sender.DISPID, ref pfDisplay);
                     if (hr == NativeMethods.S_OK && pfDisplay){
                           attrEvent.Add(BrowsableAttribute.Yes);
                     }
                  }
               }
               Debug.Assert(sender.TargetObject == null || sender.TargetObject is NativeMethods.IVsPerPropertyBrowsing, "Object is not " + Interface.Name + "!");
         }

         private void OnCanResetPropertyValue(Com2PropertyDescriptor sender, GetBoolValueEvent boolEvent) {
               if (sender.TargetObject is NativeMethods.IVsPerPropertyBrowsing) {
                    NativeMethods.IVsPerPropertyBrowsing target = (NativeMethods.IVsPerPropertyBrowsing)sender.TargetObject;
                    bool canReset = boolEvent.Value;
                    int hr = target.CanResetPropertyValue(sender.DISPID, ref canReset);

                    if (NativeMethods.Succeeded(hr)){
                        boolEvent.Value = canReset;
                    }
               }
               Debug.Assert(sender.TargetObject == null || sender.TargetObject is NativeMethods.IVsPerPropertyBrowsing, "Object is not " + Interface.Name + "!");
         }

         /// <include file='doc\COM2IVsPerPropertyBrowsingHandler.uex' path='docs/doc[@for="Com2IVsPerPropertyBrowsingHandler.OnGetDisplayName"]/*' />
         /// <devdoc>
         /// Here is where we handle IVsPerPropertyBrowsing.GetLocalizedPropertyInfo (part 2)
         /// </devdoc>
         private void OnGetDisplayName(Com2PropertyDescriptor sender, GetNameItemEvent nameItem){
               if (sender.TargetObject is NativeMethods.IVsPerPropertyBrowsing){
                  NativeMethods.IVsPerPropertyBrowsing vsObj = (NativeMethods.IVsPerPropertyBrowsing)sender.TargetObject;

                  // get the localized name, if applicable
                  string[] pNameString = new string[1];
                  int hr = vsObj.GetLocalizedPropertyInfo(sender.DISPID, CultureInfo.CurrentCulture.LCID, pNameString, null);
                  if (hr == NativeMethods.S_OK && pNameString[0] != null){
                     nameItem.Name = pNameString[0];
                  }
               }
               Debug.Assert(sender.TargetObject == null || sender.TargetObject is NativeMethods.IVsPerPropertyBrowsing, "Object is not " + Interface.Name + "!");
         }

         /// <include file='doc\COM2IVsPerPropertyBrowsingHandler.uex' path='docs/doc[@for="Com2IVsPerPropertyBrowsingHandler.OnGetIsReadOnly"]/*' />
         /// <devdoc>
         /// Here is where we handle IVsPerPropertyBrowsing.IsPropertyReadOnly
         /// </devdoc>
         private void OnGetIsReadOnly(Com2PropertyDescriptor sender, GetBoolValueEvent gbvevent){
               if (sender.TargetObject is NativeMethods.IVsPerPropertyBrowsing){
                  NativeMethods.IVsPerPropertyBrowsing vsObj = (NativeMethods.IVsPerPropertyBrowsing)sender.TargetObject;

                  // should we make this read only?
                  bool pfResult = false;
                  int hr = vsObj.IsPropertyReadOnly(sender.DISPID, ref pfResult);

                  if (hr == NativeMethods.S_OK){
                     gbvevent.Value = pfResult;
                  }
               }
         }
         
         /// <include file='doc\COM2IVsPerPropertyBrowsingHandler.uex' path='docs/doc[@for="Com2IVsPerPropertyBrowsingHandler.OnGetTypeConverterAndTypeEditor"]/*' />
         /// <devdoc>
         /// Here is where we handle IVsPerPropertyBrowsing.DisplayChildProperties
         /// </devdoc>
         private void OnGetTypeConverterAndTypeEditor(Com2PropertyDescriptor sender, GetTypeConverterAndTypeEditorEvent gveevent) {
            if (sender.TargetObject is NativeMethods.IVsPerPropertyBrowsing){

                  // we only do this for IDispatch types
                  if (sender.CanShow && typeof(UnsafeNativeMethods.IDispatch).IsAssignableFrom(sender.PropertyType)){
                     NativeMethods.IVsPerPropertyBrowsing vsObj = (NativeMethods.IVsPerPropertyBrowsing)sender.TargetObject;

                     // should we make this read only?
                     bool pfResult = false;
                     int hr = vsObj.DisplayChildProperties(sender.DISPID, ref pfResult);
                     
                     if (gveevent.TypeConverter is Com2IDispatchConverter){
                        gveevent.TypeConverter = new Com2IDispatchConverter(sender, (hr == NativeMethods.S_OK && pfResult));
                     }
                     else{
                        gveevent.TypeConverter = new Com2IDispatchConverter(sender, (hr == NativeMethods.S_OK && pfResult), gveevent.TypeConverter);
                     }
                  }
               }
               Debug.Assert(sender.TargetObject == null || sender.TargetObject is NativeMethods.IVsPerPropertyBrowsing, "Object is not " + Interface.Name + "!");
         }
         
         private void OnResetPropertyValue(Com2PropertyDescriptor sender, EventArgs e) {
               if (sender.TargetObject is NativeMethods.IVsPerPropertyBrowsing) {

                    
                    NativeMethods.IVsPerPropertyBrowsing target = (NativeMethods.IVsPerPropertyBrowsing)sender.TargetObject;
                    int dispid = sender.DISPID;
                    bool canReset = false;
                    int hr = target.CanResetPropertyValue(dispid, ref canReset);

                    if (NativeMethods.Succeeded(hr)){
                        target.ResetPropertyValue(dispid);
                    }
               }
               Debug.Assert(sender.TargetObject == null || sender.TargetObject is NativeMethods.IVsPerPropertyBrowsing, "Object is not " + Interface.Name + "!");
         }

         private void OnShouldSerializeValue(Com2PropertyDescriptor sender, GetBoolValueEvent gbvevent){
               if (sender.TargetObject is NativeMethods.IVsPerPropertyBrowsing){
                     NativeMethods.IVsPerPropertyBrowsing vsObj = (NativeMethods.IVsPerPropertyBrowsing)sender.TargetObject;

                     // by default we say it's default
                     bool pfResult = true;
                     int hr = vsObj.HasDefaultValue(sender.DISPID,ref pfResult);

                     if (hr == NativeMethods.S_OK && !pfResult){
                        // specify a default value editor
                        gbvevent.Value = true;
                     }
               }
               Debug.Assert(sender.TargetObject == null || sender.TargetObject is NativeMethods.IVsPerPropertyBrowsing, "Object is not " + Interface.Name + "!");
         }
    }
}
