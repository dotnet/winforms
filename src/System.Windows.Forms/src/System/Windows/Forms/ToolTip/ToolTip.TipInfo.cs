// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ToolTip
{
    private partial class TipInfo
    {
        private readonly string? _designerText;
        private string? _caption;

        public TipInfo(string? caption, Type type)
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
    }
}
