// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.ComponentModel.Design
{
    public partial class ComponentDesigner : System.ComponentModel.Design.IComponentInitializer, System.ComponentModel.Design.IDesigner, System.ComponentModel.Design.IDesignerFilter, System.ComponentModel.Design.ITreeDesigner, System.IDisposable
    {
        System.Collections.ICollection ITreeDesigner.Children { get { throw null; } }
        IDesigner ITreeDesigner.Parent { get { throw null; } }
        public ComponentDesigner() { }
        public virtual System.ComponentModel.Design.DesignerActionListCollection ActionLists { get { throw null; } }
        public virtual System.Collections.ICollection AssociatedComponents { get { throw null; } }
        public System.ComponentModel.IComponent Component { get { throw null; } }
        protected virtual System.ComponentModel.InheritanceAttribute InheritanceAttribute { get { throw null; } }
        protected bool Inherited { get { throw null; } }
        protected virtual System.ComponentModel.IComponent ParentComponent { get { throw null; } }
        protected System.ComponentModel.Design.ComponentDesigner.ShadowPropertyCollection ShadowProperties { get { throw null; } }
        public virtual System.ComponentModel.Design.DesignerVerbCollection Verbs { get { throw null; } }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        public virtual void DoDefaultAction() { }
        ~ComponentDesigner() { }
        protected virtual object GetService(System.Type serviceType) { throw null; }
        public virtual void Initialize(System.ComponentModel.IComponent component) { }
        public virtual void InitializeExistingComponent(System.Collections.IDictionary defaultValues) { }
        public virtual void InitializeNewComponent(System.Collections.IDictionary defaultValues) { }
        [System.ObsoleteAttribute("This method has been deprecated. Use InitializeExistingComponent instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public virtual void InitializeNonDefault() { }
        protected System.ComponentModel.InheritanceAttribute InvokeGetInheritanceAttribute(System.ComponentModel.Design.ComponentDesigner toInvoke) { throw null; }
        [System.ObsoleteAttribute("This method has been deprecated. Use InitializeNewComponent instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public virtual void OnSetComponentDefaults() { }
        protected virtual void PostFilterAttributes(System.Collections.IDictionary attributes) { }
        protected virtual void PostFilterEvents(System.Collections.IDictionary events) { }
        protected virtual void PostFilterProperties(System.Collections.IDictionary properties) { }
        protected virtual void PreFilterAttributes(System.Collections.IDictionary attributes) { }
        protected virtual void PreFilterEvents(System.Collections.IDictionary events) { }
        protected virtual void PreFilterProperties(System.Collections.IDictionary properties) { }
        protected void RaiseComponentChanged(System.ComponentModel.MemberDescriptor member, object oldValue, object newValue) { }
        protected void RaiseComponentChanging(System.ComponentModel.MemberDescriptor member) { }
        void System.ComponentModel.Design.IDesignerFilter.PostFilterAttributes(System.Collections.IDictionary attributes) { }
        void System.ComponentModel.Design.IDesignerFilter.PostFilterEvents(System.Collections.IDictionary events) { }
        void System.ComponentModel.Design.IDesignerFilter.PostFilterProperties(System.Collections.IDictionary properties) { }
        void System.ComponentModel.Design.IDesignerFilter.PreFilterAttributes(System.Collections.IDictionary attributes) { }
        void System.ComponentModel.Design.IDesignerFilter.PreFilterEvents(System.Collections.IDictionary events) { }
        void System.ComponentModel.Design.IDesignerFilter.PreFilterProperties(System.Collections.IDictionary properties) { }
        protected sealed partial class ShadowPropertyCollection
        {
            internal ShadowPropertyCollection(ComponentDesigner designer){}

            public object this[string propertyName] { get { throw null; } set { } }
            public bool Contains(string propertyName) { throw null; }
        }
    }
    public abstract partial class DesignerActionItem
    {
        public DesignerActionItem(string displayName, string category, string description) { }
        public bool AllowAssociate { get { throw null; } set { } }
        public virtual string Category { get { throw null; } }
        public virtual string Description { get { throw null; } }
        public virtual string DisplayName { get { throw null; } }
        public System.Collections.IDictionary Properties { get { throw null; } }
        public bool ShowInSourceView { get { throw null; } set { } }
    }
    public partial class DesignerActionItemCollection : System.Collections.CollectionBase
    {
        public DesignerActionItemCollection() { }
        public System.ComponentModel.Design.DesignerActionItem this[int index] { get { throw null; } set { } }
        public int Add(System.ComponentModel.Design.DesignerActionItem value) { throw null; }
        public bool Contains(System.ComponentModel.Design.DesignerActionItem value) { throw null; }
        public void CopyTo(System.ComponentModel.Design.DesignerActionItem[] array, int index) { }
        public int IndexOf(System.ComponentModel.Design.DesignerActionItem value) { throw null; }
        public void Insert(int index, System.ComponentModel.Design.DesignerActionItem value) { }
        public void Remove(System.ComponentModel.Design.DesignerActionItem value) { }
    }
    public partial class DesignerActionList
    {
        public DesignerActionList(System.ComponentModel.IComponent component) { }
        public virtual bool AutoShow { get { throw null; } set { } }
        public System.ComponentModel.IComponent Component { get { throw null; } }
        public object GetService(System.Type serviceType) { throw null; }
        public virtual System.ComponentModel.Design.DesignerActionItemCollection GetSortedActionItems() { throw null; }
    }
    public partial class DesignerActionListCollection : System.Collections.CollectionBase
    {
        public DesignerActionListCollection() { }
        public DesignerActionListCollection(System.ComponentModel.Design.DesignerActionList[] value) { }
        public System.ComponentModel.Design.DesignerActionList this[int index] { get { throw null; } set { } }
        public int Add(System.ComponentModel.Design.DesignerActionList value) { throw null; }
        public void AddRange(System.ComponentModel.Design.DesignerActionListCollection value) { }
        public void AddRange(System.ComponentModel.Design.DesignerActionList[] value) { }
        public bool Contains(System.ComponentModel.Design.DesignerActionList value) { throw null; }
        public void CopyTo(System.ComponentModel.Design.DesignerActionList[] array, int index) { }
        public int IndexOf(System.ComponentModel.Design.DesignerActionList value) { throw null; }
        public void Insert(int index, System.ComponentModel.Design.DesignerActionList value) { }
        protected override void OnClear() { }
        protected override void OnInsert(int index, object value) { }
        protected override void OnRemove(int index, object value) { }
        protected override void OnSet(int index, object oldValue, object newValue) { }
        protected override void OnValidate(object value) { }
        public void Remove(System.ComponentModel.Design.DesignerActionList value) { }
    }
}
namespace System.ComponentModel.Design.Serialization
{
    [CLSCompliant(false)]
    public partial class CodeDomSerializer : System.ComponentModel.Design.Serialization.CodeDomSerializerBase
    {
        public CodeDomSerializer() { }
        public virtual object Deserialize(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object codeObject) { throw null; }
        protected object DeserializeStatementToInstance(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, System.CodeDom.CodeStatement statement) { throw null; }
        public virtual string GetTargetComponentName(System.CodeDom.CodeStatement statement, System.CodeDom.CodeExpression expression, System.Type targetType) { throw null; }
        public virtual object Serialize(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object value) { throw null; }
        public virtual object SerializeAbsolute(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object value) { throw null; }
        public virtual System.CodeDom.CodeStatementCollection SerializeMember(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object owningObject, System.ComponentModel.MemberDescriptor member) { throw null; }
        public virtual System.CodeDom.CodeStatementCollection SerializeMemberAbsolute(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object owningObject, System.ComponentModel.MemberDescriptor member) { throw null; }
        [System.ObsoleteAttribute("This method has been deprecated. Use SerializeToExpression or GetExpression instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        protected System.CodeDom.CodeExpression SerializeToReferenceExpression(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object value) { throw null; }
    }
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    [CLSCompliant(false)]
    public abstract partial class CodeDomSerializerBase
    {
        internal CodeDomSerializerBase() { }
        protected virtual object DeserializeInstance(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, System.Type type, object[] parameters, string name, bool addToContainer) { throw null; }
        protected void DeserializePropertiesFromResources(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object value, System.Attribute[] filter) { }
        protected void DeserializeStatement(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, System.CodeDom.CodeStatement statement) { }
        protected static System.ComponentModel.AttributeCollection GetAttributesFromTypeHelper(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, System.Type type) { throw null; }
        protected static System.ComponentModel.AttributeCollection GetAttributesHelper(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object instance) { throw null; }
        protected static System.ComponentModel.EventDescriptorCollection GetEventsHelper(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object instance, System.Attribute[] attributes) { throw null; }
        protected System.CodeDom.CodeExpression GetExpression(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object value) { throw null; }
        protected static System.ComponentModel.PropertyDescriptorCollection GetPropertiesHelper(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object instance, System.Attribute[] attributes) { throw null; }
        protected static System.Type GetReflectionTypeFromTypeHelper(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, System.Type type) { throw null; }
        protected static System.Type GetReflectionTypeHelper(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object instance) { throw null; }
        protected System.ComponentModel.Design.Serialization.CodeDomSerializer GetSerializer(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object value) { throw null; }
        protected System.ComponentModel.Design.Serialization.CodeDomSerializer GetSerializer(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, System.Type valueType) { throw null; }
        protected static System.ComponentModel.TypeDescriptionProvider GetTargetFrameworkProvider(System.IServiceProvider provider, object instance) { throw null; }
        protected string GetUniqueName(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object value) { throw null; }
        protected bool IsSerialized(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object value) { throw null; }
        protected bool IsSerialized(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object value, bool honorPreset) { throw null; }
        protected System.CodeDom.CodeExpression SerializeCreationExpression(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object value, out bool isComplete) { throw null; }
        protected void SerializeEvent(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, System.CodeDom.CodeStatementCollection statements, object value, System.ComponentModel.EventDescriptor descriptor) { }
        protected void SerializeEvents(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, System.CodeDom.CodeStatementCollection statements, object value, params System.Attribute[] filter) { }
        protected void SerializeProperties(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, System.CodeDom.CodeStatementCollection statements, object value, System.Attribute[] filter) { }
        protected void SerializePropertiesToResources(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, System.CodeDom.CodeStatementCollection statements, object value, System.Attribute[] filter) { }
        protected void SerializeProperty(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, System.CodeDom.CodeStatementCollection statements, object value, System.ComponentModel.PropertyDescriptor propertyToSerialize) { }
        protected void SerializeResource(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, string resourceName, object value) { }
        protected void SerializeResourceInvariant(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, string resourceName, object value) { }
        protected System.CodeDom.CodeExpression SerializeToExpression(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object value) { throw null; }
        protected System.CodeDom.CodeExpression SerializeToResourceExpression(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object value) { throw null; }
        protected System.CodeDom.CodeExpression SerializeToResourceExpression(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object value, bool ensureInvariant) { throw null; }
        protected void SetExpression(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object value, System.CodeDom.CodeExpression expression) { }
        protected void SetExpression(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object value, System.CodeDom.CodeExpression expression, bool isPreset) { }
    }
}
namespace System.Windows.Forms.Design
{
    public partial class AxImporter
    {
        public AxImporter(Options options) { }
        public partial interface IReferenceResolver
        {
            string ResolveActiveXReference(System.Runtime.InteropServices.UCOMITypeLib typeLib);
            string ResolveComReference(System.Reflection.AssemblyName name);
            string ResolveComReference(System.Runtime.InteropServices.UCOMITypeLib typeLib);
            string ResolveManagedReference(string assemName);
        }
        public sealed partial class Options
        {
            public bool delaySign;
            public bool genSources;
            public bool ignoreRegisteredOcx;
            public string keyContainer;
            public string keyFile;
            public System.Reflection.StrongNameKeyPair keyPair;
            public bool msBuildErrors;
            public bool noLogo;
            public string outputDirectory;
            public string outputName;
            public bool overwriteRCW;
            public byte[] publicKey;
            public System.Windows.Forms.Design.AxImporter.IReferenceResolver references;
            public bool silentMode;
            public bool verboseMode;
            public Options() { }
        }
    }

    public partial class ComponentDocumentDesigner : System.ComponentModel.Design.ComponentDesigner, System.ComponentModel.Design.IDesigner, System.ComponentModel.Design.IRootDesigner, System.ComponentModel.Design.ITypeDescriptorFilterService, System.Drawing.Design.IToolboxUser, System.IDisposable
    {
        System.ComponentModel.Design.ViewTechnology[] System.ComponentModel.Design.IRootDesigner.SupportedTechnologies { get { throw null; } }
        bool System.ComponentModel.Design.ITypeDescriptorFilterService.FilterAttributes(System.ComponentModel.IComponent component, Collections.IDictionary attributes) {  throw null; }
        bool System.ComponentModel.Design.ITypeDescriptorFilterService.FilterEvents(System.ComponentModel.IComponent component, Collections.IDictionary events) { throw null; }
        bool System.ComponentModel.Design.ITypeDescriptorFilterService.FilterProperties(System.ComponentModel.IComponent component, Collections.IDictionary properties) { throw null; }
        void Drawing.Design.IToolboxUser.ToolPicked(Drawing.Design.ToolboxItem tool) { throw null; }
        public ComponentDocumentDesigner() { }
        public System.Windows.Forms.Control Control { get { throw null; } }
        public bool TrayAutoArrange { get { throw null; } set { } }
        public bool TrayLargeIcon { get { throw null; } set { } }
        protected override void Dispose(bool disposing) { }
        [CLSCompliant(false)]
        protected virtual bool GetToolSupported(System.Drawing.Design.ToolboxItem tool) { throw null; }
        public override void Initialize(System.ComponentModel.IComponent component) { }
        protected override void PreFilterProperties(System.Collections.IDictionary properties) { }
        object System.ComponentModel.Design.IRootDesigner.GetView(System.ComponentModel.Design.ViewTechnology technology) { throw null; }
        bool System.Drawing.Design.IToolboxUser.GetToolSupported(System.Drawing.Design.ToolboxItem tool) { throw null; }
    }
    [System.ComponentModel.DesignTimeVisibleAttribute(false)]
    [System.ComponentModel.ProvidePropertyAttribute("Location", typeof(System.ComponentModel.IComponent))]
    [System.ComponentModel.ProvidePropertyAttribute("TrayLocation", typeof(System.ComponentModel.IComponent))]
    [System.ComponentModel.ToolboxItemAttribute(false)]
    public partial class ComponentTray : System.Windows.Forms.ScrollableControl, System.ComponentModel.IExtenderProvider
    {
        bool System.ComponentModel.IExtenderProvider.CanExtend(object component) { throw null; }
        public ComponentTray(System.ComponentModel.Design.IDesigner mainDesigner, System.IServiceProvider serviceProvider) { }
        public bool AutoArrange { get { throw null; } set { } }
        public int ComponentCount { get { throw null; } }
        public bool ShowLargeIcons { get { throw null; } set { } }
        public virtual void AddComponent(System.ComponentModel.IComponent component) { }
        [System.CLSCompliantAttribute(false)]
        protected virtual bool CanCreateComponentFromTool(System.Drawing.Design.ToolboxItem tool) { throw null; }
        protected virtual bool CanDisplayComponent(System.ComponentModel.IComponent component) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public void CreateComponentFromTool(System.Drawing.Design.ToolboxItem tool) { }
        protected void DisplayError(System.Exception e) { }
        protected override void Dispose(bool disposing) { }
        System.Windows.Forms.Design.SelectionRules GetComponentRules(object component) { throw null; }
        [System.ComponentModel.BrowsableAttribute(false)]
        [System.ComponentModel.CategoryAttribute("Layout")]
        [System.ComponentModel.DesignerSerializationVisibilityAttribute((System.ComponentModel.DesignerSerializationVisibility)(0))]
        [System.ComponentModel.DesignOnlyAttribute(true)]
        [System.ComponentModel.LocalizableAttribute(false)]
        public System.Drawing.Point GetLocation(System.ComponentModel.IComponent receiver) { throw null; }
        public System.ComponentModel.IComponent GetNextComponent(System.ComponentModel.IComponent component, bool forward) { throw null; }
        protected override object GetService(System.Type serviceType) { throw null; }
        [System.ComponentModel.BrowsableAttribute(false)]
        [System.ComponentModel.CategoryAttribute("Layout")]
        [System.ComponentModel.DesignOnlyAttribute(true)]
        [System.ComponentModel.LocalizableAttribute(false)]
        public System.Drawing.Point GetTrayLocation(System.ComponentModel.IComponent receiver) { throw null; }
        public bool IsTrayComponent(System.ComponentModel.IComponent comp) { throw null; }
        protected override void OnDragDrop(System.Windows.Forms.DragEventArgs de) { }
        protected override void OnDragEnter(System.Windows.Forms.DragEventArgs de) { }
        protected override void OnDragLeave(System.EventArgs e) { }
        protected override void OnDragOver(System.Windows.Forms.DragEventArgs de) { }
        protected override void OnGiveFeedback(System.Windows.Forms.GiveFeedbackEventArgs gfevent) { }
        protected override void OnLayout(System.Windows.Forms.LayoutEventArgs levent) { }
        protected virtual void OnLostCapture() { }
        protected override void OnMouseDoubleClick(System.Windows.Forms.MouseEventArgs e) { }
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e) { }
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e) { }
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e) { }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs pe) { }
        protected virtual void OnSetCursor() { }
        public virtual void RemoveComponent(System.ComponentModel.IComponent component) { }
        public void SetLocation(System.ComponentModel.IComponent receiver, System.Drawing.Point location) { }
        public void SetTrayLocation(System.ComponentModel.IComponent receiver, System.Drawing.Point location) { }
        protected override void WndProc(ref System.Windows.Forms.Message m) { }
    }
    [CLSCompliant(false)]
    public partial class ControlDesigner : System.ComponentModel.Design.ComponentDesigner
    {
        protected System.Windows.Forms.AccessibleObject accessibilityObj;
        public ControlDesigner() { }
        public virtual System.Windows.Forms.AccessibleObject AccessibilityObject { get { throw null; } }
        public override System.Collections.ICollection AssociatedComponents { get { throw null; } }
        public bool AutoResizeHandles { get { throw null; } set { } }
        protected System.Windows.Forms.Design.Behavior.BehaviorService BehaviorService { get { throw null; } }
        public virtual System.Windows.Forms.Control Control { get { throw null; } }
        protected virtual bool EnableDragRect { get { throw null; } }
        protected override System.ComponentModel.InheritanceAttribute InheritanceAttribute { get { throw null; } }
        protected override System.ComponentModel.IComponent ParentComponent { get { throw null; } }
        public virtual bool ParticipatesWithSnapLines { get { throw null; } }
        public virtual System.Windows.Forms.Design.SelectionRules SelectionRules { get { throw null; } }
        public virtual System.Collections.IList SnapLines { get { throw null; } }
        protected void BaseWndProc(ref System.Windows.Forms.Message m) { }
        public virtual bool CanBeParentedTo(System.ComponentModel.Design.IDesigner parentDesigner) { throw null; }
        protected void DefWndProc(ref System.Windows.Forms.Message m) { }
        protected void DisplayError(System.Exception e) { }
        protected override void Dispose(bool disposing) { }
        protected bool EnableDesignMode(System.Windows.Forms.Control child, string name) { throw null; }
        protected void EnableDragDrop(bool value) { }
        protected virtual System.Windows.Forms.Design.Behavior.ControlBodyGlyph GetControlGlyph(System.Windows.Forms.Design.Behavior.GlyphSelectionType selectionType) { throw null; }
        public virtual System.Windows.Forms.Design.Behavior.GlyphCollection GetGlyphs(System.Windows.Forms.Design.Behavior.GlyphSelectionType selectionType) { throw null; }
        protected virtual bool GetHitTest(System.Drawing.Point point) { throw null; }
        protected void HookChildControls(System.Windows.Forms.Control firstChild) { }
        public override void Initialize(System.ComponentModel.IComponent component) { }
        public override void InitializeExistingComponent(System.Collections.IDictionary defaultValues) { }
        public override void InitializeNewComponent(System.Collections.IDictionary defaultValues) { }
        public virtual System.Windows.Forms.Design.ControlDesigner InternalControlDesigner(int internalControlIndex) { throw null; }
        public virtual int NumberOfInternalControlDesigners() { throw null; }
        protected virtual void OnContextMenu(int x, int y) { }
        protected virtual void OnCreateHandle() { }
        protected virtual void OnDragComplete(System.Windows.Forms.DragEventArgs de) { }
        protected virtual void OnDragDrop(System.Windows.Forms.DragEventArgs de) { }
        protected virtual void OnDragEnter(System.Windows.Forms.DragEventArgs de) { }
        protected virtual void OnDragLeave(System.EventArgs e) { }
        protected virtual void OnDragOver(System.Windows.Forms.DragEventArgs de) { }
        protected virtual void OnGiveFeedback(System.Windows.Forms.GiveFeedbackEventArgs e) { }
        protected virtual void OnMouseDragBegin(int x, int y) { }
        protected virtual void OnMouseDragEnd(bool cancel) { }
        protected virtual void OnMouseDragMove(int x, int y) { }
        protected virtual void OnMouseEnter() { }
        protected virtual void OnMouseHover() { }
        protected virtual void OnMouseLeave() { }
        protected virtual void OnPaintAdornments(System.Windows.Forms.PaintEventArgs pe) { }
        [System.ObsoleteAttribute("This method has been deprecated. Use InitializeNewComponent instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public override void OnSetComponentDefaults() { }
        protected virtual void OnSetCursor() { }
        protected override void PreFilterProperties(System.Collections.IDictionary properties) { }
        protected void UnhookChildControls(System.Windows.Forms.Control firstChild) { }
        protected virtual void WndProc(ref System.Windows.Forms.Message m) { }
        public partial class ControlDesignerAccessibleObject : System.Windows.Forms.AccessibleObject
        {
            public ControlDesignerAccessibleObject(System.Windows.Forms.Design.ControlDesigner designer, System.Windows.Forms.Control control) { }
            public override System.Drawing.Rectangle Bounds { get { throw null; } }
            public override string DefaultAction { get { throw null; } }
            public override string Description { get { throw null; } }
            public override string Name { get { throw null; } }
            public override System.Windows.Forms.AccessibleObject Parent { get { throw null; } }
            public override System.Windows.Forms.AccessibleRole Role { get { throw null; } }
            public override System.Windows.Forms.AccessibleStates State { get { throw null; } }
            public override string Value { get { throw null; } }
            public override System.Windows.Forms.AccessibleObject GetChild(int index) { throw null; }
            public override int GetChildCount() { throw null; }
            public override System.Windows.Forms.AccessibleObject GetFocused() { throw null; }
            public override System.Windows.Forms.AccessibleObject GetSelected() { throw null; }
            public override System.Windows.Forms.AccessibleObject HitTest(int x, int y) { throw null; }
        }
    }
    public partial class DesignerOptions
    {
        public DesignerOptions() { }
        [System.ComponentModel.BrowsableAttribute(false)]
        public virtual bool EnableInSituEditing { get { throw null; } set { } }
        public virtual System.Drawing.Size GridSize { get { throw null; } set { } }
        public virtual bool ObjectBoundSmartTagAutoShow { get { throw null; } set { } }
        public virtual bool ShowGrid { get { throw null; } set { } }
        public virtual bool SnapToGrid { get { throw null; } set { } }
        public virtual bool UseOptimizedCodeGeneration { get { throw null; } set { } }
        public virtual bool UseSmartTags { get { throw null; } set { } }
        public virtual bool UseSnapLines { get { throw null; } set { } }
    }

    [System.ComponentModel.ToolboxItemFilterAttribute("System.Windows.Forms")]
    [CLSCompliant(false)]
    public partial class DocumentDesigner : System.Windows.Forms.Design.ScrollableControlDesigner, System.ComponentModel.Design.IDesigner, System.ComponentModel.Design.IRootDesigner, System.Drawing.Design.IToolboxUser, System.IDisposable
    {
        object System.ComponentModel.Design.IRootDesigner.GetView(System.ComponentModel.Design.ViewTechnology technology) { throw null;}

        System.ComponentModel.Design.ViewTechnology[] System.ComponentModel.Design.IRootDesigner.SupportedTechnologies { get { throw null; } }
        protected System.Windows.Forms.Design.IMenuEditorService menuEditorService;
        public DocumentDesigner() { }
        public override System.Windows.Forms.Design.SelectionRules SelectionRules { get { throw null; } }
        protected override void Dispose(bool disposing) { }
        protected virtual void EnsureMenuEditorService(System.ComponentModel.IComponent c) { }
        public override System.Windows.Forms.Design.Behavior.GlyphCollection GetGlyphs(System.Windows.Forms.Design.Behavior.GlyphSelectionType selectionType) { throw null; }
        [System.CLSCompliantAttribute(false)]
        protected virtual bool GetToolSupported(System.Drawing.Design.ToolboxItem tool) { throw null; }
        public override void Initialize(System.ComponentModel.IComponent component) { }
        protected override void OnContextMenu(int x, int y) { }
        protected override void OnCreateHandle() { }
        protected override void PreFilterProperties(System.Collections.IDictionary properties) { }
        bool System.Drawing.Design.IToolboxUser.GetToolSupported(System.Drawing.Design.ToolboxItem tool) { throw null; }
        void System.Drawing.Design.IToolboxUser.ToolPicked(System.Drawing.Design.ToolboxItem tool) { }
        [System.CLSCompliantAttribute(false)]
        protected virtual void ToolPicked(System.Drawing.Design.ToolboxItem tool) { }
        protected override void WndProc(ref System.Windows.Forms.Message m) { }
    }
    public sealed partial class EventHandlerService
    {
        public EventHandlerService(System.Windows.Forms.Control focusWnd) { }
        public System.Windows.Forms.Control FocusWindow { get { throw null; } }
        public event System.EventHandler EventHandlerChanged { add { } remove { } }
        public object GetHandler(System.Type handlerType) { throw null; }
        public void PopHandler(object handler) { }
        public void PushHandler(object handler) { }
    }

    [CLSCompliant(false)]
    public partial class ImageListCodeDomSerializer : System.ComponentModel.Design.Serialization.CodeDomSerializer
    {
        public ImageListCodeDomSerializer() { }
        public override object Deserialize(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object codeObject) { throw null; }
        public override object Serialize(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager, object value) { throw null; }
    }
    public partial interface IMenuEditorService
    {
        System.Windows.Forms.Menu GetMenu();
        bool IsActive();
        bool MessageFilter(ref System.Windows.Forms.Message m);
        void SetMenu(System.Windows.Forms.Menu menu);
        void SetSelection(System.Windows.Forms.MenuItem item);
    }
    public sealed partial class MenuCommands : System.ComponentModel.Design.StandardCommands
    {
        public static readonly System.ComponentModel.Design.CommandID ComponentTrayMenu;
        public static readonly System.ComponentModel.Design.CommandID ContainerMenu;
        public static readonly System.ComponentModel.Design.CommandID DesignerProperties;
        public static readonly System.ComponentModel.Design.CommandID EditLabel;
        public static readonly System.ComponentModel.Design.CommandID KeyCancel;
        public static readonly System.ComponentModel.Design.CommandID KeyDefaultAction;
        public static readonly System.ComponentModel.Design.CommandID KeyEnd;
        public static readonly System.ComponentModel.Design.CommandID KeyHome;
        public static readonly System.ComponentModel.Design.CommandID KeyInvokeSmartTag;
        public static readonly System.ComponentModel.Design.CommandID KeyMoveDown;
        public static readonly System.ComponentModel.Design.CommandID KeyMoveLeft;
        public static readonly System.ComponentModel.Design.CommandID KeyMoveRight;
        public static readonly System.ComponentModel.Design.CommandID KeyMoveUp;
        public static readonly System.ComponentModel.Design.CommandID KeyNudgeDown;
        public static readonly System.ComponentModel.Design.CommandID KeyNudgeHeightDecrease;
        public static readonly System.ComponentModel.Design.CommandID KeyNudgeHeightIncrease;
        public static readonly System.ComponentModel.Design.CommandID KeyNudgeLeft;
        public static readonly System.ComponentModel.Design.CommandID KeyNudgeRight;
        public static readonly System.ComponentModel.Design.CommandID KeyNudgeUp;
        public static readonly System.ComponentModel.Design.CommandID KeyNudgeWidthDecrease;
        public static readonly System.ComponentModel.Design.CommandID KeyNudgeWidthIncrease;
        public static readonly System.ComponentModel.Design.CommandID KeyReverseCancel;
        public static readonly System.ComponentModel.Design.CommandID KeySelectNext;
        public static readonly System.ComponentModel.Design.CommandID KeySelectPrevious;
        public static readonly System.ComponentModel.Design.CommandID KeyShiftEnd;
        public static readonly System.ComponentModel.Design.CommandID KeyShiftHome;
        public static readonly System.ComponentModel.Design.CommandID KeySizeHeightDecrease;
        public static readonly System.ComponentModel.Design.CommandID KeySizeHeightIncrease;
        public static readonly System.ComponentModel.Design.CommandID KeySizeWidthDecrease;
        public static readonly System.ComponentModel.Design.CommandID KeySizeWidthIncrease;
        public static readonly System.ComponentModel.Design.CommandID KeyTabOrderSelect;
        public static readonly System.ComponentModel.Design.CommandID SelectionMenu;
        public static readonly System.ComponentModel.Design.CommandID SetStatusRectangle;
        public static readonly System.ComponentModel.Design.CommandID SetStatusText;
        public static readonly System.ComponentModel.Design.CommandID TraySelectionMenu;
        public MenuCommands() { }
    }
    [CLSCompliant(false)]
    public partial class ParentControlDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        public ParentControlDesigner() { }
        protected virtual bool AllowControlLasso { get { throw null; } }
        protected virtual bool AllowGenericDragBox { get { throw null; } }
        protected internal virtual bool AllowSetChildIndexOnDrop { get { throw null; } }
        protected virtual System.Drawing.Point DefaultControlLocation { get { throw null; } }
        protected virtual bool DrawGrid { get { throw null; } set { } }
        protected override bool EnableDragRect { get { throw null; } }
        protected System.Drawing.Size GridSize { get { throw null; } set { } }
        [System.CLSCompliantAttribute(false)]
        protected System.Drawing.Design.ToolboxItem MouseDragTool { get { throw null; } }
        public override System.Collections.IList SnapLines { get { throw null; } }
        protected void AddPaddingSnapLines(ref System.Collections.ArrayList snapLines) { }
        protected internal virtual bool CanAddComponent(System.ComponentModel.IComponent component) { throw null; }
        public virtual bool CanParent(System.Windows.Forms.Control control) { throw null; }
        public virtual bool CanParent(System.Windows.Forms.Design.ControlDesigner controlDesigner) { throw null; }
        [System.CLSCompliantAttribute(false)]
        protected void CreateTool(System.Drawing.Design.ToolboxItem tool) { }
        [System.CLSCompliantAttribute(false)]
        protected void CreateTool(System.Drawing.Design.ToolboxItem tool, System.Drawing.Point location) { }
        [System.CLSCompliantAttribute(false)]
        protected void CreateTool(System.Drawing.Design.ToolboxItem tool, System.Drawing.Rectangle bounds) { }
        [System.CLSCompliantAttribute(false)]
        protected virtual System.ComponentModel.IComponent[] CreateToolCore(System.Drawing.Design.ToolboxItem tool, int x, int y, int width, int height, bool hasLocation, bool hasSize) { throw null; }
        protected override void Dispose(bool disposing) { }
        protected System.Windows.Forms.Control GetControl(object component) { throw null; }
        protected override System.Windows.Forms.Design.Behavior.ControlBodyGlyph GetControlGlyph(System.Windows.Forms.Design.Behavior.GlyphSelectionType selectionType) { throw null; }
        public override System.Windows.Forms.Design.Behavior.GlyphCollection GetGlyphs(System.Windows.Forms.Design.Behavior.GlyphSelectionType selectionType) { throw null; }
        protected virtual System.Windows.Forms.Control GetParentForComponent(System.ComponentModel.IComponent component) { throw null; }
        protected System.Drawing.Rectangle GetUpdatedRect(System.Drawing.Rectangle originalRect, System.Drawing.Rectangle dragRect, bool updateSize) { throw null; }
        public override void Initialize(System.ComponentModel.IComponent component) { }
        public override void InitializeNewComponent(System.Collections.IDictionary defaultValues) { }
        [System.CLSCompliantAttribute(false)]
        protected static void InvokeCreateTool(System.Windows.Forms.Design.ParentControlDesigner toInvoke, System.Drawing.Design.ToolboxItem tool) { }
        protected override void OnDragComplete(System.Windows.Forms.DragEventArgs de) { }
        protected override void OnDragDrop(System.Windows.Forms.DragEventArgs de) { }
        protected override void OnDragEnter(System.Windows.Forms.DragEventArgs de) { }
        protected override void OnDragLeave(System.EventArgs e) { }
        protected override void OnDragOver(System.Windows.Forms.DragEventArgs de) { }
        protected override void OnMouseDragBegin(int x, int y) { }
        protected override void OnMouseDragEnd(bool cancel) { }
        protected override void OnMouseDragMove(int x, int y) { }
        protected override void OnPaintAdornments(System.Windows.Forms.PaintEventArgs pe) { }
        protected override void OnSetCursor() { }
        protected override void PreFilterProperties(System.Collections.IDictionary properties) { }
    }
    [CLSCompliant(false)]
    public partial class ScrollableControlDesigner : System.Windows.Forms.Design.ParentControlDesigner
    {
        public ScrollableControlDesigner() { }
        protected override bool GetHitTest(System.Drawing.Point pt) { throw null; }
        protected override void WndProc(ref System.Windows.Forms.Message m) { }
    }
    [System.FlagsAttribute]
    public enum SelectionRules
    {
        AllSizeable = 15,
        BottomSizeable = 2,
        LeftSizeable = 4,
        Locked = -2147483648,
        Moveable = 268435456,
        None = 0,
        RightSizeable = 8,
        TopSizeable = 1,
        Visible = 1073741824,
    }

    public partial class WindowsFormsDesignerOptionService : System.ComponentModel.Design.DesignerOptionService
    {
        public WindowsFormsDesignerOptionService() { }
        public virtual System.Windows.Forms.Design.DesignerOptions CompatibilityOptions { get { throw null; } }
        protected override void PopulateOptionCollection(System.ComponentModel.Design.DesignerOptionService.DesignerOptionCollection options) { }
    }
}

namespace System.Runtime.InteropServices
{
    public partial interface UCOMITypeLib
    {
        void GetDocumentation(int index, out string strName, out string strDocString, out int dwHelpContext, out string strHelpFile);
        void GetLibAttr(out System.IntPtr ppTLibAttr);
        int GetTypeInfoCount();
        void GetTypeInfoType(int index, out System.Runtime.InteropServices.ComTypes.TYPEKIND pTKind);
        bool IsName(string szNameBuf, int lHashVal);
        void ReleaseTLibAttr(System.IntPtr pTLibAttr);
    }
}

namespace System.Windows.Forms.Design.Behavior
{
    [CLSCompliant(false)]
    public sealed partial class Adorner
    {
        public Adorner() { }
        public System.Windows.Forms.Design.Behavior.BehaviorService BehaviorService { get { throw null; } set { } }
        public bool Enabled { get { throw null; } set { } }
        public System.Windows.Forms.Design.Behavior.GlyphCollection Glyphs { get { throw null; } }
        public void Invalidate() { }
        public void Invalidate(System.Drawing.Rectangle rectangle) { }
        public void Invalidate(System.Drawing.Region region) { }
    }
    [CLSCompliant(false)]
    public abstract partial class Behavior
    {
        protected Behavior() { }
        protected Behavior(bool callParentBehavior, System.Windows.Forms.Design.Behavior.BehaviorService behaviorService) { }
        public virtual System.Windows.Forms.Cursor Cursor { get { throw null; } }
        public virtual bool DisableAllCommands { get { throw null; } }
        public virtual System.ComponentModel.Design.MenuCommand FindCommand(System.ComponentModel.Design.CommandID commandId) { throw null; }
        public virtual void OnDragDrop(System.Windows.Forms.Design.Behavior.Glyph g, System.Windows.Forms.DragEventArgs e) { }
        public virtual void OnDragEnter(System.Windows.Forms.Design.Behavior.Glyph g, System.Windows.Forms.DragEventArgs e) { }
        public virtual void OnDragLeave(System.Windows.Forms.Design.Behavior.Glyph g, System.EventArgs e) { }
        public virtual void OnDragOver(System.Windows.Forms.Design.Behavior.Glyph g, System.Windows.Forms.DragEventArgs e) { }
        public virtual void OnGiveFeedback(System.Windows.Forms.Design.Behavior.Glyph g, System.Windows.Forms.GiveFeedbackEventArgs e) { }
        public virtual void OnLoseCapture(System.Windows.Forms.Design.Behavior.Glyph g, System.EventArgs e) { }
        public virtual bool OnMouseDoubleClick(System.Windows.Forms.Design.Behavior.Glyph g, System.Windows.Forms.MouseButtons button, System.Drawing.Point mouseLoc) { throw null; }
        public virtual bool OnMouseDown(System.Windows.Forms.Design.Behavior.Glyph g, System.Windows.Forms.MouseButtons button, System.Drawing.Point mouseLoc) { throw null; }
        public virtual bool OnMouseEnter(System.Windows.Forms.Design.Behavior.Glyph g) { throw null; }
        public virtual bool OnMouseHover(System.Windows.Forms.Design.Behavior.Glyph g, System.Drawing.Point mouseLoc) { throw null; }
        public virtual bool OnMouseLeave(System.Windows.Forms.Design.Behavior.Glyph g) { throw null; }
        public virtual bool OnMouseMove(System.Windows.Forms.Design.Behavior.Glyph g, System.Windows.Forms.MouseButtons button, System.Drawing.Point mouseLoc) { throw null; }
        public virtual bool OnMouseUp(System.Windows.Forms.Design.Behavior.Glyph g, System.Windows.Forms.MouseButtons button) { throw null; }
        public virtual void OnQueryContinueDrag(System.Windows.Forms.Design.Behavior.Glyph g, System.Windows.Forms.QueryContinueDragEventArgs e) { }
    }
    public partial class BehaviorDragDropEventArgs : System.EventArgs
    {
        public BehaviorDragDropEventArgs(System.Collections.ICollection dragComponents) { }
        public System.Collections.ICollection DragComponents { get { throw null; } }
    }
    public delegate void BehaviorDragDropEventHandler(object sender, System.Windows.Forms.Design.Behavior.BehaviorDragDropEventArgs e);
    [CLSCompliant(false)]
    public sealed partial class BehaviorService : System.IDisposable
    {
        internal BehaviorService(System.IServiceProvider serviceProvider, Control windowFrame){}
        public System.Windows.Forms.Design.Behavior.BehaviorServiceAdornerCollection Adorners { get { throw null; } }
        public System.Drawing.Graphics AdornerWindowGraphics { get { throw null; } }
        public System.Windows.Forms.Design.Behavior.Behavior CurrentBehavior { get { throw null; } }
        public event System.Windows.Forms.Design.Behavior.BehaviorDragDropEventHandler BeginDrag { add { } remove { } }
        public event System.Windows.Forms.Design.Behavior.BehaviorDragDropEventHandler EndDrag { add { } remove { } }
        public event System.EventHandler Synchronize { add { } remove { } }
        public System.Drawing.Point AdornerWindowPointToScreen(System.Drawing.Point p) { throw null; }
        public System.Drawing.Point AdornerWindowToScreen() { throw null; }
        public System.Drawing.Rectangle ControlRectInAdornerWindow(System.Windows.Forms.Control c) { throw null; }
        public System.Drawing.Point ControlToAdornerWindow(System.Windows.Forms.Control c) { throw null; }
        public void Dispose() { }
        public System.Windows.Forms.Design.Behavior.Behavior GetNextBehavior(System.Windows.Forms.Design.Behavior.Behavior behavior) { throw null; }
        public void Invalidate() { }
        public void Invalidate(System.Drawing.Rectangle rect) { }
        public void Invalidate(System.Drawing.Region r) { }
        public System.Drawing.Point MapAdornerWindowPoint(System.IntPtr handle, System.Drawing.Point pt) { throw null; }
        public System.Windows.Forms.Design.Behavior.Behavior PopBehavior(System.Windows.Forms.Design.Behavior.Behavior behavior) { throw null; }
        public void PushBehavior(System.Windows.Forms.Design.Behavior.Behavior behavior) { }
        public void PushCaptureBehavior(System.Windows.Forms.Design.Behavior.Behavior behavior) { }
        public System.Drawing.Point ScreenToAdornerWindow(System.Drawing.Point p) { throw null; }
        public void SyncSelection() { }
    }
    [CLSCompliant(false)]
    public sealed partial class BehaviorServiceAdornerCollection : System.Collections.CollectionBase
    {
        public BehaviorServiceAdornerCollection(System.Windows.Forms.Design.Behavior.Adorner[] value) { }
        public BehaviorServiceAdornerCollection(System.Windows.Forms.Design.Behavior.BehaviorService behaviorService) { }
        public BehaviorServiceAdornerCollection(System.Windows.Forms.Design.Behavior.BehaviorServiceAdornerCollection value) { }
        public System.Windows.Forms.Design.Behavior.Adorner this[int index] { get { throw null; } set { } }
        public int Add(System.Windows.Forms.Design.Behavior.Adorner value) { throw null; }
        public void AddRange(System.Windows.Forms.Design.Behavior.Adorner[] value) { }
        public void AddRange(System.Windows.Forms.Design.Behavior.BehaviorServiceAdornerCollection value) { }
        public bool Contains(System.Windows.Forms.Design.Behavior.Adorner value) { throw null; }
        public void CopyTo(System.Windows.Forms.Design.Behavior.Adorner[] array, int index) { }
        public new System.Windows.Forms.Design.Behavior.BehaviorServiceAdornerCollectionEnumerator GetEnumerator() { throw null; }
        public int IndexOf(System.Windows.Forms.Design.Behavior.Adorner value) { throw null; }
        public void Insert(int index, System.Windows.Forms.Design.Behavior.Adorner value) { }
        public void Remove(System.Windows.Forms.Design.Behavior.Adorner value) { }
    }
    [CLSCompliant(false)]
    public partial class BehaviorServiceAdornerCollectionEnumerator : System.Collections.IEnumerator
    {
        public BehaviorServiceAdornerCollectionEnumerator(System.Windows.Forms.Design.Behavior.BehaviorServiceAdornerCollection mappings) { }
        public System.Windows.Forms.Design.Behavior.Adorner Current { get { throw null; } }
        object System.Collections.IEnumerator.Current { get { throw null; } }
        public bool MoveNext() { throw null; }
        public void Reset() { }
    }
    [CLSCompliant(false)]
    public partial class ComponentGlyph : System.Windows.Forms.Design.Behavior.Glyph
    {
        public ComponentGlyph(System.ComponentModel.IComponent relatedComponent) : base(default(System.Windows.Forms.Design.Behavior.Behavior)) { }
        public ComponentGlyph(System.ComponentModel.IComponent relatedComponent, System.Windows.Forms.Design.Behavior.Behavior behavior) : base(default(System.Windows.Forms.Design.Behavior.Behavior)) { }
        public System.ComponentModel.IComponent RelatedComponent { get { throw null; } }
        public override System.Windows.Forms.Cursor GetHitTest(System.Drawing.Point p) { throw null; }
        public override void Paint(System.Windows.Forms.PaintEventArgs pe) { }
    }
    [CLSCompliant(false)]
    public partial class ControlBodyGlyph : System.Windows.Forms.Design.Behavior.ComponentGlyph
    {
        public ControlBodyGlyph(System.Drawing.Rectangle bounds, System.Windows.Forms.Cursor cursor, System.ComponentModel.IComponent relatedComponent, System.Windows.Forms.Design.Behavior.Behavior behavior) : base(default(System.ComponentModel.IComponent), default(System.Windows.Forms.Design.Behavior.Behavior)) { }
        public ControlBodyGlyph(System.Drawing.Rectangle bounds, System.Windows.Forms.Cursor cursor, System.ComponentModel.IComponent relatedComponent, System.Windows.Forms.Design.ControlDesigner designer) : base(default(System.ComponentModel.IComponent), default(System.Windows.Forms.Design.Behavior.Behavior)) { }
        public override System.Drawing.Rectangle Bounds { get { throw null; } }
        public override System.Windows.Forms.Cursor GetHitTest(System.Drawing.Point p) { throw null; }
    }
    [CLSCompliant(false)]
    public abstract partial class Glyph
    {
        protected Glyph(System.Windows.Forms.Design.Behavior.Behavior behavior) { }
        public virtual System.Windows.Forms.Design.Behavior.Behavior Behavior { get { throw null; } }
        public virtual System.Drawing.Rectangle Bounds { get { throw null; } }
        public abstract System.Windows.Forms.Cursor GetHitTest(System.Drawing.Point p);
        public abstract void Paint(System.Windows.Forms.PaintEventArgs pe);
        protected void SetBehavior(System.Windows.Forms.Design.Behavior.Behavior behavior) { }
    }
    [CLSCompliant(false)]
    public partial class GlyphCollection : System.Collections.CollectionBase
    {
        public GlyphCollection() { }
        public GlyphCollection(System.Windows.Forms.Design.Behavior.GlyphCollection value) { }
        public GlyphCollection(System.Windows.Forms.Design.Behavior.Glyph[] value) { }
        public System.Windows.Forms.Design.Behavior.Glyph this[int index] { get { throw null; } set { } }
        public int Add(System.Windows.Forms.Design.Behavior.Glyph value) { throw null; }
        public void AddRange(System.Windows.Forms.Design.Behavior.GlyphCollection value) { }
        public void AddRange(System.Windows.Forms.Design.Behavior.Glyph[] value) { }
        public bool Contains(System.Windows.Forms.Design.Behavior.Glyph value) { throw null; }
        public void CopyTo(System.Windows.Forms.Design.Behavior.Glyph[] array, int index) { }
        public int IndexOf(System.Windows.Forms.Design.Behavior.Glyph value) { throw null; }
        public void Insert(int index, System.Windows.Forms.Design.Behavior.Glyph value) { }
        public void Remove(System.Windows.Forms.Design.Behavior.Glyph value) { }
    }
    public enum GlyphSelectionType
    {
        NotSelected = 0,
        Selected = 1,
        SelectedPrimary = 2,
    }
    public sealed partial class SnapLine
    {
        public SnapLine(System.Windows.Forms.Design.Behavior.SnapLineType type, int offset) { }
        public SnapLine(System.Windows.Forms.Design.Behavior.SnapLineType type, int offset, string filter) { }
        public SnapLine(System.Windows.Forms.Design.Behavior.SnapLineType type, int offset, string filter, System.Windows.Forms.Design.Behavior.SnapLinePriority priority) { }
        public SnapLine(System.Windows.Forms.Design.Behavior.SnapLineType type, int offset, System.Windows.Forms.Design.Behavior.SnapLinePriority priority) { }
        public string Filter { get { throw null; } }
        public bool IsHorizontal { get { throw null; } }
        public bool IsVertical { get { throw null; } }
        public int Offset { get { throw null; } }
        public System.Windows.Forms.Design.Behavior.SnapLinePriority Priority { get { throw null; } }
        public System.Windows.Forms.Design.Behavior.SnapLineType SnapLineType { get { throw null; } }
        public void AdjustOffset(int adjustment) { }
        public static bool ShouldSnap(System.Windows.Forms.Design.Behavior.SnapLine line1, System.Windows.Forms.Design.Behavior.SnapLine line2) { throw null; }
        public override string ToString() { throw null; }
    }
    public enum SnapLinePriority
    {
        Always = 4,
        High = 3,
        Low = 1,
        Medium = 2,
    }
    public enum SnapLineType
    {
        Baseline = 6,
        Bottom = 1,
        Horizontal = 4,
        Left = 2,
        Right = 3,
        Top = 0,
        Vertical = 5,
    }
}
