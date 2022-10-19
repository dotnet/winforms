﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.Windows.Forms;

namespace System.ComponentModel.Design.Serialization
{
    public abstract partial class CodeDomDesignerLoader
    {
        /// <summary>
        ///  This extender provider provides the "Modifiers" property.
        /// </summary>
        [ProvideProperty("Modifiers", typeof(IComponent))]
        [ProvideProperty("GenerateMember", typeof(IComponent))]
        private class ModifiersExtenderProvider : IExtenderProvider
        {
            private IDesignerHost _host;

            /// <summary>
            ///  Determines if ths extender provider can extend the given object.  We extend
            ///  all objects, so we always return true.
            /// </summary>
            public bool CanExtend(object o)
            {
                if (!(o is IComponent c))
                {
                    return false;
                }

                // We don't add modifiers to the base component.
                IComponent baseComponent = GetBaseComponent(c);

                if (o == baseComponent)
                {
                    return false;
                }

                // Now see if this object is inherited.  If so, then we don't want to
                // extend.
                if (!TypeDescriptor.GetAttributes(o)[typeof(InheritanceAttribute)].Equals(InheritanceAttribute.NotInherited))
                {
                    return false;
                }

                return true;
            }

            private IComponent GetBaseComponent(IComponent c)
            {
                IComponent baseComponent = null;

                if (c is null)
                {
                    return null;
                }

                if (_host is null)
                {
                    ISite site = c.Site;

                    if (site is not null)
                    {
                        _host = (IDesignerHost)site.GetService(typeof(IDesignerHost));
                    }
                }

                if (_host is not null)
                {
                    baseComponent = _host.RootComponent;
                }

                return baseComponent;
            }

            /// <summary>
            ///  This is an extender property that we offer to all components
            ///  on the form.  It implements the "GenerateMember" property, which
            ///  is a boolean that, if true, causes a field member to be generated for
            ///  the object.
            /// </summary>
            [DesignOnly(true)]
            [DefaultValue(true)]
            [SRDescription(nameof(SR.CodeDomDesignerLoaderPropGenerateMember))]
            [Category("Design")]
            [HelpKeyword("Designer_GenerateMember")]
            public static bool GetGenerateMember(IComponent comp)
            {
                ISite site = comp.Site;

                if (site is null)
                {
                    return true;
                }

                IDictionaryService dictionary = (IDictionaryService)site.GetService(typeof(IDictionaryService));

                if (dictionary is not null)
                {
                    object value = dictionary.GetValue("GenerateMember");

                    if (value is bool)
                    {
                        return (bool)value;
                    }
                }

                return true;
            }

            /// <summary>
            ///  This is an extender property that we offer to all components
            ///  on the form.  It implements the "Modifiers" property, which
            ///  is an enum representing the "public/protected/private" scope
            ///  of a component.
            /// </summary>
            [DesignOnly(true)]
            [TypeConverter(typeof(ModifierConverter))]
            [DefaultValue(MemberAttributes.Private)]
            [SRDescription(nameof(SR.CodeDomDesignerLoaderPropModifiers))]
            [Category("Design")]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            [HelpKeyword("Designer_Modifiers")]
            public static MemberAttributes GetModifiers(IComponent comp)
            {
                ISite site = comp.Site;

                if (site is not null)
                {
                    IDictionaryService dictionary = (IDictionaryService)site.GetService(typeof(IDictionaryService));

                    if (dictionary is not null)
                    {
                        object value = dictionary.GetValue("Modifiers");

                        if (value is MemberAttributes)
                        {
                            return (MemberAttributes)value;
                        }
                    }
                }

                // Check to see if someone offered up a "DefaultModifiers" property so we can
                // decide a default.
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(comp);
                PropertyDescriptor prop = props["DefaultModifiers"];

                if (prop is not null && prop.PropertyType == typeof(MemberAttributes))
                {
                    return (MemberAttributes)prop.GetValue(comp);
                }

                return MemberAttributes.Private;
            }

            /// <summary>
            ///  This is an extender property that we offer to all components
            ///  on the form.  It implements the "GenerateMember" property, which
            ///  is a boolean that, if true, causes a field member to be generated for
            ///  the object.
            /// </summary>
            public static void SetGenerateMember(IComponent comp, bool generate)
            {
                ISite site = comp.Site;

                if (site is null)
                {
                    return;
                }

                IDictionaryService dictionary = (IDictionaryService)site.GetService(typeof(IDictionaryService));
                bool oldValue = GetGenerateMember(comp);

                dictionary?.SetValue("GenerateMember", generate);

                // If the old value was true and the new value is false, we've got
                // to remove the existing member declaration for this
                // component
                if (!oldValue || generate)
                {
                    return;
                }

                string compName = site.Name;

                if (!(site.GetService(typeof(CodeTypeDeclaration)) is CodeTypeDeclaration typeDecl) || compName is null)
                {
                    return;
                }

                foreach (CodeTypeMember member in typeDecl.Members)
                {
                    if (member is CodeMemberField field && field.Name.Equals(compName))
                    {
                        typeDecl.Members.Remove(field);
                        break;
                    }
                }
            }

            /// <summary>
            ///  This is an extender property that we offer to all components
            ///  on the form.  It implements the "Modifiers" property, which
            ///  is an enum representing the "public/protected/private" scope
            ///  of a component.
            /// </summary>
            public static void SetModifiers(IComponent comp, MemberAttributes modifiers)
            {
                ISite site = comp.Site;

                if (site is null)
                {
                    return;
                }

                IDictionaryService dictionary = (IDictionaryService)site.GetService(typeof(IDictionaryService));

                dictionary?.SetValue("Modifiers", modifiers);
            }
        }
    }
}
