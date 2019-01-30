// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;

namespace System.Drawing.Design
{
    /// <summary>
    /// Provides a base implementation of a toolbox item.
    /// </summary>
    [Serializable]
    public class ToolboxItem : ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the ToolboxItem class.
        /// </summary>
        public ToolboxItem() { }

        /// <summary>
        /// Initializes a new instance of the ToolboxItem class using the specified type.
        /// </summary>
        public ToolboxItem(Type toolType) : this()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='System.Drawing.Design.ToolboxItem'/>
        /// class using the specified serialization information.
        /// </summary>
        private ToolboxItem(SerializationInfo info, StreamingContext context) : this()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// The assembly name for this toolbox item. The assembly name describes the assembly
        /// information needed to load the toolbox item's type.
        /// </summary>
        public AssemblyName AssemblyName
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// The assembly name for this toolbox item. The assembly name describes the assembly
        /// information needed to load the toolbox item's type.
        /// </summary>
        public AssemblyName[] DependentAssemblies
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Gets or sets the bitmap that will be used on the toolbox for this item. 
        /// Use this property on the design surface as this bitmap is scaled according to the current the DPI setting.
        /// </summary>
        public Bitmap Bitmap
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Gets or sets the original bitmap that will be used on the toolbox for this item.
        /// This bitmap should be 16x16 pixel and should be used in the Visual Studio toolbox, not on the design surface.
        /// </summary>
        public Bitmap OriginalBitmap
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Gets or sets the company name for this <see cref='System.Drawing.Design.ToolboxItem'/>.
        /// This defaults to the companyname attribute retrieved from type.Assembly, if set.
        /// </summary>
        public string Company
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// The Component Type is ".Net Component" -- unless otherwise specified by a derived toolboxitem
        /// </summary>
        public virtual string ComponentType
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Description is a free-form, multiline capable text description that will be displayed in the tooltip
        /// for the toolboxItem.  It defaults to the path of the assembly that contains the item, but can be overridden.
        /// </summary>
        public string Description
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Gets or sets the display name for this <see cref='System.Drawing.Design.ToolboxItem'/>.
        /// </summary>
        public string DisplayName
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Gets or sets the filter for this toolbox item.  The filter is a collection of
        /// ToolboxItemFilterAttribute objects.
        /// </summary>
        public ICollection Filter
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// If true, it indicates that this toolbox item should not be stored in
        /// any toolbox database when an application that is providing a toolbox
        /// closes down.  This property defaults to false.
        /// </summary>
        public bool IsTransient
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Determines if this toolbox item is locked.  Once locked, a toolbox item will
        /// not accept any changes to its properties.
        /// </summary>
        public virtual bool Locked
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// The properties dictionary is a set of name/value pairs.  The keys are property
        /// names and the values are property values.  This dictionary becomes read-only
        /// after the toolbox item has been locked.
        /// Values in the properties dictionary are validated through ValidateProperty
        /// and default values are obtained from GetDefalutProperty.
        /// </summary>
        public IDictionary Properties
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Gets or sets the fully qualified name of the type this toolbox item will create.
        /// </summary>
        public string TypeName
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Gets the version for this toolboxitem.  It defaults to AssemblyName.Version unless
        /// overridden in a derived toolboxitem.  This can be overridden to return an empty string
        /// to suppress its display in the toolbox tooltip.
        /// </summary>
        public virtual string Version
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
        }


        /// <summary>
        /// Occurs when components are created.
        /// </summary>
        public event ToolboxComponentsCreatedEventHandler ComponentsCreated
        {
            add
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }
            remove
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }
        }

        /// <summary>
        /// Occurs before components are created.
        /// </summary>
        public event ToolboxComponentsCreatingEventHandler ComponentsCreating
        {
            add
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }
            remove
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }
        }

        /// <summary>
        /// This method checks that the toolbox item is currently unlocked (read-write) and
        /// throws an appropriate exception if it isn't.
        /// </summary>
        protected void CheckUnlocked()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Creates objects from the type contained in this toolbox item.
        /// </summary>
        /// <returns></returns>
        public IComponent[] CreateComponents()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Creates objects from the type contained in this toolbox item.  If designerHost is non-null
        /// this will also add them to the designer.
        /// </summary>
        public IComponent[] CreateComponents(IDesignerHost host)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Creates objects from the type contained in this toolbox item.  If designerHost is non-null
        /// this will also add them to the designer.
        /// </summary>
        /// <returns></returns>
        public IComponent[] CreateComponents(IDesignerHost host, IDictionary defaultValues)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Creates objects from the type contained in this toolbox item.  If designerHost is non-null
        /// this will also add them to the designer.
        /// </summary>
        protected virtual IComponent[] CreateComponentsCore(IDesignerHost host)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Creates objects from the type contained in this toolbox item.  If designerHost is non-null
        /// this will also add them to the designer.
        /// </summary>
        protected virtual IComponent[] CreateComponentsCore(IDesignerHost host, IDictionary defaultValues)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        protected virtual void Deserialize(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Filters a property value before returning it.  This allows a property to always clone values,
        ///    or to provide a default value when none exists.
        /// </summary>
        protected virtual object FilterPropertyValue(string propertyName, object value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Allows access to the type associated with the toolbox item.
        /// The designer host is used to access an implementation of ITypeResolutionService.
        /// However, the loaded type is not added to the list of references in the designer host.
        /// </summary>
        public Type GetType(IDesignerHost host)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// This utility function can be used to load a type given a name.  AssemblyName and
        ///     designer host can be null, but if they are present they will be used to help
        ///     locate the type.  If reference is true, the given assembly name will be added
        ///     to the designer host's set of references.
        /// </summary>
        protected virtual Type GetType(IDesignerHost host, AssemblyName assemblyName, string typeName, bool reference)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Initializes a toolbox item with a given type.  A locked toolbox item cannot be initialized.
        /// </summary>
        public virtual void Initialize(Type type)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Locks this toolbox item.  Locking a toolbox item makes it read-only and 
        /// prevents any changes to its properties.
        /// </summary>
        public virtual void Lock()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///Saves the state of this ToolboxItem to the specified serialization info
        /// </summary>
        protected virtual void Serialize(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
        /// <summary>
        /// Raises the OnComponentsCreated event. This
        /// will be called when this <see cref='System.Drawing.Design.ToolboxItem'/> creates a component.
        /// </summary>
        protected virtual void OnComponentsCreated(ToolboxComponentsCreatedEventArgs args)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Raises the OnCreateComponentsInvoked event. This
        /// will be called before this <see cref='System.Drawing.Design.ToolboxItem'/> creates a component.
        /// </summary>
        protected virtual void OnComponentsCreating(ToolboxComponentsCreatingEventArgs args)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public override string ToString()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// Called as a helper to ValidatePropertyValue to validate that an object
        ///    is of a given type.
        /// </summary>
        protected void ValidatePropertyType(string propertyName, object value, Type expectedType, bool allowNull)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// This is called whenever a value is set in the property dictionary.  It gives you a chance
        /// to change the value of an object before comitting it, our reject it by throwing an 
        /// exception.
        /// </summary>
        protected virtual object ValidatePropertyValue(string propertyName, object value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}

