using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Accessibility;
using static Interop;
using static Interop.UiaCore;

namespace System.Windows.Forms
{
    public partial class ListView
    {
        /// <summary>
        ///  ListBox control accessible object with UI Automation provider functionality.
        ///  This inherits from the base ListBoxExAccessibleObject and ListBoxAccessibleObject
        ///  to have all base functionality.
        /// </summary>
        [ComVisible(true)]
        internal class ListViewItemAccessibleObject : AccessibleObject
        {
            private readonly ListViewItem _itemEntry;
            private readonly ListViewAccessibleObject _owningAccessibleObject;
            private readonly Dictionary<ListViewItem.ListViewSubItem, ListViewSubItemAccessibleObject> _subItemAccessibleObjects;
            private readonly ListView _owningListView;
            private readonly IAccessible _systemIAccessible;


            public ListViewItemAccessibleObject(ListView owningListView, object itemEntry, ListViewAccessibleObject owningAccessibleObject)
            {
                _owningListView = owningListView;
                _itemEntry = (ListViewItem)itemEntry;
                _owningAccessibleObject = owningAccessibleObject;
                _subItemAccessibleObjects = new Dictionary<ListViewItem.ListViewSubItem, ListViewSubItemAccessibleObject>();
                _systemIAccessible = owningAccessibleObject.GetSystemIAccessibleInternal();
                FillSubItemAccessibleObjectsCollection();
            }

            public override Rectangle Bounds
            {
                get
                {
                    return new Rectangle(
                        _owningListView.AccessibilityObject.Bounds.X + _itemEntry.Bounds.X,
                        _owningListView.AccessibilityObject.Bounds.Y + _itemEntry.Bounds.Y,
                        _itemEntry.Bounds.Width,
                        _itemEntry.Bounds.Height);
                }
            }

            private string AutomationId
            {
                get
                {
                    return string.Format("{0}-{1}", typeof(ListViewItem).Name, CurrentIndex);
                }
            }

            private int CurrentIndex => _owningListView.Items.IndexOf(_itemEntry);

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => _owningAccessibleObject;

            internal override bool IsItemSelected
            {
                get
                {
                    return (State & AccessibleStates.Selected) != 0;
                }
            }

            /// <summary>
            ///  Gets or sets the accessible name.
            /// </summary>
            public override string Name
            {
                get
                {
                    return _itemEntry.Text;
                }
                set => base.Name = value;
            }

            /// <summary>
            ///  Gets the accessible role.
            /// </summary>
            public override AccessibleRole Role => AccessibleRole.ListItem;

            /// <summary>
            ///  Gets the accessible state.
            /// </summary>
            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable | AccessibleStates.MultiSelectable;

                    if (_owningListView.SelectedItems.Contains(_itemEntry))
                    {
                        return state |= AccessibleStates.Selected | AccessibleStates.Focused;
                    }

                    return state |= (AccessibleStates)(_systemIAccessible.get_accState(GetChildId()));
                }
            }

            internal override void AddToSelection()
            {
                SelectItem();
            }

            public override string DefaultAction => SR.AccessibleActionDoubleClick;

            public override void DoDefaultAction()
            {
                SetFocus();
            }

            internal override IRawElementProviderFragment FragmentNavigate(NavigateDirection direction)
            {
                int firstItemIndex = 0;
                int lastItemIndex = _owningListView.Items.Count - 1;
                int currentIndex = CurrentIndex;

                switch (direction)
                {
                    case NavigateDirection.Parent:
                        return _owningListView.AccessibilityObject;
                    case NavigateDirection.NextSibling:
                        var listViewAccessibilityObject = _owningListView.AccessibilityObject;
                        if (listViewAccessibilityObject != null)
                        {
                            int itemsCount = listViewAccessibilityObject.GetChildCount();
                            int nextItemIndex = currentIndex + 1;
                            if (itemsCount > nextItemIndex)
                            {
                                return listViewAccessibilityObject.GetChild(nextItemIndex);
                            }
                        }

                        break;
                    case NavigateDirection.PreviousSibling:
                        listViewAccessibilityObject = _owningListView.AccessibilityObject;
                        if (listViewAccessibilityObject != null)
                        {
                            int previousItemIndex = currentIndex - 1;
                            if (previousItemIndex >= 0)
                            {
                                return listViewAccessibilityObject.GetChild(previousItemIndex);
                            }
                        }

                        break;
                    case NavigateDirection.FirstChild:
                        if (_itemEntry.SubItems.Count > 0)
                        {
                            return GetChild(firstItemIndex);
                        }

                        return null;
                    case NavigateDirection.LastChild:
                        if (_itemEntry.SubItems.Count > 0)
                        {
                            return GetChild(lastItemIndex);
                        }

                        return null;
                }

                return null;
            }

            private void FillSubItemAccessibleObjectsCollection()
            {
                foreach (var item in _itemEntry.SubItems)
                {
                    _subItemAccessibleObjects.Add((ListViewItem.ListViewSubItem)item, new ListViewSubItemAccessibleObject(_owningListView, _itemEntry, (ListViewItem.ListViewSubItem)item));
                }
            }

            public override AccessibleObject GetChild(int index)
            {
                if (index < 0 || index >= _itemEntry.SubItems.Count)
                {
                    return null;
                }

                ListViewItem.ListViewSubItem item = _itemEntry.SubItems[index];

                return _subItemAccessibleObjects[item];
            }

