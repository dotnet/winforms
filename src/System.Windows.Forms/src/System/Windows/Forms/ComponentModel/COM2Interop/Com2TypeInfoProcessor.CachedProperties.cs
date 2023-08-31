// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal static partial class Com2TypeInfoProcessor
{
    internal class CachedProperties
    {
        private readonly Com2PropertyDescriptor[] _properties;

        public readonly uint MajorVersion;
        public readonly uint MinorVersion;

        internal CachedProperties(Com2PropertyDescriptor[] properties, int defaultIndex, uint majorVersion, uint minorVersion)
        {
            _properties = ClonePropertyDescriptors(properties);
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
            DefaultIndex = defaultIndex;
        }

        public Com2PropertyDescriptor[] Properties => ClonePropertyDescriptors(_properties);

        public int DefaultIndex { get; }

        private static Com2PropertyDescriptor[] ClonePropertyDescriptors(Com2PropertyDescriptor[] properties)
        {
            Com2PropertyDescriptor[] clonedProperties = new Com2PropertyDescriptor[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                clonedProperties[i] = properties[i] is ICloneable cloneable
                    ? (Com2PropertyDescriptor)cloneable.Clone()
                    : properties[i];
            }

            return clonedProperties;
        }
    }
}
