using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Interop;
using static Interop.UiaCore;

namespace System.Windows.Forms
{
    public partial class ListView
    {
        [ComVisible(true)]
        internal class ListViewSubItemAccessibleObject : AccessibleObject
        {
            private ListView _owningListView;
            private ListViewItem _owningItem;
            private ListViewItem.ListViewSubItem _owningSubItem;

            public ListViewSubItemAccessibleObject(ListView owningListView, ListViewItem owningItem, ListViewItem.ListViewSubItem owningSubItem)
            {
                _owningListView = owningListView ?? throw new ArgumentNullException(nameof(owningListView));
                _owningItem = owningItem ?? throw new ArgumentNullException(nameof(owningItem));
                _owningSubItem = owningSubItem ?? throw new ArgumentNullException(nameof(owningSubItem));
            }

            internal override IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return _owningListView.AccessibilityObject;
                }
            }

            public override Rectangle Bounds
            {
                get
                {
                    return new Rectangle(
                        _owningListView.AccessibilityObject.Bounds.X + _owningSubItem.Bounds.X,
                        _owningListView.AccessibilityObject.Bounds.Y + _owningSubItem.Bounds.Y,
                        _owningListView.Columns[GetCurrentIndex()].Width,
                        _owningSubItem.Bounds.Height);
                }
            }

            internal override IRawElementProviderFragment FragmentNavigate(NavigateDirection direction)
            {
                switch (direction)
                {
                    case NavigateDirection.Parent:
                        return ParentFragment;
                    case NavigateDirection.NextSibling:
                        int nextSubItemIndex = GetCurrentIndex() + 1;
                        if (_owningItem.SubItems.Count > nextSubItemIndex)
                        {
                            var subItemCollection = (ParentFragment as ListViewItemAccessibleObject).SubItemAccessibleObjects;
                            var nextSubItem = _owningItem.SubItems[nextSubItemIndex];
                            return subItemCollection[nextSubItem] as IRawElementProviderFragment;
                        }

                        break;
                    case NavigateDirection.PreviousSibling:
                        int previousSubItemIndex = GetCurrentIndex() - 1;
                        if (previousSubItemIndex >= 0)
                        {
                            var subItemCollection = (ParentFragment as ListViewItemAccessibleObject).SubItemAccessibleObjects;
                            var previousSubItem = _owningItem.SubItems[previousSubItemIndex];
                            return subItemCollection[previousSubItem] as IRawElementProviderFragment;
                        }

                        break;
                }

                return base.FragmentNavigate(direction);
            }

            /// <summary>
            ///  Gets or sets the accessible name.
            /// </summary>
            public override string Name
            {
                get
                {
                    return _owningSubItem.Text;
                }
                set => base.Name = value;
            }

            internal override int[] RuntimeId
            {
                get
                {
                    var itemCollection = (_owningListView.AccessibilityObject as ListViewAccessibleObject).ItemAccessibleObjects;
                    var itemUiaProvider = itemCollection[_owningItem] as IRawElementProviderFragment;
                    var owningItemRuntimeId = itemUiaProvider.GetRuntimeId();

                    var runtimeId = new int[5];
                    runtimeId[0] = owningItemRuntimeId[0];
                    runtimeId[1] = owningItemRuntimeId[1];
                    runtimeId[2] = owningItemRuntimeId[2];
                    runtimeId[3] = owningItemRuntimeId[3];
                    runtimeId[4] = GetCurrentIndex();
                    return runtimeId;
                }
            }

            internal override object GetPropertyValue(UIA propertyID)
            {
                switch (propertyID)
                {
                    case UIA.ControlTypePropertyId:
                        return UIA.TextControlTypeId;
                    case UIA.NamePropertyId:
                        return Name;
                    case UIA.FrameworkIdPropertyId:
                        return "WinForm";
                    case UIA.ProcessIdPropertyId:
                        return Process.GetCurrentProcess().Id;
                    case UIA.AutomationIdPropertyId:
                        return AutomationId;
                    case UIA.RuntimeIdPropertyId:
                        return RuntimeId;
                    case UIA.HasKeyboardFocusPropertyId:
                        return _owningListView.Focused && _owningListView.FocusedItem == _owningItem;
                    case UIA.IsKeyboardFocusablePropertyId:
                        return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                    case UIA.IsEnabledPropertyId:
                        return _owningListView.Enabled;
                    case UIA.IsOffscreenPropertyId:
                        return (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
                    case UIA.BoundingRectanglePropertyId:
                        return Bounds;
                    case UIA.IsGridItemPatternAvailablePropertyId:
                        return IsPatternSupported(UIA.GridItemPatternId);
                    case UIA.IsTableItemPatternAvailablePropertyId:
                        return IsPatternSupported(UIA.TableItemPatternId);
                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            /// <summary>
            ///  Gets the accessible state.
            /// </summary>
            public override AccessibleStates State
            {
                get
                {
                    return AccessibleStates.Focusable;
                }
            }

            internal override IRawElementProviderSimple ContainingGrid => _owningListView.AccessibilityObject;

            internal override int Row => _owningItem.Index;

            internal override int Column => _owningItem.SubItems.IndexOf(_owningSubItem);

            internal override IRawElementProviderSimple[] GetColumnHeaderItems()
            {
                var columnHeaders = new List<IRawElementProviderSimple>();
                columnHeaders.Add(new ListViewColumnAccessibleObject(_owningListView.Columns[Column] as ColumnHeader));

                return columnHeaders.ToArray();
            }

            internal override bool IsPatternSupported(UIA patternId)
            {
                if(patternId == UIA.GridItemPatternId ||
                   patternId == UIA.TableItemPatternId)
                {
                    return true;
                }
                return base.IsPatternSupported(patternId);
            }

            private string AutomationId
            {
                get
                {
                    return string.Format("{0}-{1}", typeof(ListViewItem.ListViewSubItem).Name, GetCurrentIndex());
                }
            }

            private IRawElementProviderFragment ParentFragment
            {
                get
                {
                    var accessibleObject = _owningListView.AccessibilityObject as ListViewAccessibleObject;
                    return accessibleObject.ItemAccessibleObjects[_owningItem] as IRawElementProviderFragment;
                }
            }

            private int GetCurrentIndex()
            {
                return _owningItem.SubItems.IndexOf(_owningSubItem);
            }
        }
    }
}
