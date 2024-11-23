// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace System.Drawing.Design;

/// <summary>
///  Provides a base implementation of a toolbox item.
/// </summary>
public class ToolboxItem : ISerializable
{
    private static bool s_isScalingInitialized;
    private const int ICON_DIMENSION = 16;
    private static int s_iconWidth = ICON_DIMENSION;
    private static int s_iconHeight = ICON_DIMENSION;

    private bool _locked;
    private LockableDictionary? _properties;
    private ToolboxComponentsCreatedEventHandler? _componentsCreatedEvent;
    private ToolboxComponentsCreatingEventHandler? _componentsCreatingEvent;

    /// <summary>
    ///  Initializes a new instance of the ToolboxItem class.
    /// </summary>
    public ToolboxItem()
    {
        if (!s_isScalingInitialized)
        {
            s_iconWidth = ScaleHelper.ScaleToInitialSystemDpi(ICON_DIMENSION);
            s_iconHeight = ScaleHelper.ScaleToInitialSystemDpi(ICON_DIMENSION);
            s_isScalingInitialized = true;
        }
    }

    /// <summary>
    ///  Initializes a new instance of the ToolboxItem class using the specified type.
    /// </summary>
    public ToolboxItem(Type? toolType) : this()
    {
        Initialize(toolType);
    }

    /// <summary>
    ///  The assembly name for this toolbox item. The assembly name describes the assembly
    ///  information needed to load the toolbox item's type.
    /// </summary>
    public AssemblyName? AssemblyName
    {
        get => (AssemblyName?)Properties["AssemblyName"];
        set => Properties["AssemblyName"] = value;
    }

    /// <summary>
    ///  The assembly name for this toolbox item. The assembly name describes the assembly
    ///  information needed to load the toolbox item's type.
    /// </summary>
    public AssemblyName[]? DependentAssemblies
    {
        get
        {
            AssemblyName[]? names = (AssemblyName[]?)Properties["DependentAssemblies"];
            return (AssemblyName[]?)names?.Clone();
        }
        set => Properties["DependentAssemblies"] = value?.Clone();
    }

    /// <summary>
    ///  Gets or sets the bitmap that will be used on the toolbox for this item.
    ///  Use this property on the design surface as this bitmap is scaled according to the current the DPI setting.
    /// </summary>
    public Bitmap? Bitmap
    {
        get => (Bitmap?)Properties["Bitmap"];
        set => Properties["Bitmap"] = value;
    }

    /// <summary>
    ///  Gets or sets the original bitmap that will be used on the toolbox for this item.
    ///  This bitmap should be 16x16 pixel and should be used in the Visual Studio toolbox, not on the design surface.
    /// </summary>
    public Bitmap? OriginalBitmap
    {
        get => (Bitmap?)Properties["OriginalBitmap"];
        set => Properties["OriginalBitmap"] = value;
    }

    /// <summary>
    ///  Gets or sets the company name for this <see cref="ToolboxItem"/>.
    ///  This defaults to the companyname attribute retrieved from type.Assembly, if set.
    /// </summary>
    public string? Company
    {
        get => (string?)Properties["Company"];
        set => Properties["Company"] = value;
    }

    /// <summary>
    ///  The Component Type is ".Net Component" -- unless otherwise specified by a derived toolboxitem
    /// </summary>
    public virtual string ComponentType => SR.DotNET_ComponentType;

    /// <summary>
    ///  Description is a free-form, multiline capable text description that will be displayed in the tooltip
    ///  for the toolboxItem. It defaults to the path of the assembly that contains the item, but can be overridden.
    /// </summary>
    public string? Description
    {
        get => (string?)Properties["Description"];
        set => Properties["Description"] = value;
    }

    /// <summary>
    ///  Gets or sets the display name for this <see cref="ToolboxItem"/>.
    /// </summary>
    [AllowNull]
    public string DisplayName
    {
        get => (string)Properties["DisplayName"]!;
        set => Properties["DisplayName"] = value;
    }

