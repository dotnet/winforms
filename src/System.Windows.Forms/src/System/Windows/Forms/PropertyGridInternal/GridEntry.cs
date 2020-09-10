// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms.Design;
using System.Windows.Forms.VisualStyles;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    /// <summary>
    ///  Base Entry for properties to be displayed in properties window.
    /// </summary>
    internal abstract class GridEntry : GridItem, ITypeDescriptorContext
    {
        protected static readonly Point InvalidPoint = new Point(int.MinValue, int.MinValue);
        private static readonly BooleanSwitch PbrsAssertPropsSwitch = new BooleanSwitch("PbrsAssertProps", "PropertyBrowser : Assert on broken properties");

        internal static AttributeTypeSorter AttributeTypeSorter = new AttributeTypeSorter();

        // Type flags
        internal const int FLAG_TEXT_EDITABLE = 0x0001;
        internal const int FLAG_ENUMERABLE = 0x0002;
        internal const int FLAG_CUSTOM_PAINT = 0x0004;
        internal const int FLAG_IMMEDIATELY_EDITABLE = 0x0008;
        internal const int FLAG_CUSTOM_EDITABLE = 0x0010;
        internal const int FLAG_DROPDOWN_EDITABLE = 0x0020;
        internal const int FLAG_LABEL_BOLD = 0x0040;
        internal const int FLAG_READONLY_EDITABLE = 0x0080;
        internal const int FLAG_RENDER_READONLY = 0x0100;
        internal const int FLAG_IMMUTABLE = 0x0200;
        internal const int FLAG_FORCE_READONLY = 0x0400;
        internal const int FLAG_RENDER_PASSWORD = 0x1000;

        internal const int FLAG_DISPOSED = 0x2000;

        internal const int FL_EXPAND = 0x00010000;
        internal const int FL_EXPANDABLE = 0x00020000;
        //protected const int FL_EXPANDABLE_VALID         = 0x00040000;
        internal const int FL_EXPANDABLE_FAILED = 0x00080000;
        internal const int FL_NO_CUSTOM_PAINT = 0x00100000;
        internal const int FL_CATEGORIES = 0x00200000;
        internal const int FL_CHECKED = unchecked((int)0x80000000);

        // rest are GridEntry constants.

        protected const int NOTIFY_RESET = 1;
        protected const int NOTIFY_CAN_RESET = 2;
        protected const int NOTIFY_DBL_CLICK = 3;
        protected const int NOTIFY_SHOULD_PERSIST = 4;
        protected const int NOTIFY_RETURN = 5;

        protected const int OUTLINE_ICON_PADDING = 5;

        protected static IComparer DisplayNameComparer = new DisplayNameSortComparer();

        private static char passwordReplaceChar;
        //Maximum number of characters we'll show in the property grid.  Too many characters leads
        //to bad performance.
        private const int maximumLengthOfPropertyString = 1000;

        [Flags]
        internal enum PaintValueFlags
        {
            None = 0,
            DrawSelected = 0x1,
            FetchValue = 0x2,
            CheckShouldSerialize = 0x4,
            PaintInPlace = 0x8
        }

        private class CacheItems
        {
            public string lastLabel;
            public Font lastLabelFont;
            public int lastLabelWidth;
            public string lastValueString;
            public Font lastValueFont;
            public int lastValueTextWidth;
            public object lastValue;
            public bool useValueString;
            public bool lastShouldSerialize;
            public bool useShouldSerialize;
            public bool useCompatTextRendering;
        }

        private CacheItems cacheItems;

        // instance variables.
        protected TypeConverter converter;
        protected UITypeEditor editor;
        internal GridEntry parentPE;
        private GridEntryCollection childCollection;
        internal int flags;
        private int propertyDepth;
        protected bool hasFocus;
        private Rectangle outlineRect = Rectangle.Empty;
        protected PropertySort PropertySort;

        protected Point labelTipPoint = InvalidPoint;
        protected Point valueTipPoint = InvalidPoint;

        protected PropertyGrid ownerGrid;

        private static readonly object EVENT_VALUE_CLICK = new object();
        private static readonly object EVENT_LABEL_CLICK = new object();
        private static readonly object EVENT_OUTLINE_CLICK = new object();
        private static readonly object EVENT_VALUE_DBLCLICK = new object();
        private static readonly object EVENT_LABEL_DBLCLICK = new object();
        private static readonly object EVENT_OUTLINE_DBLCLICK = new object();
        private static readonly object EVENT_RECREATE_CHILDREN = new object();

        private GridEntryAccessibleObject accessibleObject;

        private bool lastPaintWithExplorerStyle;

        private static Color InvertColor(Color color)
        {
            return Color.FromArgb(color.A, (byte)~color.R, (byte)~color.G, (byte)~color.B);
        }

        protected GridEntry(PropertyGrid owner, GridEntry peParent)
        {
            parentPE = peParent;
            ownerGrid = owner;

            Debug.Assert(ownerGrid != null, "GridEntry w/o PropertyGrid owner, text rendering will fail.");

            if (peParent != null)
            {
                propertyDepth = peParent.PropertyDepth + 1;
                PropertySort = peParent.PropertySort;

                if (peParent.ForceReadOnly)
                {
                    flags |= FLAG_FORCE_READONLY;
                }
            }
            else
            {
                propertyDepth = -1;
            }
        }

        /// <summary>
        ///  Outline Icon padding
        /// </summary>
        private int OutlineIconPadding
        {
            get
            {
                if (DpiHelper.IsScalingRequirementMet)
                {
                    if (GridEntryHost != null)
                    {
                        return GridEntryHost.LogicalToDeviceUnits(OUTLINE_ICON_PADDING);
                    }
                }

                return OUTLINE_ICON_PADDING;
            }
        }

        private bool colorInversionNeededInHC
        {
            get
            {
                return SystemInformation.HighContrast && !OwnerGrid._developerOverride;
            }
        }

        public AccessibleObject AccessibilityObject
        {
            get
            {
                if (accessibleObject is null)
                {
                    accessibleObject = GetAccessibilityObject();
                }
                return accessibleObject;
            }
        }

        protected virtual GridEntryAccessibleObject GetAccessibilityObject()
        {
            return new GridEntryAccessibleObject(this);
        }

        /// <summary>
        ///  Specify that this grid entry should be allowed to be merged for multi-select.
        /// </summary>
        public virtual bool AllowMerge => true;

        internal virtual bool AlwaysAllowExpand => false;

        internal virtual AttributeCollection Attributes => TypeDescriptor.GetAttributes(PropertyType);

        /// <summary>
        ///  Gets the value of the background brush to use. Override this member to cause the entry to paint it's
        ///  background in a different color. The base implementation returns null.
        /// </summary>
        protected virtual Color GetBackgroundColor() => GridEntryHost.BackColor;

        protected virtual Color LabelTextColor
            => ShouldRenderReadOnly ? GridEntryHost.GrayTextColor : GridEntryHost.GetTextColor();

        /// <summary>
        ///  The set of attributes that will be used for browse filtering
        /// </summary>
        public virtual AttributeCollection BrowsableAttributes
        {
            get => parentPE?.BrowsableAttributes;
            set => parentPE.BrowsableAttributes = value;
        }

        /// <summary>
        ///  Retrieves the component that is invoking the
        ///  method on the formatter object.  This may
        ///  return null if there is no component
        ///  responsible for the call.
        /// </summary>
        public virtual IComponent Component
        {
            get
            {
                object owner = GetValueOwner();
                if (owner is IComponent)
                {
                    return (IComponent)owner;
                }
                if (parentPE != null)
                {
                    return parentPE.Component;
                }
                return null;
            }
        }

        protected virtual IComponentChangeService ComponentChangeService
        {
            get
            {
                return parentPE.ComponentChangeService;
            }
        }

        /// <summary>
        ///  Retrieves the container that contains the
        ///  set of objects this formatter may work
        ///  with.  It may return null if there is no
        ///  container, or of the formatter should not
        ///  use any outside objects.
        /// </summary>
        public virtual IContainer Container
        {
            get
            {
                IComponent component = Component;
                if (component != null)
                {
                    ISite site = component.Site;
                    if (site != null)
                    {
                        return site.Container;
                    }
                }
                return null;
            }
        }

        protected GridEntryCollection ChildCollection
        {
            get
            {
                if (childCollection is null)
                {
                    childCollection = new GridEntryCollection(this, null);
                }
                return childCollection;
            }
            set
            {
                Debug.Assert(value is null || !Disposed, "Why are we putting new children in after we are disposed?");
                if (childCollection != value)
                {
                    if (childCollection != null)
                    {
                        childCollection.Dispose();
                        childCollection = null;
                    }
                    childCollection = value;
                }
            }
        }

        public int ChildCount
        {
            get
            {
                if (Children != null)
                {
                    return Children.Count;
                }
                return 0;
            }
        }

        public virtual GridEntryCollection Children
        {
            get
            {
                if (childCollection is null && !Disposed)
                {
                    CreateChildren();
                }
                return childCollection;
            }
        }

        public virtual PropertyTab CurrentTab
        {
            get
            {
                if (parentPE != null)
                {
                    return parentPE.CurrentTab;
                }
                return null;
            }
            set
            {
                if (parentPE != null)
                {
                    parentPE.CurrentTab = value;
                }
            }
        }

        /// <summary>
        ///  Returns the default child GridEntry of this item.  Usually the default property
        ///  of the target object.
        /// </summary>
        internal virtual GridEntry DefaultChild
        {
            get
            {
                return null;
            }
            set { }
        }

        internal virtual IDesignerHost DesignerHost
        {
            get
            {
                if (parentPE != null)
                {
                    return parentPE.DesignerHost;
                }
                return null;
            }
            set
            {
                if (parentPE != null)
                {
                    parentPE.DesignerHost = value;
                }
            }
        }

        internal bool Disposed
        {
            get
            {
                return GetFlagSet(FLAG_DISPOSED);
            }
        }

        internal virtual bool Enumerable
        {
            get
            {
                return (Flags & GridEntry.FLAG_ENUMERABLE) != 0;
            }
        }

        public override bool Expandable
        {
            get
            {
                bool fExpandable = GetFlagSet(FL_EXPANDABLE);

                if (fExpandable && childCollection != null && childCollection.Count > 0)
                {
                    return true;
                }

                if (GetFlagSet(FL_EXPANDABLE_FAILED))
                {
                    return false;
                }

                if (fExpandable && (cacheItems is null || cacheItems.lastValue is null) && PropertyValue is null)
                {
                    fExpandable = false;
                }

                return fExpandable;
            }
        }

        public override bool Expanded
        {
            get
            {
                return InternalExpanded;
            }
            set
            {
                GridEntryHost.SetExpand(this, value);
            }
        }

        internal virtual bool ForceReadOnly
        {
            get
            {
                return (flags & FLAG_FORCE_READONLY) != 0;
            }
        }

        internal virtual bool InternalExpanded
        {
            get
            {
                // short circuit if we don't have children
                if (childCollection is null || childCollection.Count == 0)
                {
                    return false;
                }
                return GetFlagSet(FL_EXPAND);
            }
            set
            {
                if (!Expandable || value == InternalExpanded)
                {
                    return;
                }

                if (childCollection != null && childCollection.Count > 0)
                {
                    SetFlag(FL_EXPAND, value);
                }
                else
                {
                    SetFlag(FL_EXPAND, false);
                    if (value)
                    {
                        bool fMakeSure = CreateChildren();
                        SetFlag(FL_EXPAND, fMakeSure);
                    }
                }

                // Notify accessibility clients of expanded state change
                // StateChange requires NameChange, too - accessible clients won't see this, unless both events are raised

                // Root item is hidden and should not raise events
                if (GridItemType != GridItemType.Root)
                {
                    int id = ((PropertyGridView)GridEntryHost).AccessibilityGetGridEntryChildID(this);
                    if (id >= 0)
                    {
                        PropertyGridView.PropertyGridViewAccessibleObject gridAccObj =
                            (PropertyGridView.PropertyGridViewAccessibleObject)((PropertyGridView)GridEntryHost).AccessibilityObject;

                        gridAccObj.NotifyClients(AccessibleEvents.StateChange, id);
                        gridAccObj.NotifyClients(AccessibleEvents.NameChange, id);
                    }
                }
            }
        }

        internal virtual int Flags
        {
            get
            {
                if ((flags & FL_CHECKED) != 0)
                {
                    return flags;
                }

                flags |= FL_CHECKED;

                TypeConverter converter = TypeConverter;
                UITypeEditor uiEditor = UITypeEditor;
                object value = Instance;
                bool forceReadOnly = ForceReadOnly;

                if (value != null)
                {
                    forceReadOnly |= TypeDescriptor.GetAttributes(value).Contains(InheritanceAttribute.InheritedReadOnly);
                }

                if (converter.GetStandardValuesSupported(this))
                {
                    flags |= GridEntry.FLAG_ENUMERABLE;
                }

                if (!forceReadOnly && converter.CanConvertFrom(this, typeof(string)) &&
                    !converter.GetStandardValuesExclusive(this))
                {
                    flags |= GridEntry.FLAG_TEXT_EDITABLE;
                }

                bool isImmutableReadOnly = TypeDescriptor.GetAttributes(PropertyType)[typeof(ImmutableObjectAttribute)].Equals(ImmutableObjectAttribute.Yes);
                bool isImmutable = isImmutableReadOnly || converter.GetCreateInstanceSupported(this);

                if (isImmutable)
                {
                    flags |= GridEntry.FLAG_IMMUTABLE;
                }

                if (converter.GetPropertiesSupported(this))
                {
                    flags |= GridEntry.FL_EXPANDABLE;

                    // If we're exapndable, but we don't support editing,
                    // make us read only editable so we don't paint grey.
                    //
                    if (!forceReadOnly && (flags & GridEntry.FLAG_TEXT_EDITABLE) == 0 && !isImmutableReadOnly)
                    {
                        flags |= GridEntry.FLAG_READONLY_EDITABLE;
                    }
                }

                if (Attributes.Contains(PasswordPropertyTextAttribute.Yes))
                {
                    flags |= GridEntry.FLAG_RENDER_PASSWORD;
                }

                if (uiEditor != null)
                {
                    if (uiEditor.GetPaintValueSupported(this))
                    {
                        flags |= GridEntry.FLAG_CUSTOM_PAINT;
                    }

                    // We only allow drop-downs if the object is NOT being inherited
                    // I would really rather this not be here, but we have other places where
                    // we make read-only properties editable if they have drop downs.  Not
                    // sure this is the right thing...is it?

                    bool allowButtons = !forceReadOnly;

                    if (allowButtons)
                    {
                        switch (uiEditor.GetEditStyle(this))
                        {
                            case UITypeEditorEditStyle.Modal:
                                flags |= GridEntry.FLAG_CUSTOM_EDITABLE;
                                if (!isImmutable && !PropertyType.IsValueType)
                                {
                                    flags |= GridEntry.FLAG_READONLY_EDITABLE;
                                }
                                break;
                            case UITypeEditorEditStyle.DropDown:
                                flags |= GridEntry.FLAG_DROPDOWN_EDITABLE;
                                break;
                        }
                    }
                }

                return flags;
            }
            set
            {
                flags = value;
            }
        }

        /// <summary>
        ///  Checks if the entry is currently expanded
        /// </summary>
        public bool Focus
        {
            get
            {
                return hasFocus;
            }
            set
            {
                if (Disposed)
                {
                    return;
                }

                if (cacheItems != null)
                {
                    cacheItems.lastValueString = null;
                    cacheItems.useValueString = false;
                    cacheItems.useShouldSerialize = false;
                }

                if (hasFocus != value)
                {
                    hasFocus = value;

                    // Notify accessibility applications that keyboard focus has changed.
                    //
                    if (value == true)
                    {
                        int id = ((PropertyGridView)GridEntryHost).AccessibilityGetGridEntryChildID(this);
                        if (id >= 0)
                        {
                            PropertyGridView.PropertyGridViewAccessibleObject gridAccObj =
                                (PropertyGridView.PropertyGridViewAccessibleObject)((PropertyGridView)GridEntryHost).AccessibilityObject;

                            gridAccObj.NotifyClients(AccessibleEvents.Focus, id);
                            gridAccObj.NotifyClients(AccessibleEvents.Selection, id);

                            AccessibilityObject.SetFocus();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Returns the label including the object name, and properties.  For example, the value
        ///  of the Font size property on a Button called Button1 would be "Button1.Font.Size"
        /// </summary>
        public string FullLabel
        {
            get
            {
                string str = null;
                if (parentPE != null)
                {
                    str = parentPE.FullLabel;
                }

                if (str != null)
                {
                    str += ".";
                }
                else
                {
                    str = string.Empty;
                }
                str += PropertyLabel;

                return str;
            }
        }

        public override GridItemCollection GridItems
        {
            get
            {
                if (Disposed)
                {
                    throw new ObjectDisposedException(SR.GridItemDisposed);
                }

                if (IsExpandable && childCollection != null && childCollection.Count == 0)
                {
                    CreateChildren();
                }

                return Children;
            }
        }

        internal virtual PropertyGridView GridEntryHost
        {
            get
            {        // ACCESSOR: virtual was missing from this get
                if (parentPE != null)
                {
                    return parentPE.GridEntryHost;
                }
                return null;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override GridItemType GridItemType
        {
            get
            {
                return GridItemType.Property;
            }
        }

        /// <summary>
        ///  Returns true if this GridEntry has a value field in the right hand column.
        /// </summary>
        internal virtual bool HasValue
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///  Retrieves the keyword that the VS help dynamic help window will
        ///  use when this IPE is selected.
        /// </summary>
        public virtual string HelpKeyword
        {
            get
            {
                string keyWord = null;

                if (parentPE != null)
                {
                    keyWord = parentPE.HelpKeyword;
                }
                if (keyWord is null)
                {
                    keyWord = string.Empty;
                }

                return keyWord;
            }
        }

        internal virtual string HelpKeywordInternal
        {
            get
            {
                return HelpKeyword;
            }
        }

        public virtual bool IsCustomPaint
        {
            get
            {
                // prevent full flag population if possible.
                if ((flags & FL_CHECKED) == 0)
                {
                    UITypeEditor typeEd = UITypeEditor;
                    if (typeEd != null)
                    {
                        if ((flags & GridEntry.FLAG_CUSTOM_PAINT) != 0 ||
                            (flags & GridEntry.FL_NO_CUSTOM_PAINT) != 0)
                        {
                            return (flags & GridEntry.FLAG_CUSTOM_PAINT) != 0;
                        }

                        if (typeEd.GetPaintValueSupported(this))
                        {
                            flags |= GridEntry.FLAG_CUSTOM_PAINT;
                            return true;
                        }
                        else
                        {
                            flags |= GridEntry.FL_NO_CUSTOM_PAINT;
                            return false;
                        }
                    }
                }
                return (Flags & GridEntry.FLAG_CUSTOM_PAINT) != 0;
            }
        }

        public virtual bool IsExpandable
        {
            get
            {
                return Expandable;
            }
            set
            {
                if (value != GetFlagSet(FL_EXPANDABLE))
                {
                    SetFlag(FL_EXPANDABLE_FAILED, false);
                    SetFlag(FL_EXPANDABLE, value);
                }
            }
        }

        public virtual bool IsTextEditable
        {
            get
            {
                return IsValueEditable && (Flags & GridEntry.FLAG_TEXT_EDITABLE) != 0;
            }
        }

        public virtual bool IsValueEditable
        {
            get
            {
                return !ForceReadOnly && 0 != (Flags & (GridEntry.FLAG_DROPDOWN_EDITABLE | GridEntry.FLAG_TEXT_EDITABLE | GridEntry.FLAG_CUSTOM_EDITABLE | GridEntry.FLAG_ENUMERABLE));
            }
        }

        /// <summary>
        ///  Retrieves the component that is invoking the
        ///  method on the formatter object.  This may
        ///  return null if there is no component
        ///  responsible for the call.
        /// </summary>
        public virtual object Instance
        {
            get
            {
                object owner = GetValueOwner();

                if (parentPE != null && owner is null)
                {
                    return parentPE.Instance;
                }
                return owner;
            }
        }

        public override string Label
        {
            get
            {
                return PropertyLabel;
            }
        }

        /// <summary>
        ///  Retrieves the PropertyDescriptor that is surfacing the given object/
        /// </summary>
        public override PropertyDescriptor PropertyDescriptor
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        ///  Returns the pixel indent of the current GridEntry's label.
        /// </summary>
        internal virtual int PropertyLabelIndent
        {
            get
            {
                int borderWidth = GridEntryHost.GetOutlineIconSize() + OUTLINE_ICON_PADDING;
                return ((propertyDepth + 1) * borderWidth) + 1;
            }
        }

        internal virtual Point GetLabelToolTipLocation(int mouseX, int mouseY)
        {
            return labelTipPoint;
        }

        internal virtual string LabelToolTipText
        {
            get
            {
                return PropertyLabel;
            }
        }

        public virtual bool NeedsDropDownButton
        {
            get
            {
                return (Flags & GridEntry.FLAG_DROPDOWN_EDITABLE) != 0;
            }
        }

        public virtual bool NeedsCustomEditorButton
        {
            get
            {
                return (Flags & GridEntry.FLAG_CUSTOM_EDITABLE) != 0 && (IsValueEditable || (Flags & GridEntry.FLAG_READONLY_EDITABLE) != 0);
            }
        }

        public PropertyGrid OwnerGrid
        {
            get
            {
                return ownerGrid;
            }
        }

        /// <summary>
        ///  Returns rect that the outline icon (+ or - or arrow) will be drawn into, relative
        ///  to the upper left corner of the GridEntry.
        /// </summary>
        public Rectangle OutlineRect
        {
            get
            {
                if (!outlineRect.IsEmpty)
                {
                    return outlineRect;
                }
                PropertyGridView gridHost = GridEntryHost;
                Debug.Assert(gridHost != null, "No propEntryHost!");
                int outlineSize = gridHost.GetOutlineIconSize();
                int borderWidth = outlineSize + OutlineIconPadding;
                int left = (propertyDepth * borderWidth) + (OutlineIconPadding) / 2;
                int top = (gridHost.GetGridEntryHeight() - outlineSize) / 2;
                outlineRect = new Rectangle(left, top, outlineSize, outlineSize);
                return outlineRect;
            }
            set
            {
                // set property is required to reset cached value when dpi changed.
                if (value != outlineRect)
                {
                    outlineRect = value;
                }
            }
        }

        public virtual GridEntry ParentGridEntry
        {
            get
            {
                return parentPE;
            }
            set
            {
                Debug.Assert(value != this, "how can we be our own parent?");
                parentPE = value;
                if (value != null)
                {
                    propertyDepth = value.PropertyDepth + 1;

                    // Microsoft, why do we do this?
                    if (childCollection != null)
                    {
                        for (int i = 0; i < childCollection.Count; i++)
                        {
                            childCollection.GetEntry(i).ParentGridEntry = this;
                        }
                    }
                }
            }
        }

        public override GridItem Parent
        {
            get
            {
                if (Disposed)
                {
                    throw new ObjectDisposedException(SR.GridItemDisposed);
                }

                GridItem parent = ParentGridEntry;

                // don't allow walking all the way up to the parent.
                //
                //if (parent is IRootGridEntry) {
                //    return null;
                //}
                return parent;
            }
        }

        /// <summary>
        ///  Returns category name of the current property
        /// </summary>
        public virtual string PropertyCategory
        {
            get
            {
                return CategoryAttribute.Default.Category;
            }
        }

        /// <summary>
        ///  Returns "depth" of this property.  That is, how many parent's between
        ///  this property and the root property.  The root property has a depth of -1.
        /// </summary>
        public virtual int PropertyDepth
        {
            get
            {
                return propertyDepth;
            }
        }

        /// <summary>
        ///  Returns the description helpstring for this GridEntry.
        /// </summary>
        public virtual string PropertyDescription
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        ///  Returns the label of this property.  Usually
        ///  this is the property name.
        /// </summary>
        public virtual string PropertyLabel
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        ///  Returns non-localized name of this property.
        /// </summary>
        public virtual string PropertyName
        {
            get
            {
                return PropertyLabel;
            }
        }

        /// <summary>
        ///  Returns the Type of the value of this GridEntry, or null if the value is null.
        /// </summary>
        public virtual Type PropertyType
        {
            get
            {
                object obj = PropertyValue;
                if (obj != null)
                {
                    return obj.GetType();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        ///  Gets or sets the value for the property that is represented
        ///  by this GridEntry.
        /// </summary>
        public virtual object PropertyValue
        {
            get
            {
                if (cacheItems != null)
                {
                    return cacheItems.lastValue;
                }
                return null;
            }
            set
            {
            }
        }

        public virtual bool ShouldRenderPassword
        {
            get
            {
                return (Flags & GridEntry.FLAG_RENDER_PASSWORD) != 0;
            }
        }

        public virtual bool ShouldRenderReadOnly
        {
            get
            {
                return ForceReadOnly || (0 != (Flags & GridEntry.FLAG_RENDER_READONLY) || (!IsValueEditable && (0 == (Flags & GridEntry.FLAG_READONLY_EDITABLE))));
            }
        }

        /// <summary>
        ///  Returns the type converter for this entry.
        /// </summary>
        internal virtual TypeConverter TypeConverter
        {
            get
            {
                if (converter is null)
                {
                    object value = PropertyValue;
                    if (value is null)
                    {
                        converter = TypeDescriptor.GetConverter(PropertyType);
                    }
                    else
                    {
                        converter = TypeDescriptor.GetConverter(value);
                    }
                }
                return converter;
            }
        }

        /// <summary>
        ///  Returns the type editor for this entry.  This may return null if there
        ///  is no type editor.
        /// </summary>
        internal virtual UITypeEditor UITypeEditor
        {
            get
            {
                if (editor is null && PropertyType != null)
                {
                    editor = (UITypeEditor)TypeDescriptor.GetEditor(PropertyType, typeof(UITypeEditor));
                }

                return editor;
            }
        }

        public override object Value
        {
            get
            {
                return PropertyValue;
            }
            // note: we don't do set because of the value class semantics, etc.
        }

        internal Point ValueToolTipLocation
        {
            get
            {
                return ShouldRenderPassword ? InvalidPoint : valueTipPoint;
            }
            set
            {
                valueTipPoint = value;
            }
        }

        internal int VisibleChildCount
        {
            get
            {
                if (!Expanded)
                {
                    return 0;
                }
                int count = ChildCount;
                int totalCount = count;
                for (int i = 0; i < count; i++)
                {
                    totalCount += ChildCollection.GetEntry(i).VisibleChildCount;
                }
                return totalCount;
            }
        }

        /// <summary>
        ///  Add an event handler to be invoked when the label portion of
        ///  the prop entry is clicked
        /// </summary>
        public virtual void AddOnLabelClick(EventHandler h)
        {
            AddEventHandler(EVENT_LABEL_CLICK, h);
        }

        /// <summary>
        ///  Add an event handler to be invoked when the label portion of
        ///  the prop entry is double
        /// </summary>
        public virtual void AddOnLabelDoubleClick(EventHandler h)
        {
            AddEventHandler(EVENT_LABEL_DBLCLICK, h);
        }

        /// <summary>
        ///  Add an event handler to be invoked when the value portion of
        ///  the prop entry is clicked
        /// </summary>
        public virtual void AddOnValueClick(EventHandler h)
        {
            AddEventHandler(EVENT_VALUE_CLICK, h);
        }

        /// <summary>
        ///  Add an event handler to be invoked when the value portion of
        ///  the prop entry is double-clicked
        /// </summary>
        public virtual void AddOnValueDoubleClick(EventHandler h)
        {
            AddEventHandler(EVENT_VALUE_DBLCLICK, h);
        }

        /// <summary>
        ///  Add an event handler to be invoked when the outline icone portion of
        ///  the prop entry is clicked
        /// </summary>
        public virtual void AddOnOutlineClick(EventHandler h)
        {
            AddEventHandler(EVENT_OUTLINE_CLICK, h);
        }

        /// <summary>
        ///  Add an event handler to be invoked when the outline icone portion of
        ///  the prop entry is double clicked
        /// </summary>
        public virtual void AddOnOutlineDoubleClick(EventHandler h)
        {
            AddEventHandler(EVENT_OUTLINE_DBLCLICK, h);
        }

        /// <summary>
        ///  Add an event handler to be invoked when the children grid entries are re-created.
        /// </summary>
        public virtual void AddOnRecreateChildren(GridEntryRecreateChildrenEventHandler h)
        {
            AddEventHandler(EVENT_RECREATE_CHILDREN, h);
        }

        internal void ClearCachedValues()
        {
            ClearCachedValues(true);
        }

        internal void ClearCachedValues(bool clearChildren)
        {
            if (cacheItems != null)
            {
                cacheItems.useValueString = false;
                cacheItems.lastValue = null;
                cacheItems.useShouldSerialize = false;
            }
            if (clearChildren)
            {
                for (int i = 0; i < ChildCollection.Count; i++)
                {
                    ChildCollection.GetEntry(i).ClearCachedValues();
                }
            }
        }

        /// <summary>
        ///  Converts the given string of text to a value.
        /// </summary>
        public object ConvertTextToValue(string text)
        {
            if (TypeConverter.CanConvertFrom(this, typeof(string)))
            {
                return TypeConverter.ConvertFromString(this, text);
            }
            return text;
        }

        /// <summary>
        ///  Create the base prop entries given an object or set of objects
        /// </summary>
        internal static IRootGridEntry Create(PropertyGridView view, object[] rgobjs, IServiceProvider baseProvider, IDesignerHost currentHost, PropertyTab tab, PropertySort initialSortType)
        {
            IRootGridEntry pe = null;

            if (rgobjs is null || rgobjs.Length == 0)
            {
                return null;
            }

            try
            {
                if (rgobjs.Length == 1)
                {
                    pe = new SingleSelectRootGridEntry(view, rgobjs[0], baseProvider, currentHost, tab, initialSortType);
                }
                else
                {
                    pe = new MultiSelectRootGridEntry(view, rgobjs, baseProvider, currentHost, tab, initialSortType);
                }
            }
            catch (Exception e)
            {
                Debug.Fail(e.ToString());
                throw;
            }
            return pe;
        }

        /// <summary>
        ///  Populates the children of this grid entry
        /// </summary>
        protected virtual bool CreateChildren()
        {
            return CreateChildren(false);
        }

        /// <summary>
        ///  Populates the children of this grid entry
        /// </summary>
        protected virtual bool CreateChildren(bool diffOldChildren)
        {
            Debug.Assert(!Disposed, "Why are we creating children after we are disposed?");

            if (!GetFlagSet(FL_EXPANDABLE))
            {
                if (childCollection != null)
                {
                    childCollection.Clear();
                }
                else
                {
                    childCollection = new GridEntryCollection(this, Array.Empty<GridEntry>());
                }
                return false;
            }

            if (!diffOldChildren && childCollection != null && childCollection.Count > 0)
            {
                return true;
            }

            GridEntry[] childProps = GetPropEntries(this,
                                                        PropertyValue,
                                                        PropertyType);

            bool fExpandable = (childProps != null && childProps.Length > 0);

            if (diffOldChildren && childCollection != null && childCollection.Count > 0)
            {
                bool same = true;
                if (childProps.Length == childCollection.Count)
                {
                    for (int i = 0; i < childProps.Length; i++)
                    {
                        if (!childProps[i].NonParentEquals(childCollection[i]))
                        {
                            same = false;
                            break;
                        }
                    }
                }
                else
                {
                    same = false;
                }

                if (same)
                {
                    return true;
                }
            }

            if (!fExpandable)
            {
                SetFlag(FL_EXPANDABLE_FAILED, true);
                if (childCollection != null)
                {
                    childCollection.Clear();
                }
                else
                {
                    childCollection = new GridEntryCollection(this, Array.Empty<GridEntry>());
                }

                if (InternalExpanded)
                {
                    InternalExpanded = false;
                }
            }
            else
            {
                if (childCollection != null)
                {
                    childCollection.Clear();
                    childCollection.AddRange(childProps);
                }
                else
                {
                    childCollection = new GridEntryCollection(this, childProps);
                }
            }
            return fExpandable;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            // make sure we don't accidentally
            // check flags in this state...
            flags |= FL_CHECKED;

            SetFlag(FLAG_DISPOSED, true);

            cacheItems = null;
            converter = null;
            editor = null;
            accessibleObject = null;

            if (disposing)
            {
                DisposeChildren();
            }
        }

        /// <summary>
        ///  Disposes the array of children
        /// </summary>
        public virtual void DisposeChildren()
        {
            if (childCollection != null)
            {
                childCollection.Dispose();
                childCollection = null;
            }
        }

        ~GridEntry()
        {
            Dispose(false);
        }

        /// <summary>
        ///  Invokes the type editor for editing this item.
        /// </summary>
        internal virtual void EditPropertyValue(PropertyGridView iva)
        {
            if (UITypeEditor != null)
            {
                try
                {
                    // Since edit value can push a modal loop
                    // there is a chance that this gridentry will be zombied before
                    // it returns.  Make sure we're not disposed.
                    //
                    object originalValue = PropertyValue;
                    object value = UITypeEditor.EditValue(this, (IServiceProvider)(ITypeDescriptorContext)this, originalValue);

                    if (Disposed)
                    {
                        return;
                    }

                    // Push the new value back into the property
                    if (value != originalValue && IsValueEditable)
                    {
                        iva.CommitValue(this, value);
                    }

                    if (InternalExpanded)
                    {
                        // QFE#3299: If edited property is expanded to show sub-properties, then we want to
                        // preserve the expanded states of it and all of its descendants. RecreateChildren()
                        // has logic that is supposed to do this, but which is fundamentally flawed.
                        PropertyGridView.GridPositionData positionData = GridEntryHost.CaptureGridPositionData();
                        InternalExpanded = false;
                        RecreateChildren();
                        positionData.Restore(GridEntryHost);
                    }
                    else
                    {
                        // If edited property has no children or is collapsed, don't need to preserve expanded states.
                        // This limits the scope of the above QFE fix to just those cases where it is actually required.
                        RecreateChildren();
                    }
                }
                catch (Exception e)
                {
                    IUIService uiSvc = (IUIService)GetService(typeof(IUIService));
                    if (uiSvc != null)
                    {
                        uiSvc.ShowError(e);
                    }
                    else
                    {
                        RTLAwareMessageBox.Show(GridEntryHost, e.Message, SR.PBRSErrorTitle, MessageBoxButtons.OK,
                                MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
                    }
                }
            }
        }

        /// <summary>
        ///  Tests two GridEntries for equality
        /// </summary>
        public override bool Equals(object obj)
        {
            if (NonParentEquals(obj))
            {
                return ((GridEntry)obj).ParentGridEntry == ParentGridEntry;
            }
            return false;
        }

        /// <summary>
        ///  Searches for a value of a given property for a value editor user
        /// </summary>
        public virtual object FindPropertyValue(string propertyName, Type propertyType)
        {
            object owner = GetValueOwner();
            PropertyDescriptor property = TypeDescriptor.GetProperties(owner)[propertyName];
            if (property != null && property.PropertyType == propertyType)
            {
                return property.GetValue(owner);
            }

            if (parentPE != null)
            {
                return parentPE.FindPropertyValue(propertyName, propertyType);
            }

            return null;
        }

        /// <summary>
        ///  Returns the index of a child GridEntry
        /// </summary>
        internal virtual int GetChildIndex(GridEntry pe)
        {
            return Children.GetEntry(pe);
        }

        /// <summary>
        ///  Gets the components that own the current value.  This is usually the value of the
        ///  root entry, which is the object being browsed.  Walks up the GridEntry tree
        ///  looking for an owner that is an IComponent
        /// </summary>
        public virtual IComponent[] GetComponents()
        {
            IComponent component = Component;
            if (component != null)
            {
                return new IComponent[] { component };
            }
            return null;
        }

        protected int GetLabelTextWidth(string labelText, Graphics g, Font f)
        {
            if (cacheItems is null)
            {
                cacheItems = new CacheItems();
            }
            else if (cacheItems.useCompatTextRendering == ownerGrid.UseCompatibleTextRendering && cacheItems.lastLabel == labelText && f.Equals(cacheItems.lastLabelFont))
            {
                return cacheItems.lastLabelWidth;
            }

            SizeF textSize = PropertyGrid.MeasureTextHelper.MeasureText(ownerGrid, g, labelText, f);

            cacheItems.lastLabelWidth = (int)textSize.Width;
            cacheItems.lastLabel = labelText;
            cacheItems.lastLabelFont = f;
            cacheItems.useCompatTextRendering = ownerGrid.UseCompatibleTextRendering;

            return cacheItems.lastLabelWidth;
        }

        internal int GetValueTextWidth(string valueString, Graphics g, Font f)
        {
            if (cacheItems is null)
            {
                cacheItems = new CacheItems();
            }
            else if (cacheItems.lastValueTextWidth != -1 && cacheItems.lastValueString == valueString && f.Equals(cacheItems.lastValueFont))
            {
                return cacheItems.lastValueTextWidth;
            }

            // Value text is rendered using GDI directly (No TextRenderer) but measured/adjusted using GDI+ (since previous releases), so don't use MeasureTextHelper.
            cacheItems.lastValueTextWidth = (int)g.MeasureString(valueString, f).Width;
            cacheItems.lastValueString = valueString;
            cacheItems.lastValueFont = f;
            return cacheItems.lastValueTextWidth;
        }
        // To check if text contains multiple lines
        //
        internal bool GetMultipleLines(string valueString)
        {
            if (valueString.IndexOf('\n') > 0 || valueString.IndexOf('\r') > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        ///  Gets the owner of the current value.  This is usually the value of the
        ///  root entry, which is the object being browsed
        /// </summary>
        public virtual object GetValueOwner()
        {
            if (parentPE is null)
            {
                return PropertyValue;
            }

            return parentPE.GetChildValueOwner(this);
        }

        /// <summary>
        ///  Gets the owners of the current value.  This is usually the value of the
        ///  root entry, which is the objects being browsed for a multiselect item
        /// </summary>
        public virtual object[] GetValueOwners()
        {
            object owner = GetValueOwner();
            if (owner != null)
            {
                return new object[] { owner };
            }
            return null;
        }

        /// <summary>
        ///  Gets the owner of the current value.  This is usually the value of the
        ///  root entry, which is the object being browsed
        /// </summary>
        public virtual object GetChildValueOwner(GridEntry childEntry)
        {
            /*// make sure this is one of our children
            int index = GetChildIndex(childEntry);

            if (index != -1){
               return this.PropertyValue;
            }

            Debug.Fail(childEntry.PropertyLabel + " is not a child of " + this.PropertyLabel);
            return null;*/
            return PropertyValue;
        }

        /// <summary>
        ///  Returns a string with info about the currently selected GridEntry
        /// </summary>
        public virtual string GetTestingInfo()
        {
            string str = "object = (";
            string textVal = GetPropertyTextValue();
            if (textVal is null)
            {
                textVal = "(null)";
            }
            else
            {
                // make sure we clear any embedded nulls
                textVal = textVal.Replace((char)0, ' ');
            }
            Type type = PropertyType;
            if (type is null)
            {
                type = typeof(object);
            }
            str += FullLabel;
            str += "), property = (" + PropertyLabel + "," + type.AssemblyQualifiedName + "), value = " + "[" + textVal + "], expandable = " + Expandable.ToString() + ", readOnly = " + ShouldRenderReadOnly;
            ;
            return str;
        }

        /// <summary>
        ///  Retrieves the type of the value for this GridEntry
        /// </summary>
        public virtual Type GetValueType()
        {
            return PropertyType;
        }

        /// <summary>
        ///  Returns the child GridEntries for this item.
        /// </summary>
        protected virtual GridEntry[] GetPropEntries(GridEntry peParent, object obj, Type objType)
        {
            // we don't want to create subprops for null objects.
            if (obj is null)
            {
                return null;
            }

            GridEntry[] entries = null;

            Attribute[] attributes = new Attribute[BrowsableAttributes.Count];
            BrowsableAttributes.CopyTo(attributes, 0);

            PropertyTab tab = CurrentTab;
            Debug.Assert(tab != null, "No current tab!");

            try
            {
                bool forceReadOnly = ForceReadOnly;

                if (!forceReadOnly)
                {
                    ReadOnlyAttribute readOnlyAttr = (ReadOnlyAttribute)TypeDescriptor.GetAttributes(obj)[typeof(ReadOnlyAttribute)];
                    forceReadOnly = (readOnlyAttr != null && !readOnlyAttr.IsDefaultAttribute());
                }

                // do we want to expose sub properties?
                //
                if (TypeConverter.GetPropertiesSupported(this) || AlwaysAllowExpand)
                {
                    // ask the tab if we have one.
                    //
                    PropertyDescriptorCollection props = null;
                    PropertyDescriptor defProp = null;
                    if (tab != null)
                    {
                        props = tab.GetProperties(this, obj, attributes);
                        defProp = tab.GetDefaultProperty(obj);
                    }
                    else
                    {
                        props = TypeConverter.GetProperties(this, obj, attributes);
                        defProp = TypeDescriptor.GetDefaultProperty(obj);
                    }

                    if (props is null)
                    {
                        return null;
                    }

                    if ((PropertySort & PropertySort.Alphabetical) != 0)
                    {
                        if (objType is null || !objType.IsArray)
                        {
                            props = props.Sort(GridEntry.DisplayNameComparer);
                        }

                        PropertyDescriptor[] propertyDescriptors = new PropertyDescriptor[props.Count];
                        props.CopyTo(propertyDescriptors, 0);

                        props = new PropertyDescriptorCollection(SortParenProperties(propertyDescriptors));
                    }

                    if (defProp is null && props.Count > 0)
                    {
                        defProp = props[0];
                    }

                    // if the target object is an array and nothing else has provided a set of
                    // properties to use, then expand the array.
                    //
                    if ((props is null || props.Count == 0) && objType != null && objType.IsArray && obj != null)
                    {
                        Array objArray = (Array)obj;

                        entries = new GridEntry[objArray.Length];

                        for (int i = 0; i < entries.Length; i++)
                        {
                            entries[i] = new ArrayElementGridEntry(ownerGrid, peParent, i);
                        }
                    }
                    else
                    {
                        // otherwise, create the proper GridEntries.
                        //
                        bool createInstanceSupported = TypeConverter.GetCreateInstanceSupported(this);
                        entries = new GridEntry[props.Count];
                        int index = 0;

                        // loop through all the props we got and create property descriptors.
                        //
                        foreach (PropertyDescriptor pd in props)
                        {
                            GridEntry newEntry;

                            // make sure we've got a valid property, otherwise hide it
                            //
                            bool hide = false;
                            try
                            {
                                object owner = obj;
                                if (obj is ICustomTypeDescriptor)
                                {
                                    owner = ((ICustomTypeDescriptor)obj).GetPropertyOwner(pd);
                                }
                                pd.GetValue(owner);
                            }
                            catch (Exception w)
                            {
                                if (PbrsAssertPropsSwitch.Enabled)
                                {
                                    Debug.Fail("Bad property '" + peParent.PropertyLabel + "." + pd.Name + "': " + w.ToString());
                                }
                                hide = true;
                            }

                            if (createInstanceSupported)
                            {
                                newEntry = new ImmutablePropertyDescriptorGridEntry(ownerGrid, peParent, pd, hide);
                            }
                            else
                            {
                                newEntry = new PropertyDescriptorGridEntry(ownerGrid, peParent, pd, hide);
                            }

                            if (forceReadOnly)
                            {
                                newEntry.flags |= FLAG_FORCE_READONLY;
                            }

                            // check to see if we've come across the default item.
                            //
                            if (pd.Equals(defProp))
                            {
                                DefaultChild = newEntry;
                            }

                            // add it to the array.
                            //
                            entries[index++] = newEntry;
                        }
                    }
                }
            }
            catch (Exception e)
            {
#if DEBUG
                if (PbrsAssertPropsSwitch.Enabled)
                {
                    // Checked builds are not giving us enough information here.  So, output as much stuff as
                    // we can.
                    Text.StringBuilder b = new Text.StringBuilder();
                    b.Append(string.Format(CultureInfo.CurrentCulture, "********* Debug log written on {0} ************\r\n", DateTime.Now));
                    b.Append(string.Format(CultureInfo.CurrentCulture, "Exception '{0}' reading properties for object {1}.\r\n", e.GetType().Name, obj));
                    b.Append(string.Format(CultureInfo.CurrentCulture, "Exception Text: \r\n{0}", e.ToString()));
                    b.Append(string.Format(CultureInfo.CurrentCulture, "Exception stack: \r\n{0}", e.StackTrace));
                    string path = string.Format(CultureInfo.CurrentCulture, "{0}\\PropertyGrid.log", Environment.GetEnvironmentVariable("SYSTEMDRIVE"));
                    IO.FileStream s = new IO.FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.None);
                    IO.StreamWriter w = new IO.StreamWriter(s);
                    w.Write(b.ToString());
                    w.Close();
                    s.Close();
                    RTLAwareMessageBox.Show(null, b.ToString(), string.Format(SR.PropertyGridInternalNoProp, path),
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
                }
#endif
                Debug.Fail("Failed to get properties: " + e.GetType().Name + "," + e.Message + "\n" + e.StackTrace);
            }
            return entries;
        }

        /// <summary>
        ///  Resets the current item
        /// </summary>
        public virtual void ResetPropertyValue()
        {
            NotifyValue(NOTIFY_RESET);
            Refresh();
        }

        /// <summary>
        ///  Returns if the property can be reset
        /// </summary>
        public virtual bool CanResetPropertyValue()
        {
            return NotifyValue(NOTIFY_CAN_RESET);
        }

        /// <summary>
        ///  Called when the item is double clicked.
        /// </summary>
        public virtual bool DoubleClickPropertyValue()
        {
            return NotifyValue(NOTIFY_DBL_CLICK);
        }

        /// <summary>
        ///  Returns the text value of this property.
        /// </summary>
        public virtual string GetPropertyTextValue()
        {
            return GetPropertyTextValue(PropertyValue);
        }

        /// <summary>
        ///  Returns the text value of this property.
        /// </summary>
        public virtual string GetPropertyTextValue(object value)
        {
            string str = null;

            TypeConverter converter = TypeConverter;
            try
            {
                str = converter.ConvertToString(this, value);
            }
            catch (Exception t)
            {
                Debug.Fail("Bad Type Converter! " + t.GetType().Name + ", " + t.Message + "," + converter.ToString(), t.ToString());
            }

            if (str is null)
            {
                str = string.Empty;
            }
            return str;
        }

        /// <summary>
        ///  Returns the text values of this property.
        /// </summary>
        public virtual object[] GetPropertyValueList()
        {
            ICollection values = TypeConverter.GetStandardValues(this);
            if (values != null)
            {
                object[] valueArray = new object[values.Count];
                values.CopyTo(valueArray, 0);
                return valueArray;
            }
            return Array.Empty<object>();
        }

        public override int GetHashCode() => HashCode.Combine(PropertyLabel, PropertyType, GetType());

        /// <summary>
        ///  Checks if a given flag is set
        /// </summary>
        protected virtual bool GetFlagSet(int flag)
        {
            return ((flag & Flags) != 0);
        }

        protected Font GetFont(bool boldFont)
        {
            if (boldFont)
            {
                return GridEntryHost.GetBoldFont();
            }
            else
            {
                return GridEntryHost.GetBaseFont();
            }
        }

        /// <summary>
        ///  Retrieves the requested service.  This may
        ///  return null if the requested service is not
        ///  available.
        /// </summary>
        public virtual object GetService(Type serviceType)
        {
            if (serviceType == typeof(GridItem))
            {
                return (GridItem)this;
            }

            if (parentPE != null)
            {
                return parentPE.GetService(serviceType);
            }
            return null;
        }

        internal virtual bool NonParentEquals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            if (obj is null)
            {
                return false;
            }

            if (!(obj is GridEntry))
            {
                return false;
            }

            GridEntry pe = (GridEntry)obj;

            return pe.PropertyLabel.Equals(PropertyLabel) &&
                   pe.PropertyType.Equals(PropertyType) && pe.PropertyDepth == PropertyDepth;
        }

        /// <summary>
        ///  Paints the label portion of this GridEntry into the given Graphics object. This is called by the GridEntry
        ///  host (the PropertyGridView) when this GridEntry is to be painted.
        /// </summary>
        public virtual void PaintLabel(Graphics g, Rectangle rect, Rectangle clipRect, bool selected, bool paintFullLabel)
        {
            PropertyGridView gridHost = GridEntryHost;
            Debug.Assert(gridHost != null, "No propEntryHost");
            string strLabel = PropertyLabel;

            int borderWidth = gridHost.GetOutlineIconSize() + OUTLINE_ICON_PADDING;

            // fill the background if necessary
            Color backColor = selected ? gridHost.GetSelectedItemWithFocusBackColor() : GetBackgroundColor();

            // if we don't have focus, paint with the line color
            if (selected && !hasFocus)
            {
                backColor = gridHost.GetLineColor();
            }

            bool fBold = ((Flags & FLAG_LABEL_BOLD) != 0);
            Font font = GetFont(fBold);

            int labelWidth = GetLabelTextWidth(strLabel, g, font);

            int neededWidth = paintFullLabel ? labelWidth : 0;
            int stringX = rect.X + PropertyLabelIndent;

            using var backBrush = backColor.GetCachedSolidBrushScope();
            if (paintFullLabel && (neededWidth >= (rect.Width - (stringX + 2))))
            {
                // GDIPLUS_SPACE = extra needed to ensure text draws completely and isn't clipped.
                int totalWidth = stringX + neededWidth + PropertyGridView.GDIPLUS_SPACE;

                // blank out the space we're going to use
                g.FillRectangle(backBrush, borderWidth - 1, rect.Y, totalWidth - borderWidth + 3, rect.Height);

                // draw an end line
                using var linePen = gridHost.GetLineColor().GetCachedPenScope();
                g.DrawLine(linePen, totalWidth, rect.Y, totalWidth, rect.Height);

                // set the new width that we can draw into
                rect.Width = totalWidth - rect.X;
            }
            else
            {
                // Normal case -- no pseudo-tooltip for the label
                g.FillRectangle(backBrush, rect.X, rect.Y, rect.Width, rect.Height);
            }

            // Draw the border stripe on the left
            using var stripeBrush = gridHost.GetLineColor().GetCachedSolidBrushScope();
            g.FillRectangle(stripeBrush, rect.X, rect.Y, borderWidth, rect.Height);

            if (selected && hasFocus)
            {
                using var focusBrush = gridHost.GetSelectedItemWithFocusBackColor().GetCachedSolidBrushScope();
                g.FillRectangle(
                    focusBrush,
                    stringX, rect.Y, rect.Width - stringX - 1, rect.Height);
            }

            int maxSpace = Math.Min(rect.Width - stringX - 1, labelWidth + PropertyGridView.GDIPLUS_SPACE);
            Rectangle textRect = new Rectangle(stringX, rect.Y + 1, maxSpace, rect.Height - 1);

            if (!Rectangle.Intersect(textRect, clipRect).IsEmpty)
            {
                Region oldClip = g.Clip;
                g.SetClip(textRect);

                // We need to Invert color only if in Highcontrast mode, targeting 4.7.1 and above, Gridcategory and
                // not a developer override. This is required to achieve required contrast ratio.
                var shouldInvertForHC = colorInversionNeededInHC && (fBold || (selected && !hasFocus));

                // Do actual drawing
                // A brush is needed if using GDI+ only (UseCompatibleTextRendering); if using GDI, only the color is needed.
                Color textColor = selected && hasFocus
                    ? gridHost.GetSelectedItemWithFocusForeColor()
                    : shouldInvertForHC
                        ? InvertColor(ownerGrid.LineColor)
                        : g.FindNearestColor(LabelTextColor);

                if (ownerGrid.UseCompatibleTextRendering)
                {
                    using var textBrush = textColor.GetCachedSolidBrushScope();
                    StringFormat stringFormat = new StringFormat(StringFormatFlags.NoWrap)
                    {
                        Trimming = StringTrimming.None
                    };
                    g.DrawString(strLabel, font, textBrush, textRect, stringFormat);
                }
                else
                {
                    TextRenderer.DrawText(g, strLabel, font, textRect, textColor, PropertyGrid.MeasureTextHelper.GetTextRendererFlags());
                }

                g.SetClip(oldClip, CombineMode.Replace);
                oldClip.Dispose();   // clip is actually copied out.

                if (maxSpace <= labelWidth)
                {
                    labelTipPoint = new Point(stringX + 2, rect.Y + 1);
                }
                else
                {
                    labelTipPoint = InvalidPoint;
                }
            }

            rect.Y -= 1;
            rect.Height += 2;

            PaintOutline(g, rect);
        }

        /// <summary>
        ///  Paints the outline portion of this GridEntry into the given Graphics object.  This
        ///  is called by the GridEntry host (the PropertyGridView) when this GridEntry is
        ///  to be painted.
        /// </summary>
        public virtual void PaintOutline(Graphics g, Rectangle r)
        {
            // draw tree-view glyphs as triangles on Vista and Windows afterword
            // when Vistual style is enabled
            if (GridEntryHost.IsExplorerTreeSupported)
            {
                // size of Explorer Tree style glyph (triangle) is different from +/- glyph,
                // so when we change the visual style (such as changing Windows theme),
                // we need to recaculate outlineRect
                if (!lastPaintWithExplorerStyle)
                {
                    outlineRect = Rectangle.Empty;
                    lastPaintWithExplorerStyle = true;
                }
                PaintOutlineWithExplorerTreeStyle(g, r, (GridEntryHost != null) ? GridEntryHost.HandleInternal : IntPtr.Zero);
            }
            // draw tree-view glyphs as +/-
            else
            {
                // size of Explorer Tree style glyph (triangle) is different from +/- glyph,
                // so when we change the visual style (such as changing Windows theme),
                // we need to recaculate outlineRect
                if (lastPaintWithExplorerStyle)
                {
                    outlineRect = Rectangle.Empty;
                    lastPaintWithExplorerStyle = false;
                }

                PaintOutlineWithClassicStyle(g, r);
            }
        }

        private void PaintOutlineWithExplorerTreeStyle(Graphics g, Rectangle r, IntPtr handle)
        {
            if (Expandable)
            {
                bool fExpanded = InternalExpanded;
                Rectangle outline = OutlineRect;

                // make sure we're in our bounds
                outline = Rectangle.Intersect(r, outline);
                if (outline.IsEmpty)
                {
                    return;
                }

                VisualStyleElement element = fExpanded
                    ? VisualStyleElement.ExplorerTreeView.Glyph.Opened
                    : VisualStyleElement.ExplorerTreeView.Glyph.Closed;

                // Invert color if it is not overriden by developer.
                if (colorInversionNeededInHC)
                {
                    Color textColor = InvertColor(ownerGrid.LineColor);
                    if (g != null)
                    {
                        using var brush = textColor.GetCachedSolidBrushScope();
                        g.FillRectangle(brush, outline);
                    }
                }

                VisualStyleRenderer explorerTreeRenderer = new VisualStyleRenderer(element);

                using var hdc = new DeviceContextHdcScope(g);
                explorerTreeRenderer.DrawBackground(hdc, outline, handle);
            }
        }

        private void PaintOutlineWithClassicStyle(Graphics g, Rectangle r)
        {
            // Draw outline box.
            if (Expandable)
            {
                bool fExpanded = InternalExpanded;
                Rectangle outline = OutlineRect;

                // make sure we're in our bounds
                outline = Rectangle.Intersect(r, outline);
                if (outline.IsEmpty)
                {
                    return;
                }

                // Draw border area box
                Color penColor = GridEntryHost.GetTextColor();

                // inverting text color to back ground to get required contrast ratio
                if (colorInversionNeededInHC)
                {
                    penColor = InvertColor(ownerGrid.LineColor);
                }
                else
                {
                    // Filling rectangle as it was in all cases where we do not invert colors.
                    Color brushColor = GetBackgroundColor();
                    using var brush = brushColor.GetCachedSolidBrushScope();
                    g.FillRectangle(brush, outline);
                }

                using var pen = penColor.GetCachedPenScope();

                g.DrawRectangle(pen, outline.X, outline.Y, outline.Width - 1, outline.Height - 1);

                // draw horizontal line for +/-
                int indent = 2;
                g.DrawLine(
                    pen,
                    outline.X + indent,
                    outline.Y + outline.Height / 2,
                    outline.X + outline.Width - indent - 1,
                    outline.Y + outline.Height / 2);

                // draw vertical line to make a +
                if (!fExpanded)
                {
                    g.DrawLine(
                        pen,
                        outline.X + outline.Width / 2,
                        outline.Y + indent,
                        outline.X + outline.Width / 2,
                        outline.Y + outline.Height - indent - 1);
                }
            }
        }

        /// <summary>
        ///  Paints the value portion of this GridEntry into the given Graphics object. This is called by the GridEntry
        ///  host (the PropertyGridView) when this GridEntry is to be painted.
        /// </summary>
        public virtual void PaintValue(object val, Graphics g, Rectangle rect, Rectangle clipRect, PaintValueFlags paintFlags)
        {
            PropertyGridView gridHost = GridEntryHost;
            Debug.Assert(gridHost != null);

            Color textColor = ShouldRenderReadOnly ? GridEntryHost.GrayTextColor : gridHost.GetTextColor();

            string text;

            if (paintFlags.HasFlag(PaintValueFlags.FetchValue))
            {
                if (cacheItems != null && cacheItems.useValueString)
                {
                    text = cacheItems.lastValueString;
                    val = cacheItems.lastValue;
                }
                else
                {
                    val = PropertyValue;
                    text = GetPropertyTextValue(val);

                    if (cacheItems is null)
                    {
                        cacheItems = new CacheItems();
                    }

                    cacheItems.lastValueString = text;
                    cacheItems.useValueString = true;
                    cacheItems.lastValueTextWidth = -1;
                    cacheItems.lastValueFont = null;
                    cacheItems.lastValue = val;
                }
            }
            else
            {
                text = GetPropertyTextValue(val);
            }

            // Paint out the main rect using the appropriate brush
            Color backColor = GetBackgroundColor();

            if (paintFlags.HasFlag(PaintValueFlags.DrawSelected))
            {
                backColor = gridHost.GetSelectedItemWithFocusBackColor();
                textColor = gridHost.GetSelectedItemWithFocusForeColor();
            }

            using var backBrush = backColor.GetCachedSolidBrushScope();
            g.FillRectangle(backBrush, clipRect);

            int paintIndent = 0;
            if (IsCustomPaint)
            {
                paintIndent = gridHost.GetValuePaintIndent();
                Rectangle rectPaint = new Rectangle(
                    rect.X + 1,
                    rect.Y + 1,
                    gridHost.GetValuePaintWidth(),
                    gridHost.GetGridEntryHeight() - 2);

                if (!Rectangle.Intersect(rectPaint, clipRect).IsEmpty)
                {
                    UITypeEditor?.PaintValue(new PaintValueEventArgs(this, val, g, rectPaint));

                    // Paint a border around the area
                    rectPaint.Width--;
                    rectPaint.Height--;
                    g.DrawRectangle(SystemPens.WindowText, rectPaint);
                }
            }

            rect.X += paintIndent + gridHost.GetValueStringIndent();
            rect.Width -= paintIndent + 2 * gridHost.GetValueStringIndent();

            // Bold the property if we need to persist it (e.g. it's not the default value)
            bool valueModified = paintFlags.HasFlag(PaintValueFlags.CheckShouldSerialize) && ShouldSerializePropertyValue();

            // If we have text to paint, paint it
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (text.Length > maximumLengthOfPropertyString)
            {
                text = text.Substring(0, maximumLengthOfPropertyString);
            }

            int textWidth = GetValueTextWidth(text, g, GetFont(valueModified));
            bool doToolTip = false;

            // Check if text contains multiple lines
            if (textWidth >= rect.Width || GetMultipleLines(text))
            {
                doToolTip = true;
            }

            if (Rectangle.Intersect(rect, clipRect).IsEmpty)
            {
                return;
            }

            // Do actual drawing, shifting to match the PropertyGridView.GridViewListbox content alignment

            if (paintFlags.HasFlag(PaintValueFlags.PaintInPlace))
            {
                rect.Offset(1, 2);
            }
            else
            {
                // Only go down one pixel when we're painting in the listbox
                rect.Offset(1, 1);
            }

            Rectangle textRectangle = new Rectangle(
                rect.X - 1,
                rect.Y - 1,
                rect.Width - 4,
                rect.Height);

            backColor = paintFlags.HasFlag(PaintValueFlags.DrawSelected)
                ? GridEntryHost.GetSelectedItemWithFocusBackColor()
                : GridEntryHost.BackColor;

            User32.DT format = User32.DT.EDITCONTROL | User32.DT.EXPANDTABS | User32.DT.NOCLIP
                | User32.DT.SINGLELINE | User32.DT.NOPREFIX;

            if (gridHost.DrawValuesRightToLeft)
            {
                format |= User32.DT.RIGHT | User32.DT.RTLREADING;
            }

            // For password mode, replace the string value with a bullet.
            if (ShouldRenderPassword)
            {
                if (passwordReplaceChar == '\0')
                {
                    // Bullet is 2022, but edit box uses round circle 25CF
                    passwordReplaceChar = '\u25CF';
                }

                text = new string(passwordReplaceChar, text.Length);
            }

            TextRenderer.DrawTextInternal(
                g,
                text,
                GetFont(boldFont: valueModified),
                textRectangle,
                textColor,
                backColor,
                (TextFormatFlags)format);

            ValueToolTipLocation = doToolTip ? new Point(rect.X + 2, rect.Y - 1) : InvalidPoint;
        }

        public virtual bool OnComponentChanging()
        {
            if (ComponentChangeService != null)
            {
                try
                {
                    ComponentChangeService.OnComponentChanging(GetValueOwner(), PropertyDescriptor);
                }
                catch (CheckoutException coEx)
                {
                    if (coEx == CheckoutException.Canceled)
                    {
                        return false;
                    }
                    throw;
                }
            }
            return true;
        }

        public virtual void OnComponentChanged()
        {
            if (ComponentChangeService != null)
            {
                ComponentChangeService.OnComponentChanged(GetValueOwner(), PropertyDescriptor, null, null);
            }
        }

        /// <summary>
        ///  Called when the label portion of this GridEntry is clicked.
        ///  Default implmentation fired the event to any listeners, so be sure
        ///  to call base.OnLabelClick(e) if this is overrideen.
        /// </summary>
        protected virtual void OnLabelClick(EventArgs e)
        {
            RaiseEvent(EVENT_LABEL_CLICK, e);
        }

        /// <summary>
        ///  Called when the label portion of this GridEntry is double-clicked.
        ///  Default implmentation fired the event to any listeners, so be sure
        ///  to call base.OnLabelDoubleClick(e) if this is overrideen.
        /// </summary>
        protected virtual void OnLabelDoubleClick(EventArgs e)
        {
            RaiseEvent(EVENT_LABEL_DBLCLICK, e);
        }

        /// <summary>
        ///  Called when the GridEntry is clicked.
        /// </summary>
        public virtual bool OnMouseClick(int x, int y, int count, MouseButtons button)
        {
            // where are we at?
            PropertyGridView gridHost = GridEntryHost;
            Debug.Assert(gridHost != null, "No prop entry host!");

            // make sure it's the left button
            if ((button & MouseButtons.Left) != MouseButtons.Left)
            {
                return false;
            }

            int labelWidth = gridHost.GetLabelWidth();

            // are we in the label?
            if (x >= 0 && x <= labelWidth)
            {
                // are we on the outline?
                if (Expandable)
                {
                    Rectangle outlineRect = OutlineRect;
                    if (outlineRect.Contains(x, y))
                    {
                        if (count % 2 == 0)
                        {
                            OnOutlineDoubleClick(EventArgs.Empty);
                        }
                        else
                        {
                            OnOutlineClick(EventArgs.Empty);
                        }
                        return true;
                    }
                }

                if (count % 2 == 0)
                {
                    OnLabelDoubleClick(EventArgs.Empty);
                }
                else
                {
                    OnLabelClick(EventArgs.Empty);
                }
                return true;
            }

            // are we in the value?
            labelWidth += gridHost.GetSplitterWidth();
            if (x >= labelWidth && x <= labelWidth + gridHost.GetValueWidth())
            {
                if (count % 2 == 0)
                {
                    OnValueDoubleClick(EventArgs.Empty);
                }
                else
                {
                    OnValueClick(EventArgs.Empty);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        ///  Called when the outline icon portion of this GridEntry is clicked.
        ///  Default implmentation fired the event to any listeners, so be sure
        ///  to call base.OnOutlineClick(e) if this is overrideen.
        /// </summary>
        protected virtual void OnOutlineClick(EventArgs e)
        {
            RaiseEvent(EVENT_OUTLINE_CLICK, e);
        }

        /// <summary>
        ///  Called when the outline icon portion of this GridEntry is double-clicked.
        ///  Default implmentation fired the event to any listeners, so be sure
        ///  to call base.OnOutlineDoubleClick(e) if this is overrideen.
        /// </summary>
        protected virtual void OnOutlineDoubleClick(EventArgs e)
        {
            RaiseEvent(EVENT_OUTLINE_DBLCLICK, e);
        }

        /// <summary>
        ///  Called when RecreateChildren is called.
        ///  Default implmentation fired the event to any listeners, so be sure
        ///  to call base.OnOutlineDoubleClick(e) if this is overrideen.
        /// </summary>
        protected virtual void OnRecreateChildren(GridEntryRecreateChildrenEventArgs e)
        {
            Delegate handler = GetEventHandler(EVENT_RECREATE_CHILDREN);
            if (handler != null)
            {
                ((GridEntryRecreateChildrenEventHandler)handler)(this, e);
            }
        }

        /// <summary>
        ///  Called when the value portion of this GridEntry is clicked.
        ///  Default implmentation fired the event to any listeners, so be sure
        ///  to call base.OnValueClick(e) if this is overrideen.
        /// </summary>
        protected virtual void OnValueClick(EventArgs e)
        {
            RaiseEvent(EVENT_VALUE_CLICK, e);
        }

        /// <summary>
        ///  Called when the value portion of this GridEntry is clicked.
        ///  Default implmentation fired the event to any listeners, so be sure
        ///  to call base.OnValueDoubleClick(e) if this is overrideen.
        /// </summary>
        protected virtual void OnValueDoubleClick(EventArgs e)
        {
            RaiseEvent(EVENT_VALUE_DBLCLICK, e);
        }

        internal bool OnValueReturnKey()
        {
            return NotifyValue(NOTIFY_RETURN);
        }

        /// <summary>
        ///  Sets the specified flag
        /// </summary>
        protected virtual void SetFlag(int flag, bool fVal)
        {
            SetFlag(flag, (fVal ? flag : 0));
        }

        /// <summary>
        ///  Sets the default child of this entry, given a valid value mask.
        /// </summary>
        protected virtual void SetFlag(int flag_valid, int flag, bool fVal)
        {
            SetFlag(flag_valid | flag,
                    flag_valid | (fVal ? flag : 0));
        }

        /// <summary>
        ///  Sets the value of a flag
        /// </summary>
        protected virtual void SetFlag(int flag, int val)
        {
            Flags = (Flags & ~(flag)) | val;
        }

        public override bool Select()
        {
            if (Disposed)
            {
                return false;
            }

            try
            {
                GridEntryHost.SelectedGridEntry = this;
                return true;
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        ///  Checks if this value should be persisited.
        /// </summary>
        internal virtual bool ShouldSerializePropertyValue()
        {
            if (cacheItems != null)
            {
                if (cacheItems.useShouldSerialize)
                {
                    return cacheItems.lastShouldSerialize;
                }
                else
                {
                    cacheItems.lastShouldSerialize = NotifyValue(NOTIFY_SHOULD_PERSIST);
                    cacheItems.useShouldSerialize = true;
                }
            }
            else
            {
                cacheItems = new CacheItems
                {
                    lastShouldSerialize = NotifyValue(NOTIFY_SHOULD_PERSIST),
                    useShouldSerialize = true
                };
            }
            return cacheItems.lastShouldSerialize;
        }

        private PropertyDescriptor[] SortParenProperties(PropertyDescriptor[] props)
        {
            PropertyDescriptor[] newProps = null;
            int newPos = 0;

            // first scan the list and move any parentesized properties to the front.
            for (int i = 0; i < props.Length; i++)
            {
                if (((ParenthesizePropertyNameAttribute)props[i].Attributes[typeof(ParenthesizePropertyNameAttribute)]).NeedParenthesis)
                {
                    if (newProps is null)
                    {
                        newProps = new PropertyDescriptor[props.Length];
                    }
                    newProps[newPos++] = props[i];
                    props[i] = null;
                }
            }

            // second pass, copy any that didn't have the parens.
            if (newPos > 0)
            {
                for (int i = 0; i < props.Length; i++)
                {
                    if (props[i] != null)
                    {
                        newProps[newPos++] = props[i];
                    }
                }
                props = newProps;
            }
            return props;
        }

        /// <summary>
        ///  Sends a notify message to this GridEntry, and returns the success result
        /// </summary>
        internal virtual bool NotifyValueGivenParent(object obj, int type)
        {
            return false;
        }

        /// <summary>
        ///  Sends a notify message to the child GridEntry, and returns the success result
        /// </summary>
        internal virtual bool NotifyChildValue(GridEntry pe, int type)
        {
            return pe.NotifyValueGivenParent(pe.GetValueOwner(), type);
        }

        internal virtual bool NotifyValue(int type)
        {
            if (parentPE is null)
            {
                return true;
            }
            else
            {
                return parentPE.NotifyChildValue(this, type);
            }
        }

        internal void RecreateChildren()
        {
            RecreateChildren(-1);
        }

        internal void RecreateChildren(int oldCount)
        {
            // cause the flags to be rebuilt as well...
            bool wasExpanded = InternalExpanded || oldCount > 0;

            if (oldCount == -1)
            {
                oldCount = VisibleChildCount;
            }

            ResetState();
            if (oldCount == 0)
            {
                return;
            }

            foreach (GridEntry child in ChildCollection)
            {
                child.RecreateChildren();
            }

            DisposeChildren();
            InternalExpanded = wasExpanded;
            OnRecreateChildren(new GridEntryRecreateChildrenEventArgs(oldCount, VisibleChildCount));
        }

        /// <summary>
        ///  Refresh the current GridEntry's value and it's children
        /// </summary>
        public virtual void Refresh()
        {
            Type type = PropertyType;
            if (type != null && type.IsArray)
            {
                CreateChildren(true);
            }

            if (childCollection != null)
            {
                // check to see if the value has changed.
                //
                if (InternalExpanded && cacheItems != null && cacheItems.lastValue != null && cacheItems.lastValue != PropertyValue)
                {
                    ClearCachedValues();
                    RecreateChildren();
                    return;
                }
                else if (InternalExpanded)
                {
                    // otherwise just do a refresh.
                    IEnumerator childEnum = childCollection.GetEnumerator();
                    while (childEnum.MoveNext())
                    {
                        object o = childEnum.Current;
                        Debug.Assert(o != null, "Collection contains a null element.  But how? Garbage collector hole?  GDI+ corrupting memory?");
                        GridEntry e = (GridEntry)o;
                        e.Refresh();
                    }
                }
                else
                {
                    DisposeChildren();
                }
            }

            ClearCachedValues();
        }

        public virtual void RemoveOnLabelClick(EventHandler h)
        {
            RemoveEventHandler(EVENT_LABEL_CLICK, h);
        }
        public virtual void RemoveOnLabelDoubleClick(EventHandler h)
        {
            RemoveEventHandler(EVENT_LABEL_DBLCLICK, h);
        }

        public virtual void RemoveOnValueClick(EventHandler h)
        {
            RemoveEventHandler(EVENT_VALUE_CLICK, h);
        }

        public virtual void RemoveOnValueDoubleClick(EventHandler h)
        {
            RemoveEventHandler(EVENT_VALUE_DBLCLICK, h);
        }

        public virtual void RemoveOnOutlineClick(EventHandler h)
        {
            RemoveEventHandler(EVENT_OUTLINE_CLICK, h);
        }

        public virtual void RemoveOnOutlineDoubleClick(EventHandler h)
        {
            RemoveEventHandler(EVENT_OUTLINE_DBLCLICK, h);
        }

        public virtual void RemoveOnRecreateChildren(GridEntryRecreateChildrenEventHandler h)
        {
            RemoveEventHandler(EVENT_RECREATE_CHILDREN, h);
        }

        protected void ResetState()
        {
            Flags = 0;
            ClearCachedValues();
        }

        /// <summary>
        ///  Sets the value of this GridEntry from text
        /// </summary>
        public virtual bool SetPropertyTextValue(string str)
        {
            bool fChildrenPrior = (childCollection != null && childCollection.Count > 0);
            PropertyValue = ConvertTextToValue(str);
            CreateChildren();
            bool fChildrenAfter = (childCollection != null && childCollection.Count > 0);
            return (fChildrenPrior != fChildrenAfter);
        }

        public override string ToString()
        {
            return GetType().FullName + " " + PropertyLabel;
        }

        private EventEntry eventList;

        protected virtual void AddEventHandler(object key, Delegate handler)
        {
            // Locking 'this' here is ok since this is an internal class.
            lock (this)
            {
                if (handler is null)
                {
                    return;
                }

                for (EventEntry e = eventList; e != null; e = e.next)
                {
                    if (e.key == key)
                    {
                        e.handler = Delegate.Combine(e.handler, handler);
                        return;
                    }
                }
                eventList = new EventEntry(eventList, key, handler);
            }
        }

        protected virtual void RaiseEvent(object key, EventArgs e)
        {
            Delegate handler = GetEventHandler(key);
            if (handler != null)
            {
                ((EventHandler)handler)(this, e);
            }
        }

        protected virtual Delegate GetEventHandler(object key)
        {
            // Locking 'this' here is ok since this is an internal class.
            lock (this)
            {
                for (EventEntry e = eventList; e != null; e = e.next)
                {
                    if (e.key == key)
                    {
                        return e.handler;
                    }
                }
                return null;
            }
        }

        protected virtual void RemoveEventHandler(object key, Delegate handler)
        {
            // Locking this here is ok since this is an internal class.
            lock (this)
            {
                if (handler is null)
                {
                    return;
                }

                for (EventEntry e = eventList, prev = null; e != null; prev = e, e = e.next)
                {
                    if (e.key == key)
                    {
                        e.handler = Delegate.Remove(e.handler, handler);
                        if (e.handler is null)
                        {
                            if (prev is null)
                            {
                                eventList = e.next;
                            }
                            else
                            {
                                prev.next = e.next;
                            }
                        }
                        return;
                    }
                }
            }
        }

        protected virtual void RemoveEventHandlers()
        {
            eventList = null;
        }

        private sealed class EventEntry
        {
            internal EventEntry next;
            internal object key;
            internal Delegate handler;

            internal EventEntry(EventEntry next, object key, Delegate handler)
            {
                this.next = next;
                this.key = key;
                this.handler = handler;
            }
        }

        public class GridEntryAccessibleObject : AccessibleObject
        {
            protected GridEntry owner;
            private delegate void SelectDelegate(AccessibleSelection flags);
            private int[] runtimeId; // Used by UIAutomation

            public GridEntryAccessibleObject(GridEntry owner) : base()
            {
                Debug.Assert(owner != null, "GridEntryAccessibleObject must have a valid owner GridEntry");
                this.owner = owner;
            }

            public override Rectangle Bounds
            {
                get => PropertyGridView != null && PropertyGridView.IsHandleCreated
                    ? PropertyGridView.AccessibilityGetGridEntryBounds(owner)
                    : Rectangle.Empty;
            }

            public override string DefaultAction
            {
                get
                {
                    if (!owner.Expandable)
                    {
                        return base.DefaultAction;
                    }
                    else if (owner.Expanded)
                    {
                        return SR.AccessibleActionCollapse;
                    }
                    else
                    {
                        return SR.AccessibleActionExpand;
                    }
                }
            }

            public override string Description
            {
                get
                {
                    return owner.PropertyDescription;
                }
            }

            public override string Help
            {
                get
                {
                    return owner.PropertyDescription;
                }
            }

            /// <summary>
            ///  Request to return the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        GridEntry parentGridEntry = owner.ParentGridEntry;
                        if (parentGridEntry != null)
                        {
                            if (parentGridEntry is SingleSelectRootGridEntry)
                            {
                                return owner.OwnerGrid.GridViewAccessibleObject;
                            }
                            else
                            {
                                return parentGridEntry.AccessibilityObject;
                            }
                        }

                        return Parent;
                    case UiaCore.NavigateDirection.PreviousSibling:
                        return Navigate(AccessibleNavigation.Previous);
                    case UiaCore.NavigateDirection.NextSibling:
                        return Navigate(AccessibleNavigation.Next);
                }

                return base.FragmentNavigate(direction);
            }

            /// <summary>
            ///  Return the element that is the root node of this fragment of UI.
            /// </summary>
            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return (PropertyGridView.PropertyGridViewAccessibleObject)Parent;
                }
            }

            #region IAccessibleEx - patterns and properties

            internal override bool IsIAccessibleExSupported()
            {
                if (owner.Expandable)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            internal override int[] RuntimeId
            {
                get
                {
                    if (owner.GridEntryHost is null || !owner.GridEntryHost.IsHandleCreated)
                    {
                        return base.RuntimeId;
                    }

                    if (runtimeId is null)
                    {
                        // we need to provide a unique ID
                        // others are implementing this in the same manner
                        // first item is static - 0x2a
                        // second item can be anything, but it's good to supply HWND
                        // third and others are optional, but in case of GridItem we need it, to make it unique
                        // grid items are not controls, they don't have hwnd - we use hwnd of PropertyGridView

                        runtimeId = new int[3];
                        runtimeId[0] = 0x2a;
                        runtimeId[1] = (int)(long)owner.GridEntryHost.InternalHandle;
                        runtimeId[2] = GetHashCode();
                    }

                    return runtimeId;
                }
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.ControlTypePropertyId:

                        // The accessible hierarchy is changed so we cannot use Button type
                        // for the grid items to not break automation logic that searches for the first
                        // button in the PropertyGridView to show dialog/drop-down. In Level < 3 action
                        // button is one of the first children of PropertyGridView.
                        return UiaCore.UIA.TreeItemControlTypeId;
                    case UiaCore.UIA.IsExpandCollapsePatternAvailablePropertyId:
                        return (Object)IsPatternSupported(UiaCore.UIA.ExpandCollapsePatternId);
                    case UiaCore.UIA.AccessKeyPropertyId:
                        return string.Empty;
                    case UiaCore.UIA.HasKeyboardFocusPropertyId:
                        return owner.hasFocus;
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                    case UiaCore.UIA.IsEnabledPropertyId:
                        return true;
                    case UiaCore.UIA.AutomationIdPropertyId:
                        return GetHashCode().ToString();
                    case UiaCore.UIA.HelpTextPropertyId:
                        return Help ?? string.Empty;
                    case UiaCore.UIA.IsPasswordPropertyId:
                        return false;
                    case UiaCore.UIA.IsOffscreenPropertyId:
                        return (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
                    case UiaCore.UIA.IsGridItemPatternAvailablePropertyId:
                    case UiaCore.UIA.IsTableItemPatternAvailablePropertyId:
                        return true;
                    case UiaCore.UIA.LegacyIAccessibleRolePropertyId:
                        return Role;
                    case UiaCore.UIA.LegacyIAccessibleDefaultActionPropertyId:
                        return DefaultAction;

                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                switch (patternId)
                {
                    case UiaCore.UIA.InvokePatternId:
                    case UiaCore.UIA.LegacyIAccessiblePatternId:
                        return true;

                    case UiaCore.UIA.ExpandCollapsePatternId:
                        if (owner != null && owner.Expandable)
                        {
                            return true;
                        }

                        break;

                    case UiaCore.UIA.GridItemPatternId:
                    case UiaCore.UIA.TableItemPatternId:
                        if (owner is null || owner.OwnerGrid is null || owner.OwnerGrid.SortedByCategories)
                        {
                            break;
                        }

                        // Only top level rows are grid items.
                        // Sub-items (for instance height in size is not a grid item)
                        GridEntry parentGridEntry = owner.ParentGridEntry;
                        if (parentGridEntry != null && parentGridEntry is SingleSelectRootGridEntry)
                        {
                            return true;
                        }

                        break;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override void Expand()
            {
                if (owner.Expandable && owner.Expanded == false)
                {
                    owner.Expanded = true;
                }
            }

            internal override void Collapse()
            {
                if (owner.Expandable && owner.Expanded == true)
                {
                    owner.Expanded = false;
                }
            }

            internal override UiaCore.ExpandCollapseState ExpandCollapseState
            {
                get
                {
                    if (owner.Expandable)
                    {
                        return owner.Expanded ? UiaCore.ExpandCollapseState.Expanded : UiaCore.ExpandCollapseState.Collapsed;
                    }
                    else
                    {
                        return UiaCore.ExpandCollapseState.LeafNode;
                    }
                }
            }

            #endregion

            public override void DoDefaultAction()
            {
                if (PropertyGridView.IsHandleCreated)
                {
                    owner.OnOutlineClick(EventArgs.Empty);
                }
            }

            public override string Name
            {
                get
                {
                    return owner?.PropertyLabel;
                }
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return owner?.GridEntryHost?.AccessibilityObject;
                }
            }

            private PropertyGridView PropertyGridView
            {
                get
                {
                    var propertyGridViewAccessibleObject = Parent as PropertyGridView.PropertyGridViewAccessibleObject;
                    if (propertyGridViewAccessibleObject != null)
                    {
                        return propertyGridViewAccessibleObject.Owner as PropertyGridView;
                    }

                    return null;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.Cell;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    if (!PropertyGridView.IsHandleCreated)
                    {
                        return AccessibleStates.None;
                    }

                    AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable;

                    // Determine focus
                    //
                    if (owner.Focus)
                    {
                        state |= AccessibleStates.Focused;
                    }

                    // Determine selected
                    //
                    Debug.Assert(Parent != null, "GridEntry AO does not have a parent AO");
                    PropertyGridView.PropertyGridViewAccessibleObject parent = (PropertyGridView.PropertyGridViewAccessibleObject)Parent;
                    if (parent.GetSelected() == this)
                    {
                        state |= AccessibleStates.Selected;
                    }

                    // Determine expanded/collapsed state
                    //
                    if (owner.Expandable)
                    {
                        if (owner.Expanded)
                        {
                            state |= AccessibleStates.Expanded;
                        }
                        else
                        {
                            state |= AccessibleStates.Collapsed;
                        }
                    }

                    // Determine readonly/editable state
                    //
                    if (owner.ShouldRenderReadOnly)
                    {
                        state |= AccessibleStates.ReadOnly;
                    }

                    // Determine password state
                    //
                    if (owner.ShouldRenderPassword)
                    {
                        state |= AccessibleStates.Protected;
                    }

                    Rectangle entryBounds = this.BoundingRectangle;
                    Rectangle propertyGridViewBounds = this.PropertyGridView.GetToolNativeScreenRectangle();

                    if (!entryBounds.IntersectsWith(propertyGridViewBounds))
                    {
                        state |= AccessibleStates.Offscreen;
                    }

                    return state;
                }
            }

            public override string Value
            {
                get
                {
                    return owner.GetPropertyTextValue();
                }

                set
                {
                    owner.SetPropertyTextValue(value);
                }
            }

            /// <summary>
            ///  Returns the currently focused child, if any.
            ///  Returns this if the object itself is focused.
            /// </summary>
            public override AccessibleObject GetFocused()
            {
                if (owner.Focus)
                {
                    return this;
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            ///  Navigate to the next or previous grid entry.
            /// </summary>
            public override AccessibleObject Navigate(AccessibleNavigation navdir)
            {
                PropertyGridView.PropertyGridViewAccessibleObject parent =
                (PropertyGridView.PropertyGridViewAccessibleObject)Parent;

                switch (navdir)
                {
                    case AccessibleNavigation.Down:
                    case AccessibleNavigation.Right:
                    case AccessibleNavigation.Next:
                        return parent.Next(owner);

                    case AccessibleNavigation.Up:
                    case AccessibleNavigation.Left:
                    case AccessibleNavigation.Previous:
                        return parent.Previous(owner);

                    case AccessibleNavigation.FirstChild:
                    case AccessibleNavigation.LastChild:
                        // Fall through and return null,
                        // as this object has no children.
                        break;
                }

                return null;
            }

            public override void Select(AccessibleSelection flags)
            {
                if (!PropertyGridView.IsHandleCreated)
                {
                    return;
                }

                // make sure we're on the right thread.
                //
                if (PropertyGridView.InvokeRequired)
                {
                    PropertyGridView.Invoke(new SelectDelegate(Select), new object[] { flags });
                    return;
                }

                // Focus the PropertyGridView window
                //
                if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
                {
                    bool focused = PropertyGridView.Focus();
                }

                // Select the grid entry
                //
                if ((flags & AccessibleSelection.TakeSelection) == AccessibleSelection.TakeSelection)
                {
                    PropertyGridView.AccessibilitySelect(owner);
                }
            }

            internal override void SetFocus()
            {
                if (!PropertyGridView.IsHandleCreated)
                {
                    return;
                }

                base.SetFocus();

                RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
            }

            internal override int Row
            {
                get
                {
                    var parent = Parent as PropertyGridView.PropertyGridViewAccessibleObject;
                    if (parent is null)
                    {
                        return -1;
                    }

                    var gridView = parent.Owner as PropertyGridView;
                    if (gridView is null)
                    {
                        return -1;
                    }

                    var topLevelGridEntries = gridView.TopLevelGridEntries;
                    if (topLevelGridEntries is null)
                    {
                        return -1;
                    }

                    for (int i = 0; i < topLevelGridEntries.Count; i++)
                    {
                        var topLevelGridEntry = topLevelGridEntries[i];
                        if (owner == topLevelGridEntry)
                        {
                            return i;
                        }
                    }

                    return -1;
                }
            }

            internal override int Column => 0;

            internal override UiaCore.IRawElementProviderSimple ContainingGrid
            {
                get => PropertyGridView.AccessibilityObject;
            }
        }

        public class DisplayNameSortComparer : IComparer
        {
            public int Compare(object left, object right)
            {
                // review: (Microsoft) Is CurrentCulture correct here?  This was already reviewed as invariant...
                return string.Compare(((PropertyDescriptor)left).DisplayName, ((PropertyDescriptor)right).DisplayName, true, CultureInfo.CurrentCulture);
            }
        }
    }

    internal class AttributeTypeSorter : IComparer
    {
        private static IDictionary typeIds;

        private static string GetTypeIdString(Attribute a)
        {
            string result;
            object typeId = a.TypeId;

            if (typeId is null)
            {
                Debug.Fail("Attribute '" + a.GetType().FullName + "' does not have a typeid.");
                return "";
            }

            if (typeIds is null)
            {
                typeIds = new Hashtable();
                result = null;
            }
            else
            {
                result = typeIds[typeId] as string;
            }

            if (result is null)
            {
                result = typeId.ToString();
                typeIds[typeId] = result;
            }
            return result;
        }

        public int Compare(object obj1, object obj2)
        {
            Attribute a1 = obj1 as Attribute;
            Attribute a2 = obj2 as Attribute;

            if (a1 is null && a2 is null)
            {
                return 0;
            }
            else if (a1 is null)
            {
                return -1;
            }
            else if (a2 is null)
            {
                return 1;
            }
            return string.Compare(AttributeTypeSorter.GetTypeIdString(a1), AttributeTypeSorter.GetTypeIdString(a2), false, CultureInfo.InvariantCulture);
        }
    }

    internal delegate void GridEntryRecreateChildrenEventHandler(object sender, GridEntryRecreateChildrenEventArgs rce);

    internal class GridEntryRecreateChildrenEventArgs : EventArgs
    {
        public readonly int OldChildCount;
        public readonly int NewChildCount;

        public GridEntryRecreateChildrenEventArgs(int oldCount, int newCount)
        {
            OldChildCount = oldCount;
            NewChildCount = newCount;
        }
    }
}
