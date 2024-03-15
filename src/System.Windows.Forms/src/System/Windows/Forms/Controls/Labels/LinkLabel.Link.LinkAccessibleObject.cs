// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class LinkLabel
{
    public partial class Link
    {
        internal sealed class LinkAccessibleObject : AccessibleObject
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
                    if (!_owningLinkLabel.IsHandleCreated || _owningLink.Owner is null)
                    {
                        return Rectangle.Empty;
                    }

                    Region? region = _owningLink.VisualRegion;
                    using Graphics graphics = Graphics.FromHwnd(_owningLink.Owner.Handle);

                    // Make sure we have a region for this link
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
                    return _owningLinkLabel.RectangleToScreen(rect);
                }
            }

            private int CurrentIndex => _owningLinkLabel.Links.IndexOf(_owningLink);

            public override string DefaultAction => SR.AccessibleActionClick;

            internal override bool CanGetDefaultActionInternal => false;

            public override string? Description => _owningLink.Description;

            internal override bool CanGetDescriptionInternal => false;

            public override void DoDefaultAction()
            {
                if (!_owningLinkLabel.IsHandleCreated)
                {
                    return;
                }

                _owningLinkLabel.OnLinkClicked(new LinkLabelLinkClickedEventArgs(_owningLink));
            }

            internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
                => direction switch
                {
                    NavigateDirection.NavigateDirection_Parent => _linkLabelAccessibleObject,
                    NavigateDirection.NavigateDirection_NextSibling => _linkLabelAccessibleObject.GetChild(CurrentIndex + 1),
                    NavigateDirection.NavigateDirection_PreviousSibling => _linkLabelAccessibleObject.GetChild(CurrentIndex - 1),
                    _ => base.FragmentNavigate(direction),
                };

            internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => _linkLabelAccessibleObject;

            internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
                => propertyID switch
                {
                    UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_HyperlinkControlTypeId,
                    UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)(_owningLinkLabel.FocusLink == _owningLink),
                    UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)_owningLinkLabel.Enabled,
                    UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focusable),
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsIAccessibleExSupported() => true;

            internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            {
                if (patternId is UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId or UIA_PATTERN_ID.UIA_InvokePatternId)
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
                    int start = ConvertToCharIndex(_owningLink.Start, text);
                    int end = ConvertToCharIndex(_owningLink.Start + _owningLink.Length, text);
                    string? name = text[start..end];

                    return _owningLinkLabel.UseMnemonic ? name = WindowsFormsUtils.TextWithoutMnemonics(name) : name;
                }
            }

            internal override bool CanGetNameInternal => false;

            public override AccessibleObject Parent => _linkLabelAccessibleObject;

            private protected override bool IsInternal => true;

            public override AccessibleRole Role => AccessibleRole.Link;

            internal override int[] RuntimeId =>
            [
                RuntimeIDFirstItem,
                (int)_owningLinkLabel.InternalHandle,
                _owningLink.GetHashCode()
            ];

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

            internal override bool CanGetValueInternal => false;
        }
    }
}
