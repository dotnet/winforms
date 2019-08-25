// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a Windows up-down control that displays string values.
    /// </summary>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultProperty(nameof(Items)),
    DefaultEvent(nameof(SelectedItemChanged)),
    DefaultBindingProperty(nameof(SelectedItem)),
    SRDescription(nameof(SR.DescriptionDomainUpDown))
    ]
    public class DomainUpDown : UpDownBase
    {
        private readonly static string DefaultValue = string.Empty;
        private readonly static bool DefaultWrap = false;

        //////////////////////////////////////////////////////////////
        // Member variables
        //
        //////////////////////////////////////////////////////////////
        /// <summary>
        ///  Allowable strings for the domain updown.
        /// </summary>
        private DomainUpDownItemCollection domainItems = null;

        private string stringValue = DefaultValue;      // Current string value
        private int domainIndex = -1;                    // Index in the domain list
        private bool sorted = false;                 // Sort the domain values

        private bool wrap = DefaultWrap;             // Wrap around domain items

        private EventHandler onSelectedItemChanged = null;

        private bool inSort = false;

        /// <summary>
        ///  Initializes a new instance of the <see cref='DomainUpDown'/> class.
        /// </summary>
        public DomainUpDown() : base()
        {
            // this class overrides GetPreferredSizeCore, let Control automatically cache the result
            SetState2(STATE2_USEPREFERREDSIZECACHE, true);
            Text = string.Empty;
        }

        // Properties

        /// <summary>
        ///  Gets the collection of objects assigned to the
        ///  up-down control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatData)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        SRDescription(nameof(SR.DomainUpDownItemsDescr)),
        Localizable(true),
        Editor("System.Windows.Forms.Design.StringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))
        ]
        public DomainUpDownItemCollection Items
        {
            get
            {
                if (domainItems == null)
                {
                    domainItems = new DomainUpDownItemCollection(this);
                }
                return domainItems;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Padding Padding
        {
            get { return base.Padding; }
            set { base.Padding = value; }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new event EventHandler PaddingChanged
        {
            add => base.PaddingChanged += value;
            remove => base.PaddingChanged -= value;
        }

        /// <summary>
        ///  Gets or sets the index value of the selected item.
        /// </summary>
        [
        Browsable(false),
        DefaultValue(-1),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.DomainUpDownSelectedIndexDescr))
        ]
        public int SelectedIndex
        {
            get
            {
                if (UserEdit)
                {
                    return -1;
                }
                else
                {
                    return domainIndex;
                }
            }

            set
            {
                if (value < -1 || value >= Items.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(SelectedIndex), value));
                }

                if (value != SelectedIndex)
                {
                    SelectIndex(value);
                }
            }
        }

        /// <summary>
        ///  Gets or sets the selected item based on the index value
        ///  of the selected item in the
        ///  collection.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.DomainUpDownSelectedItemDescr))
        ]
        public object SelectedItem
        {
            get
            {
                int index = SelectedIndex;
                return (index == -1) ? null : Items[index];
            }
            set
            {

                // Treat null as selecting no item
                //
                if (value == null)
                {
                    SelectedIndex = -1;
                }
                else
                {
                    // Attempt to find the given item in the list of items
                    //
                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (value != null && value.Equals(Items[i]))
                        {
                            SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the item collection is sorted.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.DomainUpDownSortedDescr))
        ]
        public bool Sorted
        {
            get
            {
                return sorted;
            }

            set
            {
                sorted = value;
                if (sorted)
                {
                    SortDomainItems();
                }
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the collection of items continues to
        ///  the first or last item if the user continues past the end of the list.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        DefaultValue(false),
        SRDescription(nameof(SR.DomainUpDownWrapDescr))
        ]
        public bool Wrap
        {
            get
            {
                return wrap;
            }

            set
            {
                wrap = value;
            }
        }

        //////////////////////////////////////////////////////////////
        // Methods
        //
        //////////////////////////////////////////////////////////////
        /// <summary>
        ///  Occurs when the <see cref='SelectedItem'/> property has
        ///  been changed.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.DomainUpDownOnSelectedItemChangedDescr))]
        public event EventHandler SelectedItemChanged
        {
            add => onSelectedItemChanged += value;
            remove => onSelectedItemChanged -= value;
        }

        /// <summary>
        ///  Constructs the new instance of the accessibility object for this control. Subclasses
        ///  should not call base.CreateAccessibilityObject.
        /// </summary>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DomainUpDownAccessibleObject(this);
        }

        /// <summary>
        ///  Displays the next item in the object collection.
        /// </summary>
        public override void DownButton()
        {
            // Make sure domain values exist, and there are >0 items
            //
            if (domainItems == null)
            {
                return;
            }
            if (domainItems.Count <= 0)
            {
                return;
            }

            // If the user has entered text, attempt to match it to the domain list
            //
            int matchIndex = -1;
            if (UserEdit)
            {
                matchIndex = MatchIndex(Text, false, domainIndex);
            }
            if (matchIndex != -1)
            {
                // Found a match, so select this value
                domainIndex = matchIndex;
                SelectIndex(matchIndex);
            }
            else
            {
                // Otherwise, get the next string in the domain list
                if (domainIndex < domainItems.Count - 1)
                {
                    SelectIndex(domainIndex + 1);
                }
                else if (Wrap)
                {
                    SelectIndex(0);
                }
            }

        }

        /// <summary>
        ///  Tries to find a match of the supplied text in the domain list.
        ///  If complete is true, a complete match is required for success
        ///  (i.e. the supplied text is the same length as the matched domain value)
        ///  Returns the index in the domain list if the match is successful,
        ///  returns -1 otherwise.
        /// </summary>
        internal int MatchIndex(string text, bool complete)
        {
            return MatchIndex(text, complete, domainIndex);
        }

        internal int MatchIndex(string text, bool complete, int startPosition)
        {
            // Make sure domain values exist
            if (domainItems == null)
            {
                return -1;
            }

            // Sanity check of parameters
            if (text.Length < 1)
            {
                return -1;
            }
            if (domainItems.Count <= 0)
            {
                return -1;
            }
            if (startPosition < 0)
            {
                startPosition = domainItems.Count - 1;
            }
            if (startPosition >= domainItems.Count)
            {
                startPosition = 0;
            }

            // Attempt to match the supplied string text with
            // the domain list. Returns the index in the list if successful,
            // otherwise returns -1.
            int index = startPosition;
            int matchIndex = -1;
            bool found = false;

            if (!complete)
            {
                text = text.ToUpper(CultureInfo.InvariantCulture);
            }

            // Attempt to match the string with Items[index]
            do
            {
                if (complete)
                {
                    found = Items[index].ToString().Equals(text);
                }
                else
                {
                    found = Items[index].ToString().ToUpper(CultureInfo.InvariantCulture).StartsWith(text);
                }

                if (found)
                {
                    matchIndex = index;
                }

                // Calculate the next index to attempt to match
                index++;
                if (index >= domainItems.Count)
                {
                    index = 0;
                }

            } while (!found && index != startPosition);

            return matchIndex;
        }

        /// <summary>
        ///  In the case of a DomainUpDown, the handler for changing
        ///  values is called OnSelectedItemChanged - so just forward it to that
        ///  function.
        /// </summary>
        protected override void OnChanged(object source, EventArgs e)
        {
            OnSelectedItemChanged(source, e);
        }

        /// <summary>
        ///  Handles the <see cref='Control.KeyPress'/>
        ///  event, using the input character to find the next matching item in our
        ///  item collection.
        /// </summary>
        protected override void OnTextBoxKeyPress(object source, KeyPressEventArgs e)
        {
            if (ReadOnly)
            {
                char[] character = new char[] { e.KeyChar };
                UnicodeCategory uc = char.GetUnicodeCategory(character[0]);

                if (uc == UnicodeCategory.LetterNumber
                    || uc == UnicodeCategory.LowercaseLetter
                    || uc == UnicodeCategory.DecimalDigitNumber
                    || uc == UnicodeCategory.MathSymbol
                    || uc == UnicodeCategory.OtherLetter
                    || uc == UnicodeCategory.OtherNumber
                    || uc == UnicodeCategory.UppercaseLetter)
                {

                    // Attempt to match the character to a domain item
                    int matchIndex = MatchIndex(new string(character), false, domainIndex + 1);
                    if (matchIndex != -1)
                    {

                        // Select the matching domain item
                        SelectIndex(matchIndex);
                    }
                    e.Handled = true;
                }
            }
            base.OnTextBoxKeyPress(source, e);
        }

        /// <summary>
        ///  Raises the <see cref='SelectedItemChanged'/> event.
        /// </summary>
        protected void OnSelectedItemChanged(object source, EventArgs e)
        {
            // Call the event handler
            onSelectedItemChanged?.Invoke(this, e);
        }

        /// <summary>
        ///  Selects the item in the domain list at the given index
        /// </summary>
        private void SelectIndex(int index)
        {
            // Sanity check index

            Debug.Assert(domainItems != null, "Domain values array is null");
            Debug.Assert(index < domainItems.Count && index >= -1, "SelectValue: index out of range");
            if (domainItems == null || index < -1 || index >= domainItems.Count)
            {
                // Defensive programming
                index = -1;
                return;
            }

            // If the selected index has changed, update the text
            //
            domainIndex = index;
            if (domainIndex >= 0)
            {
                stringValue = domainItems[domainIndex].ToString();
                UserEdit = false;
                UpdateEditText();
            }
            else
            {
                UserEdit = true;
            }

            Debug.Assert(domainIndex >= 0 || UserEdit == true, "UserEdit should be true when domainIndex < 0 " + UserEdit);
        }

        /// <summary>
        ///  Sorts the domain values
        /// </summary>
        private void SortDomainItems()
        {
            if (inSort)
            {
                return;
            }

            inSort = true;
            try
            {
                // Sanity check
                Debug.Assert(sorted == true, "Sorted == false");
                if (!sorted)
                {
                    return;
                }

                if (domainItems != null)
                {

                    // Sort the domain values
                    ArrayList.Adapter(domainItems).Sort(new DomainUpDownItemCompare());

                    // Update the domain index
                    if (!UserEdit)
                    {
                        int newIndex = MatchIndex(stringValue, true);
                        if (newIndex != -1)
                        {
                            SelectIndex(newIndex);
                        }
                    }
                }
            }
            finally
            {
                inSort = false;
            }
        }

        /// <summary>
        ///  Provides some interesting info about this control in String form.
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();

            if (Items != null)
            {
                s += ", Items.Count: " + Items.Count.ToString(CultureInfo.CurrentCulture);
                s += ", SelectedIndex: " + SelectedIndex.ToString(CultureInfo.CurrentCulture);
            }
            return s;
        }

        /// <summary>
        ///  Displays the previous item in the collection.
        /// </summary>
        public override void UpButton()
        {
            // Make sure domain values exist, and there are >0 items
            if (domainItems == null)
            {
                return;
            }
            if (domainItems.Count <= 0)
            {
                return;
            }

            // If the user has entered text, attempt to match it to the domain list
            int matchIndex = -1;
            if (UserEdit)
            {
                matchIndex = MatchIndex(Text, false, domainIndex);
            }
            if (matchIndex != -1)
            {
                // Found a match, so set the domain index accordingly
                domainIndex = matchIndex;
                SelectIndex(matchIndex);
            }
            else
            {
                // Otherwise, get the previous string in the domain list
                if (domainIndex > 0)
                {
                    SelectIndex(domainIndex - 1);
                }
                else if (Wrap)
                {
                    SelectIndex(domainItems.Count - 1);
                }
            }
        }

        /// <summary>
        ///  Updates the text in the up-down control to display the selected item.
        /// </summary>
        protected override void UpdateEditText()
        {
            Debug.Assert(!UserEdit, "UserEdit should be false");
            // Defensive programming
            UserEdit = false;

            ChangingText = true;
            Text = stringValue;
        }
        // This is not a breaking change -- Even though this control previously autosized to hieght,
        // it didn't actually have an AutoSize property.  The new AutoSize property enables the
        // smarter behavior.
        internal override Size GetPreferredSizeCore(Size proposedConstraints)
        {
            int height = PreferredHeight;
            int width = LayoutUtils.OldGetLargestStringSizeInCollection(Font, Items).Width;

            // AdjuctWindowRect with our border, since textbox is borderless.
            width = SizeFromClientSize(width, height).Width + upDownButtons.Width;
            return new Size(width, height) + Padding.Size;
        }

        // DomainUpDown collection class

        /// <summary>
        ///  Encapsulates a collection of objects for use by the <see cref='DomainUpDown'/>
        ///  class.
        /// </summary>
        public class DomainUpDownItemCollection : ArrayList
        {
            readonly DomainUpDown owner;

            internal DomainUpDownItemCollection(DomainUpDown owner)
            : base()
            {
                this.owner = owner;
            }

            /// <summary>
            /// </summary>
            [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public override object this[int index]
            {
                get
                {
                    return base[index];
                }

                set
                {
                    base[index] = value;

                    if (owner.SelectedIndex == index)
                    {
                        owner.SelectIndex(index);
                    }

                    if (owner.Sorted)
                    {
                        owner.SortDomainItems();
                    }
                }
            }

            /// <summary>
            /// </summary>
            public override int Add(object item)
            {
                // Overridden to perform sorting after adding an item

                int ret = base.Add(item);
                if (owner.Sorted)
                {
                    owner.SortDomainItems();
                }
                return ret;
            }

            /// <summary>
            /// </summary>
            public override void Remove(object item)
            {
                int index = IndexOf(item);

                if (index == -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(item), item, string.Format(SR.InvalidArgument, nameof(item), item));
                }
                else
                {
                    RemoveAt(index);
                }
            }

            /// <summary>
            /// </summary>
            public override void RemoveAt(int item)
            {
                // Overridden to update the domain index if neccessary
                base.RemoveAt(item);

                if (item < owner.domainIndex)
                {
                    // The item removed was before the currently selected item
                    owner.SelectIndex(owner.domainIndex - 1);
                }
                else if (item == owner.domainIndex)
                {
                    // The currently selected item was removed
                    //
                    owner.SelectIndex(-1);
                }
            }

            /// <summary>
            /// </summary>
            public override void Insert(int index, object item)
            {
                base.Insert(index, item);
                if (owner.Sorted)
                {
                    owner.SortDomainItems();
                }
            }
        } // end class DomainUpDownItemCollection

        private sealed class DomainUpDownItemCompare : IComparer
        {
            public int Compare(object p, object q)
            {
                if (p == q)
                {
                    return 0;
                }

                if (p == null || q == null)
                {
                    return 0;
                }

                return string.Compare(p.ToString(), q.ToString(), false, CultureInfo.CurrentCulture);
            }
        }

        [ComVisible(true)]
        public class DomainUpDownAccessibleObject : ControlAccessibleObject
        {
            private DomainItemListAccessibleObject itemList;

            /// <summary>
            /// </summary>
            public DomainUpDownAccessibleObject(Control owner) : base(owner)
            {
            }

            /// <summary>
            ///  Gets or sets the accessible name.
            /// </summary>
            public override string Name
            {
                get
                {
                    string baseName = base.Name;
                    return ((DomainUpDown)Owner).GetAccessibleName(baseName);
                }
                set
                {
                    base.Name = value;
                }
            }

            private DomainItemListAccessibleObject ItemList
            {
                get
                {
                    if (itemList == null)
                    {
                        itemList = new DomainItemListAccessibleObject(this);
                    }
                    return itemList;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }
                    else
                    {
                        return AccessibleRole.SpinButton;
                    }
                }
            }

            /// <summary>
            /// </summary>
            public override AccessibleObject GetChild(int index)
            {
                switch (index)
                {
                    // TextBox child
                    //
                    case 0:
                        return ((UpDownBase)Owner).TextBox.AccessibilityObject.Parent;

                    // Up/down buttons
                    //
                    case 1:
                        return ((UpDownBase)Owner).UpDownButtonsInternal.AccessibilityObject.Parent;

                    case 2:
                        return ItemList;
                }

                return null;
            }

            /// <summary>
            /// </summary>
            public override int GetChildCount()
            {
                return 3;
            }
        }

        internal class DomainItemListAccessibleObject : AccessibleObject
        {
            private readonly DomainUpDownAccessibleObject parent;

            public DomainItemListAccessibleObject(DomainUpDownAccessibleObject parent) : base()
            {
                this.parent = parent;
            }

            public override string Name
            {
                get
                {
                    string baseName = base.Name;
                    if (baseName == null || baseName.Length == 0)
                    {
                        return "Items";
                    }
                    return baseName;
                }
                set
                {
                    base.Name = value;
                }
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return parent;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.List;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    return AccessibleStates.Invisible | AccessibleStates.Offscreen;
                }
            }

            public override AccessibleObject GetChild(int index)
            {

                if (index >= 0 && index < GetChildCount())
                {
                    return new DomainItemAccessibleObject(((DomainUpDown)parent.Owner).Items[index].ToString(), this);
                }

                return null;
            }

            public override int GetChildCount()
            {
                return ((DomainUpDown)parent.Owner).Items.Count;
            }

        }

        [ComVisible(true)]
        public class DomainItemAccessibleObject : AccessibleObject
        {
            private string name;
            private readonly DomainItemListAccessibleObject parent;

            public DomainItemAccessibleObject(string name, AccessibleObject parent) : base()
            {
                this.name = name;
                this.parent = (DomainItemListAccessibleObject)parent;
            }

            public override string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return parent;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.ListItem;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    return AccessibleStates.Selectable;
                }
            }

            public override string Value
            {
                get
                {
                    return name;
                }
            }
        }

    }
}
