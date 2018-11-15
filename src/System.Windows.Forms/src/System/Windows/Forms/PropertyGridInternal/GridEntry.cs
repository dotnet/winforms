// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//#define PBRS_PAINT_DEBUG

namespace System.Windows.Forms.PropertyGridInternal {
    using System.Security.Permissions;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Globalization;

    using System.Drawing.Design;    
    using System.ComponentModel.Design;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;
    using System.Windows.Forms.Internal;
    using System.Windows.Forms.VisualStyles;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using Microsoft.Win32;

    /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry"]/*' />
    /// <devdoc>
    ///     Base Entry for properties to be displayed in properties window.
    /// </devdoc>
    internal abstract class GridEntry : GridItem, ITypeDescriptorContext {

        protected static readonly Point InvalidPoint = new Point(int.MinValue, int.MinValue);
        private static BooleanSwitch PbrsAssertPropsSwitch = new BooleanSwitch("PbrsAssertProps", "PropertyBrowser : Assert on broken properties");

        internal static AttributeTypeSorter AttributeTypeSorter = new AttributeTypeSorter();
        
        // Type flags
        internal const int FLAG_TEXT_EDITABLE             = 0x0001;
        internal const int FLAG_ENUMERABLE                = 0x0002;
        internal const int FLAG_CUSTOM_PAINT              = 0x0004;
        internal const int FLAG_IMMEDIATELY_EDITABLE      = 0x0008;
        internal const int FLAG_CUSTOM_EDITABLE           = 0x0010;
        internal const int FLAG_DROPDOWN_EDITABLE         = 0x0020;
        internal const int FLAG_LABEL_BOLD                = 0x0040;
        internal const int FLAG_READONLY_EDITABLE         = 0x0080;
        internal const int FLAG_RENDER_READONLY           = 0x0100;
        internal const int FLAG_IMMUTABLE                 = 0x0200;
        internal const int FLAG_FORCE_READONLY            = 0x0400;
        internal const int FLAG_RENDER_PASSWORD           = 0x1000;

        internal const int FLAG_DISPOSED                  = 0x2000;

        internal const int FL_EXPAND                   = 0x00010000;
        internal const int FL_EXPANDABLE               = 0x00020000;
        //protected const int FL_EXPANDABLE_VALID         = 0x00040000;
        internal const int FL_EXPANDABLE_FAILED        = 0x00080000;
        internal const int FL_NO_CUSTOM_PAINT          = 0x00100000;
        internal const int FL_CATEGORIES               = 0x00200000;
        internal const int FL_CHECKED                  = unchecked((int)0x80000000);

        // rest are GridEntry constants.

        protected const int NOTIFY_RESET                = 1;
        protected const int NOTIFY_CAN_RESET            = 2;
        protected const int NOTIFY_DBL_CLICK            = 3;
        protected const int NOTIFY_SHOULD_PERSIST       = 4;
        protected const int NOTIFY_RETURN               = 5;

        protected const int     OUTLINE_ICON_PADDING    = 5;
        
        protected static IComparer DisplayNameComparer    = new DisplayNameSortComparer();
        
        private static char passwordReplaceChar;
        //Maximum number of characters we'll show in the property grid.  Too many characters leads
        //to bad performance.
        private const int maximumLengthOfPropertyString = 1000;

        [Flags]
        internal enum PaintValueFlags{
            None = 0,
            DrawSelected = 0x1,
            FetchValue   = 0x2,
            CheckShouldSerialize = 0x4,
            PaintInPlace = 0x8
        }

        private class CacheItems {
            public string lastLabel;
            public Font   lastLabelFont;
            public int    lastLabelWidth;
            public string lastValueString;
            public Font   lastValueFont;
            public int    lastValueTextWidth;
            public object lastValue;
            public bool   useValueString;
            public bool   lastShouldSerialize;
            public bool   useShouldSerialize;
            public bool   useCompatTextRendering;
        }

        private CacheItems cacheItems;


        // instance variables.
        protected TypeConverter     converter     = null;
        protected UITypeEditor      editor        = null;
        internal  GridEntry         parentPE      = null;
        private   GridEntryCollection childCollection = null;
        internal  int               flags         = 0;
        private   int               propertyDepth = 0;
        protected bool              hasFocus      = false;
        private   Rectangle         outlineRect   = Rectangle.Empty;
        protected PropertySort      PropertySort;

        protected Point             labelTipPoint = InvalidPoint;
        protected Point             valueTipPoint = InvalidPoint;

        protected PropertyGrid      ownerGrid;

        private   static object      EVENT_VALUE_CLICK = new object();
        private   static object      EVENT_LABEL_CLICK = new object();
        private   static object      EVENT_OUTLINE_CLICK = new object();
        private   static object      EVENT_VALUE_DBLCLICK = new object();
        private   static object      EVENT_LABEL_DBLCLICK = new object();
        private   static object      EVENT_OUTLINE_DBLCLICK = new object();
        private   static object      EVENT_RECREATE_CHILDREN = new object();

        private GridEntryAccessibleObject accessibleObject = null;

        private   bool               lastPaintWithExplorerStyle = false;

        private static Color InvertColor(Color color) {
            return Color.FromArgb(color.A, (byte)~color.R, (byte)~color.G, (byte)~color.B);
        }

        protected GridEntry(PropertyGrid owner, GridEntry peParent) {
            parentPE = peParent;
            ownerGrid = owner;

            Debug.Assert( this.ownerGrid != null, "GridEntry w/o PropertyGrid owner, text rendering will fail." );

            if (peParent != null) {
                propertyDepth = peParent.PropertyDepth + 1;
                this.PropertySort = peParent.PropertySort;

                if (peParent.ForceReadOnly) {
                    flags |= FLAG_FORCE_READONLY;
                }

            }
            else {
                propertyDepth = -1;
            }
        }

        /// <summary>
        /// Outline Icon padding
        /// </summary>
        private int OutlineIconPadding {
            get {
                if (DpiHelper.IsScalingRequirementMet) {
                    if (this.GridEntryHost != null) {
                        return this.GridEntryHost.LogicalToDeviceUnits(OUTLINE_ICON_PADDING);
                    }
                }

                return OUTLINE_ICON_PADDING;
            }
        }

        private bool colorInversionNeededInHC {
            get {
                 return SystemInformation.HighContrast && !OwnerGrid.developerOverride && AccessibilityImprovements.Level1;
            }
        }

        public AccessibleObject AccessibilityObject {

            get {
                if (accessibleObject == null) {
                    accessibleObject = GetAccessibilityObject();
                }
                return accessibleObject;
            }
        }