    /// <summary>
    ///  Gets or sets the filter for this toolbox item. The filter is a collection of
    ///  ToolboxItemFilterAttribute objects.
    /// </summary>
    [AllowNull]
    public ICollection Filter
    {
        get => (ICollection)Properties["Filter"]!;
        set => Properties["Filter"] = value;
    }

    /// <summary>
    ///  If true, it indicates that this toolbox item should not be stored in
    ///  any toolbox database when an application that is providing a toolbox
    ///  closes down. This property defaults to false.
    /// </summary>
    public bool IsTransient
    {
        get => (bool)Properties["IsTransient"]!;
        set => Properties["IsTransient"] = value;
    }

    /// <summary>
    ///  Determines if this toolbox item is locked. Once locked, a toolbox item will
    ///  not accept any changes to its properties.
    /// </summary>
    public virtual bool Locked => _locked;

    /// <summary>
    ///  The properties dictionary is a set of name/value pairs. The keys are property
    ///  names and the values are property values. This dictionary becomes read-only
    ///  after the toolbox item has been locked.
    ///  Values in the properties dictionary are validated through ValidateProperty
    ///  and default values are obtained from GetDefaultProperty.
    /// </summary>
    public IDictionary Properties => _properties ??= new LockableDictionary(this, 8 /* # of properties we have */);

    /// <summary>
    ///  Gets or sets the fully qualified name of the type this toolbox item will create.
    /// </summary>
    [AllowNull]
    public string TypeName
    {
        get => (string)Properties["TypeName"]!;
        set => Properties["TypeName"] = value;
    }

    /// <summary>
    ///  Gets the version for this toolboxitem. It defaults to AssemblyName.Version unless
    ///  overridden in a derived toolboxitem. This can be overridden to
    ///  return an empty string to suppress its display in the toolbox tooltip.
    /// </summary>
    public virtual string Version => AssemblyName?.Version?.ToString() ?? string.Empty;

    /// <summary>
    ///  Occurs when components are created.
    /// </summary>
    public event ToolboxComponentsCreatedEventHandler? ComponentsCreated
    {
        add => _componentsCreatedEvent += value;
        remove => _componentsCreatedEvent -= value;
    }

    /// <summary>
    ///  Occurs before components are created.
    /// </summary>
    public event ToolboxComponentsCreatingEventHandler? ComponentsCreating
    {
        add => _componentsCreatingEvent += value;
        remove => _componentsCreatingEvent -= value;
    }

    /// <summary>
    ///  This method checks that the toolbox item is currently unlocked (read-write) and
    ///  throws an appropriate exception if it isn't.
    /// </summary>
    protected void CheckUnlocked()
    {
        if (Locked)
        {
            throw new InvalidOperationException(SR.ToolboxItemLocked);
        }
    }

    /// <summary>
    ///  Creates objects from the type contained in this toolbox item.
    /// </summary>
    /// <returns></returns>
    public IComponent[]? CreateComponents() => CreateComponents(null);

    /// <summary>
    ///  Creates objects from the type contained in this toolbox item. If designerHost is non-null
    ///  this will also add them to the designer.
    /// </summary>
    public IComponent[]? CreateComponents(IDesignerHost? host)
    {
        OnComponentsCreating(new ToolboxComponentsCreatingEventArgs(host));
        IComponent[]? comps = CreateComponentsCore(host, new Hashtable());
        if (comps is not null && comps.Length > 0)
        {
            OnComponentsCreated(new ToolboxComponentsCreatedEventArgs(comps));
        }

        return comps;
    }

    /// <summary>
    ///  Creates objects from the type contained in this toolbox item. If designerHost is non-null
    ///  this will also add them to the designer.
    /// </summary>
    /// <returns></returns>
    public IComponent[]? CreateComponents(IDesignerHost? host, IDictionary? defaultValues)
    {
        OnComponentsCreating(new ToolboxComponentsCreatingEventArgs(host));
        IComponent[]? comps = CreateComponentsCore(host, defaultValues);
        if (comps is not null && comps.Length > 0)
        {
            OnComponentsCreated(new ToolboxComponentsCreatedEventArgs(comps));
        }

        return comps;
    }

