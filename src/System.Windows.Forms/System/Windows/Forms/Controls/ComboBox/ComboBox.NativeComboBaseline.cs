// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class ComboBox
{
    /// <summary>
    ///  Captures the untouched native ComboBox layout for one handle lifetime.
    /// </summary>
    private readonly struct NativeComboBaseline
    {
        public bool IsCaptured { get; init; }

        public int DeviceDpi { get; init; }

        public int SelectionFieldItemHeight { get; init; }

        public int SelectionFieldFrameHeight { get; init; }

        public Rectangle EditBounds { get; init; }

        public Rectangle SimpleListBounds { get; init; }

        public Size ClientSize { get; init; }
    }
}
