// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Security;
using System.Runtime.InteropServices;
using System.Net;
using System.Globalization;
using System.Collections;
using System.Reflection;

namespace System.Windows.Forms {


    /// <devdoc>
    ///  This class is here for IHTML*3.AttachHandler style eventing.
    ///  We need a way of routing requests for DISPID(0) to a particular CLR event without creating
    ///  a public class.  In order to accomplish this we implement IReflect and handle InvokeMethod
    ///  to call back on a CLR event handler.
    /// </devdoc>

    internal class HtmlToClrEventProxy : IReflect {
        private EventHandler eventHandler;
        private IReflect typeIReflectImplementation;
        private object sender = null;
        private string eventName;
        
        public HtmlToClrEventProxy(object sender, string eventName, EventHandler eventHandler) {
            this.eventHandler = eventHandler;
            this.eventName = eventName;

            Type htmlToClrEventProxyType = typeof(HtmlToClrEventProxy);
            typeIReflectImplementation = htmlToClrEventProxyType as IReflect;
        }

        public string EventName {
            get { return eventName; }
        }
        
        [DispId(0)]
        public void OnHtmlEvent() {
            InvokeClrEvent();
        }

        private void InvokeClrEvent() {
            if (eventHandler != null) {
                eventHandler(sender, EventArgs.Empty);
            }
        }


#region IReflect
  
        Type IReflect.UnderlyingSystemType { 
            get {
                return typeIReflectImplementation.UnderlyingSystemType;
            }
        }

        // Methods
        System.Reflection.FieldInfo IReflect.GetField(string name, System.Reflection.BindingFlags bindingAttr) {
            return typeIReflectImplementation.GetField(name, bindingAttr);
        }
        System.Reflection.FieldInfo[] IReflect.GetFields(System.Reflection.BindingFlags bindingAttr) {
            return typeIReflectImplementation.GetFields(bindingAttr);
        }
        System.Reflection.MemberInfo[] IReflect.GetMember(string name, System.Reflection.BindingFlags bindingAttr) {
            return typeIReflectImplementation.GetMember(name, bindingAttr);
        }
        System.Reflection.MemberInfo[] IReflect.GetMembers(System.Reflection.BindingFlags bindingAttr){
            return typeIReflectImplementation.GetMembers(bindingAttr);
        }
        System.Reflection.MethodInfo IReflect.GetMethod(string name, System.Reflection.BindingFlags bindingAttr) {
            return typeIReflectImplementation.GetMethod(name, bindingAttr);
        }
        System.Reflection.MethodInfo IReflect.GetMethod(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, Type[] types, System.Reflection.ParameterModifier[] modifiers) {
            return typeIReflectImplementation.GetMethod(name, bindingAttr, binder, types, modifiers);
        }
        System.Reflection.MethodInfo[] IReflect.GetMethods(System.Reflection.BindingFlags bindingAttr) {
            return typeIReflectImplementation.GetMethods(bindingAttr);
        }
        System.Reflection.PropertyInfo[] IReflect.GetProperties(System.Reflection.BindingFlags bindingAttr) {
            return typeIReflectImplementation.GetProperties(bindingAttr);
        }
        System.Reflection.PropertyInfo IReflect.GetProperty(string name, System.Reflection.BindingFlags bindingAttr) {
            return typeIReflectImplementation.GetProperty(name, bindingAttr);
        }
        System.Reflection.PropertyInfo IReflect.GetProperty(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, Type returnType, Type[] types, System.Reflection.ParameterModifier[] modifiers) {
            return typeIReflectImplementation.GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
        }

        // InvokeMember:
        // If we get a call for DISPID=0, fire the CLR event.
        //
        object IReflect.InvokeMember(string name, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, object target, object[] args, System.Reflection.ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters) {

            // 
            if (name == "[DISPID=0]") {
                // we know we're getting called back to fire the event - translate this now into a CLR event.
                OnHtmlEvent();
                // since there's no return value for void, return null.
                return null;
            }
            else {
                return typeIReflectImplementation.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
            }
        }
        #endregion
    }
}
