// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;

namespace System.Windows.Forms
{
    public partial class ListViewItem
    {
        [TypeConverter(typeof(ListViewSubItemConverter))]
        [ToolboxItem(false)]
        [DesignTimeVisible(false)]
        [DefaultProperty(nameof(Text))]
        [Serializable] // This type is participating in resx serialization scenarios.
        public partial class ListViewSubItem
        {
            [NonSerialized]
            internal ListViewItem _owner;
#pragma warning disable IDE1006
            private string text;  // Do NOT rename (binary serialization).

            [OptionalField(VersionAdded = 2)]
            private string name = null;  // Do NOT rename (binary serialization).

            private SubItemStyle style;  // Do NOT rename (binary serialization).

            [OptionalField(VersionAdded = 2)]
            private object userData;  // Do NOT rename (binary serialization).
#pragma warning restore IDE1006

            [NonSerialized]
            private AccessibleObject _accessibilityObject;

            public ListViewSubItem()
            {
            }

            public ListViewSubItem(ListViewItem owner, string text)
            {
                this._owner = owner;
                this.text = text;
            }

            public ListViewSubItem(ListViewItem owner, string text, Color foreColor, Color backColor, Font font)
            {
                this._owner = owner;
                this.text = text;
                style = new SubItemStyle
                {
                    foreColor = foreColor,
                    backColor = backColor,
                    font = font
                };
            }

            internal AccessibleObject AccessibilityObject
            {
                get
                {
                    if (_accessibilityObject is null)
                    {
                        _accessibilityObject = new ListViewSubItemAccessibleObject(this, _owner);
                    }

                    return _accessibilityObject;
                }
            }

            public Color BackColor
            {
                get
                {
                    if (style is not null && style.backColor != Color.Empty)
                    {
                        return style.backColor;
                    }

                    if (_owner is not null && _owner.listView is not null)
                    {
                        return _owner.listView.BackColor;
                    }

                    return SystemColors.Window;
                }
                set
                {
                    if (style is null)
                    {
                        style = new SubItemStyle();
                    }

                    if (style.backColor != value)
                    {
                        style.backColor = value;
                        _owner?.InvalidateListView();
                    }
                }
            }

            [Browsable(false)]
            public Rectangle Bounds
            {
                get
                {
                    if (_owner is not null && _owner.listView is not null && _owner.listView.IsHandleCreated)
                    {
                        return _owner.listView.GetSubItemRect(_owner.Index, _owner.SubItems.IndexOf(this));
                    }
                    else
                    {
                        return Rectangle.Empty;
                    }
                }
            }

            internal bool CustomBackColor
            {
                get
                {
                    Debug.Assert(style is not null, "Should have checked CustomStyle");
                    return !style.backColor.IsEmpty;
                }
            }

            internal bool CustomFont
            {
                get
                {
                    Debug.Assert(style is not null, "Should have checked CustomStyle");
                    return style.font is not null;
                }
            }

            internal bool CustomForeColor
            {
                get
                {
                    Debug.Assert(style is not null, "Should have checked CustomStyle");
                    return !style.foreColor.IsEmpty;
                }
            }

            internal bool CustomStyle => style is not null;

            [Localizable(true)]
            public Font Font
            {
                get
                {
                    if (style is not null && style.font is not null)
                    {
                        return style.font;
                    }

                    if (_owner is not null && _owner.listView is not null)
                    {
                        return _owner.listView.Font;
                    }

                    return Control.DefaultFont;
                }
                set
                {
                    if (style is null)
                    {
                        style = new SubItemStyle();
                    }

                    if (style.font != value)
                    {
                        style.font = value;
                        _owner?.InvalidateListView();
                    }
                }
            }

            public Color ForeColor
            {
                get
                {
                    if (style is not null && style.foreColor != Color.Empty)
                    {
                        return style.foreColor;
                    }

                    if (_owner is not null && _owner.listView is not null)
                    {
                        return _owner.listView.ForeColor;
                    }

                    return SystemColors.WindowText;
                }
                set
                {
                    if (style is null)
                    {
                        style = new SubItemStyle();
                    }

                    if (style.foreColor != value)
                    {
                        style.foreColor = value;
                        _owner?.InvalidateListView();
                    }
                }
            }

            [SRCategory(nameof(SR.CatData))]
            [Localizable(false)]
            [Bindable(true)]
            [SRDescription(nameof(SR.ControlTagDescr))]
            [DefaultValue(null)]
            [TypeConverter(typeof(StringConverter))]
            public object Tag
            {
                get => userData;
                set => userData = value;
            }

            [Localizable(true)]
            public string Text
            {
                get => text ?? string.Empty;
                set
                {
                    text = value;
                    _owner?.UpdateSubItems(-1);
                }
            }

            [Localizable(true)]
            public string Name
            {
                get => name ?? string.Empty;
                set
                {
                    name = value;
                    _owner?.UpdateSubItems(-1);
                }
            }

            [OnDeserializing]
            private void OnDeserializing(StreamingContext ctx)
            {
            }

            [OnDeserialized]
            private void OnDeserialized(StreamingContext ctx)
            {
                name = null;
                userData = null;
            }

            [OnSerializing]
            private void OnSerializing(StreamingContext ctx)
            {
            }

            [OnSerialized]
            private void OnSerialized(StreamingContext ctx)
            {
            }

            public void ResetStyle()
            {
                if (style is not null)
                {
                    style = null;
                    _owner?.InvalidateListView();
                }
            }

            public override string ToString() => "ListViewSubItem: {" + Text + "}";
        }
    }
}
