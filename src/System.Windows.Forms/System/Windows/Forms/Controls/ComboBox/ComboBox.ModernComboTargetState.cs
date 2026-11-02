// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class ComboBox
{
    /// <summary>
    ///  Describes the complete native ComboBox target for the current managed state.
    /// </summary>
    private readonly struct ModernComboTargetState
    {
        public int SelectionFieldItemHeight { get; init; }

        public Padding EditMargins { get; init; }

        public Rectangle EditBounds { get; init; }

        public Rectangle SimpleListBounds { get; init; }
    }
}
