// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class is here for IHTML*3.AttachHandler style eventing.
    ///  We need a way of routing requests for DISPID(0) to a particular CLR event without creating
    ///  a public class.  In order to accomplish this we implement IReflect and handle InvokeMethod
    ///  to call back on a CLR event handler.
    /// </summary>
    internal class HtmlToClrEventProxy : IReflect
    {
        private readonly EventHandler eventHandler;
        private readonly IReflect typeIReflectImplementation;
        private readonly string eventName;

        public HtmlToClrEventProxy(object sender, string eventName, EventHandler eventHandler)
        {
            this.eventHandler = eventHandler;
            this.eventName = eventName;

            Type htmlToClrEventProxyType = typeof(HtmlToClrEventProxy);
            typeIReflectImplementation = htmlToClrEventProxyType as IReflect;
        }

        public string EventName
        {
            get { return eventName; }
        }

        [DispId(0)]
        public void OnHtmlEvent()
        {
            InvokeClrEvent();
        }

        private void InvokeClrEvent()
        {
            eventHandler?.Invoke(null, EventArgs.Empty);
        }

        #region IReflect

        Type IReflect.UnderlyingSystemType
        {
            get
            {
                return typeIReflectImplementation.UnderlyingSystemType;
            }
        }

        // Methods
        FieldInfo IReflect.GetField(string name, BindingFlags bindingAttr)
        {
            return typeIReflectImplementation.GetField(name, bindingAttr);
        }
        FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr)
        {
            return typeIReflectImplementation.GetFields(bindingAttr);
        }
        MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr)
        {
            return typeIReflectImplementation.GetMember(name, bindingAttr);
        }
        MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
        {
            return typeIReflectImplementation.GetMembers(bindingAttr);
        }
        MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr)
        {
            return typeIReflectImplementation.GetMethod(name, bindingAttr);
        }
        MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
        {
            return typeIReflectImplementation.GetMethod(name, bindingAttr, binder, types, modifiers);
        }
        MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr)
        {
            return typeIReflectImplementation.GetMethods(bindingAttr);
        }
        PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
        {
            return typeIReflectImplementation.GetProperties(bindingAttr);
        }
        PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr)
        {
            return typeIReflectImplementation.GetProperty(name, bindingAttr);
        }
        PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            return typeIReflectImplementation.GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
        }

        // InvokeMember:
        // If we get a call for DISPID=0, fire the CLR event.
        //
        object IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, Globalization.CultureInfo culture, string[] namedParameters)
        {
            //
            if (name == "[DISPID=0]")
            {
                // we know we're getting called back to fire the event - translate this now into a CLR event.
                OnHtmlEvent();
                // since there's no return value for void, return null.
                return null;
            }
            else
            {
                return typeIReflectImplementation.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
            }
        }
        #endregion
    }
}
