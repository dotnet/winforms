﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms
{
    public partial class LinkLabel
    {
        [TypeConverter(typeof(LinkConverter))]
        public class Link
        {
            private int _start;
            private bool _enabled = true;
            internal int _length;
            private string _name;

            public Link()
            {
            }

            public Link(int start, int length)
            {
                _start = start;
                _length = length;
            }

            public Link(int start, int length, object linkData)
            {
                _start = start;
                _length = length;
                LinkData = linkData;
            }

            internal Link(LinkLabel owner)
            {
                Owner = owner;
            }

            /// <summary>
            ///  Description for accessibility
            /// </summary>
            public string Description { get; set; }

            [DefaultValue(true)]
            public bool Enabled
            {
                get => _enabled;
                set
                {
                    if (_enabled != value)
                    {
                        _enabled = value;

                        if ((int)(State & (LinkState.Hover | LinkState.Active)) != 0)
                        {
                            State &= ~(LinkState.Hover | LinkState.Active);
                            if (Owner != null)
                            {
                                Owner.OverrideCursor = null;
                            }
                        }

                        Owner?.InvalidateLink(this);
                    }
                }
            }

            public int Length
            {
                get
                {
                    if (_length == -1)
                    {
                        if (Owner != null && !string.IsNullOrEmpty(Owner.Text))
                        {
                            StringInfo stringInfo = new StringInfo(Owner.Text);
                            return stringInfo.LengthInTextElements - Start;
                        }
                        else
                        {
                            return 0;
                        }
                    }

                    return _length;
                }
                set
                {
                    if (_length != value)
                    {
                        _length = value;
                        if (Owner != null)
                        {
                            Owner.InvalidateTextLayout();
                            Owner.Invalidate();
                        }
                    }
                }
            }

            [DefaultValue(null)]
            public object LinkData { get; set; }

            /// <summary>
            ///  The LinkLabel object that owns this link.
            /// </summary>
            internal LinkLabel Owner { get; set; }

            internal LinkState State { get; set; } = LinkState.Normal;

            /// <summary>
            ///  The name for the link - useful for indexing by key.
            /// </summary>
            [DefaultValue("")]
            [SRCategory(nameof(SR.CatAppearance))]
            [SRDescription(nameof(SR.TreeNodeNodeNameDescr))]
            public string Name
            {
                get => _name ?? string.Empty;
                set => _name = value;
            }

            public int Start
            {
                get => _start;
                set
                {
                    if (_start != value)
                    {
                        _start = value;

                        if (Owner != null)
                        {
                            Owner._links.Sort(LinkLabel.s_linkComparer);
                            Owner.InvalidateTextLayout();
                            Owner.Invalidate();
                        }
                    }
                }
            }

            [SRCategory(nameof(SR.CatData))]
            [Localizable(false)]
            [Bindable(true)]
            [SRDescription(nameof(SR.ControlTagDescr))]
            [DefaultValue(null)]
            [TypeConverter(typeof(StringConverter))]
            public object Tag { get; set; }

            [DefaultValue(false)]
            public bool Visited
            {
                get => (State & LinkState.Visited) == LinkState.Visited;
                set
                {
                    if (value != Visited)
                    {
                        if (value)
                        {
                            State |= LinkState.Visited;
                        }
                        else
                        {
                            State &= ~LinkState.Visited;
                        }

                        Owner?.InvalidateLink(this);
                    }
                }
            }

            internal Region VisualRegion { get; set; }
        }
    }
}
