// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal class Com2PropertyDescriptorRefresh
    {
        public const int Attributes = 0x0001;
        public const int DisplayName = 0x0002;
        public const int ReadOnly = 0x0004;
        public const int TypeConverter = 0x0020;
        public const int TypeEditor = 0x0040;

        public const int All = 0x00FF;

        public const int TypeConverterAttr = 0x2000;
        public const int TypeEditorAttr = 0x4000;
        public const int BaseAttributes = 0x8000;
    }
}
