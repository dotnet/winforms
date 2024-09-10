// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.ComponentModel.Design.Serialization;

public sealed partial class CodeDomComponentSerializationService
{
    private sealed partial class CodeDomSerializationStore
    {
        private class PassThroughSerializationManager : IDesignerSerializationManager
        {
            private readonly HashSet<string> _resolved = [];
            private ResolveNameEventHandler? _resolveNameEventHandler;

            public PassThroughSerializationManager(DesignerSerializationManager manager) => Manager = manager;

            public DesignerSerializationManager Manager { get; }

            ContextStack IDesignerSerializationManager.Context
            {
                get => ((IDesignerSerializationManager)Manager).Context;
            }

            PropertyDescriptorCollection IDesignerSerializationManager.Properties
            {
                get => ((IDesignerSerializationManager)Manager).Properties;
            }

            event ResolveNameEventHandler IDesignerSerializationManager.ResolveName
            {
                add
                {
                    ((IDesignerSerializationManager)Manager).ResolveName += value;
                    _resolveNameEventHandler += value;
                }
                remove
                {
                    ((IDesignerSerializationManager)Manager).ResolveName -= value;
                    _resolveNameEventHandler -= value;
                }
            }

            event EventHandler IDesignerSerializationManager.SerializationComplete
            {
                add => ((IDesignerSerializationManager)Manager).SerializationComplete += value;
                remove => ((IDesignerSerializationManager)Manager).SerializationComplete -= value;
            }

            void IDesignerSerializationManager.AddSerializationProvider(IDesignerSerializationProvider provider)
            {
                ((IDesignerSerializationManager)Manager).AddSerializationProvider(provider);
            }

            object IDesignerSerializationManager.CreateInstance(Type type, ICollection? arguments, string? name, bool addToContainer)
            {
                return ((IDesignerSerializationManager)Manager).CreateInstance(type, arguments, name, addToContainer);
            }

            object? IDesignerSerializationManager.GetInstance(string name)
            {
                object? instance = ((IDesignerSerializationManager)Manager).GetInstance(name);

                // If an object is retrieved from the current container as a result of GetInstance(),
                // we need to make sure and fully deserialize it before returning it.
                // To do this, we will force a resolve on this name and not interfere the next time GetInstance()
                // is called with this component. This will force the component to completely deserialize.
                if (_resolveNameEventHandler is not null && instance is not null &&
                    Manager.PreserveNames && Manager.Container?.Components[name] is not null && _resolved.Add(name))
                {
                    _resolveNameEventHandler(this, new ResolveNameEventArgs(name));
                }

                return instance;
            }

            string? IDesignerSerializationManager.GetName(object value)
            {
                return ((IDesignerSerializationManager)Manager).GetName(value);
            }

            object? IDesignerSerializationManager.GetSerializer(Type? objectType, Type serializerType)
            {
                return ((IDesignerSerializationManager)Manager).GetSerializer(objectType, serializerType);
            }

            Type? IDesignerSerializationManager.GetType(string typeName)
            {
                return ((IDesignerSerializationManager)Manager).GetType(typeName);
            }

            void IDesignerSerializationManager.RemoveSerializationProvider(IDesignerSerializationProvider provider)
            {
                ((IDesignerSerializationManager)Manager).RemoveSerializationProvider(provider);
            }

            void IDesignerSerializationManager.ReportError(object errorInformation)
            {
                ((IDesignerSerializationManager)Manager).ReportError(errorInformation);
            }

            void IDesignerSerializationManager.SetName(object instance, string name)
            {
                ((IDesignerSerializationManager)Manager).SetName(instance, name);
            }

            object? IServiceProvider.GetService(Type serviceType)
            {
                return ((IDesignerSerializationManager)Manager).GetService(serviceType);
            }
        }
    }
}
