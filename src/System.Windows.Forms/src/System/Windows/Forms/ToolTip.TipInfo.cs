﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    public partial class ToolTip
    {
        private class TipInfo
        {
            private readonly string? _designerText;
            private string? _caption;

            public TipInfo(string caption, Type type)
            {
                _caption = caption;
                TipType = type;
                if (type == Type.Auto)
                {
                    _designerText = caption;
                }
            }

            public string? Caption
            {
                get => ((TipType & (Type.Absolute | Type.SemiAbsolute)) != 0) ? _caption : _designerText;
                set => _caption = value;
            }

            public Point Position { get; set; }

            public Type TipType { get; set; } = Type.Auto;

            [Flags]
            public enum Type
            {
                None = 0x0000,
                Auto = 0x0001,
                Absolute = 0x0002,
                SemiAbsolute = 0x0004
            }
        }
    }
}