    /// <summary>
    ///  Creates objects from the type contained in this toolbox item. If designerHost is non-null
    ///  this will also add them to the designer.
    /// </summary>
    protected virtual IComponent[]? CreateComponentsCore(IDesignerHost? host)
    {
        List<IComponent> comps = [];
        Type? createType = GetType(host, AssemblyName, TypeName, true);
        if (createType is not null)
        {
            if (host is not null)
            {
                comps.Add(host.CreateComponent(createType));
            }
            else if (typeof(IComponent).IsAssignableFrom(createType))
            {
                comps.Add((IComponent)TypeDescriptor.CreateInstance(provider: null, createType, argTypes: null, args: null)!);
            }
        }

        return [.. comps];
    }

    /// <summary>
    ///  Creates objects from the type contained in this toolbox item. If designerHost is non-null
    ///  this will also add them to the designer.
    /// </summary>
    protected virtual IComponent[]? CreateComponentsCore(IDesignerHost? host, IDictionary? defaultValues)
    {
        IComponent[]? components = CreateComponentsCore(host);

        if (host is not null && components is not null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (host.GetDesigner(components[i]) is IComponentInitializer init)
                {
                    bool removeComponent = true;

                    try
                    {
                        init.InitializeNewComponent(defaultValues);
                        removeComponent = false;
                    }
                    finally
                    {
                        if (removeComponent)
                        {
                            for (int index = 0; index < components.Length; index++)
                            {
                                host.DestroyComponent(components[index]);
                            }
                        }
                    }
                }
            }
        }

