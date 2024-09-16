// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal partial class DesignerExtenders
{
    /// <summary>
    ///  This is the base extender provider for all WinForms document
    ///  designers. It provides the "Name" property.
    /// </summary>
    [ProvideProperty("Name", typeof(IComponent))]
    private class NameExtenderProvider : IExtenderProvider
    {
        private IComponent? _baseComponent;

        /// <summary>
        ///  Creates a new DocumentExtenderProvider.
        /// </summary>
        internal NameExtenderProvider()
        {
        }

        protected IComponent? GetBaseComponent(object o)
        {
            if (_baseComponent is null)
            {
                ISite? site = ((IComponent)o).Site;
                if (site is not null)
                {
                    IDesignerHost? host = (IDesignerHost?)site.GetService(typeof(IDesignerHost));
                    if (host is not null)
                    {
                        _baseComponent = host.RootComponent;
                    }
                }
            }

            return _baseComponent;
        }

        /// <summary>
        ///  Determines if ths extender provider can extend the given object. We extend
        ///  all objects, so we always return true.
        /// </summary>
        public virtual bool CanExtend(object o)
        {
            // We always extend the root
            IComponent? baseComp = GetBaseComponent(o);
            if (baseComp == o)
            {
                return true;
            }

            // See if this object is inherited. If so, then we don't want to extend.
            if (!TypeDescriptor.GetAttributes(o)[typeof(InheritanceAttribute)]?.Equals(InheritanceAttribute.NotInherited) ?? false)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///  This is an extender property that we offer to all components
        ///  on the form. It implements the "Name" property.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ParenthesizePropertyName(true)]
        [MergableProperty(false)]
        [SRDescription(nameof(SR.DesignerPropName))]
        [Category("Design")]
        public virtual string? GetName(IComponent comp)
        {
            ISite? site = comp.Site;
            if (site is not null)
            {
                return site.Name;
            }

            return null;
        }

        /// <summary>
        ///  This is an extender property that we offer to all components
        ///  on the form. It implements the "Name" property.
        /// </summary>
        public static void SetName(IComponent comp, string newName)
        {
            ISite? site = comp.Site;
            if (site is not null)
            {
                site.Name = newName;
            }
        }
    }
}
