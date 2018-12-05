// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Windows.Forms;

    public class ListControlConvertEventArgs : ConvertEventArgs {
        object listItem;
        public ListControlConvertEventArgs(object value, Type desiredType, object listItem) : base(value, desiredType) {
            this.listItem = listItem;
        }

        public object ListItem {
            get {
                return this.listItem;
            }
        }
    }
}
