// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class LinkLabel
    {
        public partial class Link
        {
            internal class LinkAccessibleObject : AccessibleObject
            {
                private readonly LinkLabelAccessibleObject _linkLabelAccessibleObject;
                private readonly Link _owningLink;
                private readonly LinkLabel _owningLinkLabel;

                public LinkAccessibleObject(Link link, LinkLabel owner)
                {
                    _owningLink = link.OrThrowIfNull();
                    _owningLinkLabel = owner.OrThrowIfNull();
                    _linkLabelAccessibleObject = (LinkLabelAccessibleObject)_owningLinkLabel.AccessibilityObject;
                }

                public override Rectangle Bounds
                {
                    get
                    {
                        if (!_owningLinkLabel.IsHandleCreated)
                        {
                            return Rectangle.Empty;
                        }

                        Region region = _owningLink.VisualRegion;
                        using Graphics graphics = Graphics.FromHwnd(_owningLink.Owner.Handle);

                        // Make sure we have a region for this link
                        //
                        if (region is null)
                        {
                            _owningLinkLabel.EnsureRun(graphics);
                            region = _owningLink.VisualRegion;
                            if (region is null)
                            {
                                return Rectangle.Empty;
                            }
                        }

                        Rectangle rect;
                        rect = Rectangle.Ceiling(region.GetBounds(graphics));

                        // Translate rect to screen coordinates
                        //
                        return _owningLinkLabel.RectangleToScreen(rect);
                    }
                }

                private int CurrentIndex => _owningLinkLabel.Links.IndexOf(_owningLink);

                public override string DefaultAction => SR.AccessibleActionClick;

                public override string Description => _owningLink.Description;

                public override void DoDefaultAction()
                {
                    if (!_owningLinkLabel.IsHandleCreated)
                    {
                        return;
                    }

                    _owningLinkLabel.OnLinkClicked(new LinkLabelLinkClickedEventArgs(_owningLink));
                }

                internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                    => direction switch
                    {
                        UiaCore.NavigateDirection.Parent => _linkLabelAccessibleObject,
                        UiaCore.NavigateDirection.NextSibling => _linkLabelAccessibleObject.GetChild(CurrentIndex + 1),
                        UiaCore.NavigateDirection.PreviousSibling => _linkLabelAccessibleObject.GetChild(CurrentIndex - 1),
                        _ => base.FragmentNavigate(direction),
                    };

                internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => _linkLabelAccessibleObject;

                internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                    => propertyID switch
                    {
                        UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.HyperlinkControlTypeId,
                        UiaCore.UIA.HasKeyboardFocusPropertyId => _owningLinkLabel.FocusLink == _owningLink,
                        UiaCore.UIA.IsEnabledPropertyId => _owningLinkLabel.Enabled,
                        UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                        _ => base.GetPropertyValue(propertyID)
                    };

                internal override bool IsIAccessibleExSupported() => true;

                internal override bool IsPatternSupported(UiaCore.UIA patternId)
                {
                    if (patternId == UiaCore.UIA.LegacyIAccessiblePatternId ||
                        patternId == UiaCore.UIA.InvokePatternId)
                    {
                        return true;
                    }

                    return base.IsPatternSupported(patternId);
                }

                public override string? Name
                {
                    get
                    {
                        string? text = _owningLinkLabel.Text;
                        int start = LinkLabel.ConvertToCharIndex(_owningLink.Start, text);
                        int end = LinkLabel.ConvertToCharIndex(_owningLink.Start + _owningLink.Length, text);
                        string? name = text.Substring(start, end - start);

                         return _owningLinkLabel.UseMnemonic ? name = WindowsFormsUtils.TextWithoutMnemonics(name) : name;
                    }
                    set => base.Name = value;
                }

                public override AccessibleObject Parent => _linkLabelAccessibleObject;

                public override AccessibleRole Role => AccessibleRole.Link;

                internal override int[] RuntimeId
                    => new int[]
                    {
                        RuntimeIDFirstItem,
                        PARAM.ToInt(_owningLinkLabel.InternalHandle),
                        _owningLink.GetHashCode()
                    };

                public override AccessibleStates State
                {
                    get
                    {
                        AccessibleStates state = AccessibleStates.Focusable;

                        // Selected state
                        //
                        if (_owningLinkLabel.FocusLink == _owningLink)
                        {
                            state |= AccessibleStates.Focused;
                        }

                        return state;
                    }
                }

                // Narrator announces Link's text twice, once as a Name property and once as a Value, thus removing value.
                // Value is optional for this role (Link).
                public override string Value => string.Empty;
            }
        }
    }
}
