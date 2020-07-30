// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;

namespace System.Windows.Forms
{
    public partial class TabControl
    {
        public new class ControlCollection : Control.ControlCollection
        {
            private readonly TabControl _owner;

            public ControlCollection(TabControl owner) : base(owner)
            {
                _owner = owner;
            }

            public override void Add(Control value)
            {
                if (!(value is TabPage))
                {
                    throw new ArgumentException(string.Format(SR.TabControlInvalidTabPageType, value.GetType().Name));
                }

                TabPage tabPage = (TabPage)value;

                // See InsertingItem property
                if (!_owner.InsertingItem)
                {
                    if (_owner.IsHandleCreated)
                    {
                        _owner.AddTabPage(tabPage);
                    }
                    else
                    {
                        _owner.Insert(_owner.TabCount, tabPage);
                    }
                }

                base.Add(tabPage);
                tabPage.Visible = false;

                // Without this check, we force handle creation on the tabcontrol
                // which is not good at all of there are any OCXs on it.
                if (_owner.IsHandleCreated)
                {
                    tabPage.Bounds = _owner.DisplayRectangle;
                }

                // Site the tabPage if necessary.
                ISite site = _owner.Site;
                if (site != null)
                {
                    ISite siteTab = tabPage.Site;
                    if (siteTab is null)
                    {
                        site.Container?.Add(tabPage);
                    }
                }

                _owner.ApplyItemSize();
                _owner.UpdateTabSelection(false);
            }

            public override void Remove(Control value)
            {
                base.Remove(value);
                if (!(value is TabPage))
                {
                    return;
                }

                int index = _owner.FindTabPage((TabPage)value);
                int curSelectedIndex = _owner.SelectedIndex;
                if (index != -1)
                {
                    _owner.RemoveTabPage(index);
                    if (index == curSelectedIndex)
                    {
                         // Always select the first tabPage is the Selected TabPage is removed.
                        _owner.SelectedIndex = 0;
                    }
            }

                _owner.UpdateTabSelection(false);
            }
        }
    }
}