        protected virtual GridEntryAccessibleObject GetAccessibilityObject() {
            return new GridEntryAccessibleObject(this);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.AllowMerge"]/*' />
        /// <devdoc>
        /// specify that this grid entry should be allowed to be merged for.
        /// multi-select.
        /// </devdoc>
        public virtual bool AllowMerge {
            get {
                return true;
            }
        }
        
        internal virtual bool AlwaysAllowExpand {
            get {
               return false;
            }
        }

        internal virtual AttributeCollection Attributes {
            get {
                return TypeDescriptor.GetAttributes(PropertyType);
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.BackgroundBrush"]/*' />
        /// <devdoc>
        /// Gets the value of the background brush to use.  Override
        /// this member to cause the entry to paint it's background in a different color.
        /// The base implementation returns null.
        /// </devdoc>
        protected virtual Brush GetBackgroundBrush(Graphics g) {
            return GridEntryHost.GetBackgroundBrush(g);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.LabelTextColor"]/*' />
        /// <devdoc>
        /// </devdoc>
        protected virtual Color LabelTextColor {
            get {
                if (this.ShouldRenderReadOnly) {
                    return GridEntryHost.GrayTextColor;
                }
                else {
                    return GridEntryHost.GetTextColor();
                }
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.BrowsableAttributes"]/*' />
        /// <devdoc>
        /// The set of attributes that will be used for browse filtering
        /// </devdoc>
        public virtual AttributeCollection BrowsableAttributes {
            get{
                if (parentPE != null) {
                    return parentPE.BrowsableAttributes;
                }
                return null;
            }
            set{
                parentPE.BrowsableAttributes = value;
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.Component"]/*' />
        /// <devdoc>
        ///      Retrieves the component that is invoking the
        ///      method on the formatter object.  This may
        ///      return null if there is no component
        ///      responsible for the call.
        /// </devdoc>
        public virtual IComponent Component {
            get {
                object owner = GetValueOwner();
                if (owner is IComponent) {
                    return(IComponent) owner;
                }
                if (parentPE != null) {
                    return parentPE.Component;
                }
                return null;
            }
        }

        protected virtual IComponentChangeService ComponentChangeService {
            get {
                return parentPE.ComponentChangeService;
            }

        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.Container"]/*' />
        /// <devdoc>
        ///      Retrieves the container that contains the
        ///      set of objects this formatter may work
        ///      with.  It may return null if there is no
        ///      container, or of the formatter should not
        ///      use any outside objects.
        /// </devdoc>
        public virtual IContainer Container {
            get {
                IComponent component = Component;
                if (component != null) {
                    ISite site = component.Site;
                    if (site != null) {
                        return site.Container;
                    }
                }
                return null;
            }
        }

        protected GridEntryCollection ChildCollection{
            get {
                if (childCollection == null) {
                    childCollection = new GridEntryCollection(this, null);
                }
                return childCollection;
            }
            set {
                Debug.Assert(value == null || !Disposed, "Why are we putting new children in after we are disposed?");
                if (this.childCollection != value) {
                    if (this.childCollection != null) {
                        this.childCollection.Dispose();
                        this.childCollection = null;
                    }
                    this.childCollection = value;
                }
            }
        }

        public int ChildCount {
            get {
                if (Children != null) {
                    return Children.Count;
                }
                return 0;
            }
        }

        public virtual GridEntryCollection Children {
            get {
                if (childCollection == null && !Disposed) {
                    CreateChildren();
                }
                return childCollection;
            }
        }

        public virtual PropertyTab CurrentTab{
            get{
                if (parentPE != null) {
                    return parentPE.CurrentTab;
                }
                return null;
            }
            set{
                if (parentPE != null) {
                    parentPE.CurrentTab = value;
                }
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.DefaultChild"]/*' />
        /// <devdoc>
        /// Returns the default child GridEntry of this item.  Usually the default property
        /// of the target object.
        /// </devdoc>
        internal virtual GridEntry DefaultChild {
            get {
                return null;
            }
            set{}
        }

        internal virtual IDesignerHost DesignerHost{
            get{
                if (parentPE != null) {
                    return parentPE.DesignerHost;
                }
                return null;
            }
            set {
                if (parentPE != null) {
                    parentPE.DesignerHost = value;
                }
            }
        }

        internal bool Disposed{
            get {
                return GetFlagSet(FLAG_DISPOSED);
            }
        }

        internal virtual bool Enumerable {
            get {
                return(this.Flags & GridEntry.FLAG_ENUMERABLE) != 0;
            }
        }


        public override bool Expandable {
            get {
                bool fExpandable = GetFlagSet(FL_EXPANDABLE);

                if (fExpandable && childCollection != null && childCollection.Count > 0) {
                    return true;
                }

                if (GetFlagSet(FL_EXPANDABLE_FAILED)) {
                    return false;
                }

                if (fExpandable && (cacheItems == null || cacheItems.lastValue == null) && this.PropertyValue == null) {
                    fExpandable = false;
                }

                return fExpandable;
            }
        }

        public override bool Expanded {
            get{
                return InternalExpanded;
            }
            set {
                GridEntryHost.SetExpand(this, value);
            }
        }

        internal virtual bool ForceReadOnly {
            get {
                return (flags & FLAG_FORCE_READONLY) != 0;
            }
        }
        
         internal virtual bool InternalExpanded {
            get{
                // short circuit if we don't have children
                if (childCollection == null || childCollection.Count == 0) {
                    return false;
                }
                return GetFlagSet(FL_EXPAND);
            }
            set {
                if (!this.Expandable || value == this.InternalExpanded) {
                    return;
                }

                if (childCollection != null && childCollection.Count > 0) {
                    SetFlag(FL_EXPAND,value);
                }
                else {
                    SetFlag(FL_EXPAND,false);
                    if (value) {
                        bool fMakeSure = CreateChildren();
                        SetFlag(FL_EXPAND,fMakeSure);
                    }
                }

                if (AccessibilityImprovements.Level1) {
                    // Notify accessibility clients of expanded state change
                    // StateChange requires NameChange, too - accessible clients won't see this, unless both events are raised

                    // Root item is hidden and should not raise events
                    if (GridItemType != GridItemType.Root) {
                        int id = ((PropertyGridView)GridEntryHost).AccessibilityGetGridEntryChildID(this);
                        if (id >= 0) {
                            PropertyGridView.PropertyGridViewAccessibleObject gridAccObj =
                                (PropertyGridView.PropertyGridViewAccessibleObject)((PropertyGridView)GridEntryHost).AccessibilityObject;

                            gridAccObj.NotifyClients(AccessibleEvents.StateChange, id);
                            gridAccObj.NotifyClients(AccessibleEvents.NameChange, id);
                        }
                    }
                }
            }
        }

        internal virtual int Flags {
            get {
                if ((flags & FL_CHECKED) != 0) {
                    return flags;
                }

                flags |= FL_CHECKED;

                TypeConverter converter = TypeConverter;
                UITypeEditor  uiEditor  = UITypeEditor;
                object value = Instance;
                bool forceReadOnly = this.ForceReadOnly;

                if (value != null) {
                     forceReadOnly |= TypeDescriptor.GetAttributes(value).Contains(InheritanceAttribute.InheritedReadOnly);
                }
                
                if (converter.GetStandardValuesSupported(this)) {
                    flags |= GridEntry.FLAG_ENUMERABLE;
                }

                if (!forceReadOnly && converter.CanConvertFrom(this, typeof(string)) &&
                    !converter.GetStandardValuesExclusive(this)) {
                    flags |= GridEntry.FLAG_TEXT_EDITABLE;
                }
                
                bool isImmutableReadOnly = TypeDescriptor.GetAttributes(this.PropertyType)[typeof(ImmutableObjectAttribute)].Equals(ImmutableObjectAttribute.Yes);
                bool isImmutable = isImmutableReadOnly || converter.GetCreateInstanceSupported(this);
                
                if (isImmutable) {
                    flags |= GridEntry.FLAG_IMMUTABLE;
                }

                if (converter.GetPropertiesSupported(this)) {
                    flags |= GridEntry.FL_EXPANDABLE;

                    // If we're exapndable, but we don't support editing,
                    // make us read only editable so we don't paint grey.
                    //
                    if (!forceReadOnly && (flags & GridEntry.FLAG_TEXT_EDITABLE) == 0 && !isImmutableReadOnly) {
                        flags |= GridEntry.FLAG_READONLY_EDITABLE;
                    }
                }

                if (Attributes.Contains(PasswordPropertyTextAttribute.Yes)) {
                    flags |= GridEntry.FLAG_RENDER_PASSWORD;
                }

                if (uiEditor != null) {
                    if (uiEditor.GetPaintValueSupported(this)) {
                        flags |= GridEntry.FLAG_CUSTOM_PAINT;
                    }

                    // We only allow drop-downs if the object is NOT being inherited
                    // I would really rather this not be here, but we have other places where
                    // we make read-only properties editable if they have drop downs.  Not
                    // sure this is the right thing...is it?

                    bool allowButtons = !forceReadOnly;
                    
                    if (allowButtons) {
                        switch (uiEditor.GetEditStyle(this)) {
                            case UITypeEditorEditStyle.Modal:
                                flags |= GridEntry.FLAG_CUSTOM_EDITABLE;
                                if (!isImmutable && !PropertyType.IsValueType) {
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
            set {
                flags = value;
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.Focus"]/*' />
        /// <devdoc>
        /// Checks if the entry is currently expanded
        /// </devdoc>
        public bool Focus {
            get{
                return this.hasFocus;
            }
            set{

                if (Disposed) {
                    return;
                }

                if (cacheItems != null) {
                    cacheItems.lastValueString = null;
                    cacheItems.useValueString = false;
                    cacheItems.useShouldSerialize = false;
                }
                
                if (this.hasFocus != value) {
                    this.hasFocus = value;

                    // Notify accessibility applications that keyboard focus has changed.
                    //
                    if (value == true) {
                        int id = ((PropertyGridView)GridEntryHost).AccessibilityGetGridEntryChildID(this);
                        if (id >= 0) {
                            PropertyGridView.PropertyGridViewAccessibleObject gridAccObj = 
                                (PropertyGridView.PropertyGridViewAccessibleObject)((PropertyGridView)GridEntryHost).AccessibilityObject;
                                
                            gridAccObj.NotifyClients(AccessibleEvents.Focus, id);
                            gridAccObj.NotifyClients(AccessibleEvents.Selection, id); 
                        }
                    }
                }
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.FullLabel"]/*' />
        /// <devdoc>
        /// Returns the label including the object name, and properties.  For example, the value
        /// of the Font size property on a Button called Button1 would be "Button1.Font.Size"
        /// </devdoc>
        public string FullLabel {
            get {
                string str = null;
                if (parentPE != null) {
                    str = parentPE.FullLabel;
                }

                if (str != null) {
                    str += ".";
                }
                else {
                    str = "";
                }
                str += this.PropertyLabel;

                return str;
            }
        }
        
        public override GridItemCollection GridItems {
            get {
                if (Disposed) {
                    throw new ObjectDisposedException(SR.GridItemDisposed);
                }
                
                if (IsExpandable && childCollection != null && childCollection.Count == 0) {
                    CreateChildren();
                }
                
                return this.Children;
            }
        }

        internal virtual PropertyGridView GridEntryHost{
            get{        // ACCESSOR: virtual was missing from this get
                if (parentPE != null) {
                    return parentPE.GridEntryHost;
                }
                return null;
            }
            set {
                throw new NotSupportedException();
            }
        }
        
        public override GridItemType GridItemType {
            get {
                return GridItemType.Property;
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.HasValue"]/*' />
        /// <devdoc>
        /// Returns true if this GridEntry has a value field in the right hand column.
        /// </devdoc>
        internal virtual bool HasValue {
            get {
                return true;
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.HelpKeyword"]/*' />
        /// <devdoc>
        ///     Retrieves the keyword that the VS help dynamic help window will
        ///     use when this IPE is selected.
        /// </devdoc>
        public virtual string HelpKeyword {
            get {
                string keyWord = null;

                if (parentPE != null) {
                    keyWord = parentPE.HelpKeyword;
                }
                if (keyWord == null) {
                    keyWord = String.Empty;
                }

                return keyWord;
            }
        }
        
        internal virtual string HelpKeywordInternal{
            get {
               return this.HelpKeyword;
            }
        }

        public virtual bool IsCustomPaint {
            get {
                // prevent full flag population if possible.
                if ((flags & FL_CHECKED) == 0) {
                    UITypeEditor typeEd = this.UITypeEditor;
                    if (typeEd != null) {
                        if ((this.flags & GridEntry.FLAG_CUSTOM_PAINT) != 0 || 
                            (this.flags & GridEntry.FL_NO_CUSTOM_PAINT) != 0) {
                            return(this.flags & GridEntry.FLAG_CUSTOM_PAINT) != 0;
                        }


                        if (typeEd.GetPaintValueSupported(this)) {
                            flags |= GridEntry.FLAG_CUSTOM_PAINT;
                            return true;
                        }
                        else {
                            flags |= GridEntry.FL_NO_CUSTOM_PAINT;
                            return false;
                        }    
                    }
                }
                return(this.Flags & GridEntry.FLAG_CUSTOM_PAINT) != 0;
            }
        }
        
        public virtual bool IsExpandable {
            get {
                return this.Expandable;
            }
            set {
                if (value != GetFlagSet(FL_EXPANDABLE)) {
                    SetFlag(FL_EXPANDABLE_FAILED, false);
                    SetFlag(FL_EXPANDABLE, value);
                }
            }
        }

        public virtual bool IsTextEditable {
            get {
                return this.IsValueEditable && (this.Flags & GridEntry.FLAG_TEXT_EDITABLE) != 0;
            }
        }

        public virtual bool IsValueEditable {
            get {
                return !ForceReadOnly && 0 != (Flags & (GridEntry.FLAG_DROPDOWN_EDITABLE | GridEntry.FLAG_TEXT_EDITABLE | GridEntry.FLAG_CUSTOM_EDITABLE | GridEntry.FLAG_ENUMERABLE));
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.Instance"]/*' />
        /// <devdoc>
        ///      Retrieves the component that is invoking the
        ///      method on the formatter object.  This may
        ///      return null if there is no component
        ///      responsible for the call.
        /// </devdoc>
        public virtual object Instance {
            get {
                object owner = GetValueOwner();

                if (parentPE != null && owner == null) {
                    return parentPE.Instance;
                }
                return owner;
            }
        }
        
        public override string Label {
            get {
                return this.PropertyLabel;
            }
        }
        
        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.PropertyDescriptor"]/*' />
        /// <devdoc>
        ///      Retrieves the PropertyDescriptor that is surfacing the given object/
        /// </devdoc>
        public override PropertyDescriptor PropertyDescriptor {
            get {
                return null;
            }
        }

        

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.PropertyLabelIndent"]/*' />
        /// <devdoc>
        /// Returns the pixel indent of the current GridEntry's label.
        /// </devdoc>
        internal virtual int PropertyLabelIndent {
            get {
                int borderWidth = this.GridEntryHost.GetOutlineIconSize() + OUTLINE_ICON_PADDING;
                return((propertyDepth + 1) * borderWidth) + 1;
            }
        }

        internal virtual Point GetLabelToolTipLocation(int mouseX, int mouseY) {
            return labelTipPoint;
        }

        internal virtual string LabelToolTipText {
            get {
                return this.PropertyLabel;
            }
        }

        public virtual bool NeedsDropDownButton{
            get {
                return(this.Flags & GridEntry.FLAG_DROPDOWN_EDITABLE) != 0;
            }
        }

        public virtual bool NeedsCustomEditorButton{
            get {
                return(this.Flags & GridEntry.FLAG_CUSTOM_EDITABLE) != 0 && (IsValueEditable || (Flags & GridEntry.FLAG_READONLY_EDITABLE) !=0);
            }
        }

        public PropertyGrid OwnerGrid{
            get{
                return this.ownerGrid;
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.OutlineRect"]/*' />
        /// <devdoc>
        /// Returns rect that the outline icon (+ or - or arrow) will be drawn into, relative
        /// to the upper left corner of the GridEntry.
        /// </devdoc>
        public Rectangle OutlineRect {
            get {
                if (!outlineRect.IsEmpty) {
                    return outlineRect;
                }
                PropertyGridView gridHost = this.GridEntryHost;
                Debug.Assert(gridHost != null, "No propEntryHost!");
                int outlineSize = gridHost.GetOutlineIconSize();
                int borderWidth = outlineSize + OutlineIconPadding;
                int left = (propertyDepth * borderWidth) + (OutlineIconPadding) / 2;
                int top = (gridHost.GetGridEntryHeight() - outlineSize) / 2;
                outlineRect = new Rectangle(left, top, outlineSize, outlineSize);
                return outlineRect;
            }
            set {
                // set property is required to reset cached value when dpi changed.
                if (value != outlineRect) {
                    outlineRect = value;
                }
            }
        }

        public virtual GridEntry ParentGridEntry{
            get {
                return this.parentPE;
            }
            set {
                Debug.Assert(value != this, "how can we be our own parent?");
                this.parentPE = value;
                if (value != null) {
                    propertyDepth = value.PropertyDepth+1;

                    // Microsoft, why do we do this?
                    if (this.childCollection != null) {
                        for (int i = 0; i < childCollection.Count; i++) {
                            childCollection.GetEntry(i).ParentGridEntry = this;
                        }
                    }
                }
            }
        }
        
        public override GridItem Parent {
            get {
                if (Disposed) {
                    throw new ObjectDisposedException(SR.GridItemDisposed);
                }
                
                GridItem parent = this.ParentGridEntry;
                
                // don't allow walking all the way up to the parent.
                //
                //if (parent is IRootGridEntry) {
                //    return null;
                //}
                return parent;
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.PropertyCategory"]/*' />
        /// <devdoc>
        /// Returns category name of the current property
        /// </devdoc>
        public virtual string PropertyCategory {
            get {
                return CategoryAttribute.Default.Category;
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.PropertyDepth"]/*' />
        /// <devdoc>
        /// Returns "depth" of this property.  That is, how many parent's between
        /// this property and the root property.  The root property has a depth of -1.
        /// </devdoc>
        public virtual int PropertyDepth {
            get {
                return propertyDepth;
            }
        }


        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.PropertyDescription"]/*' />
        /// <devdoc>
        /// Returns the description helpstring for this GridEntry.
        /// </devdoc>
        public virtual string PropertyDescription {
            get {
                return null;
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.PropertyLabel"]/*' />
        /// <devdoc>
        /// Returns the label of this property.  Usually
        /// this is the property name.
        /// </devdoc>
        public virtual string PropertyLabel {
            get {
                return null;
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.PropertyName"]/*' />
        /// <devdoc>
        /// Returns non-localized name of this property.
        /// </devdoc>
        public virtual string PropertyName {
            get {
                return this.PropertyLabel;
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.PropertyType"]/*' />
        /// <devdoc>
        /// Returns the Type of the value of this GridEntry, or null if the value is null.
        /// </devdoc>
        public virtual Type PropertyType {
            get {
                object obj = this.PropertyValue;
                if (obj != null) {
                    return obj.GetType();
                }
                else {
                    return null;
                }
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.PropertyValue"]/*' />
        /// <devdoc>
        /// Gets or sets the value for the property that is represented
        /// by this GridEntry.
        /// </devdoc>
        public virtual object PropertyValue{
            get {
                if (cacheItems != null) {
                    return cacheItems.lastValue;
                }
                return null;
            }
            set {
            }
        }

        public virtual bool ShouldRenderPassword {
            get {
                return (this.Flags & GridEntry.FLAG_RENDER_PASSWORD) != 0;
            }
        }

        public virtual bool ShouldRenderReadOnly {
            get {
                return ForceReadOnly || (0 != (this.Flags & GridEntry.FLAG_RENDER_READONLY) || (!this.IsValueEditable && (0 == (this.Flags & GridEntry.FLAG_READONLY_EDITABLE))));
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.TypeConverter"]/*' />
        /// <devdoc>
        /// Returns the type converter for this entry.
        /// </devdoc>
        internal virtual TypeConverter TypeConverter {
            get {
                if (converter == null) {
                    object value = this.PropertyValue;
                    if (value == null) {
                        converter = TypeDescriptor.GetConverter(this.PropertyType);
                    }
                    else {
                        converter = TypeDescriptor.GetConverter(value);
                    }
                }
                return converter;
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.UITypeEditor"]/*' />
        /// <devdoc>
        /// Returns the type editor for this entry.  This may return null if there
        /// is no type editor.
        /// </devdoc>
        internal virtual UITypeEditor UITypeEditor {
            get {
                if (editor == null && this.PropertyType != null) {
                    editor = (UITypeEditor)TypeDescriptor.GetEditor(this.PropertyType, typeof(System.Drawing.Design.UITypeEditor));
                }

                return editor;
            }
        }
        
        public override object Value {
            get {
                return this.PropertyValue;
            }
            // note: we don't do set because of the value class semantics, etc.
        }

        internal Point ValueToolTipLocation {
            get {
                return ShouldRenderPassword ? InvalidPoint : valueTipPoint;
            }
            set{
                valueTipPoint = value;
            }
        }

        internal int VisibleChildCount {
            get{
                if (!Expanded) {
                    return 0;
                }
                int count = ChildCount;
                int totalCount = count;
                for (int i = 0; i < count; i++) {
                     totalCount += ChildCollection.GetEntry(i).VisibleChildCount;
                }
                return totalCount;
            }
        }


        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.AddOnLabelClick"]/*' />
        /// <devdoc>
        /// Add an event handler to be invoked when the label portion of
        /// the prop entry is clicked
        /// </devdoc>
        public virtual void AddOnLabelClick(EventHandler h) {
            AddEventHandler(EVENT_LABEL_CLICK, h);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.AddOnLabelDoubleClick"]/*' />
        /// <devdoc>
        /// Add an event handler to be invoked when the label portion of
        /// the prop entry is double
        /// </devdoc>
        public virtual void AddOnLabelDoubleClick(EventHandler h) {
            AddEventHandler(EVENT_LABEL_DBLCLICK, h);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.AddOnValueClick"]/*' />
        /// <devdoc>
        /// Add an event handler to be invoked when the value portion of
        /// the prop entry is clicked
        /// </devdoc>
        public virtual void AddOnValueClick(EventHandler h) {
            AddEventHandler(EVENT_VALUE_CLICK, h);
        }


        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.AddOnValueDoubleClick"]/*' />
        /// <devdoc>
        /// Add an event handler to be invoked when the value portion of
        /// the prop entry is double-clicked
        /// </devdoc>
        public virtual void AddOnValueDoubleClick(EventHandler h) {
            AddEventHandler(EVENT_VALUE_DBLCLICK, h);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.AddOnOutlineClick"]/*' />
        /// <devdoc>
        /// Add an event handler to be invoked when the outline icone portion of
        /// the prop entry is clicked
        /// </devdoc>
        public virtual void AddOnOutlineClick(EventHandler h) {
            AddEventHandler(EVENT_OUTLINE_CLICK, h);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.AddOnOutlineDoubleClick"]/*' />
        /// <devdoc>
        /// Add an event handler to be invoked when the outline icone portion of
        /// the prop entry is double clicked
        /// </devdoc>
        public virtual void AddOnOutlineDoubleClick(EventHandler h) {
            AddEventHandler(EVENT_OUTLINE_DBLCLICK, h);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.AddOnRecreateChildren"]/*' />
        /// <devdoc>
        /// Add an event handler to be invoked when the children grid entries are re-created.
        /// </devdoc>
        public virtual void AddOnRecreateChildren(GridEntryRecreateChildrenEventHandler h) {
            AddEventHandler(EVENT_RECREATE_CHILDREN, h);
        }

        internal void ClearCachedValues() {
            ClearCachedValues(true);
        }

        internal void ClearCachedValues(bool clearChildren) {
               if (cacheItems != null) {
                  cacheItems.useValueString = false;
                  cacheItems.lastValue = null;
                  cacheItems.useShouldSerialize = false;
               }
               if (clearChildren) {
                   for (int i = 0; i < ChildCollection.Count; i++) {
                       ChildCollection.GetEntry(i).ClearCachedValues();
                   }
               }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.ConvertTextToValue"]/*' />
        /// <devdoc>
        /// Converts the given string of text to a value.
        /// </devdoc>
        public object ConvertTextToValue(string text) {
            if (TypeConverter.CanConvertFrom(this, typeof(string))) {
                return TypeConverter.ConvertFromString(this, text);
            }
            return text;
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.Create"]/*' />
        /// <devdoc>
        /// Create the base prop entries given an object or set of objects
        /// </devdoc>
        internal static IRootGridEntry Create(PropertyGridView view, object[] rgobjs, IServiceProvider baseProvider, IDesignerHost currentHost, PropertyTab tab, PropertySort initialSortType) {
            IRootGridEntry pe = null;

            if (rgobjs == null || rgobjs.Length == 0) {
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
                //Debug.fail("Couldn't create a top-level GridEntry");
                Debug.Fail(e.ToString());
                throw;
            }
            return pe;
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.CreateChildren"]/*' />
        /// <devdoc>
        /// Populates the children of this grid entry
        /// </devdoc>
        protected virtual bool CreateChildren() {
            return CreateChildren(false);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.CreateChildren1"]/*' />
        /// <devdoc>
        /// Populates the children of this grid entry
        /// </devdoc>
        protected virtual bool CreateChildren(bool diffOldChildren) {

            Debug.Assert(!Disposed, "Why are we creating children after we are disposed?");

            if (!GetFlagSet(FL_EXPANDABLE)) {
                if (this.childCollection != null) {
                    this.childCollection.Clear();
                }
                else {
                    this.childCollection = new GridEntryCollection(this, new GridEntry[0]);
                }
                return false;
            }


            if (!diffOldChildren && childCollection != null && childCollection.Count > 0) {
                return true;
            }


            GridEntry [] childProps    = GetPropEntries(this,
                                                        this.PropertyValue,
                                                        this.PropertyType);


            bool fExpandable = (childProps != null && childProps.Length > 0);                                            

            if (diffOldChildren && childCollection != null && childCollection.Count > 0) {
                bool same = true;
                if (childProps.Length == childCollection.Count) {
                    for (int i = 0; i < childProps.Length; i++) {
                        if (!childProps[i].NonParentEquals(childCollection[i])) {
                            same = false;
                            break;
                        }
                    }
                }
                else {
                    same = false;
                }

                if (same) {
                    return true;
                }
            }



            if (!fExpandable) {
                SetFlag(FL_EXPANDABLE_FAILED,true);
                if (this.childCollection != null) {
                    this.childCollection.Clear();
                }
                else {
                    this.childCollection = new GridEntryCollection(this, new GridEntry[0]);
                }

                if (this.InternalExpanded) {
                    this.InternalExpanded = false;
                }

            }
            else {
                if (this.childCollection != null) {
                    this.childCollection.Clear();
                    this.childCollection.AddRange(childProps);
                }
                else {
                    this.childCollection = new GridEntryCollection(this, childProps);
                }
            }
            return fExpandable;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing) {
            // make sure we don't accidentally
            // check flags in this state...
            flags  |= FL_CHECKED;

            SetFlag(FLAG_DISPOSED, true);
            
            cacheItems = null;
            converter = null;
            editor = null;
            accessibleObject = null;

            if (disposing) {
                DisposeChildren();
            }
        }


        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.DisposeChildren"]/*' />
        /// <devdoc>
        /// Disposes the array of children
        /// </devdoc>
        public virtual void DisposeChildren() {
            if (childCollection != null) {
                childCollection.Dispose();
                childCollection = null;
            }
        }


        ~GridEntry() {
            Dispose(false);
        }


        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.EditPropertyValue"]/*' />
        /// <devdoc>
        /// Invokes the type editor for editing this item.
        /// </devdoc>
        internal virtual void EditPropertyValue(PropertyGridView iva) {
            if (UITypeEditor != null) {
                try {
                    // this is another icky part.  since edit value can push a modal loop
                    // there is a chance that this gridentry will be zombied before
                    // it returns.  make sure we're not disposed.
                    //
                    object originalValue = this.PropertyValue;
                    object value = UITypeEditor.EditValue(this, (IServiceProvider)(ITypeDescriptorContext)this, originalValue);

                    if (Disposed) {
                        return;
                    }

                    // Push the new value back into the property
                    if (value != originalValue && this.IsValueEditable) {
                        iva.CommitValue(this, value);
                    }

                    if (this.InternalExpanded) {
                        // QFE#3299: If edited property is expanded to show sub-properties, then we want to
                        // preserve the expanded states of it and all of its descendants. RecreateChildren()
                        // has logic that is supposed to do this, but which is fundamentally flawed.
                        PropertyGridView.GridPositionData positionData = GridEntryHost.CaptureGridPositionData();
                        this.InternalExpanded = false;
                        RecreateChildren();
                        positionData.Restore(GridEntryHost);
                    }
                    else {
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

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.Equals"]/*' />
        /// <devdoc>
        /// Tests two GridEntries for equality
        /// </devdoc>
        public override bool Equals(object obj) {

            if (NonParentEquals(obj)) {
                return((GridEntry)obj).ParentGridEntry == this.ParentGridEntry;
            }
            return false;
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.FindPropertyValue"]/*' />
        /// <devdoc>
        /// Searches for a value of a given property for a value editor user
        /// </devdoc>
        public virtual object FindPropertyValue(string propertyName, Type propertyType) {
            object owner = GetValueOwner();
            PropertyDescriptor property = TypeDescriptor.GetProperties(owner)[propertyName];
            if (property != null && property.PropertyType == propertyType) {
                return property.GetValue(owner);
            }

            if (parentPE != null)
                return parentPE.FindPropertyValue(propertyName, propertyType);

            return null;
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.GetChildIndex"]/*' />
        /// <devdoc>
        /// Returns the index of a child GridEntry
        /// </devdoc>
        internal virtual int GetChildIndex(GridEntry pe) {
            return this.Children.GetEntry(pe);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.GetComponents"]/*' />
        /// <devdoc>
        /// Gets the components that own the current value.  This is usually the value of the
        /// root entry, which is the object being browsed.  Walks up the GridEntry tree
        /// looking for an owner that is an IComponent
        /// </devdoc>
        public virtual IComponent[] GetComponents() {
            IComponent component = Component;
            if (component != null) {
                return new IComponent[] { component};
            }
            return null;
        }

        protected int GetLabelTextWidth(string labelText, Graphics g, Font f) {

            if (cacheItems == null) {
                cacheItems = new CacheItems();
            }
            else if (cacheItems.useCompatTextRendering == ownerGrid.UseCompatibleTextRendering && cacheItems.lastLabel == labelText && f.Equals(cacheItems.lastLabelFont)) {
                return cacheItems.lastLabelWidth;
            }

            SizeF textSize = PropertyGrid.MeasureTextHelper.MeasureText( this.ownerGrid, g, labelText, f);

            cacheItems.lastLabelWidth = (int) textSize.Width;
            cacheItems.lastLabel = labelText;
            cacheItems.lastLabelFont = f;
            cacheItems.useCompatTextRendering = ownerGrid.UseCompatibleTextRendering;

            return cacheItems.lastLabelWidth;
        }

        internal int GetValueTextWidth(string valueString, Graphics g, Font f) {

            if (cacheItems == null) {
               cacheItems = new CacheItems();
            } 
            else if (cacheItems.lastValueTextWidth != -1 && cacheItems.lastValueString == valueString && f.Equals(cacheItems.lastValueFont)) {
               return cacheItems.lastValueTextWidth;
            }

            // Value text is rendered using GDI directly (No TextRenderer) but measured/adjusted using GDI+ (since previous releases), so don't use MeasureTextHelper.
            cacheItems.lastValueTextWidth = (int) g.MeasureString(valueString, f).Width;
            cacheItems.lastValueString = valueString;
            cacheItems.lastValueFont = f;
            return cacheItems.lastValueTextWidth;
        }
        // To check if text contains multiple lines
        //
        internal bool GetMultipleLines(string valueString) {

            if (valueString.IndexOf('\n') > 0  ||  valueString.IndexOf('\r') > 0 )
                return true;
            else 
                return false;
        }
        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.GetValueOwner"]/*' />
        /// <devdoc>
        /// Gets the owner of the current value.  This is usually the value of the
        /// root entry, which is the object being browsed
        /// </devdoc>
        public virtual object GetValueOwner() {
            if (parentPE == null) {
                return this.PropertyValue;
            }

            return parentPE.GetChildValueOwner(this);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.GetValueOwners"]/*' />
        /// <devdoc>
        /// Gets the owners of the current value.  This is usually the value of the
        /// root entry, which is the objects being browsed for a multiselect item
        /// </devdoc>
        public virtual object[] GetValueOwners() {
            object owner = GetValueOwner();
            if (owner != null) {
                return new object[] { owner};
            }
            return null;
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.GetChildValueOwner"]/*' />
        /// <devdoc>
        /// Gets the owner of the current value.  This is usually the value of the
        /// root entry, which is the object being browsed
        /// </devdoc>
        public virtual object GetChildValueOwner(GridEntry childEntry) {

            /*// make sure this is one of our children
            int index = GetChildIndex(childEntry);

            if (index != -1){
               return this.PropertyValue;
            }


            Debug.Fail(childEntry.PropertyLabel + " is not a child of " + this.PropertyLabel);
            return null;*/
            return this.PropertyValue;
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.GetTestingInfo"]/*' />
        /// <devdoc>
        /// Returns a string with info about the currently selected GridEntry
        /// </devdoc>
        public virtual string GetTestingInfo() {
            string str = "object = (";
            string textVal = GetPropertyTextValue();
            if (textVal == null) {
                textVal = "(null)";
            }
            else {
                // make sure we clear any embedded nulls
                textVal = textVal.Replace((char)0, ' ');
            }
            Type type = this.PropertyType;
            if (type==null) {
                type = typeof(object);
            }
            str += this.FullLabel;
            str += "), property = (" + this.PropertyLabel + "," + type.AssemblyQualifiedName + "), value = " + "[" + textVal + "], expandable = " + this.Expandable.ToString() + ", readOnly = " + ShouldRenderReadOnly;;
            return str;
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.GetValueType"]/*' />
        /// <devdoc>
        /// Retrieves the type of the value for this GridEntry
        /// </devdoc>
        public virtual Type GetValueType() {
            return this.PropertyType;
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.GetPropEntries"]/*' />
        /// <devdoc>
        /// Returns the child GridEntries for this item.
        /// </devdoc>
        protected virtual GridEntry[] GetPropEntries(GridEntry peParent, object obj, Type objType) {


            // we don't want to create subprops for null objects.
            if (obj == null) {
                return null;
            }

            GridEntry[] entries = null;

            Attribute[] attributes = new Attribute[this.BrowsableAttributes.Count];
            this.BrowsableAttributes.CopyTo(attributes, 0);

            PropertyTab tab = this.CurrentTab;
            Debug.Assert(tab != null, "No current tab!");

            try
            {

                bool forceReadOnly = this.ForceReadOnly;

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

                    if (props == null)
                    {
                        return null;
                    }

                    if ((this.PropertySort & PropertySort.Alphabetical) != 0)
                    {
                        if (objType == null || !objType.IsArray)
                        {
                            props = props.Sort(GridEntry.DisplayNameComparer);
                        }

                        PropertyDescriptor[] propertyDescriptors = new PropertyDescriptor[props.Count];
                        props.CopyTo(propertyDescriptors, 0);

                        props = new PropertyDescriptorCollection(SortParenProperties(propertyDescriptors));
                    }

                    if (defProp == null && props.Count > 0)
                    {
                        defProp = props[0];
                    }

                    // if the target object is an array and nothing else has provided a set of
                    // properties to use, then expand the array.
                    //
                    if ((props == null || props.Count == 0) && objType != null && objType.IsArray && obj != null)
                    {

                        Array objArray = (Array)obj;

                        entries = new GridEntry[objArray.Length];

                        for (int i = 0; i < entries.Length; i++)
                        {
                            entries[i] = new ArrayElementGridEntry(this.ownerGrid, peParent, i);
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
                                newEntry = new ImmutablePropertyDescriptorGridEntry(this.ownerGrid, peParent, pd, hide);
                            }
                            else
                            {
                                newEntry = new PropertyDescriptorGridEntry(this.ownerGrid, peParent, pd, hide);
                            }

                            if (forceReadOnly)
                            {
                                newEntry.flags |= FLAG_FORCE_READONLY;
                            }

                            // check to see if we've come across the default item.
                            //
                            if (pd.Equals(defProp))
                            {
                                this.DefaultChild = newEntry;
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
                if (PbrsAssertPropsSwitch.Enabled) {
                    // Checked builds are not giving us enough information here.  So, output as much stuff as
                    // we can.
                    System.Text.StringBuilder b = new System.Text.StringBuilder();
                    b.Append(string.Format(CultureInfo.CurrentCulture, "********* Debug log written on {0} ************\r\n", DateTime.Now));
                    b.Append(string.Format(CultureInfo.CurrentCulture, "Exception '{0}' reading properties for object {1}.\r\n", e.GetType().Name, obj));
                    b.Append(string.Format(CultureInfo.CurrentCulture, "Exception Text: \r\n{0}", e.ToString()));
                    b.Append(string.Format(CultureInfo.CurrentCulture, "Exception stack: \r\n{0}", e.StackTrace));
                    string path = string.Format(CultureInfo.CurrentCulture, "{0}\\PropertyGrid.log", Environment.GetEnvironmentVariable("SYSTEMDRIVE"));
                    System.IO.FileStream s = new System.IO.FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.None);
                    System.IO.StreamWriter w = new System.IO.StreamWriter(s);
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

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.ResetPropertyValue"]/*' />
        /// <devdoc>
        /// Resets the current item
        /// </devdoc>
        public virtual void ResetPropertyValue() {
            NotifyValue(NOTIFY_RESET);
            Refresh();
        }

        /*
        /// <summary>
        /// Checks if the value of the current item can be modified by the user.
        /// </summary>
        /// <returns>
        /// True if the value can be modified
        /// </returns>
        public virtual bool CanSetPropertyValue() {
            return 0 != (Flags & (GridEntry.FLAG_DROPDOWN_EDITABLE | GridEntry.FLAG_TEXT_EDITABLE | GridEntry.FLAG_CUSTOM_EDITABLE | GridEntry.FLAG_ENUMERABLE));
        }
        */

        /*
        /// <summary>
        /// Returns if it's an editable item.  An example of a readonly
        /// editable item is a collection property -- the property itself
        /// can not be modified, but it's value (e.g. it's children) can, so
        /// we don't want to draw it as readonly.
        /// </summary>
        /// <returns>
        /// True if the value associated with this property (e.g. it's children) can be modified even if it's readonly.
        /// </returns>
        public virtual bool CanSetReadOnlyPropertyValue() {
            return GetFlagSet(GridEntry.FLAG_READONLY_EDITABLE);
        }
        */

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.CanResetPropertyValue"]/*' />
        /// <devdoc>
        /// Returns if the property can be reset
        /// </devdoc>
        public virtual bool CanResetPropertyValue() {
            return NotifyValue(NOTIFY_CAN_RESET);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.DoubleClickPropertyValue"]/*' />
        /// <devdoc>
        /// Called when the item is double clicked.
        /// </devdoc>
        public virtual bool DoubleClickPropertyValue() {
            return NotifyValue(NOTIFY_DBL_CLICK);
        }


        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.GetPropertyTextValue"]/*' />
        /// <devdoc>
        /// Returns the text value of this property.
        /// </devdoc>
        public virtual string GetPropertyTextValue() {
            return GetPropertyTextValue(this.PropertyValue);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.GetPropertyTextValue1"]/*' />
        /// <devdoc>
        /// Returns the text value of this property.
        /// </devdoc>
        public virtual string GetPropertyTextValue(object value) {
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

            if (str == null) {
                str = String.Empty;
            }
            return str;
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.GetPropertyValueList"]/*' />
        /// <devdoc>
        /// Returns the text values of this property.
        /// </devdoc>
        public virtual object[] GetPropertyValueList() {
            ICollection values = TypeConverter.GetStandardValues(this);
            if (values != null) {
                object[] valueArray = new object[values.Count];
                values.CopyTo(valueArray, 0);
                return valueArray;
            }
            return new object[0];
        }

        public override int GetHashCode() {
            // These can be null, so workaround giving hashcode = 0 for null objects.
            object label = this.PropertyLabel;
            object type = this.PropertyType;
            UInt32 h1 = (UInt32)((label == null)  ? 0 : label.GetHashCode());
            UInt32 h2 = (UInt32)((type == null)   ? 0 : type.GetHashCode());
            UInt32 h3 = (UInt32)GetType().GetHashCode();

            return(int)(h1 ^ ((h2 << 13) | (h2 >> 19)) ^ ((h3 << 26) | (h3 >> 6)));
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.GetFlagSet"]/*' />
        /// <devdoc>
        /// Checks if a given flag is set
        /// </devdoc>
        protected virtual bool GetFlagSet(int flag) {
            return((flag & Flags) != 0);
        }

        protected Font GetFont(bool boldFont) {
            if (boldFont)
                return GridEntryHost.GetBoldFont();
            else
                return GridEntryHost.GetBaseFont();
        }

        protected IntPtr GetHfont(bool boldFont) {
            if (boldFont)
                return GridEntryHost.GetBoldHfont();
            else
                return GridEntryHost.GetBaseHfont();
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.GetService"]/*' />
        /// <devdoc>
        ///      Retrieves the requested service.  This may
        ///      return null if the requested service is not
        ///      available.
        /// </devdoc>
        public virtual object GetService(Type serviceType) {
        
            if (serviceType == typeof(GridItem)) {
                return (GridItem)this;
            }
            
            if (parentPE != null) {
                return parentPE.GetService(serviceType);
            }
            return null;
        }

        internal virtual bool NonParentEquals(object obj) {
            if (obj == this) return true;
            if (obj == null) return false;
            if (!(obj is GridEntry)) return false;
            GridEntry pe = (GridEntry)obj;

            return pe.PropertyLabel.Equals(this.PropertyLabel) &&
                   pe.PropertyType.Equals(this.PropertyType) && pe.PropertyDepth == this.PropertyDepth; 
        }


        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.PaintLabel"]/*' />
        /// <devdoc>
        /// Paints the label portion of this GridEntry into the given Graphics object.  This
        /// is called by the GridEntry host (the PropertyGridView) when this GridEntry is
        /// to be painted.
        /// </devdoc>
        public virtual void PaintLabel(System.Drawing.Graphics g, Rectangle rect, Rectangle clipRect, bool selected, bool paintFullLabel) {
            PropertyGridView gridHost = this.GridEntryHost;
            Debug.Assert(gridHost != null, "No propEntryHost");
            string strLabel = this.PropertyLabel;
            
            int borderWidth = gridHost.GetOutlineIconSize()+OUTLINE_ICON_PADDING;

            // fill the background if necessary
            Brush bkBrush = selected ? gridHost.GetSelectedItemWithFocusBackBrush(g) : this.GetBackgroundBrush(g);
            // if we don't have focus, paint with the line color
            if (selected && !hasFocus) {
                bkBrush = gridHost.GetLineBrush(g);
            }

            bool fBold = ((this.Flags & GridEntry.FLAG_LABEL_BOLD) != 0);
            Font font = GetFont(fBold);

            int labelWidth = GetLabelTextWidth(strLabel, g, font);

            int neededWidth = paintFullLabel ? labelWidth : 0;
            int stringX = rect.X + this.PropertyLabelIndent;
            Brush blank = bkBrush;

            if (paintFullLabel && (neededWidth >= (rect.Width-(stringX+2)))) {
                int totalWidth = stringX + neededWidth + PropertyGridView.GDIPLUS_SPACE; // 5 = extra needed to ensure text draws completely and isn't clipped.
#if PBRS_PAINT_DEBUG
                blank = Brushes.Green;
#endif

                // blank out the space we're going to use
                g.FillRectangle(blank, borderWidth-1, rect.Y, totalWidth-borderWidth+3, rect.Height);

                // draw an end line
                Pen linePen = new Pen(gridHost.GetLineColor());
                g.DrawLine(linePen, totalWidth, rect.Y, totalWidth, rect.Height);
                linePen.Dispose();

                // set the new width that we can draw into
                rect.Width = totalWidth - rect.X;
            }
            else { // Normal case -- no pseudo-tooltip for the label

#if PBRS_PAINT_DEBUG
                blank = Brushes.Red;
#endif
                // Debug.WriteLine(rect.X.ToString() +" "+ rect.Y.ToString() +" "+ rect.Width.ToString() +" "+ rect.Height.ToString());
                g.FillRectangle(blank, rect.X, rect.Y, rect.Width, rect.Height);
            }
            
            // draw the border stripe on the left
            Brush stripeBrush = gridHost.GetLineBrush(g);
            g.FillRectangle(stripeBrush, rect.X, rect.Y, borderWidth, rect.Height);
      
            if (selected && hasFocus) {
                g.FillRectangle(gridHost.GetSelectedItemWithFocusBackBrush(g), stringX, rect.Y, rect.Width - stringX - 1, rect.Height);
            }
            
            int maxSpace = Math.Min(rect.Width-stringX-1, labelWidth + PropertyGridView.GDIPLUS_SPACE);
            Rectangle textRect = new Rectangle(stringX, rect.Y + 1, maxSpace, rect.Height - 1);
            

            if (!Rectangle.Intersect(textRect, clipRect).IsEmpty)  {
                Region oldClip = g.Clip;
                g.SetClip(textRect);
                                               
                //We need to Invert color only if in Highcontrast mode, targeting 4.7.1 and above, Gridcategory and not a developer override. This is required to achieve required contrast ratio.
                var shouldInvertForHC = colorInversionNeededInHC && (fBold || (selected && !hasFocus));

                // Do actual drawing
                // A brush is needed if using GDI+ only (UseCompatibleTextRendering); if using GDI, only the color is needed.
                Color textColor = selected && hasFocus ? gridHost.GetSelectedItemWithFocusForeColor() : shouldInvertForHC ? InvertColor(ownerGrid.LineColor) : g.GetNearestColor(this.LabelTextColor);
             
                if( this.ownerGrid.UseCompatibleTextRendering ) {
                    using( Brush textBrush = new SolidBrush(textColor)){
                        StringFormat stringFormat = new StringFormat(StringFormatFlags.NoWrap);
                        stringFormat.Trimming = StringTrimming.None;
                        g.DrawString(strLabel, font, textBrush, textRect, stringFormat);
                    }
                }
                else{
                    TextRenderer.DrawText( g, strLabel, font, textRect, textColor, PropertyGrid.MeasureTextHelper.GetTextRendererFlags() );
                }
    #if PBRS_PAINT_DEBUG
                textRect.Width --;
                textRect.Height--;
                g.DrawRectangle(new Pen(Color.Blue), textRect);
    #endif
                g.SetClip(oldClip, CombineMode.Replace);
                oldClip.Dispose();   // clip is actually copied out.

                if (maxSpace <= labelWidth) {
                    this.labelTipPoint = new Point(stringX+2, rect.Y+1);
                }
                else {
                    this.labelTipPoint = InvalidPoint;
                }
            }

            rect.Y -= 1;
            rect.Height += 2;

            PaintOutline(g, rect);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.PaintOutline"]/*' />
        /// <devdoc>
        /// Paints the outline portion of this GridEntry into the given Graphics object.  This
        /// is called by the GridEntry host (the PropertyGridView) when this GridEntry is
        /// to be painted.
        /// </devdoc>
        public virtual void PaintOutline(System.Drawing.Graphics g, Rectangle r) {
            // draw tree-view glyphs as triangles on Vista and Windows afterword
            // when Vistual style is enabled
            if (GridEntryHost.IsExplorerTreeSupported) {

                // size of Explorer Tree style glyph (triangle) is different from +/- glyph,
                // so when we change the visual style (such as changing Windows theme),
                // we need to recaculate outlineRect 
                if(!lastPaintWithExplorerStyle) {
                    outlineRect = Rectangle.Empty;
                    lastPaintWithExplorerStyle = true;
                }
                PaintOutlineWithExplorerTreeStyle(g, r, (GridEntryHost!=null) ? this.GridEntryHost.HandleInternal: IntPtr.Zero);
            }
            // draw tree-view glyphs as +/-
            else {

                // size of Explorer Tree style glyph (triangle) is different from +/- glyph,
                // so when we change the visual style (such as changing Windows theme),
                // we need to recaculate outlineRect 
                if (lastPaintWithExplorerStyle) {
                    outlineRect = Rectangle.Empty;
                    lastPaintWithExplorerStyle = false;
                }

                PaintOutlineWithClassicStyle(g, r);
            }
        }

        private void PaintOutlineWithExplorerTreeStyle(System.Drawing.Graphics g, Rectangle r, IntPtr handle) {
            if (this.Expandable) {
                bool fExpanded = this.InternalExpanded;
                Rectangle outline = this.OutlineRect;

                // make sure we're in our bounds
                outline = Rectangle.Intersect(r, outline);
                if (outline.IsEmpty) return;

                VisualStyleElement element = null;
                if (fExpanded)
                    element = VisualStyleElement.ExplorerTreeView.Glyph.Opened;
                else
                    element = VisualStyleElement.ExplorerTreeView.Glyph.Closed;
                
                // Invert color if it is not overriden by developer.
                if (colorInversionNeededInHC) {
                    Color textColor = InvertColor(ownerGrid.LineColor);
                    if (g != null) {
                        Brush b = new SolidBrush(textColor);
                        g.FillRectangle(b, outline);
                        b.Dispose();
                    }
                }               

                VisualStyleRenderer explorerTreeRenderer = new VisualStyleRenderer(element);
                explorerTreeRenderer.DrawBackground(g, outline, handle);
            }
        }

        private void PaintOutlineWithClassicStyle(System.Drawing.Graphics g, Rectangle r) {
            // draw outline box.
            if (this.Expandable) {
                bool fExpanded = this.InternalExpanded;
                Rectangle outline = this.OutlineRect;

                // make sure we're in our bounds
                outline = Rectangle.Intersect(r, outline);
                if (outline.IsEmpty) return;

                // draw border area box
                Brush b = this.GetBackgroundBrush(g);
                Pen p;
                Color penColor = GridEntryHost.GetTextColor();

                // inverting text color to back ground to get required contrast ratio
                if (colorInversionNeededInHC) {
                    penColor = InvertColor(ownerGrid.LineColor);
                }
                else { 
                    // Filling rectangle as it was in all cases where we do not invert colors.
                    g.FillRectangle(b, outline); 
                }
                

                if (penColor.IsSystemColor) {
                    p = SystemPens.FromSystemColor(penColor);
                }
                else {
                    p = new Pen(penColor);
                }

                g.DrawRectangle(p, outline.X, outline.Y, outline.Width - 1, outline.Height - 1);

                // draw horizontal line for +/-
                int indent = 2;
                g.DrawLine(p, outline.X + indent,outline.Y + outline.Height / 2,
                           outline.X + outline.Width - indent - 1,outline.Y + outline.Height/2);

                // draw vertical line to make a +
                if (!fExpanded) {
                    g.DrawLine(p, outline.X + outline.Width/2, outline.Y + indent,
                               outline.X + outline.Width/2, outline.Y + outline.Height - indent - 1);
                }

                if (!penColor.IsSystemColor) {
                    p.Dispose();
                }
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.PaintValue"]/*' />
        /// <devdoc>
        /// Paints the value portion of this GridEntry into the given Graphics object.  This
        /// is called by the GridEntry host (the PropertyGridView) when this GridEntry is
        /// to be painted.
        /// </devdoc>
        public virtual void PaintValue(object val, System.Drawing.Graphics g, Rectangle rect, Rectangle clipRect, PaintValueFlags paintFlags) { 
            PropertyGridView gridHost = this.GridEntryHost;
            Debug.Assert(gridHost != null, "No propEntryHost");
            int cPaint = 0;

            Color textColor = gridHost.GetTextColor();
            if (this.ShouldRenderReadOnly) {
                textColor = GridEntryHost.GrayTextColor;
            }
            
            string strValue;
            
            if ((paintFlags & PaintValueFlags.FetchValue) != PaintValueFlags.None) {
               if (cacheItems != null && cacheItems.useValueString) {
                  strValue = cacheItems.lastValueString;
                  val = cacheItems.lastValue;
               }
               else {
                  val = this.PropertyValue;
                  strValue = GetPropertyTextValue(val);
                  if (cacheItems == null) {
                     cacheItems = new CacheItems();
                  }
                  cacheItems.lastValueString = strValue;
                  cacheItems.useValueString = true;
                  cacheItems.lastValueTextWidth = -1;
                  cacheItems.lastValueFont = null;
                  cacheItems.lastValue = val;
               }
            }
            else {
               strValue = GetPropertyTextValue(val);                  
            }

            // paint out the main rect using the appropriate brush
            Brush bkBrush = this.GetBackgroundBrush(g);
            Debug.Assert(bkBrush != null, "We didn't find a good background brush for PaintValue");

            if ((paintFlags & PaintValueFlags.DrawSelected) != PaintValueFlags.None) {
                bkBrush = gridHost.GetSelectedItemWithFocusBackBrush(g);
                textColor = gridHost.GetSelectedItemWithFocusForeColor();
            }

            Brush blank = bkBrush;
#if PBRS_PAINT_DEBUG
            blank = Brushes.Yellow;
#endif
            //g.FillRectangle(blank, rect.X-1, rect.Y, rect.Width+1, rect.Height);
            g.FillRectangle(blank, clipRect);
            
            if (IsCustomPaint) {
                cPaint = gridHost.GetValuePaintIndent();
                Rectangle rectPaint = new Rectangle(rect.X + 1, rect.Y + 1, gridHost.GetValuePaintWidth(), gridHost.GetGridEntryHeight() - 2);
                
                if (!Rectangle.Intersect(rectPaint, clipRect).IsEmpty) {
                   UITypeEditor uie = UITypeEditor;
                   if (uie != null) {
                       uie.PaintValue(new PaintValueEventArgs(this, val, g, rectPaint));
                   }
   
                   // paint a border around the area
                   rectPaint.Width --;
                   rectPaint.Height--;
                   g.DrawRectangle(SystemPens.WindowText, rectPaint);
                }
            }

            rect.X += cPaint + gridHost.GetValueStringIndent();
            rect.Width -= cPaint + 2 * gridHost.GetValueStringIndent();

            // bold the property if we need to persist it (e.g. it's not the default value)
            bool valueModified = ((paintFlags & PaintValueFlags.CheckShouldSerialize) != PaintValueFlags.None) && ShouldSerializePropertyValue();
            
            // If we have text to paint, paint it
            if (strValue != null && strValue.Length > 0) {
                
                Font f = GetFont(valueModified);

                if (strValue.Length > maximumLengthOfPropertyString)
                {
                    strValue = strValue.Substring(0, maximumLengthOfPropertyString);
                }
                int textWidth = GetValueTextWidth(strValue, g, f);
                bool doToolTip = false;
                
                // To check if text contains multiple lines
                //
                if (textWidth >= rect.Width ||  GetMultipleLines(strValue)) 
                     doToolTip = true;
                               
                if (Rectangle.Intersect(rect, clipRect).IsEmpty) {
                     return;
                }
                
                // Do actual drawing
                
                //strValue = ReplaceCRLF(strValue);

                // bump the text down 2 pixels and over 1 pixel.
                if ((paintFlags & PaintValueFlags.PaintInPlace) != PaintValueFlags.None) { 
                    rect.Offset(1, 2);
                }
                else {
                    // only go down one pixel when we're painting in the listbox
                    rect.Offset(1, 1);
                } 

                Matrix m = g.Transform;
                IntPtr hdc = g.GetHdc();
                IntNativeMethods.RECT lpRect = IntNativeMethods.RECT.FromXYWH(rect.X + (int)m.OffsetX + 2, rect.Y + (int)m.OffsetY - 1, rect.Width - 4, rect.Height);
                IntPtr hfont = GetHfont(valueModified);
                
                int oldTextColor = 0;
                int oldBkColor = 0;

                Color bkColor = ((paintFlags & PaintValueFlags.DrawSelected) != PaintValueFlags.None) ? GridEntryHost.GetSelectedItemWithFocusBackColor() : GridEntryHost.BackColor;
                
                try {
                    oldTextColor = SafeNativeMethods.SetTextColor(new HandleRef(g, hdc), SafeNativeMethods.RGBToCOLORREF(textColor.ToArgb()));
                    oldBkColor = SafeNativeMethods.SetBkColor(new HandleRef(g, hdc), SafeNativeMethods.RGBToCOLORREF(bkColor.ToArgb()));
                    hfont = SafeNativeMethods.SelectObject(new HandleRef(g, hdc), new HandleRef(null, hfont));
                    int format = IntNativeMethods.DT_EDITCONTROL | IntNativeMethods.DT_EXPANDTABS | IntNativeMethods.DT_NOCLIP | IntNativeMethods.DT_SINGLELINE | IntNativeMethods.DT_NOPREFIX;
                    if (gridHost.DrawValuesRightToLeft) {
                        format |= IntNativeMethods.DT_RIGHT | IntNativeMethods.DT_RTLREADING;
                    }

                    // For password mode, Replace the string value either with * or a bullet, depending on the OS platform
                    if (ShouldRenderPassword) {

                        if (passwordReplaceChar == (char)0) {
                            if (Environment.OSVersion.Version.Major > 4) {
                                passwordReplaceChar = (char)0x25CF; // Bullet is 2022, but edit box uses round circle 25CF
                            }
                            else {
                                passwordReplaceChar = '*';
                            }
                        }

                        strValue = new string(passwordReplaceChar, strValue.Length);
                    }

                    IntUnsafeNativeMethods.DrawText(new HandleRef(g, hdc), strValue, ref lpRect, format);
                }
                finally {
                    SafeNativeMethods.SetTextColor(new HandleRef(g, hdc), oldTextColor);
                    SafeNativeMethods.SetBkColor(new HandleRef(g, hdc), oldBkColor);
                    hfont = SafeNativeMethods.SelectObject(new HandleRef(g, hdc), new HandleRef(null, hfont));
                    g.ReleaseHdcInternal(hdc);
                }
                
                
                #if PBRS_PAINT_DEBUG
                    rect.Width --;
                    rect.Height--;
                    g.DrawRectangle(new Pen(Color.Purple), rect);
               #endif
               
                if (doToolTip) {
                    this.ValueToolTipLocation = new Point(rect.X+2, rect.Y-1);
                }
                else {
                     this.ValueToolTipLocation = InvalidPoint;
                }
            }

            return;
        }

        public virtual bool OnComponentChanging() {
            if (ComponentChangeService != null) {
                try {
                    ComponentChangeService.OnComponentChanging(GetValueOwner(), PropertyDescriptor);
                }
                catch (CheckoutException coEx) {
                    if (coEx == CheckoutException.Canceled) {
                        return false;
                    }
                    throw coEx;
                }
            }
            return true;
        }

        public virtual void OnComponentChanged() {
            if (ComponentChangeService != null) {
                ComponentChangeService.OnComponentChanged(GetValueOwner(), PropertyDescriptor, null, null);
            }
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.OnLabelClick"]/*' />
        /// <devdoc>
        /// Called when the label portion of this GridEntry is clicked.
        /// Default implmentation fired the event to any listeners, so be sure
        /// to call base.OnLabelClick(e) if this is overrideen.
        /// </devdoc>
        protected virtual void OnLabelClick(EventArgs e) {
            RaiseEvent(EVENT_LABEL_CLICK, e);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.OnLabelDoubleClick"]/*' />
        /// <devdoc>
        /// Called when the label portion of this GridEntry is double-clicked.
        /// Default implmentation fired the event to any listeners, so be sure
        /// to call base.OnLabelDoubleClick(e) if this is overrideen.
        /// </devdoc>
        protected virtual void         OnLabelDoubleClick(EventArgs e) {
            RaiseEvent(EVENT_LABEL_DBLCLICK, e);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.OnMouseClick"]/*' />
        /// <devdoc>
        /// Called when the GridEntry is clicked.
        /// </devdoc>
        public virtual bool OnMouseClick(int x, int y, int count, MouseButtons button) {
            // where are we at?
            PropertyGridView gridHost = this.GridEntryHost;
            Debug.Assert(gridHost != null, "No prop entry host!");

            // make sure it's the left button
            if ((button & MouseButtons.Left) != MouseButtons.Left) {
                return false;
            }


            int labelWidth  = gridHost.GetLabelWidth();

            // are we in the label?
            if (x >= 0 && x <= labelWidth) {

                // are we on the outline?
                if (Expandable) {
                    Rectangle outlineRect = OutlineRect;
                    if (outlineRect.Contains(x, y)) {
                        if (count % 2 == 0) {
                            OnOutlineDoubleClick(EventArgs.Empty);
                        }
                        else {
                            OnOutlineClick(EventArgs.Empty);
                        }
                        return true;
                    }
                }

                if (count % 2 == 0) {
                    OnLabelDoubleClick(EventArgs.Empty);
                }
                else {
                    OnLabelClick(EventArgs.Empty);
                }
                return true;
            }

            // are we in the value?
            labelWidth += gridHost.GetSplitterWidth();
            if (x >= labelWidth  && x <= labelWidth + gridHost.GetValueWidth()) {
                if (count % 2 == 0) {
                    OnValueDoubleClick(EventArgs.Empty);
                }
                else {
                    OnValueClick(EventArgs.Empty);
                }
                return true;
            }
            return false;
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.OnOutlineClick"]/*' />
        /// <devdoc>
        /// Called when the outline icon portion of this GridEntry is clicked.
        /// Default implmentation fired the event to any listeners, so be sure
        /// to call base.OnOutlineClick(e) if this is overrideen.
        /// </devdoc>
        protected virtual void OnOutlineClick(EventArgs e) {
            RaiseEvent(EVENT_OUTLINE_CLICK, e);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.OnOutlineDoubleClick"]/*' />
        /// <devdoc>
        /// Called when the outline icon portion of this GridEntry is double-clicked.
        /// Default implmentation fired the event to any listeners, so be sure
        /// to call base.OnOutlineDoubleClick(e) if this is overrideen.
        /// </devdoc>
        protected virtual void OnOutlineDoubleClick(EventArgs e) {
            RaiseEvent(EVENT_OUTLINE_DBLCLICK, e);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.OnRecreateChildren"]/*' />
        /// <devdoc>
        /// Called when RecreateChildren is called.
        /// Default implmentation fired the event to any listeners, so be sure
        /// to call base.OnOutlineDoubleClick(e) if this is overrideen.
        /// </devdoc>
        protected virtual void OnRecreateChildren(GridEntryRecreateChildrenEventArgs e) {
            Delegate handler = GetEventHandler(EVENT_RECREATE_CHILDREN);
            if (handler != null) ((GridEntryRecreateChildrenEventHandler)handler)(this, e);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.OnValueClick"]/*' />
        /// <devdoc>
        /// Called when the value portion of this GridEntry is clicked.
        /// Default implmentation fired the event to any listeners, so be sure
        /// to call base.OnValueClick(e) if this is overrideen.
        /// </devdoc>
        protected virtual void OnValueClick(EventArgs e) {
            RaiseEvent(EVENT_VALUE_CLICK, e);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.OnValueDoubleClick"]/*' />
        /// <devdoc>
        /// Called when the value portion of this GridEntry is clicked.
        /// Default implmentation fired the event to any listeners, so be sure
        /// to call base.OnValueDoubleClick(e) if this is overrideen.
        /// </devdoc>
        protected virtual void OnValueDoubleClick(EventArgs e) {
            RaiseEvent(EVENT_VALUE_DBLCLICK, e);
        }


        
        internal bool OnValueReturnKey() {
            return NotifyValue(NOTIFY_RETURN);
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.SetFlag"]/*' />
        /// <devdoc>
        /// Sets the specified flag
        /// </devdoc>
        protected virtual void SetFlag(int flag, bool fVal) {
            SetFlag(flag, (fVal ? flag : 0));
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.SetFlag1"]/*' />
        /// <devdoc>
        /// Sets the default child of this entry, given a valid value mask.
        /// </devdoc>
        protected virtual void SetFlag(int flag_valid, int flag, bool fVal) {
            SetFlag(flag_valid | flag,
                    flag_valid | (fVal ? flag : 0));
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.SetFlag2"]/*' />
        /// <devdoc>
        /// Sets the value of a flag
        /// </devdoc>
        protected virtual void SetFlag(int flag, int val) {
            Flags = (Flags & ~(flag)) | val;
        }
        
        public override bool Select() {
            if (Disposed) {
                return false;
            }
            
            try {
                GridEntryHost.SelectedGridEntry = this;
                return true;
            }
            catch {
            }
            return false;
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.ShouldSerializePropertyValue"]/*' />
        /// <devdoc>
        /// Checks if this value should be persisited.
        /// </devdoc>
        internal virtual bool ShouldSerializePropertyValue() {
            
            if (cacheItems != null) {
                if (cacheItems.useShouldSerialize) {
                     return cacheItems.lastShouldSerialize;
                }
                else {
                    cacheItems.lastShouldSerialize = NotifyValue(NOTIFY_SHOULD_PERSIST);
                    cacheItems.useShouldSerialize = true;
                }
            }
            else {
               cacheItems = new CacheItems();
               cacheItems.lastShouldSerialize = NotifyValue(NOTIFY_SHOULD_PERSIST);
               cacheItems.useShouldSerialize = true;
            }
            return cacheItems.lastShouldSerialize;
        }

        private PropertyDescriptor[] SortParenProperties(PropertyDescriptor[] props) {

            PropertyDescriptor[] newProps = null;
            int newPos = 0;


            // first scan the list and move any parentesized properties to the front.
            for (int i = 0; i < props.Length; i++) {
                if (((ParenthesizePropertyNameAttribute)props[i].Attributes[typeof(ParenthesizePropertyNameAttribute)]).NeedParenthesis) {
                    if (newProps == null) {
                        newProps = new PropertyDescriptor[props.Length];
                    }
                    newProps[newPos++] = props[i];
                    props[i] = null;
                }
            }


            // second pass, copy any that didn't have the parens.
            if (newPos > 0) {
                for (int i = 0; i < props.Length; i++) {
                    if (props[i] != null) {
                        newProps[newPos++] = props[i];
                    }
                } 
                props = newProps;
            }
            return props;
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.NotifyValueGivenParent"]/*' />
        /// <devdoc>
        /// Sends a notify message to this GridEntry, and returns the success result
        /// </devdoc>
        internal virtual bool NotifyValueGivenParent(object obj, int type) {
            return false;
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.NotifyChildValue"]/*' />
        /// <devdoc>
        /// Sends a notify message to the child GridEntry, and returns the success result
        /// </devdoc>
        internal virtual bool NotifyChildValue(GridEntry pe, int type) {
            
            return pe.NotifyValueGivenParent(pe.GetValueOwner(),type);
        }

        internal virtual bool NotifyValue(int type) {
            if (parentPE == null) {
                return true;
            }
            else {
                return parentPE.NotifyChildValue(this, type);
            }
        }

        internal void RecreateChildren() {
            RecreateChildren(-1);
        }
        
        internal void RecreateChildren(int oldCount) {
        
            // cause the flags to be rebuilt as well...
            bool wasExpanded = this.InternalExpanded || oldCount > 0;
            
            if (oldCount == -1) {
                oldCount = this.VisibleChildCount;
            }
            
            ResetState();
            if (oldCount == 0) {
                return;
            }
            
            foreach(GridEntry child in ChildCollection) {
                child.RecreateChildren();
            }
            
            DisposeChildren();
            this.InternalExpanded = wasExpanded;
            OnRecreateChildren(new GridEntryRecreateChildrenEventArgs(oldCount, VisibleChildCount));
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.Refresh"]/*' />
        /// <devdoc>
        /// Refresh the current GridEntry's value and it's children
        /// </devdoc>
        public virtual void Refresh() {

            Type type = this.PropertyType;
            if (type != null && type.IsArray) {
                CreateChildren(true);
            }

            if (this.childCollection != null) {
            
                // check to see if the value has changed.
                //
                if (this.InternalExpanded && cacheItems != null && cacheItems.lastValue != null && cacheItems.lastValue != this.PropertyValue) {
                    ClearCachedValues();
                    RecreateChildren();
                    return;
                }
                else if (this.InternalExpanded) {
                    // otherwise just do a refresh.
                    IEnumerator childEnum = childCollection.GetEnumerator();
                    while (childEnum.MoveNext()) {
                        object o = childEnum.Current;
                        Debug.Assert(o != null, "Collection contains a null element.  But how? Garbage collector hole?  GDI+ corrupting memory?");
                        GridEntry e = (GridEntry) o;
                        e.Refresh();
                    }
                }
                else {
                    DisposeChildren();
                }
            }

            ClearCachedValues();
        }

        public virtual void         RemoveOnLabelClick(EventHandler h) {
            RemoveEventHandler(EVENT_LABEL_CLICK, h);
        }
        public virtual void         RemoveOnLabelDoubleClick(EventHandler h) {
            RemoveEventHandler(EVENT_LABEL_DBLCLICK, h);
        }

        public virtual void         RemoveOnValueClick(EventHandler h) {
            RemoveEventHandler(EVENT_VALUE_CLICK, h);
        }

        public virtual void         RemoveOnValueDoubleClick(EventHandler h) {
            RemoveEventHandler(EVENT_VALUE_DBLCLICK, h);
        }

        public virtual void         RemoveOnOutlineClick(EventHandler h) {
            RemoveEventHandler(EVENT_OUTLINE_CLICK, h);
        }

        public virtual void         RemoveOnOutlineDoubleClick(EventHandler h) {
            RemoveEventHandler(EVENT_OUTLINE_DBLCLICK, h);
        }

        public virtual void         RemoveOnRecreateChildren(GridEntryRecreateChildrenEventHandler h) {
            RemoveEventHandler(EVENT_RECREATE_CHILDREN, h);
        }

        /*
        private string ReplaceCRLF(string str) {
            str = str.Replace('\r', (char)1);
            str = str.Replace('\n', (char)1);
            return str;
        }
        */

        protected void ResetState() {
            this.Flags = 0;
            ClearCachedValues();
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.SetPropertyTextValue"]/*' />
        /// <devdoc>
        /// Sets the value of this GridEntry from text
        /// </devdoc>
        public virtual bool SetPropertyTextValue(string str) {
            bool fChildrenPrior = (childCollection != null && childCollection.Count > 0);
            this.PropertyValue = ConvertTextToValue(str);
            CreateChildren();
            bool fChildrenAfter = (childCollection != null && childCollection.Count > 0);
            return(fChildrenPrior != fChildrenAfter);
        }

        public override string ToString() {
            return GetType().FullName + " " + this.PropertyLabel;
        }



#if !DONT_SUPPORT_ADD_EVENT_HANDLER
        private EventEntry eventList;

        protected virtual void AddEventHandler(object key, Delegate handler) {
            // Locking 'this' here is ok since this is an internal class.
            lock(this) {
                if (handler == null) return;
                for (EventEntry e = eventList; e != null; e = e.next) {
                    if (e.key == key) {
                        e.handler = Delegate.Combine(e.handler, handler);
                        return;
                    }
                }
                eventList = new EventEntry(eventList, key, handler);
            }
        }

        protected virtual void RaiseEvent(object key, EventArgs e) {
            Delegate handler = GetEventHandler(key);
            if (handler != null) ((EventHandler)handler)(this, e);
        }

        protected virtual Delegate GetEventHandler(object key) {
            // Locking 'this' here is ok since this is an internal class.
            lock(this) {
                for (EventEntry e = eventList; e != null; e = e.next) {
                    if (e.key == key) return e.handler;
                }
                return null;
            }
        }

        protected virtual void RemoveEventHandler(object key, Delegate handler) {
            // Locking this here is ok since this is an internal class.
            lock(this) {
                if (handler == null) return;
                for (EventEntry e = eventList, prev = null; e != null; prev = e, e = e.next) {
                    if (e.key == key) {
                        e.handler = Delegate.Remove(e.handler, handler);
                        if (e.handler == null) {
                            if (prev == null) {
                                eventList = e.next;
                            }
                            else {
                                prev.next = e.next;
                            }
                        }
                        return;
                    }
                }
            }
        }

        protected virtual void RemoveEventHandlers() {
            eventList = null;
        }

        /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.EventEntry"]/*' />
        /// <devdoc>
        /// </devdoc>
        private sealed class EventEntry {
            internal EventEntry next;
            internal object key;
            internal Delegate handler;

            internal EventEntry(EventEntry next, object key, Delegate handler) {
                this.next = next;
                this.key = key;
                this.handler = handler;
            }
        }
#endif

        [ComVisible(true)]
        public class GridEntryAccessibleObject : AccessibleObject {

            protected GridEntry owner = null;
            private delegate void SelectDelegate(AccessibleSelection flags);
            private int[] runtimeId = null; // Used by UIAutomation

            public GridEntryAccessibleObject(GridEntry owner) : base() {
                Debug.Assert(owner != null, "GridEntryAccessibleObject must have a valid owner GridEntry");
                this.owner = owner;
            }

            public override Rectangle Bounds {
                get {
                    return PropertyGridView.AccessibilityGetGridEntryBounds(owner);
                }
            }

            public override string DefaultAction {
                get {
                    if (!owner.Expandable) {
                        return base.DefaultAction;
                    }
                    else if (owner.Expanded) {
                        return SR.AccessibleActionCollapse;
                    }
                    else {
                        return SR.AccessibleActionExpand;
                    }
                }
            }

            public override string Description {
                get {
                    return owner.PropertyDescription;
                }
            }

            public override string Help {
                get {
                    if (AccessibilityImprovements.Level1) {
                        return owner.PropertyDescription;
                    }
                    else {
                        return base.Help;
                    }
                }
            }

            #region IAccessibleEx - patterns and properties

            internal override bool IsIAccessibleExSupported() {
                if (owner.Expandable && AccessibilityImprovements.Level1) {
                    return true;
                }
                else {
                    return false;
                }
            }

            internal override int[] RuntimeId {
                get {
                    if (runtimeId == null) {
                        // we need to provide a unique ID
                        // others are implementing this in the same manner
                        // first item is static - 0x2a
                        // second item can be anything, but it's good to supply HWND
                        // third and others are optional, but in case of GridItem we need it, to make it unique
                        // grid items are not controls, they don't have hwnd - we use hwnd of PropertyGridView

                        runtimeId = new int[3];
                        runtimeId[0] = 0x2a;
                        runtimeId[1] = (int)(long)owner.GridEntryHost.Handle;
                        runtimeId[2] = this.GetHashCode();
                    }

                    return runtimeId;
                }
            }

            internal override object GetPropertyValue(int propertyID) {
                if (propertyID == NativeMethods.UIA_NamePropertyId) {
                    return Name;
                }
                else if (propertyID == NativeMethods.UIA_ControlTypePropertyId) {
                    return NativeMethods.UIA_ButtonControlTypeId;
                }
                else if (propertyID == NativeMethods.UIA_IsExpandCollapsePatternAvailablePropertyId) {
                    return (Object)IsPatternSupported(NativeMethods.UIA_ExpandCollapsePatternId);
                }

                return null;
            }

            internal override bool IsPatternSupported(int patternId) {
                if (owner.Expandable) {
                    if (patternId == NativeMethods.UIA_ExpandCollapsePatternId) {
                        return true;
                    }
                }

                return false;
            }

            internal override void Expand() {
                if (owner.Expandable && owner.Expanded == false) {
                    owner.Expanded = true;
                }
            }

            internal override void Collapse() {
                if (owner.Expandable && owner.Expanded == true) {
                    owner.Expanded = false;
                }
            }

            internal override UnsafeNativeMethods.ExpandCollapseState ExpandCollapseState {
                get {
                    if (owner.Expandable) {
                        return owner.Expanded ? UnsafeNativeMethods.ExpandCollapseState.Expanded : UnsafeNativeMethods.ExpandCollapseState.Collapsed;
                    }
                    else {
                        return UnsafeNativeMethods.ExpandCollapseState.LeafNode;
                    }
                }
            }

            #endregion

            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void DoDefaultAction() {
                owner.OnOutlineClick(EventArgs.Empty);
            }

            public override string Name {
                get {
                    return owner.PropertyLabel;
                }
            }

            public override AccessibleObject Parent {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get {
                    return((Control)this.owner.GridEntryHost).AccessibilityObject;
                }
            }

            private PropertyGridView PropertyGridView {
                get {
                    return(PropertyGridView)((PropertyGridView.PropertyGridViewAccessibleObject)Parent).Owner;
                }
            }

            public override AccessibleRole Role {
                get {
                    if (AccessibilityImprovements.Level1)  {
                        if (owner.Expandable) {
                            return AccessibleRole.ButtonDropDownGrid;
                        }
                        else {
                            return AccessibleRole.Cell;
                        }
                    }
                    return AccessibleRole.Row;
                }
            }

            public override AccessibleStates State {
                get {
                    AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable;

                    // Determine focus
                    //
                    if (owner.Focus) {
                        state |= AccessibleStates.Focused;
                    }

                    // Determine selected
                    //
                    Debug.Assert(Parent != null, "GridEntry AO does not have a parent AO");
                    PropertyGridView.PropertyGridViewAccessibleObject parent = (PropertyGridView.PropertyGridViewAccessibleObject)Parent;
                    if (parent.GetSelected() == this) {
                        state |= AccessibleStates.Selected;
                    }
                    
                    // Determine expanded/collapsed state
                    //
                    if (owner.Expandable) {
                        if (owner.Expanded) {
                            state |= AccessibleStates.Expanded;
                        }
                        else {
                            state |= AccessibleStates.Collapsed;
                        }
                    }

                    // Determine readonly/editable state
                    //
                    if (owner.ShouldRenderReadOnly) {
                        state |= AccessibleStates.ReadOnly;
                    }

                    // Determine password state
                    //
                    if (owner.ShouldRenderPassword) {
                        state |= AccessibleStates.Protected;
                    }

                    return state;
                }
            }

            public override string Value {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get {
                    return owner.GetPropertyTextValue();
                }

                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                set {
                    owner.SetPropertyTextValue(value);
                }                
            }

            /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.GridEntryAccessibleObject.GetFocused"]/*' />
            /// <devdoc>
            ///      Returns the currently focused child, if any.
            ///      Returns this if the object itself is focused.
            /// </devdoc>
            public override AccessibleObject GetFocused() {

                if (owner.Focus) {
                    return this;
                }
                else {
                    return null;
                }
            }


            /// <include file='doc\GridEntry.uex' path='docs/doc[@for="GridEntry.GridEntryAccessibleObject.Navigate"]/*' />
            /// <devdoc>
            ///      Navigate to the next or previous grid entry.
            /// </devdoc>
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override AccessibleObject Navigate(AccessibleNavigation navdir) {

                PropertyGridView.PropertyGridViewAccessibleObject parent =
                (PropertyGridView.PropertyGridViewAccessibleObject)Parent;

                switch (navdir) {
                    case AccessibleNavigation.Down:
                    case AccessibleNavigation.Right:
                    case AccessibleNavigation.Next:
                        return parent.Next(this.owner);

                    case AccessibleNavigation.Up:
                    case AccessibleNavigation.Left:
                    case AccessibleNavigation.Previous:
                        return parent.Previous(this.owner);

                    case AccessibleNavigation.FirstChild:
                    case AccessibleNavigation.LastChild:
                        // Fall through and return null,
                        // as this object has no children.
                        break;
                }

                return null;

            }

            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void Select(AccessibleSelection flags) {

                // make sure we're on the right thread.
                //
                if (PropertyGridView.InvokeRequired) {
                    PropertyGridView.Invoke(new SelectDelegate(this.Select), new object[]{flags});
                    return;
                }

                // Focus the PropertyGridView window
                //
                if ( (flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus) {
                    bool focused = PropertyGridView.FocusInternal();
                }

                // Select the grid entry
                //
                if ( (flags & AccessibleSelection.TakeSelection) == AccessibleSelection.TakeSelection) {
                    PropertyGridView.AccessibilitySelect(this.owner);
                }
            }

        }

        public class DisplayNameSortComparer : IComparer {
            public int Compare(object left, object right) {
		// review: (Microsoft) Is CurrentCulture correct here?  This was already reviewed as invariant...
                return String.Compare(((PropertyDescriptor)left).DisplayName, ((PropertyDescriptor)right).DisplayName, true, CultureInfo.CurrentCulture);
            }
        }
    }

    internal class AttributeTypeSorter : IComparer{

        private static IDictionary typeIds;

        private static string GetTypeIdString(Attribute a) {

            string result;
            object typeId = a.TypeId;


            if (typeId == null) {
                Debug.Fail("Attribute '" + a.GetType().FullName + "' does not have a typeid."); 
                return "";
            }

            if (typeIds == null) {
                typeIds = new Hashtable();
                result = null;
            }
            else {
                result = typeIds[typeId] as string;
            }

            if (result == null) {
                result = typeId.ToString();
                typeIds[typeId] = result;
            }
            return result;
        }

        public int Compare(object obj1, object obj2) {
            Attribute a1 = obj1 as Attribute;
            Attribute a2 = obj2 as Attribute;
        
            if (a1 == null && a2 == null) {
                return 0;
            }
            else if (a1 == null) {
                return -1;
            }
            else if (a2 == null) {
                return 1;
            }
            return String.Compare(AttributeTypeSorter.GetTypeIdString(a1), AttributeTypeSorter.GetTypeIdString(a2), false, CultureInfo.InvariantCulture);
        }
    }
    
    internal delegate void GridEntryRecreateChildrenEventHandler(object sender, GridEntryRecreateChildrenEventArgs rce);
    
    internal class GridEntryRecreateChildrenEventArgs : EventArgs {
        public readonly int OldChildCount;
        public readonly int NewChildCount;
        
        public GridEntryRecreateChildrenEventArgs(int oldCount, int newCount) {
            this.OldChildCount = oldCount;
            this.NewChildCount = newCount;
        }
    }

}
