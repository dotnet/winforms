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
        // [Serializable] necessary why??? (@Devendar)

        [TypeConverterAttribute(typeof(ListViewSubItemConverter))]
        [ToolboxItem(false)]
        [DesignTimeVisible(false)]
        [DefaultProperty(nameof(Text))]
        [Serializable]
        public class ListViewSubItem
        {
            [NonSerialized]
            internal ListViewItem owner;

            private string _text;

            [OptionalField(VersionAdded = 2)]
            private string _name = null;

            private SubItemStyle _style;

            [OptionalField(VersionAdded = 2)]
            private object _userData;

            public ListViewSubItem()
            {
            }

            public ListViewSubItem(ListViewItem owner, string text)
            {
                this.owner = owner;
                this._text = text ?? string.Empty;
            }

            public ListViewSubItem(ListViewItem owner, string text, Color foreColor, Color backColor, Font font)
            {
                this.owner = owner;
                this._text = text ?? string.Empty;
                _style = new SubItemStyle
                {
                    foreColor = foreColor,
                    backColor = backColor,
                    font = font
                };
            }


            public Color BackColor
            {
                get => _style != null && _style.backColor != Color.Empty ?
                            _style.backColor :
                            (owner?.listView != null ?
                                owner.listView.BackColor :
                                    SystemColors.Window);
                
                set
                {
                    if (_style == null)
                    {
                        _style = new SubItemStyle();
                    }

                    if (_style.backColor != value)
                    {
                        _style.backColor = value;
                        owner?.InvalidateListView();
                    }
                }
            }
            [Browsable(false)]
            public Rectangle Bounds
            {
                get => owner?.listView?.IsHandleCreated == true ?
                            owner.listView.GetSubItemRect(owner.Index, owner.SubItems.IndexOf(this)) :
                            Rectangle.Empty;
            }

            internal bool CustomBackColor
                => _style?.backColor.IsEmpty == false;

            internal bool CustomFont
                => _style.font != null;

            internal bool CustomForeColor
                => _style?.foreColor.IsEmpty == false;

            internal bool CustomStyle 
                => _style != null;

            [Localizable(true)]
            public Font Font
            {
                get => _style?.font != null ?
                            _style.font :
                            owner?.listView != null ?
                                owner.listView.Font :
                                Control.DefaultFont;
                
                set
                {
                    _style = _style ?? new SubItemStyle();

                    if (_style.font != value)
                    {
                        _style.font = value;
                        owner?.InvalidateListView();
                    }
                }
            }

            public Color ForeColor
            {
                get => _style != null && _style.foreColor != Color.Empty ?
                            _style.foreColor :
                            owner?.listView != null ?
                                owner.listView.ForeColor :
                                 SystemColors.WindowText;
                
                set
                {
                    _style = _style ?? new SubItemStyle();

                    if (_style.foreColor != value)
                    {
                        _style.foreColor = value;
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
                get => _userData;
                set => _userData = value;
            }

            [Localizable(true)]
            public string Text
            {
                get => _text ?? string.Empty;
                set
                {
                    _text = value;
                    owner?.UpdateSubItems(-1);
                }
            }

            [Localizable(true)]
            public string Name
            {
                get => _name ?? string.Empty;
                set
                {
                    _name = value;
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
                _name = null;
                _userData = null;
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
                if (_style != null)
                {
                    _style = null;
                    owner?.InvalidateListView();
                }
            }

            public override string ToString() 
                => "ListViewSubItem: {" + Text + "}";
        }
    }
}
