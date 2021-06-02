// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms
{
    public sealed partial class Application
    {
        public class ApplicationDefaults
        {
            internal ApplicationDefaults()
            {
                DefaultProperties = new Dictionary<(Type, string), object>();
            }

            private Dictionary<(Type, string), object> DefaultProperties { get; }

            internal bool TryAddDefaultValue<ComponentType>(string propertyName, object value) where ComponentType : Component
                => DefaultProperties.TryAdd((typeof(ComponentType), propertyName), value);

            internal bool TryGetDefaultValue<ComponentType>(string propertyName, out object value) where ComponentType : Component
            {
                if (DefaultProperties.TryGetValue((typeof(ComponentType), propertyName), out var valueInternal))
                {
                    value = valueInternal;
                    return true;
                }

                value = null;
                return false;
            }

            internal bool TryAddDefaultValue(string propertyName, object value)
                => DefaultProperties.TryAdd((typeof(Control), propertyName), value);

            internal bool TryGetDefaultValue(string propertyName, out object value)
            {
                if (DefaultProperties.TryGetValue((typeof(Control), propertyName), out var valueInternal))
                {
                    value = valueInternal;
                    return true;
                }

                value = null;
                return false;
            }

            internal T GetValueOrDefault<T>(string propertyName)
                => (T)DefaultProperties.GetValueOrDefault((typeof(Control), propertyName));

            internal T GetValueOrDefault<ControlType, T>(string propertyName) where ControlType : Component
                => (T)DefaultProperties.GetValueOrDefault((typeof(ControlType), propertyName));

            internal void Remove(string propertyName)
                => DefaultProperties.Remove((typeof(Control), propertyName));
        }
    }
}
