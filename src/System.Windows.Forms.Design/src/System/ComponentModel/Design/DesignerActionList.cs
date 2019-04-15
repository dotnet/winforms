﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;

namespace System.ComponentModel.Design
{
    /// <summary>
    /// DesignerActionList is the abstract base class which control authors inherit from to create a task sheet.
    /// Typical usage is to add properties and methods and then implement the abstract
    /// GetSortedActionItems method to return an array of DesignerActionItems in the order they are to be displayed.
    /// </summary>
    public class DesignerActionList
    {
        public DesignerActionList(IComponent component)
        {
            Component = component;
        }

        public virtual bool AutoShow { get; set; }

        public IComponent Component { get; }

        public object GetService(Type serviceType)
        {
            return Component?.Site?.GetService(serviceType);
        }

        public virtual DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new SortedList<string, DesignerActionItem>();

            // we want to ignore the public methods and properties for THIS class (only take the inherited ones)
            IList<MethodInfo> originalMethods = Array.AsReadOnly(typeof(DesignerActionList).GetMethods(BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public));
            IList<PropertyInfo> originalProperties = Array.AsReadOnly(typeof(DesignerActionList).GetProperties(BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public));

            // Do methods
            MethodInfo[] methods = GetType().GetMethods(BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
            foreach (MethodInfo info in methods)
            {
                if (originalMethods.Contains(info))
                {
                    continue;
                }

                // Make sure there are only methods that take no parameters
                if (info.GetParameters().Length == 0 && !info.IsSpecialName)
                {
                    GetMemberDisplayProperties(info, out string dispName, out string desc, out string cat);
                    items.Add(info.Name, new DesignerActionMethodItem(this, info.Name, dispName, cat, desc));
                }
            }

            // Do properties
            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
            foreach (PropertyInfo info in properties)
            {
                if (originalProperties.Contains(info))
                {
                    continue;
                }

                GetMemberDisplayProperties(info, out string dispName, out string desc, out string cat);
                items.Add(dispName, new DesignerActionPropertyItem(info.Name, dispName, cat, desc));
            }

            var returnValue = new DesignerActionItemCollection();
            foreach (DesignerActionItem dai in items.Values)
            {
                returnValue.Add(dai);
            }

            return returnValue;
        }

        private object GetCustomAttribute(MemberInfo info, Type attributeType)
        {
            object[] attributes = info.GetCustomAttributes(attributeType, true);
            return attributes.Length > 0 ? attributes[0] : null;
        }

        private void GetMemberDisplayProperties(MemberInfo info, out string displayName, out string description, out string category)
        {
            displayName = string.Empty;
            description = string.Empty;
            category = string.Empty;

            if (GetCustomAttribute(info, typeof(DescriptionAttribute)) is DescriptionAttribute descAttr)
            {
                description = descAttr.Description;
            }

            DisplayNameAttribute dispNameAttr = GetCustomAttribute(info, typeof(DisplayNameAttribute)) as DisplayNameAttribute;
            if (dispNameAttr != null)
            {
                displayName = dispNameAttr.DisplayName;
            }

            CategoryAttribute catAttr = GetCustomAttribute(info, typeof(CategoryAttribute)) as CategoryAttribute;
            if (dispNameAttr != null)
            {
                category = catAttr.Category;
            }

            if (string.IsNullOrEmpty(displayName))
            {
                displayName = info.Name;
            }
        }
    }
}
