// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace System.Drawing.Design
{
    /// <summary>
    ///  Provides a base implementation of a toolbox item.
    /// </summary>
    public class ToolboxItem : ISerializable
    {
        private static readonly TraceSwitch s_toolboxItemPersist = new TraceSwitch("ToolboxPersisting", "ToolboxItem: write data");

        private static readonly object s_eventComponentsCreated = new object();
        private static readonly object s_eventComponentsCreating = new object();

        private static bool s_isScalingInitialized = false;
        private const int ICON_DIMENSION = 16;
        private static int s_iconWidth = ICON_DIMENSION;
        private static int s_iconHeight = ICON_DIMENSION;

        private bool _locked;
        private LockableDictionary _properties;
        private ToolboxComponentsCreatedEventHandler _componentsCreatedEvent;
        private ToolboxComponentsCreatingEventHandler _componentsCreatingEvent;

        /// <summary>
        ///  Initializes a new instance of the ToolboxItem class.
        /// </summary>
        public ToolboxItem()
        {
            if (!s_isScalingInitialized)
            {
                if (DpiHelper.IsScalingRequired)
                {
                    s_iconWidth = DpiHelper.LogicalToDeviceUnitsX(ICON_DIMENSION);
                    s_iconHeight = DpiHelper.LogicalToDeviceUnitsY(ICON_DIMENSION);
                }
                s_isScalingInitialized = true;
            }
        }

        /// <summary>
        ///  Initializes a new instance of the ToolboxItem class using the specified type.
        /// </summary>
        public ToolboxItem(Type toolType) : this()
        {
            Initialize(toolType);
        }

        /// <summary>
        ///  The assembly name for this toolbox item. The assembly name describes the assembly
        ///  information needed to load the toolbox item's type.
        /// </summary>
        public AssemblyName AssemblyName
        {
            get
            {
                return (AssemblyName)Properties["AssemblyName"];
            }
            set
            {
                Properties["AssemblyName"] = value;
            }
        }

        /// <summary>
        ///  The assembly name for this toolbox item. The assembly name describes the assembly
        ///  information needed to load the toolbox item's type.
        /// </summary>
        public AssemblyName[] DependentAssemblies
        {
            get
            {
                AssemblyName[] names = (AssemblyName[])Properties["DependentAssemblies"];
                if (names != null)
                {
                    return (AssemblyName[])names.Clone();
                }
                return null;
            }
            set
            {
                Properties["DependentAssemblies"] = value?.Clone();
            }
        }

        /// <summary>
        ///  Gets or sets the bitmap that will be used on the toolbox for this item.
        ///  Use this property on the design surface as this bitmap is scaled according to the current the DPI setting.
        /// </summary>
        public Bitmap Bitmap
        {
            get
            {
                return (Bitmap)Properties["Bitmap"];
            }
            set
            {
                Properties["Bitmap"] = value;
            }
        }

        /// <summary>
        ///  Gets or sets the original bitmap that will be used on the toolbox for this item.
        ///  This bitmap should be 16x16 pixel and should be used in the Visual Studio toolbox, not on the design surface.
        /// </summary>
        public Bitmap OriginalBitmap
        {
            get
            {
                return (Bitmap)Properties["OriginalBitmap"];
            }
            set
            {
                Properties["OriginalBitmap"] = value;
            }
        }

        /// <summary>
        ///  Gets or sets the company name for this <see cref='System.Drawing.Design.ToolboxItem'/>.
        ///  This defaults to the companyname attribute retrieved from type.Assembly, if set.
        /// </summary>
        public string Company
        {
            get
            {
                return (string)Properties["Company"];
            }
            set
            {
                Properties["Company"] = value;
            }
        }

        /// <summary>
        ///  The Component Type is ".Net Component" -- unless otherwise specified by a derived toolboxitem
        /// </summary>
        public virtual string ComponentType
        {
            get
            {
                return SR.DotNET_ComponentType;
            }
        }

        /// <summary>
        ///  Description is a free-form, multiline capable text description that will be displayed in the tooltip
        ///  for the toolboxItem.  It defaults to the path of the assembly that contains the item, but can be overridden.
        /// </summary>
        public string Description
        {
            get
            {
                return (string)Properties["Description"];
            }
            set
            {
                Properties["Description"] = value;
            }
        }

        /// <summary>
        ///  Gets or sets the display name for this <see cref='System.Drawing.Design.ToolboxItem'/>.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return (string)Properties["DisplayName"];
            }
            set
            {
                Properties["DisplayName"] = value;
            }
        }

        /// <summary>
        ///  Gets or sets the filter for this toolbox item.  The filter is a collection of
        ///  ToolboxItemFilterAttribute objects.
        /// </summary>
        public ICollection Filter
        {
            get
            {
                return (ICollection)Properties["Filter"];
            }
            set
            {
                Properties["Filter"] = value;
            }
        }

        /// <summary>
        ///  If true, it indicates that this toolbox item should not be stored in
        ///  any toolbox database when an application that is providing a toolbox
        ///  closes down.  This property defaults to false.
        /// </summary>
        public bool IsTransient
        {
            get
            {
                return (bool)Properties["IsTransient"];
            }
            set
            {
                Properties["IsTransient"] = value;
            }
        }

        /// <summary>
        ///  Determines if this toolbox item is locked.  Once locked, a toolbox item will
        ///  not accept any changes to its properties.
        /// </summary>
        public virtual bool Locked
        {
            get
            {
                return _locked;
            }
        }

        /// <summary>
        ///  The properties dictionary is a set of name/value pairs.  The keys are property
        ///  names and the values are property values.  This dictionary becomes read-only
        ///  after the toolbox item has been locked.
        ///  Values in the properties dictionary are validated through ValidateProperty
        ///  and default values are obtained from GetDefalutProperty.
        /// </summary>
        public IDictionary Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = new LockableDictionary(this, 8 /* # of properties we have */);
                }

                return _properties;
            }
        }

        /// <summary>
        ///  Gets or sets the fully qualified name of the type this toolbox item will create.
        /// </summary>
        public string TypeName
        {
            get
            {
                return (string)Properties["TypeName"];
            }
            set
            {
                Properties["TypeName"] = value;
            }
        }

        /// <summary>
        ///  Gets the version for this toolboxitem. It defaults to AssemblyName.Version unless
        ///  overridden in a derived toolboxitem. This can be overridden to
        ///  return an empty string to suppress its display in the toolbox tooltip.
        /// </summary>
        public virtual string Version
        {
            get => AssemblyName?.Version?.ToString() ?? string.Empty;
        }

        /// <summary>
        ///  Occurs when components are created.
        /// </summary>
        public event ToolboxComponentsCreatedEventHandler ComponentsCreated
        {
            add => _componentsCreatedEvent += value;
            remove => _componentsCreatedEvent -= value;
        }

        /// <summary>
        ///  Occurs before components are created.
        /// </summary>
        public event ToolboxComponentsCreatingEventHandler ComponentsCreating
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
        public IComponent[] CreateComponents()
        {
            return CreateComponents(null);
        }

        /// <summary>
        ///  Creates objects from the type contained in this toolbox item.  If designerHost is non-null
        ///  this will also add them to the designer.
        /// </summary>
        public IComponent[] CreateComponents(IDesignerHost host)
        {
            OnComponentsCreating(new ToolboxComponentsCreatingEventArgs(host));
            IComponent[] comps = CreateComponentsCore(host, new Hashtable());
            if (comps != null && comps.Length > 0)
            {
                OnComponentsCreated(new ToolboxComponentsCreatedEventArgs(comps));
            }
            return comps;
        }

        /// <summary>
        ///  Creates objects from the type contained in this toolbox item.  If designerHost is non-null
        ///  this will also add them to the designer.
        /// </summary>
        /// <returns></returns>
        public IComponent[] CreateComponents(IDesignerHost host, IDictionary defaultValues)
        {
            OnComponentsCreating(new ToolboxComponentsCreatingEventArgs(host));
            IComponent[] comps = CreateComponentsCore(host, defaultValues);
            if (comps != null && comps.Length > 0)
            {
                OnComponentsCreated(new ToolboxComponentsCreatedEventArgs(comps));
            }
            return comps;
        }

        /// <summary>
        ///  Creates objects from the type contained in this toolbox item.  If designerHost is non-null
        ///  this will also add them to the designer.
        /// </summary>
        protected virtual IComponent[] CreateComponentsCore(IDesignerHost host)
        {
            ArrayList comps = new ArrayList();

            Type createType = GetType(host, AssemblyName, TypeName, true);
            if (createType != null)
            {
                if (host != null)
                {
                    comps.Add(host.CreateComponent(createType));
                }
                else if (typeof(IComponent).IsAssignableFrom(createType))
                {
                    comps.Add(TypeDescriptor.CreateInstance(null, createType, null, null));
                }
            }

            IComponent[] temp = new IComponent[comps.Count];
            comps.CopyTo(temp, 0);
            return temp;
        }

        /// <summary>
        ///  Creates objects from the type contained in this toolbox item.  If designerHost is non-null
        ///  this will also add them to the designer.
        /// </summary>
        protected virtual IComponent[] CreateComponentsCore(IDesignerHost host, IDictionary defaultValues)
        {
            IComponent[] components = CreateComponentsCore(host);

            if (host != null && components != null)
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
            // out our dictionary of property names.  We need to do this	
            // for backwards compatibilty because if we throw everything	
            // into the property dictionary we'll duplicate stuff people	
            // have serialized by hand.	

            string[] propertyNames = null;
            foreach (SerializationEntry entry in info)
            {
                if (entry.Name.Equals("PropertyNames"))
                {
                    propertyNames = entry.Value as string[];
                    break;
                }
            }

            if (propertyNames == null)
            {
                // For backwards compat, here are the default property	
                // names we use	
                propertyNames = new string[] {
                    "AssemblyName",
                    "Bitmap",
                    "DisplayName",
                    "Filter",
                    "IsTransient",
                    "TypeName"
                };
            }

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

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (obj == null)
            {
                return false;
            }

            if (!(obj.GetType() == GetType()))
            {
                return false;
            }

            ToolboxItem otherItem = (ToolboxItem)obj;

            return TypeName == otherItem.TypeName &&
                  AreAssemblyNamesEqual(AssemblyName, otherItem.AssemblyName) &&
                  DisplayName == otherItem.DisplayName;
        }

        private static bool AreAssemblyNamesEqual(AssemblyName name1, AssemblyName name2)
        {
            return name1 == name2 ||
                   (name1 != null && name2 != null && name1.FullName == name2.FullName);
        }

        public override int GetHashCode() => HashCode.Combine(TypeName, DisplayName);

        /// <summary>
        ///  Filters a property value before returning it.  This allows a property to always clone values,
        ///  or to provide a default value when none exists.
        /// </summary>
        protected virtual object FilterPropertyValue(string propertyName, object value)
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
                    if (value == null)
                    {
                        value = string.Empty;
                    }

                    break;

                case "Filter":
                    if (value == null)
                    {
                        value = Array.Empty<ToolboxItemFilterAttribute>();
                    }

                    break;

                case "IsTransient":
                    if (value == null)
                    {
                        value = false;
                    }

                    break;
            }
            return value;
        }

        /// <summary>
        ///  Allows access to the type associated with the toolbox item.
        ///  The designer host is used to access an implementation of ITypeResolutionService.
        ///  However, the loaded type is not added to the list of references in the designer host.
        /// </summary>
        public Type GetType(IDesignerHost host)
        {
            return GetType(host, AssemblyName, TypeName, false);
        }

        /// <summary>
        ///  This utility function can be used to load a type given a name.  AssemblyName and
        ///  designer host can be null, but if they are present they will be used to help
        ///  locate the type.  If reference is true, the given assembly name will be added
        ///  to the designer host's set of references.
        /// </summary>
        protected virtual Type GetType(IDesignerHost host, AssemblyName assemblyName, string typeName, bool reference)
        {
            ITypeResolutionService ts = null;
            Type type = null;

            if (typeName == null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            if (host != null)
            {
                ts = host.GetService(typeof(ITypeResolutionService)) as ITypeResolutionService;
            }

            if (ts != null)
            {

                if (reference)
                {
                    if (assemblyName != null)
                    {
                        ts.ReferenceAssembly(assemblyName);
                        type = ts.GetType(typeName);
                    }
                    else
                    {
                        // Just try loading the type.  If we succeed, then use this as the	
                        // reference.	
                        type = ts.GetType(typeName);
                        if (type == null)
                        {
                            type = Type.GetType(typeName);
                        }
                        if (type != null)
                        {
                            ts.ReferenceAssembly(type.Assembly.GetName());
                        }
                    }
                }
                else
                {
                    if (assemblyName != null)
                    {
                        Assembly a = ts.GetAssembly(assemblyName);
                        if (a != null)
                        {
                            type = a.GetType(typeName);
                        }
                    }

                    if (type == null)
                    {
                        type = ts.GetType(typeName);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(typeName))
                {
                    if (assemblyName != null)
                    {
                        Assembly a = null;
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

                        if (a == null && !string.IsNullOrEmpty(assemblyName.CodeBase))
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

                        if (a != null)
                        {
                            type = a.GetType(typeName);
                        }
                    }

                    if (type == null)
                    {
                        type = Type.GetType(typeName, false);
                    }
                }
            }

            return type;
        }

        /// <summary>
        ///  Initializes a toolbox item with a given type.  A locked toolbox item cannot be initialized.
        /// </summary>
        public virtual void Initialize(Type type)
        {
            CheckUnlocked();

            if (type != null)
            {
                TypeName = type.FullName;
                AssemblyName assemblyName = type.Assembly.GetName(true);

                Dictionary<string, AssemblyName> parents = new Dictionary<string, AssemblyName>();
                Type parentType = type;
                while (parentType != null)
                {
                    AssemblyName policiedname = parentType.Assembly.GetName(true);

                    AssemblyName aname = GetNonRetargetedAssemblyName(type, policiedname);

                    if (aname != null && !parents.ContainsKey(aname.FullName))
                    {
                        parents[aname.FullName] = aname;
                    }
                    parentType = parentType.BaseType;
                }

                AssemblyName[] parentAssemblies = new AssemblyName[parents.Count];
                int i = 0;
                foreach (AssemblyName an in parents.Values)
                {
                    parentAssemblies[i++] = an;
                }

                DependentAssemblies = parentAssemblies;

                AssemblyName = assemblyName;
                DisplayName = type.Name;

                //if the Type is a reflectonly type, these values must be set through a config object or manually	
                //after construction.	
                if (!type.Assembly.ReflectionOnly)
                {

                    object[] companyattrs = type.Assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
                    if (companyattrs != null && companyattrs.Length > 0)
                    {
                        if (companyattrs[0] is AssemblyCompanyAttribute company && company.Company != null)
                        {
                            Company = company.Company;
                        }
                    }

                    //set the description based off the description attribute of the given type.	
                    DescriptionAttribute descattr = (DescriptionAttribute)TypeDescriptor.GetAttributes(type)[typeof(DescriptionAttribute)];
                    if (descattr != null)
                    {
                        Description = descattr.Description;
                    }

                    ToolboxBitmapAttribute attr = (ToolboxBitmapAttribute)TypeDescriptor.GetAttributes(type)[typeof(ToolboxBitmapAttribute)];
                    if (attr != null)
                    {
                        Bitmap itemBitmap = attr.GetImage(type, false) as Bitmap;
                        if (itemBitmap != null)
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
                    ArrayList array = new ArrayList();
                    foreach (Attribute a in TypeDescriptor.GetAttributes(type))
                    {
                        if (a is ToolboxItemFilterAttribute ta)
                        {
                            if (ta.FilterString.Equals(TypeName))
                            {
                                filterContainsType = true;
                            }
                            array.Add(ta);
                        }
                    }

                    if (!filterContainsType)
                    {
                        array.Add(new ToolboxItemFilterAttribute(TypeName));
                    }

                    Filter = (ToolboxItemFilterAttribute[])array.ToArray(typeof(ToolboxItemFilterAttribute));
                }
            }
        }

        private AssemblyName GetNonRetargetedAssemblyName(Type type, AssemblyName policiedAssemblyName)
        {
            Debug.Assert(type != null);
            if (policiedAssemblyName == null)
            {
                return null;
            }

            //if looking for myself, just return it. (not a reference)	
            if (type.Assembly.FullName == policiedAssemblyName.FullName)
            {
                return policiedAssemblyName;
            }

            //first search for an exact match -- we prefer this over a partial match.	
            foreach (AssemblyName name in type.Assembly.GetReferencedAssemblies())
            {
                if (name.FullName == policiedAssemblyName.FullName)
                {
                    return name;
                }
            }

            //next search for a partial match -- we just compare the Name portions (ignore version and publickey)	
            foreach (AssemblyName name in type.Assembly.GetReferencedAssemblies())
            {
                if (name.Name == policiedAssemblyName.Name)
                {
                    return name;
                }
            }

            //finally, the most expensive -- its possible that retargeting policy is on an assembly whose name changes	
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
        ///  Locks this toolbox item.  Locking a toolbox item makes it read-only and
        ///  prevents any changes to its properties.
        /// </summary>
        public virtual void Lock()
        {
            _locked = true;
        }

        /// <summary>
        ///Saves the state of this ToolboxItem to the specified serialization info
        /// </summary>
        protected virtual void Serialize(SerializationInfo info, StreamingContext context)
        {
            if (s_toolboxItemPersist.TraceVerbose)
            {
                Debug.WriteLine("Persisting: " + GetType().Name);
                Debug.WriteLine("\tDisplay Name: " + DisplayName);
            }

            info.AddValue(nameof(Locked), Locked);
            ArrayList propertyNames = new ArrayList(Properties.Count);
            foreach (DictionaryEntry de in Properties)
            {
                propertyNames.Add(de.Key);
                info.AddValue((string)de.Key, de.Value);
            }
            info.AddValue("PropertyNames", (string[])propertyNames.ToArray(typeof(string)));
        }
        /// <summary>
        ///  Raises the OnComponentsCreated event. This
        ///  will be called when this <see cref='System.Drawing.Design.ToolboxItem'/> creates a component.
        /// </summary>
        protected virtual void OnComponentsCreated(ToolboxComponentsCreatedEventArgs args)
        {
            _componentsCreatedEvent?.Invoke(this, args);
        }

        /// <summary>
        ///  Raises the OnCreateComponentsInvoked event. This
        ///  will be called before this <see cref='System.Drawing.Design.ToolboxItem'/> creates a component.
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
        protected void ValidatePropertyType(string propertyName, object value, Type expectedType, bool allowNull)
        {
            if (value == null)
            {
                if (!allowNull)
                {
                    throw new ArgumentNullException(nameof(value));
                }
            }
            else
            {
                if (!expectedType.IsInstanceOfType(value))
                {
                    throw new ArgumentException(string.Format(SR.ToolboxItemInvalidPropertyType, propertyName, expectedType.FullName), nameof(value));
                }
            }
        }

        /// <summary>
        ///  This is called whenever a value is set in the property dictionary.  It gives you a chance
        ///  to change the value of an object before comitting it, our reject it by throwing an
        ///  exception.
        /// </summary>
        protected virtual object ValidatePropertyValue(string propertyName, object value)
        {
            switch (propertyName)
            {
                case "AssemblyName":
                    ValidatePropertyType(propertyName, value, typeof(AssemblyName), true);
                    break;

                case "Bitmap":
                    ValidatePropertyType(propertyName, value, typeof(Bitmap), true);
                    break;

                case "OriginalBitmap":
                    ValidatePropertyType(propertyName, value, typeof(Bitmap), true);
                    break;

                case "Company":
                case "Description":
                case "DisplayName":
                case "TypeName":
                    ValidatePropertyType(propertyName, value, typeof(string), true);
                    if (value == null)
                    {
                        value = string.Empty;
                    }

                    break;

                case "Filter":
                    ValidatePropertyType(propertyName, value, typeof(ICollection), true);

                    int filterCount = 0;
                    ICollection col = (ICollection)value;

                    if (col != null)
                    {
                        foreach (object f in col)
                        {
                            if (f is ToolboxItemFilterAttribute)
                            {
                                filterCount++;
                            }
                        }
                    }

                    ToolboxItemFilterAttribute[] filter = new ToolboxItemFilterAttribute[filterCount];

                    if (col != null)
                    {
                        filterCount = 0;
                        foreach (object f in col)
                        {
                            if (f is ToolboxItemFilterAttribute tfa)
                            {
                                filter[filterCount++] = tfa;
                            }
                        }
                    }

                    value = filter;
                    break;

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

            public override bool IsFixedSize
            {
                get
                {
                    return _item.Locked;
                }
            }

            public override bool IsReadOnly
            {
                get
                {
                    return _item.Locked;
                }
            }

            public override object this[object key]
            {
                get
                {
                    string propertyName = GetPropertyName(key);
                    object value = base[propertyName];

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

            public override void Add(object key, object value)
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

            private string GetPropertyName(object key)
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (!(key is string propertyName) || propertyName.Length == 0)
                {
                    throw new ArgumentException(SR.ToolboxItemInvalidKey, nameof(key));
                }

                return propertyName;
            }

            public override void Remove(object key)
            {
                _item.CheckUnlocked();
                base.Remove(key);
            }
        }
    }
}

