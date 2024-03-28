// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

public partial class PropertyGrid
{
    internal class SelectedObjectConverter : ReferenceConverter
    {
        public SelectedObjectConverter() : base(typeof(IComponent))
        {
        }
    }
}