        return components;
    }

    protected virtual void Deserialize(SerializationInfo info, StreamingContext context)
    {
        // Do this in a couple of passes -- first pass, try to pull
        // out our dictionary of property names. We need to do this
        // for backwards compatibility because if we throw everything
        // into the property dictionary we'll duplicate stuff people
        // have serialized by hand.

        string[]? propertyNames = null;
        foreach (SerializationEntry entry in info)
        {
            if (entry.Name.Equals("PropertyNames"))
            {
                propertyNames = entry.Value as string[];
                break;
            }
        }

        // For backwards compat, here are the default property
        // names we use
        propertyNames ??=
        [
                "AssemblyName",
                "Bitmap",
                "DisplayName",
                "Filter",
                "IsTransient",
                "TypeName"
        ];

        foreach (SerializationEntry entry in info)
        {
            // Check to see if this name is in our
            // propertyNames array.
            foreach (string validName in propertyNames)
            {
                if (validName.Equals(entry.Name))
                {
                    Properties[entry.Name] = entry.Value;
                    break;
                }
            }
        }

        // Always do "Locked" last (otherwise we can't do the others!)
        bool isLocked = info.GetBoolean("Locked");
        if (isLocked)
        {
            Lock();
        }
    }

    public override bool Equals(object? obj)
    {
        if (this == obj)
        {
            return true;
        }

        if (obj is null)
        {
            return false;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        ToolboxItem otherItem = (ToolboxItem)obj;

        return TypeName == otherItem.TypeName &&
              AreAssemblyNamesEqual(AssemblyName, otherItem.AssemblyName) &&
              DisplayName == otherItem.DisplayName;
    }

    private static bool AreAssemblyNamesEqual(AssemblyName? name1, AssemblyName? name2)
    {
        return name1 == name2 ||
               (name1 is not null && name2 is not null && name1.FullName == name2.FullName);
    }

    public override int GetHashCode() => HashCode.Combine(TypeName, DisplayName);

    /// <summary>
    ///  Filters a property value before returning it. This allows a property to always clone values,
    ///  or to provide a default value when none exists.
    /// </summary>
    protected virtual object? FilterPropertyValue(string propertyName, object? value)
    {
        switch (propertyName)
        {
            case "AssemblyName":
                if (value is AssemblyName valueName)
                {
                    value = valueName.Clone();
                }

                break;

            case "DisplayName":
            case "TypeName":
                value ??= string.Empty;

                break;

            case "Filter":
                value ??= Array.Empty<ToolboxItemFilterAttribute>();

                break;

            case "IsTransient":
                value ??= false;

                break;
        }

        return value;
    }

    /// <summary>
    ///  Allows access to the type associated with the toolbox item.
    ///  The designer host is used to access an implementation of ITypeResolutionService.
    ///  However, the loaded type is not added to the list of references in the designer host.
    /// </summary>
    public Type? GetType(IDesignerHost? host) => GetType(host, AssemblyName, TypeName, false);

    /// <summary>
    ///  This utility function can be used to load a type given a name. AssemblyName and
    ///  designer host can be null, but if they are present they will be used to help
    ///  locate the type. If reference is true, the given assembly name will be added
    ///  to the designer host's set of references.
    /// </summary>
    [UnconditionalSuppressMessage("SingleFile", "IL3002", Justification = "Single-file case is handled")]
    protected virtual Type? GetType(IDesignerHost? host, AssemblyName? assemblyName, string typeName, bool reference)
    {
        Type? type = null;

        ArgumentNullException.ThrowIfNull(typeName);

        if (host.TryGetService(out ITypeResolutionService? ts))
        {
            if (reference)
            {
                if (assemblyName is not null)
                {
                    ts.ReferenceAssembly(assemblyName);
                    type = ts.GetType(typeName);
                }
                else
                {
                    // Just try loading the type. If we succeed, then use this as the
                    // reference.
                    type = ts.GetType(typeName);
                    type ??= Type.GetType(typeName);

                    if (type is not null)
                    {
                        ts.ReferenceAssembly(type.Assembly.GetName());
                    }
                }
            }
            else
            {
                if (assemblyName is not null)
                {
                    Assembly? a = ts.GetAssembly(assemblyName);
                    if (a is not null)
                    {
                        type = a.GetType(typeName);
                    }
                }

                type ??= ts.GetType(typeName);
            }
        }
        else if (!string.IsNullOrEmpty(typeName))
        {
            if (assemblyName is not null)
            {
                Assembly? a = null;
                try
                {
                    a = Assembly.Load(assemblyName);
                }
                catch (FileNotFoundException)
                {
                }
                catch (BadImageFormatException)
                {
                }
                catch (IOException)
                {
                }

#pragma warning disable SYSLIB0044 // Type or member is obsolete. Ref https://github.com/dotnet/winforms/issues/7308
#pragma warning disable IL3000 // Avoid accessing Assembly file path when publishing as a single file

                // If we're single file, CodeBase will be empty, that is expected. We can't load from a path
                // that doesn't exist, so falling through to the next case is the right thing to do.
                if (a is null && !string.IsNullOrEmpty(assemblyName.CodeBase))
                {
                    try
                    {
                        a = Assembly.LoadFrom(assemblyName.CodeBase);
                    }
                    catch (FileNotFoundException)
                    {
                    }
                    catch (BadImageFormatException)
                    {
                    }
                    catch (IOException)
                    {
                    }
                }
#pragma warning restore IL3000
#pragma warning restore SYSLIB0044

                if (a is not null)
                {
                    type = a.GetType(typeName);
                }
            }

            type ??= Type.GetType(typeName, false);
        }

        return type;
    }

    /// <summary>
    ///  Initializes a toolbox item with a given type. A locked toolbox item cannot be initialized.
    /// </summary>
    public virtual void Initialize(Type? type)
    {
        CheckUnlocked();

        if (type is not null)
        {
            TypeName = type.FullName;
            AssemblyName assemblyName = type.Assembly.GetName(true);

            Dictionary<string, AssemblyName> parents = [];
            Type? parentType = type;

            do
            {
                AssemblyName policiedName = parentType.Assembly.GetName(true);

                AssemblyName? aname = GetNonRetargetedAssemblyName(type, policiedName);

                if (aname is not null)
                {
                    parents.TryAdd(aname.FullName, aname);
                }

                parentType = parentType.BaseType;
            }
            while (parentType is not null);

            AssemblyName[] parentAssemblies = new AssemblyName[parents.Count];
            parents.Values.CopyTo(parentAssemblies, 0);

            DependentAssemblies = parentAssemblies;

            AssemblyName = assemblyName;
            DisplayName = type.Name;

            // if the Type is a reflectonly type, these values must be set through a config object or manually
            // after construction.
            if (!type.Assembly.ReflectionOnly)
            {
                object[] companyattrs = type.Assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
                if (companyattrs is [AssemblyCompanyAttribute { Company: not null } company, ..])
                {
                    Company = company.Company;
                }

                // set the description based off the description attribute of the given type.
                if (TypeDescriptorHelper.TryGetAttribute(type, out DescriptionAttribute? descattr))
                {
                    Description = descattr.Description;
                }

                if (TypeDescriptorHelper.TryGetAttribute(type, out ToolboxBitmapAttribute? attr))
                {
                    Bitmap? itemBitmap = attr.GetImage(type, false) as Bitmap;
                    if (itemBitmap is not null)
                    {
                        // Original bitmap is used when adding the item to the Visual Studio toolbox
                        // if running on a machine with HDPI scaling enabled.
                        OriginalBitmap = itemBitmap;
                        if ((itemBitmap.Width != s_iconWidth || itemBitmap.Height != s_iconHeight))
                        {
                            itemBitmap = new Bitmap(itemBitmap, new Size(s_iconWidth, s_iconHeight));
                        }
                    }

                    Bitmap = itemBitmap;
                }

                bool filterContainsType = false;
                List<ToolboxItemFilterAttribute> filterItems = [];
                foreach (Attribute a in TypeDescriptor.GetAttributes(type))
                {
                    if (a is ToolboxItemFilterAttribute ta)
                    {
                        if (ta.FilterString.Equals(TypeName))
                        {
                            filterContainsType = true;
                        }

                        filterItems.Add(ta);
                    }
                }

                if (!filterContainsType)
                {
                    filterItems.Add(new ToolboxItemFilterAttribute(TypeName));
                }

                Filter = filterItems.ToArray();
            }
        }
    }

    private static AssemblyName? GetNonRetargetedAssemblyName(Type type, AssemblyName policiedAssemblyName)
    {
        Debug.Assert(type is not null);
        if (policiedAssemblyName is null)
        {
            return null;
        }

        // if looking for myself, just return it. (not a reference)
        if (type.Assembly.FullName == policiedAssemblyName.FullName)
        {
            return policiedAssemblyName;
        }

        // first search for an exact match -- we prefer this over a partial match.
        foreach (AssemblyName name in type.Assembly.GetReferencedAssemblies())
        {
            if (name.FullName == policiedAssemblyName.FullName)
            {
                return name;
            }
        }

        // next search for a partial match -- we just compare the Name portions (ignore version and publickey)
        foreach (AssemblyName name in type.Assembly.GetReferencedAssemblies())
        {
            if (name.Name == policiedAssemblyName.Name)
            {
                return name;
            }
        }

        // finally, the most expensive -- its possible that retargeting policy is on an assembly whose name changes
        // an example of this is the device System.Windows.Forms.Datagrid.dll
        // in this case, we need to try to load each device assemblyname through policy to see if it results
        // in assemblyname.
        foreach (AssemblyName name in type.Assembly.GetReferencedAssemblies())
        {
            try
            {
                Assembly a = Assembly.Load(name);
                if (a.FullName == policiedAssemblyName.FullName)
                {
                    return name;
                }
            }
            catch
            {
                // Ignore all exceptions and just fall through if it fails (it shouldn't, but who knows).
            }
        }

        return null;
    }

    /// <summary>
    ///  Locks this toolbox item. Locking a toolbox item makes it read-only and
    ///  prevents any changes to its properties.
    /// </summary>
    public virtual void Lock()
    {
        _locked = true;
    }

    /// <summary>
    ///  Saves the state of this ToolboxItem to the specified serialization info
    /// </summary>
    protected virtual void Serialize(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Locked), Locked);
        List<string> propertyNames = new(Properties.Count);
        foreach (DictionaryEntry de in Properties)
        {
            propertyNames.Add((string)de.Key);
            info.AddValue((string)de.Key, de.Value);
        }

        info.AddValue("PropertyNames", propertyNames.ToArray());
    }

    /// <summary>
    ///  Raises the OnComponentsCreated event. This
    ///  will be called when this <see cref="ToolboxItem"/> creates a component.
    /// </summary>
    protected virtual void OnComponentsCreated(ToolboxComponentsCreatedEventArgs args)
    {
        _componentsCreatedEvent?.Invoke(this, args);
    }

    /// <summary>
    ///  Raises the OnCreateComponentsInvoked event. This
    ///  will be called before this <see cref="ToolboxItem"/> creates a component.
    /// </summary>
    protected virtual void OnComponentsCreating(ToolboxComponentsCreatingEventArgs args)
    {
        _componentsCreatingEvent?.Invoke(this, args);
    }

    public override string ToString() => DisplayName ?? string.Empty;

    /// <summary>
    ///  Called as a helper to ValidatePropertyValue to validate that an object
    ///  is of a given type.
    /// </summary>
    protected void ValidatePropertyType(string propertyName, object? value, Type expectedType, bool allowNull)
    {
        if (!allowNull)
        {
            ArgumentNullException.ThrowIfNull(value);
        }

        if (value is not null && !expectedType.IsInstanceOfType(value))
        {
            throw new ArgumentException(string.Format(SR.ToolboxItemInvalidPropertyType, propertyName, expectedType.FullName), nameof(value));
        }
    }

    /// <summary>
    ///  This is called whenever a value is set in the property dictionary. It gives you a chance
    ///  to change the value of an object before committing it, our reject it by throwing an
    ///  exception.
    /// </summary>
    [return: NotNullIfNotNull(nameof(value))]
    protected virtual object? ValidatePropertyValue(string propertyName, object? value)
    {
        switch (propertyName)
        {
            case "AssemblyName":
                ValidatePropertyType(propertyName, value, typeof(AssemblyName), true);
                break;

            case "Bitmap":
            case "OriginalBitmap":
                ValidatePropertyType(propertyName, value, typeof(Bitmap), true);
                break;

            case "Company":
            case "Description":
            case "DisplayName":
            case "TypeName":
                ValidatePropertyType(propertyName, value, typeof(string), true);
                return value ?? string.Empty;

            case "Filter":
                ValidatePropertyType(propertyName, value, typeof(ICollection), true);

                ICollection? col = (ICollection?)value;
                return col?.OfType<ToolboxItemFilterAttribute>().ToArray() ?? [];

            case "DependentAssemblies":
                ValidatePropertyType(propertyName, value, typeof(AssemblyName[]), true);
                break;

            case "IsTransient":
                ValidatePropertyType(propertyName, value, typeof(bool), false);
                break;
        }

        return value;
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
        throw new PlatformNotSupportedException();
    }

    private class LockableDictionary : Hashtable
    {
        private readonly ToolboxItem _item;
        internal LockableDictionary(ToolboxItem item, int capacity) : base(capacity)
        {
            _item = item;
        }

        public override bool IsFixedSize => _item.Locked;

        public override bool IsReadOnly => _item.Locked;

        public override object? this[object key]
        {
            get
            {
                string propertyName = GetPropertyName(key);
                object? value = base[propertyName];

                return _item.FilterPropertyValue(propertyName, value);
            }
            set
            {
                string propertyName = GetPropertyName(key);
                value = _item.ValidatePropertyValue(propertyName, value);
                _item.CheckUnlocked();
                base[propertyName] = value;
            }
        }

        public override void Add(object key, object? value)
        {
            string propertyName = GetPropertyName(key);
            value = _item.ValidatePropertyValue(propertyName, value);
            _item.CheckUnlocked();
            base.Add(propertyName, value);
        }

        public override void Clear()
        {
            _item.CheckUnlocked();
            base.Clear();
        }

        private static string GetPropertyName(object key)
        {
            ArgumentNullException.ThrowIfNull(key);

            return key is string { Length: > 0 } propertyName
                ? propertyName
                : throw new ArgumentException(SR.ToolboxItemInvalidKey, nameof(key));
        }

        public override void Remove(object key)
        {
            _item.CheckUnlocked();
            base.Remove(key);
        }
    }
}
