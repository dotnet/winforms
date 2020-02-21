// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public class ListControlConvertEventArgs : ConvertEventArgs
    {
        public ListControlConvertEventArgs(object value, Type desiredType, object listItem) : base(value, desiredType)
        {
            ListItem = listItem;
        }

        public object ListItem { get; }
    }
}
