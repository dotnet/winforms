// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms.PropertyGridInternal {
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.Collections;
    using System.Reflection;    
    using System.ComponentModel.Design;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;
    using System.Drawing;
    using Microsoft.Win32;


    internal class SingleSelectRootGridEntry : GridEntry, IRootGridEntry {
        protected object             objValue;
        protected String             objValueClassName;
        protected GridEntry          propDefault;
        protected IDesignerHost      host;
        protected IServiceProvider baseProvider = null;
        protected PropertyTab        tab = null;
        protected PropertyGridView     gridEntryHost    = null;
        protected AttributeCollection  browsableAttributes = null;
        private   IComponentChangeService changeService;
        protected bool forceReadOnlyChecked;
        
        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // GridEntry classes are internal so we have complete
                                                                                                    // control over who does what in the constructor.
        ]
        internal SingleSelectRootGridEntry(PropertyGridView gridEntryHost, object value, GridEntry parent, IServiceProvider baseProvider, IDesignerHost host, PropertyTab tab, PropertySort sortType)
        : base(gridEntryHost.OwnerGrid, parent) {
            Debug.Assert(value != null,"Can't browse a null object!");
            this.host = host;
            this.gridEntryHost = gridEntryHost;
            this.baseProvider = baseProvider;
            this.tab = tab;
            this.objValue = value;
            this.objValueClassName = TypeDescriptor.GetClassName(this.objValue);

            this.IsExpandable = true;
            // default to categories
            this.PropertySort = sortType;
            this.InternalExpanded = true;
        }

        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // GridEntry classes are internal so we have complete
                                                                                                    // control over who does what in the constructor.
        ]
        internal SingleSelectRootGridEntry(PropertyGridView view, object value, IServiceProvider baseProvider, IDesignerHost host, PropertyTab tab, PropertySort sortType) : this(view, value,null, baseProvider, host, tab, sortType) {
        }   

        /// <include file='doc\SingleSelectRootGridEntry.uex' path='docs/doc[@for="SingleSelectRootGridEntry.BrowsableAttributes"]/*' />
        /// <devdoc>
        /// The set of attributes that will be used for browse filtering
        /// </devdoc>
        public override AttributeCollection BrowsableAttributes {
            get {
                if (browsableAttributes == null) {
                    browsableAttributes = new AttributeCollection(new Attribute[]{BrowsableAttribute.Yes});
                }
                return this.browsableAttributes;
            }
            set {
                if (value == null) {
                    ResetBrowsableAttributes();
                    return;
                }

                bool same = true;

                if (this.browsableAttributes != null && value != null && this.browsableAttributes.Count == value.Count) {
                    Attribute[] attr1 = new Attribute[browsableAttributes.Count];
                    Attribute[] attr2 = new Attribute[value.Count];

                    browsableAttributes.CopyTo(attr1, 0);
                    value.CopyTo(attr2, 0);
                    
                    Array.Sort(attr1, GridEntry.AttributeTypeSorter);
                    Array.Sort(attr2, GridEntry.AttributeTypeSorter);
                    for (int i = 0; i < attr1.Length; i++) {
                        if (!attr1[i].Equals(attr2[i])) {
                            same = false;
                            break;
                        }
                    }
                }
                else {
                    same = false;
                }

                this.browsableAttributes = value;

                if (!same && Children != null && Children.Count > 0) {
                    DisposeChildren();
                }
            }
        }
        
        protected override IComponentChangeService ComponentChangeService {
            get {
               if (changeService == null) {
                    changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
               }
               return changeService;
            }
        }
        
        internal override bool AlwaysAllowExpand {
            get {
               return true;
            }
        }

        public override PropertyTab CurrentTab {
            get {
                return tab;
            }
            set {
                this.tab = value;
            }
        }

        internal override GridEntry DefaultChild {
            get {
                return propDefault;
            }
            set {
                this.propDefault = value;
            }
        }

        internal override IDesignerHost DesignerHost {
            get {
                return host;
            }
            set {
                host = value;
            }
        }

        internal override bool ForceReadOnly {
            get {
                if (!forceReadOnlyChecked) {
                    ReadOnlyAttribute readOnlyAttr = (ReadOnlyAttribute)TypeDescriptor.GetAttributes(this.objValue)[typeof(ReadOnlyAttribute)];
                    if ((readOnlyAttr != null && !readOnlyAttr.IsDefaultAttribute()) || TypeDescriptor.GetAttributes(this.objValue).Contains(InheritanceAttribute.InheritedReadOnly)) {
                        flags |= FLAG_FORCE_READONLY;
                    }
                    forceReadOnlyChecked = true;
                }
                return base.ForceReadOnly || (GridEntryHost != null && !GridEntryHost.Enabled);
            }
        }

        internal override PropertyGridView GridEntryHost {
            get {       
                return this.gridEntryHost;
            }
            set {
                this.gridEntryHost = value;
            }
        }
        
        public override GridItemType GridItemType {
            get {
                return GridItemType.Root;
            }
        } 

        /// <include file='doc\SingleSelectRootGridEntry.uex' path='docs/doc[@for="SingleSelectRootGridEntry.HelpKeyword"]/*' />
        /// <devdoc>
        ///     Retrieves the keyword that the VS help dynamic help window will
        ///     use when this IPE is selected.
        /// </devdoc>
        public override string HelpKeyword {
            get {
                
               HelpKeywordAttribute helpAttribute = (HelpKeywordAttribute)TypeDescriptor.GetAttributes(objValue)[typeof(HelpKeywordAttribute)];

               if (helpAttribute != null && !helpAttribute.IsDefaultAttribute()) {
                    return helpAttribute.HelpKeyword;
               }

               return this.objValueClassName;
            }
        }

        public override string PropertyLabel {
            get {
                if (objValue is IComponent) {
                    ISite site = ((IComponent)objValue).Site;
                    if (site == null) {
                        return objValue.GetType().Name;
                    }
                    else {
                        return site.Name;
                    }
                }
                else if (objValue != null) {
                    return objValue.ToString();
                }
                return null;
            }
        }
          
        /// <include file='doc\SingleSelectRootGridEntry.uex' path='docs/doc[@for="SingleSelectRootGridEntry.PropertyValue"]/*' />
        /// <devdoc>
        /// Gets or sets the value for the property that is represented 
        /// by this GridEntry.
        /// </devdoc>
        public override object PropertyValue{
            get {
                return objValue;
            }
            set {
                object old = objValue;
                objValue = value;
                objValueClassName = TypeDescriptor.GetClassName(objValue);
                ownerGrid.ReplaceSelectedObject(old, value);
            }
        }

        protected override bool CreateChildren() {
            bool fReturn = base.CreateChildren();
            CategorizePropEntries();
            return fReturn;
        }
        
        protected override void Dispose(bool disposing) {
            if (disposing) {
                host = null;
                baseProvider = null;
                tab = null;
                gridEntryHost = null;
                changeService = null;
            }
            this.objValue = null;
            this.objValueClassName = null;
            this.propDefault = null;
            base.Dispose(disposing);
        }

        public override object GetService(Type serviceType) {
            object service = null;
            
            if (host != null) {
                service = host.GetService(serviceType);
            }
            if (service == null && baseProvider != null) {
                service = baseProvider.GetService(serviceType);
            }
            return service;
        }

        /// <include file='doc\SingleSelectRootGridEntry.uex' path='docs/doc[@for="SingleSelectRootGridEntry.ResetBrowsableAttributes"]/*' />
        /// <devdoc>
        /// Reset the Browsable attributes to the default (BrowsableAttribute.Yes)
        /// </devdoc>
        public void ResetBrowsableAttributes() {
            this.browsableAttributes = new AttributeCollection(new Attribute[]{BrowsableAttribute.Yes});
        }


        /// <include file='doc\SingleSelectRootGridEntry.uex' path='docs/doc[@for="SingleSelectRootGridEntry.ShowCategories"]/*' />
        /// <devdoc>
        /// Sets the value of this GridEntry from text
        /// </devdoc>
        public virtual void ShowCategories(bool fCategories) {
            if (((this.PropertySort &= PropertySort.Categorized) !=0) != fCategories) {
                
                if (fCategories) { 
                  this.PropertySort |= PropertySort.Categorized;
                }
                else {
                  this.PropertySort &= ~PropertySort.Categorized;
                }
                
                
                // recreate the children
                if (this.Expandable && this.ChildCollection != null) {
                    CreateChildren();
                }
            }
        }

        internal void CategorizePropEntries() {
            if (Children.Count > 0) {
                
                GridEntry[] childEntries = new GridEntry[this.Children.Count];
                this.Children.CopyTo(childEntries, 0);
                
                if ((this.PropertySort & PropertySort.Categorized) != 0) {


                    // first, walk through all the entires and 
                    // group them by their category by adding
                    // them to a hashtable of arraylists.
                    //
                    Hashtable bins = new Hashtable();
                    for (int i = 0; i < childEntries.Length; i++) {
                        GridEntry pe = childEntries[i];
						Debug.Assert(pe != null);
						if (pe != null) {
							string category = pe.PropertyCategory;
							ArrayList bin = (ArrayList)bins[category];
							if (bin == null) {
								bin = new ArrayList();
								bins[category] = bin;
							}
							bin.Add(pe);
						}
                    }

                    // now walk through the hashtable
                    // and create a categorygridentry for each
                    // category that holds all the properties
                    // of that category.
                    //
                    ArrayList propList = new ArrayList();
                    IDictionaryEnumerator enumBins = (IDictionaryEnumerator)bins.GetEnumerator();
                    while (enumBins.MoveNext()) {
                        ArrayList bin = (ArrayList)enumBins.Value;
                        if (bin != null) {
                            string category = (string)enumBins.Key;
                            if (bin.Count > 0) {
                                GridEntry[] rgpes = new GridEntry[bin.Count];
                                bin.CopyTo(rgpes, 0);
                                try {
                                    propList.Add(new CategoryGridEntry(this.ownerGrid, this, category, rgpes));
                                }
                                catch {
                                }
                            }
                        }
                    }

                    childEntries = new GridEntry[propList.Count];
                    propList.CopyTo(childEntries, 0);
                    StringSorter.Sort(childEntries);
                    
                    ChildCollection.Clear();
                    ChildCollection.AddRange(childEntries);
              }
           }
        }
       }
}
