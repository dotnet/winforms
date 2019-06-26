// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.Serialization;

namespace System.Windows.Forms
{
    public partial class ListViewItem
    {
        // TODO: add serializable reasoning (@Devendar)

        [TypeConverterAttribute(typeof(ListViewSubItemConverter))]
        [ToolboxItem(false)]
        [DesignTimeVisible(false)]
        [DefaultProperty(nameof(Text))]
        [Serializable]
        public class ListViewSubItem
        {

            [NonSerialized]
            internal ListViewItem owner;

            private string text;

            [OptionalField(VersionAdded = 2)]
            private string name = null;

            private SubItemStyle style;

            [OptionalField(VersionAdded = 2)]
            private object userData;

            public ListViewSubItem()
            {
            }

            public ListViewSubItem(ListViewItem owner, string text)
            {
                this.owner = owner;
                this.text = text;
            }

            public ListViewSubItem(ListViewItem owner, string text, Color foreColor, Color backColor, Font font)
            {
                this.owner = owner;
                this.text = text;
                style = new SubItemStyle
                {
                    foreColor = foreColor,
                    backColor = backColor,
                    font = font
                };
            }


            public Color BackColor
            {
                get
                {
                    if (style != null && style.backColor != Color.Empty)
                    {
                        return style.backColor;
                    }

                    if (owner != null && owner.listView != null)
                    {
                        return owner.listView.BackColor;
                    }

                    return SystemColors.Window;
                }
                set
                {
                    if (style == null)
                    {
                        style = new SubItemStyle();
                    }

                    if (style.backColor != value)
                    {
                        style.backColor = value;
                        owner?.InvalidateListView();
                    }
                }
            }
            [Browsable(false)]
            public Rectangle Bounds
            {
                get
                {
                    if (owner != null && owner.listView != null && owner.listView.IsHandleCreated)
                    {
                        return owner.listView.GetSubItemRect(owner.Index, owner.SubItems.IndexOf(this));
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
                    Debug.Assert(style != null, "Should have checked CustomStyle");
                    return !style.backColor.IsEmpty;
                }
            }

            internal bool CustomFont
            {
                get
                {
                    Debug.Assert(style != null, "Should have checked CustomStyle");
                    return style.font != null;
                }
            }

            internal bool CustomForeColor
            {
                get
                {
                    Debug.Assert(style != null, "Should have checked CustomStyle");
                    return !style.foreColor.IsEmpty;
                }
            }

            internal bool CustomStyle => style != null;

            [Localizable(true)]
            public Font Font
            {
                get
                {
                    if (style != null && style.font != null)
                    {
                        return style.font;
                    }

                    if (owner != null && owner.listView != null)
                    {
                        return owner.listView.Font;
                    }

                    return Control.DefaultFont;
                }
                set
                {
                    if (style == null)
                    {
                        style = new SubItemStyle();
                    }

                    if (style.font != value)
                    {
                        style.font = value;
                        owner?.InvalidateListView();
                    }
                }
            }

            public Color ForeColor
            {
                get
                {
                    if (style != null && style.foreColor != Color.Empty)
                    {
                        return style.foreColor;
                    }

                    if (owner != null && owner.listView != null)
                    {
                        return owner.listView.ForeColor;
                    }

                    return SystemColors.WindowText;
                }
                set
                {
                    if (style == null)
                    {
                        style = new SubItemStyle();
                    }

                    if (style.foreColor != value)
                    {
                        style.foreColor = value;
                        owner?.InvalidateListView();
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
                    owner?.UpdateSubItems(-1);
                }
            }

            [Localizable(true)]
            public string Name
            {
                get => name ?? string.Empty;
                set
                {
                    name = value;
                    owner?.UpdateSubItems(-1);
                }
            }

            [OnDeserializing]
            private void OnDeserializing(StreamingContext ctx)
            {
            }

            [OnDeserialized]
            [SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")]
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
                if (style != null)
                {
                    style = null;
                    owner?.InvalidateListView();
                }
            }

            public override string ToString() => "ListViewSubItem: {" + Text + "}";

            [Serializable]
            private class SubItemStyle
            {
                public Color backColor = Color.Empty;
                public Color foreColor = Color.Empty;
                public Font font = null;
            }
        }
    }
}
