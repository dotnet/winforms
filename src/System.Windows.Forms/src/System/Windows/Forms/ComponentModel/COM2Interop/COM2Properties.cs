// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop {
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Win32;


    /// <include file='doc\COM2Properties.uex' path='docs/doc[@for="Com2Properties"]/*' />
    /// <devdoc>
    /// This class is responsible for managing a set or properties for a native object.  It determines
    /// when the properties need to be refreshed, and owns the extended handlers for those properties.
    /// </devdoc>
    internal class Com2Properties {
    
        private static TraceSwitch DbgCom2PropertiesSwitch = new TraceSwitch("DbgCom2Properties", "Com2Properties: debug Com2 properties manager");
        
        
        
        /// <include file='doc\COM2Properties.uex' path='docs/doc[@for="Com2Properties.AGE_THRESHHOLD"]/*' />
        /// <devdoc>
        /// This is the interval that we'll hold props for.  If someone doesn't touch an object
        /// for this amount of time, we'll dump the properties from our cache.
        /// 
        /// 5 minutes -- ticks are 1/10,000,000th of a second
        /// </devdoc>
        private static long AGE_THRESHHOLD = (long)(10000000L * 60L * 5L);

        
        /// <include file='doc\COM2Properties.uex' path='docs/doc[@for="Com2Properties.weakObjRef"]/*' />
        /// <devdoc>
        /// This is the object that gave us the properties.  We hold a WeakRef so we don't addref the object.
        /// </devdoc>
        internal WeakReference weakObjRef;
        
        /// <include file='doc\COM2Properties.uex' path='docs/doc[@for="Com2Properties.props"]/*' />
        /// <devdoc>
        /// This is our list of properties.
        /// </devdoc>
        private Com2PropertyDescriptor[] props;
        
        /// <include file='doc\COM2Properties.uex' path='docs/doc[@for="Com2Properties.defaultIndex"]/*' />
        /// <devdoc>
        /// The index of the default property
        /// </devdoc>
        private int           defaultIndex = -1;
        
        
        /// <include file='doc\COM2Properties.uex' path='docs/doc[@for="Com2Properties.touchedTime"]/*' />
        /// <devdoc>
        /// The timestamp of the last operation on this property manager, usually
        /// when the property list was fetched.
        /// </devdoc>
        private long          touchedTime;  

        /// <devdoc>
        /// For non-IProvideMultipleClassInfo ITypeInfos, this is the version number on the last
        /// ITypeInfo we looked at.  If this changes, we know we need to dump the cache.
        /// </devdoc>
        private long[]       typeInfoVersions;
       

#if DEBUG
        private string        dbgObjName;
        private string        dbgObjClass;
#endif

        private int          alwaysValid = 0;

        /// <include file='doc\COM2Properties.uex' path='docs/doc[@for="Com2Properties.extendedInterfaces"]/*' />
        /// <devdoc>
        /// These are the interfaces we recognize for extended browsing.
        /// </devdoc>
        private static Type[] extendedInterfaces = new Type[]{
                                                        typeof(NativeMethods.ICategorizeProperties),
                                                        typeof(NativeMethods.IProvidePropertyBuilder),
                                                        typeof(NativeMethods.IPerPropertyBrowsing),
                                                        typeof(NativeMethods.IVsPerPropertyBrowsing),
                                                        typeof(NativeMethods.IManagedPerPropertyBrowsing)};

        /// <include file='doc\COM2Properties.uex' path='docs/doc[@for="Com2Properties.extendedInterfaceHandlerTypes"]/*' />
        /// <devdoc>
        /// These are the classes of handlers corresponding to the extended
        /// interfaces above.
        /// </devdoc>
        private static Type[] extendedInterfaceHandlerTypes = new Type[]{
                                                        typeof(Com2ICategorizePropertiesHandler),
                                                        typeof(Com2IProvidePropertyBuilderHandler),
                                                        typeof(Com2IPerPropertyBrowsingHandler),
                                                        typeof(Com2IVsPerPropertyBrowsingHandler),
                                                        typeof(Com2IManagedPerPropertyBrowsingHandler)};
                                                


        public event EventHandler Disposed;


        /// <include file='doc\COM2Properties.uex' path='docs/doc[@for="Com2Properties.Com2Properties"]/*' />
        /// <devdoc>
        /// Default ctor.
        /// </devdoc>
        public Com2Properties(object obj, Com2PropertyDescriptor[] props, int defaultIndex) {
#if DEBUG
            ComNativeDescriptor cnd = new ComNativeDescriptor();
            this.dbgObjName = cnd.GetName(obj);
            if (this.dbgObjName == null) {
                this.dbgObjName = "(null)";
            }
            this.dbgObjClass = cnd.GetClassName(obj);
            if (this.dbgObjClass == null) {
                this.dbgObjClass = "(null)";
            }
            if (DbgCom2PropertiesSwitch.TraceVerbose) Debug.WriteLine("Creating Com2Properties for object " + dbgObjName + ", class=" + dbgObjClass);
#endif
            
            // set up our variables
            SetProps(props);
            weakObjRef = new WeakReference(obj);

            this.defaultIndex = defaultIndex;

            typeInfoVersions = GetTypeInfoVersions(obj);

            touchedTime = DateTime.Now.Ticks;

        }

        internal bool AlwaysValid {
            get {
                return this.alwaysValid > 0;
            }
            set {
                if (value) {
                    if (alwaysValid == 0 && !CheckValid()) {
                        return;
                    }
                    this.alwaysValid++;
                }
                else {
                    if (alwaysValid > 0) {
                        this.alwaysValid--;
                    }
                }
            }
        }

        /// <include file='doc\COM2Properties.uex' path='docs/doc[@for="Com2Properties.DefaultProperty"]/*' />
        /// <devdoc>
        /// Retrieve the default property.
        /// </devdoc>
        public Com2PropertyDescriptor DefaultProperty{
            get{
                if (!CheckValid(true)) {
                    return null;
                }
                if (defaultIndex == -1) {
                    if (props.Length > 0) {
                        return props[0];
                    }
                    else {
                        return null;
                    }
                }
                Debug.Assert(defaultIndex < props.Length, "Whoops! default index is > props.Length");
                return props[defaultIndex];
            }
        }

        /// <include file='doc\COM2Properties.uex' path='docs/doc[@for="Com2Properties.TargetObject"]/*' />
        /// <devdoc>
        /// The object that created the list of properties.  This will
        /// return null if the timeout has passed or the ref has died.
        /// </devdoc>
        public object TargetObject{
            get{
                if (!CheckValid(false) || touchedTime == 0) {
#if DEBUG
                    if (DbgCom2PropertiesSwitch.TraceVerbose) Debug.WriteLine("CheckValid called on dead object!");
#endif
                    return null;
                }
                return weakObjRef.Target;
            }
        }

        /// <include file='doc\COM2Properties.uex' path='docs/doc[@for="Com2Properties.TicksSinceTouched"]/*' />
        /// <devdoc>
        /// How long since these props have been queried.
        /// </devdoc>
        public long TicksSinceTouched{
            get{
                if (touchedTime == 0) {
                    return 0;
                }
                return DateTime.Now.Ticks - touchedTime;
            }
        }

        /// <include file='doc\COM2Properties.uex' path='docs/doc[@for="Com2Properties.Properties"]/*' />
        /// <devdoc>
        /// Returns the list of properties
        /// </devdoc>
        public Com2PropertyDescriptor[] Properties{
            get{
                CheckValid(true);
                if (touchedTime == 0 || props == null) {
                    return null;
                }
                touchedTime = DateTime.Now.Ticks;

                // refresh everyone!
                for (int i = 0; i < props.Length; i++) {
                    props[i].SetNeedsRefresh(Com2PropertyDescriptorRefresh.All, true);
                }

#if DEBUG
                if (DbgCom2PropertiesSwitch.TraceVerbose) Debug.WriteLine("Returning prop array for object " + dbgObjName + ", class=" + dbgObjClass);
#endif
                return props;
            }
        }

        /// <include file='doc\COM2Properties.uex' path='docs/doc[@for="Com2Properties.TooOld"]/*' />
        /// <devdoc>
        /// Should this guy be refreshed because of old age?
        /// </devdoc>
        public bool TooOld{
            get{
                // check if the property is valid but don't dispose it if it's not
                CheckValid(false, false);
                if (touchedTime == 0) {
                    return false;
                }
                return TicksSinceTouched > AGE_THRESHHOLD;
            }
        }

        /// <include file='doc\COM2Properties.uex' path='docs/doc[@for="Com2Properties.AddExtendedBrowsingHandlers"]/*' />
        /// <devdoc>
        /// Checks the source object for eache extended browsing inteface
        /// listed in extendedInterfaces and creates a handler from extendedInterfaceHandlerTypes
        /// to handle it.
        /// </devdoc>
        public void AddExtendedBrowsingHandlers(Hashtable handlers) {

            object target = this.TargetObject;
            if (target == null) {
                return;
            }


            // process all our registered types
            Type t;
            for (int i = 0; i < extendedInterfaces.Length; i++) {
                t = extendedInterfaces[i];
                
                // is this object an implementor of the interface?
                //
                if (t.IsInstanceOfType(target)) {
                
                    // since handlers must be stateless, check to see if we've already
                    // created one of this type
                    //
                    Com2ExtendedBrowsingHandler handler = (Com2ExtendedBrowsingHandler)handlers[t];
                    if (handler == null) {
                        handler = (Com2ExtendedBrowsingHandler)Activator.CreateInstance(extendedInterfaceHandlerTypes[i]);
                        handlers[t] = handler;
                    }
                    
                    // make sure we got the right one
                    //
                    if (t.IsAssignableFrom(handler.Interface)) {
#if DEBUG
                        if (DbgCom2PropertiesSwitch.TraceVerbose) Debug.WriteLine("Adding browsing handler type " + handler.Interface.Name + " to object " + dbgObjName + ", class=" + dbgObjClass);
#endif
                        // allow the handler to attach itself to the appropriate properties
                        //
                        handler.SetupPropertyHandlers(props);
                    }
                    else {
                        throw new ArgumentException(string.Format(SR.COM2BadHandlerType, t.Name, handler.Interface.Name));
                    }
                }
            }
        }

       
        public void Dispose() {
#if DEBUG
            if (DbgCom2PropertiesSwitch.TraceVerbose) Debug.WriteLine("Disposing property manager for " + dbgObjName + ", class=" + dbgObjClass);
#endif

           if (props != null) {

                if (Disposed != null) {

                    Disposed(this, EventArgs.Empty);
                }
            
                weakObjRef = null;
                props = null;
                touchedTime = 0;
           }
        }

        public bool CheckValid() {
            return CheckValid(false);
        }

        /// <include file='doc\COM2Properties.uex' path='docs/doc[@for="Com2Properties.CheckValid"]/*' />
        /// <devdoc>
        /// Make sure this property list is still valid.
        ///
        /// 1) WeakRef is still alive
        /// 2) Our timeout hasn't passed
        /// </devdoc>
        public bool CheckValid(bool checkVersions) {
            return CheckValid(checkVersions, true);
        }

        
        /// <devdoc>
        /// Gets a list of version longs for each type info in the COM object
        /// representing hte current version stamp, function and variable count.
        /// If any of these things change, we'll re-fetch the properties.
        /// </devdoc>
        private long[] GetTypeInfoVersions(object comObject) {

            // get type infos
            //
            UnsafeNativeMethods.ITypeInfo[] pTypeInfos = Com2TypeInfoProcessor.FindTypeInfos(comObject, false);

            // build up the info.
            //
            long[] versions = new long[pTypeInfos.Length];
            for (int i = 0; i < pTypeInfos.Length; i++) {
                versions [i] = GetTypeInfoVersion(pTypeInfos[i]);
            }           
            return versions;
        }

        private static int countOffset = -1;
        private static int versionOffset = -1;

        // we define a struct here so we can use unsafe code to marshal it
        // as a blob of memory.  This is here as a reference to how big and where the members are.
        //
        /*private struct tagTYPEATTR {
            public Guid guid;                       //16
            public   int lcid;                      // 4
            public   int dwReserved;                // 4
            public   int memidConstructor;          // 4
            public   int memidDestructor;           // 4
            public   IntPtr lpstrSchema;            // platform
            public   int cbSizeInstance;            // 4
            public    int typekind;                 // 4
            public   short cFuncs;                  // 2
            public   short cVars;                   // 2
            public   short cImplTypes;              // 2
            public   short cbSizeVft;               // 2
            public   short cbAlignment;             // 2
            public   short wTypeFlags;              // 2
            public   short wMajorVerNum;            // 2
            public   short wMinorVerNum;            // 2
            
            public   int tdescAlias_unionMember;            
            public   short tdescAlias_vt;            
            public   int idldescType_dwReserved;            
            public   short idldescType_wIDLFlags;
        
        }*/


        // the offset of the cFunc member in the TYPEATTR structure.
        //        
        private static int CountMemberOffset {

                get {
                    if (countOffset == -1) {
                        countOffset = Marshal.SizeOf(typeof(Guid)) + IntPtr.Size + 24;
                    }
                    return countOffset;
                }
        }

        // the offset of the cMajorVerNum member in the TYPEATTR structure.
        //
        private static int VersionOffset {
                get {
                    if (versionOffset == -1) {
                        versionOffset = CountMemberOffset + 12;
                    }
                    return versionOffset;
                }
                    
        }

        private unsafe long GetTypeInfoVersion(UnsafeNativeMethods.ITypeInfo pTypeInfo) {


            IntPtr pTypeAttr = IntPtr.Zero;
            int hr = pTypeInfo.GetTypeAttr(ref pTypeAttr);
            if (!NativeMethods.Succeeded(hr)) {
                return 0;
            }

            System.Runtime.InteropServices.ComTypes.TYPEATTR pTAStruct;
            try {
                try {
                    // just access directly...no marshalling needed!
                    //
                    pTAStruct = *(System.Runtime.InteropServices.ComTypes.TYPEATTR*)pTypeAttr;
                }
                catch {

                    return 0;
                }

                long result = 0;

                // we pull two things out of the struct: the 
                // number of functions and variables, and the version.
                // since they are next to each other, we just pull the memory directly.
                //
                // the cFuncs and cVars are both shorts, so we read them as one block of ints.
                //
                //
                int* pResult = (int*)&result;
                byte* pbStruct = (byte*)&pTAStruct;

                // in the low byte, pull the number of props.
                //
                *pResult = *(int*)(pbStruct + CountMemberOffset);

                // move up to the high word of the long.
                //                
                pResult++;

                // now pull out the version info.
                //
                *pResult = *(int*)(pbStruct + VersionOffset);

                // return that composite long.
                //
                return result;
            }
            finally {
               pTypeInfo.ReleaseTypeAttr(pTypeAttr);
            }
        }

        internal bool CheckValid(bool checkVersions, bool callDispose) {

            if (this.AlwaysValid) {
                return true;
            }

            bool valid = weakObjRef != null && weakObjRef.IsAlive;

            // check the version information for each ITypeInfo the object exposes.
            //
            if (valid && checkVersions) {

               // 
               long[] newTypeInfoVersions = GetTypeInfoVersions(weakObjRef.Target);

               if (newTypeInfoVersions.Length != typeInfoVersions.Length) {
                   valid = false;
               } else {
                   // compare each version number to the old one.
                   //
                   for (int i = 0; i < newTypeInfoVersions.Length; i++) {

                        if (newTypeInfoVersions[i] != typeInfoVersions[i]) {                            
                            valid = false;
                            break;
                        }
                   }
               }

               if (!valid) {

                   // update to the new version list we have.
                   // 
                   typeInfoVersions = newTypeInfoVersions;
               }
            }
            
            if (!valid && callDispose) {
                // weak ref has died, so remove this from the hash table
                //
#if DEBUG
                if (DbgCom2PropertiesSwitch.TraceVerbose) Debug.WriteLine("Disposing reference to object " + dbgObjName + ", class=" + dbgObjClass + " (weakRef " + (weakObjRef == null ? "null" : "dead") + ")");
#endif

                Dispose();
            }

            return valid;
        }

        /// <include file='doc\COM2Properties.uex' path='docs/doc[@for="Com2Properties.SetProps"]/*' />
        /// <devdoc>
        /// Set the props for this object, and notify each property
        /// that we are now it's manager
        /// </devdoc>
        internal void SetProps(Com2PropertyDescriptor[] props) {
            this.props = props;
            if (props != null) {
                for (int i = 0; i < props.Length; i++) {
                    props[i].PropertyManager = this;
                }
            }
        }        
    }
}
