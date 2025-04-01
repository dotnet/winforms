// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.ComponentModel;

namespace Test
{
    // Custom IComponent interface in a different namespace
    // This should not be detected by the analyzer
    namespace CustomComponents
    {
        public interface IComponent : IDisposable
        {
            ISite Site { get; set; }
            event EventHandler Disposed;
        }

        public interface ISite : IServiceProvider
        {
            IComponent Component { get; }
            IContainer Container { get; }
            bool DesignMode { get; }
            string Name { get; set; }
        }

        public interface IContainer : IDisposable
        {
            ComponentCollection Components { get; }
            void Add(IComponent component);
            void Add(IComponent component, string name);
            void Remove(IComponent component);
        }

        public class ComponentCollection
        {
            // Implementation omitted
        }

        // Component implementing the custom IComponent
        // Properties here should not be flagged
        public class CustomComponent : CustomComponents.IComponent
        {
            private ISite _site;

            public ISite Site
            {
                get { return _site; }
                set { _site = value; }
            }

            // This should not be flagged because it's from a custom IComponent
            public string CustomProperty { get; set; }

            public event EventHandler Disposed;

            public void Dispose()
            {
                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    // Component implementing System.ComponentModel.IComponent
    public class MyComponent : System.ComponentModel.IComponent
    {
        private System.ComponentModel.ISite _site;

        public System.ComponentModel.ISite Site
        {
            get { return _site; }
            set { _site = value; }
        }

        public event EventHandler Disposed;

        // This should not be flagged because it's static
        public static string StaticProperty { get; set; }

        // This should not be flagged because it has a private setter
        public string PrivateSetterProperty { get; private set; }

        // This should not be flagged because it's internal with a private setter
        internal string InternalPrivateSetterProperty { get; private set; }

        // This WOULD be flagged in a normal scenario (public read/write property)
        public string RegularProperty { get; set; }

        public void Dispose()
        {
            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }

    // Base component with properly attributed properties
    public class BaseComponent : System.ComponentModel.IComponent
    {
        private System.ComponentModel.ISite _site;

        public System.ComponentModel.ISite Site
        {
            get { return _site; }
            set { _site = value; }
        }

        public event EventHandler Disposed;

        // Properly attributed with DesignerSerializationVisibility
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string AttributedProperty { get; set; }

        // Properly attributed with DefaultValue
        [DefaultValue("Default")]
        public virtual string DefaultValueProperty { get; set; }

        public void Dispose()
        {
            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }

    // Derived component with overridden properties
    public class DerivedComponent : BaseComponent
    {
        // These should not be flagged because they are overrides
        // and the base property is already properly attributed
        public override string AttributedProperty { get; set; }
        public override string DefaultValueProperty { get; set; }
    }
}