            internal override int[] RuntimeId
            {
                get
                {
                    var owningListViewRuntimeId = _owningListView.AccessibilityObject.RuntimeId;

                    var runtimeId = new int[4];
                    runtimeId[0] = owningListViewRuntimeId[0];
                    runtimeId[1] = owningListViewRuntimeId[1];
                    runtimeId[2] = 4; // Win32-like const.
                    runtimeId[3] = CurrentIndex;
                    return runtimeId;
                }
            }

            public Dictionary<ListViewItem.ListViewSubItem, ListViewSubItemAccessibleObject> SubItemAccessibleObjects
            {
                get
                {
                    return _subItemAccessibleObjects;
                }
            }

            internal override object GetPropertyValue(UIA propertyID)
            {
                switch (propertyID)
                {
                    case UIA.RuntimeIdPropertyId:
                        return RuntimeId;
                    case UIA.AutomationIdPropertyId:
                        return AutomationId;
                    case UIA.BoundingRectanglePropertyId:
                        return Bounds;
                    case UIA.FrameworkIdPropertyId:
                        return "WinForm";
                    case UIA.ControlTypePropertyId:
                        return UIA.ListItemControlTypeId;
                    case UIA.NamePropertyId:
                        return Name;
                    case UIA.HasKeyboardFocusPropertyId:
                        return _owningListView.Focused && _owningListView.FocusedItem == _itemEntry;
                    case UIA.IsKeyboardFocusablePropertyId:
                        return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                    case UIA.IsEnabledPropertyId:
                        return _owningListView.Enabled;
                    case UIA.IsOffscreenPropertyId:
                        return (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
                    case UIA.NativeWindowHandlePropertyId:
                        return _owningListView.Handle;
                    case UIA.IsSelectionItemPatternAvailablePropertyId:
                        return IsPatternSupported(UIA.SelectionItemPatternId);
                    case UIA.IsScrollItemPatternAvailablePropertyId:
                        return IsPatternSupported(UIA.ScrollItemPatternId);
                    case UIA.IsInvokePatternAvailablePropertyId:
                        return IsPatternSupported(UIA.InvokePatternId);
                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            /// <summary>
            ///  Indicates whether specified pattern is supported.
            /// </summary>
            /// <param name="patternId">The pattern ID.</param>
            /// <returns>True if specified </returns>
            internal override bool IsPatternSupported(UIA patternId)
            {
                if (patternId == UIA.ScrollItemPatternId ||
                    patternId == UIA.LegacyIAccessiblePatternId ||
                    patternId == UIA.SelectionItemPatternId ||
                    patternId == UIA.InvokePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override void RemoveFromSelection()
            {
                // Do nothing, C++ implementation returns UIA_E_INVALIDOPERATION 0x80131509
            }

            internal override IRawElementProviderSimple ItemSelectionContainer => _owningListView.AccessibilityObject;

            internal override void ScrollIntoView()
            {
                int currentIndex = CurrentIndex;

                if (_owningListView.SelectedItems == null) //no items selected
                {
                    User32.SendMessageW(_owningListView, (User32.WM)User32.LB.SETCARETINDEX, (IntPtr)currentIndex);
                    return;
                }

                int firstVisibleIndex = User32.SendMessageW(_owningListView, (User32.WM)User32.LB.GETTOPINDEX).ToInt32();
                if (currentIndex < firstVisibleIndex)
                {
                    User32.SendMessageW(_owningListView, (User32.WM)User32.LB.SETTOPINDEX, (IntPtr)currentIndex);
                    return;
                }

                int itemsHeightSum = 0;
                int visibleItemsCount = 0;
                int listBoxHeight = _owningListView.ClientRectangle.Height;
                int itemsCount = _owningListView.Items.Count;

                for (int i = firstVisibleIndex; i < itemsCount; i++)
                {
                    int itemHeight = User32.SendMessageW(_owningListView, (User32.WM)User32.LB.GETITEMHEIGHT, (IntPtr)i).ToInt32();

                    if ((itemsHeightSum += itemHeight) <= listBoxHeight)
                    {
                        continue;
                    }

                    int lastVisibleIndex = i - 1; // - 1 because last "i" index is invisible
                    visibleItemsCount = lastVisibleIndex - firstVisibleIndex + 1; // + 1 because array indexes begin with 0

                    if (currentIndex > lastVisibleIndex)
                    {
                        User32.SendMessageW(_owningListView, (User32.WM)User32.LB.SETTOPINDEX, (IntPtr)(currentIndex - visibleItemsCount + 1));
                    }

                    break;
                }
            }

            internal unsafe override void SelectItem()
            {
                _owningListView.selectedIndexCollection.Add(CurrentIndex);

                User32.InvalidateRect(new HandleRef(this, _owningListView.Handle), null, BOOL.FALSE);
                RaiseAutomationEvent(UIA.AutomationFocusChangedEventId);
                RaiseAutomationEvent(UIA.SelectionItem_ElementSelectedEventId);
            }

            internal override void SetFocus()
            {
                RaiseAutomationEvent(UIA.AutomationFocusChangedEventId);
                SelectItem();
            }

            public override void Select(AccessibleSelection flags)
            {
                try
                {
                    _systemIAccessible.accSelect((int)flags, GetChildId());
                }
                catch (ArgumentException)
                {
                    // In Everett, the ListBox accessible children did not have any selection capability.
                    // In Whidbey, they delegate the selection capability to OLEACC.
                    // However, OLEACC does not deal w/ several Selection flags: ExtendSelection, AddSelection, RemoveSelection.
                    // OLEACC instead throws an ArgumentException.
                    // Since Whidbey API's should not throw an exception in places where Everett API's did not, we catch
                    // the ArgumentException and fail silently.
                }
            }
        }
    }
}
