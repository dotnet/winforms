// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.Reflection;
using System.Windows.Forms;

namespace System.ComponentModel.Design.Serialization;

public abstract partial class CodeDomDesignerLoader
{
    /// <summary>
    ///  This extender provider offers up read-only versions of
    ///  "Modifiers" for inherited components.
    /// </summary>
    [ProvideProperty("Modifiers", typeof(IComponent))]
    private class ModifiersInheritedExtenderProvider : IExtenderProvider
    {
        private IDesignerHost? _host;

        /// <summary>
        ///  Determines if ths extender provider can extend the given object. We extend
        ///  all objects, so we always return true.
        /// </summary>
        public bool CanExtend(object o)
        {
            if (o is not IComponent component)
            {
                return false;
            }

            // We don't add modifiers to the base component.
            IComponent? baseComponent = GetBaseComponent(component);

            if (o == baseComponent)
            {
                return false;
            }

            // Now see if this object is inherited. If so, then we are interested in it.
            AttributeCollection attributes = TypeDescriptor.GetAttributes(o);

            if (!attributes[typeof(InheritanceAttribute)]!.Equals(InheritanceAttribute.NotInherited))
            {
                return true;
            }

            return false;
        }

        private IComponent? GetBaseComponent(IComponent? component)
        {
            if (component is null)
            {
                return null;
            }

            _host ??= component.Site?.GetService<IDesignerHost>();

            return _host?.RootComponent;
        }

        /// <summary>
        ///  This is an extender property that we offer to all components
        ///  on the form. It implements the "Modifiers" property, which
        ///  is an enum representing the "public/protected/private" scope
        ///  of a component.
        /// </summary>
        [DesignOnly(true)]
        [TypeConverter(typeof(ModifierConverter))]
        [DefaultValue(MemberAttributes.Private)]
        [SRDescription(nameof(SR.CodeDomDesignerLoaderPropModifiers))]
        [Category("Design")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MemberAttributes GetModifiers(IComponent comp)
        {
            IComponent? baseComponent = GetBaseComponent(comp);
            Debug.Assert(baseComponent is not null, "Root component was null");
            Type baseType = baseComponent.GetType();
            ISite? site = comp.Site;

            string? name = site?.Name;

            if (name is null)
            {
                return MemberAttributes.Private;
            }

            FieldInfo? field = TypeDescriptor.GetReflectionType(baseType).GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            if (field is not null)
            {
                if (field.IsPrivate)
                    return MemberAttributes.Private;

                if (field.IsPublic)
                    return MemberAttributes.Public;

                if (field.IsFamily)
                    return MemberAttributes.Family;

                if (field.IsAssembly)
                    return MemberAttributes.Assembly;

                if (field.IsFamilyOrAssembly)
                    return MemberAttributes.FamilyOrAssembly;

                if (field.IsFamilyAndAssembly)
                    return MemberAttributes.FamilyAndAssembly;
            }

            // Visual Basic uses a property called Foo and generates a field called _Foo. We need to check the
            // visibility of this accessor to fix the modifiers up.
            PropertyInfo? prop = TypeDescriptor.GetReflectionType(baseType).GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            MethodInfo?[]? accessors = prop?.GetAccessors(true);
            if (accessors is not [MethodInfo methodInfo, ..])
            {
                return MemberAttributes.Private;
            }

            if (methodInfo.IsPrivate)
                return MemberAttributes.Private;

            if (methodInfo.IsPublic)
                return MemberAttributes.Public;

            if (methodInfo.IsFamily)
                return MemberAttributes.Family;

            if (methodInfo.IsAssembly)
                return MemberAttributes.Assembly;

            if (methodInfo.IsFamilyOrAssembly)
                return MemberAttributes.FamilyOrAssembly;

            if (methodInfo.IsFamilyAndAssembly)
                return MemberAttributes.FamilyAndAssembly;

            return MemberAttributes.Private;
        }
    }
}
