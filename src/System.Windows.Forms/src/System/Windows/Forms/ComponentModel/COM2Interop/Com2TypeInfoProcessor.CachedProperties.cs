// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal static partial class Com2TypeInfoProcessor
    {
        internal class CachedProperties
        {
            private readonly PropertyDescriptor[] _properties;

            public readonly uint MajorVersion;
            public readonly uint MinorVersion;

            internal CachedProperties(PropertyDescriptor[] props, int defIndex, uint majVersion, uint minVersion)
            {
                _properties = ClonePropertyDescriptors(props);
                MajorVersion = majVersion;
                MinorVersion = minVersion;
                DefaultIndex = defIndex;
            }

            public PropertyDescriptor[] Properties => ClonePropertyDescriptors(_properties);

            public int DefaultIndex { get; }

            private static PropertyDescriptor[] ClonePropertyDescriptors(PropertyDescriptor[] props)
            {
                PropertyDescriptor[] retProps = new PropertyDescriptor[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    retProps[i] = props[i] is ICloneable cloneable ? (PropertyDescriptor)cloneable.Clone() : props[i];
                }

                return retProps;
            }
        }
    }
}
