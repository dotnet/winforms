// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  DesignerLoader.  This class is responsible for loading a designer document.  
    ///  Where and how this load occurs is a private matter for the designer loader.
    ///  The designer loader will be handed to an IDesignerHost instance.  This instance, 
    ///  when it is ready to load the document, will call BeginLoad, passing an instance
    ///  of IDesignerLoaderHost.  The designer loader will load up the design surface
    ///  using the host interface, and call EndLoad on the interface when it is done.
    ///  The error collection passed into EndLoad should be empty or null to indicate a
    ///  successful load, or it should contain a collection of exceptions that 
    ///  describe the error.
    ///
    ///  Once a document is loaded, the designer loader is also responsible for
    ///  writing any changes made to the document back whatever storage the
    ///  loader used when loading the document.  
    /// </summary>
    public abstract partial class CodeDomDesignerLoader : BasicDesignerLoader, INameCreationService, IDesignerSerializationService
    {
        private static readonly TraceSwitch s_traceCDLoader = new TraceSwitch("CodeDomDesignerLoader", "Trace CodeDomDesignerLoader");
        private static readonly CodeMarkers s_codemarkers = CodeMarkers.Instance;
        private static readonly int s_stateCodeDomDirty = BitVector32.CreateMask();                                 // True if the code dom tree is dirty, meaning it must be integrated back with the code file.
        private static readonly int s_stateCodeParserChecked = BitVector32.CreateMask(s_stateCodeDomDirty);         // True if we have searched for a parser.
        private static readonly int s_stateOwnTypeResolution = BitVector32.CreateMask(s_stateCodeParserChecked);    // True if we have added our own type resolution service

        // State for the designer loader.
        private BitVector32 _state = new BitVector32();
        private IExtenderProvider[] _extenderProviders;
        private IExtenderProviderService _extenderProviderService;

        // State for the code dom parser / generator
        private ICodeGenerator _codeGenerator;

        // The following fields are setup by EnsureDocument and deleted by ClearDocument.
        private CodeDomSerializer _rootSerializer;
        private TypeCodeDomSerializer _typeSerializer;
        private CodeCompileUnit _documentCompileUnit;
        private CodeNamespace _documentNamespace;
        private CodeTypeDeclaration _documentType;

        /// <summary>
        ///  This abstract property returns the code dom provider that should
        ///  be used by this designer loader.
        /// </summary>
        protected abstract CodeDomProvider CodeDomProvider { get; }

        /// <summary>
        ///  The TypeResolutionService property returns a type resolution service that the code dom 
        ///  serializers will use when resolving types.  The CodeDomDesignerLoader automatically 
        ///  adds this type resolver as a service to the service container during Initialize if it
        ///  is non-null.  While the type resolution service is optional in many scenarios, it is required for 
        ///  code interpretation because source code contains type names, but no assembly references.
        /// </summary>
        protected abstract ITypeResolutionService TypeResolutionService { get; }

        /// <summary>
        ///  This is the reverse of EnsureDocument.  It clears the document state which will
        ///  cause us to parse the next time we need to access it.
        /// </summary>
        private void ClearDocument()
        {
            if (_documentType != null)
            {
                LoaderHost.RemoveService(typeof(CodeTypeDeclaration));
                _documentType = null;
                _documentNamespace = null;
                _documentCompileUnit = null;
                _rootSerializer = null;
                _typeSerializer = null;
            }
        }

        /// <summary>
        ///  Disposes this designer loader.  The designer host will call 
        ///  this method when the design document itself is being destroyed.  
        ///  Once called, the designer loader will never be called again.
        ///  This implementation flushes any changes and removes any
        ///  previously added services.
        /// </summary>
        public override void Dispose()
        {
            if (GetService(typeof(IComponentChangeService)) is IComponentChangeService cs)
            {
                cs.ComponentRemoved -= new ComponentEventHandler(this.OnComponentRemoved);
                cs.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
            }

            if (GetService(typeof(IDesignerHost)) is IDesignerHost host)
            {
                host.RemoveService(typeof(INameCreationService));
                host.RemoveService(typeof(IDesignerSerializationService));
                host.RemoveService(typeof(ComponentSerializationService));

                if (_state[s_stateOwnTypeResolution])
                {
                    host.RemoveService(typeof(ITypeResolutionService));
                    _state[s_stateOwnTypeResolution] = false;
                }
            }

            if (_extenderProviderService != null)
            {
                foreach (IExtenderProvider p in _extenderProviders)
                {
                    _extenderProviderService.RemoveExtenderProvider(p);
                }
            }

            base.Dispose();
        }

        /// <summary>
        ///  Internal debug method to dump a code dom tree to text.
        /// </summary>
#if DEBUG
        internal static void DumpTypeDeclaration(CodeTypeDeclaration typeDecl)
        {
            if (typeDecl == null || !s_traceCDLoader.TraceVerbose)
            {
                return;
            }

#pragma warning disable 618
            ICodeGenerator codeGenerator = new Microsoft.CSharp.CSharpCodeProvider().CreateGenerator();
#pragma warning restore 618
            StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);

            try
            {
                codeGenerator.GenerateCodeFromType(typeDecl, sw, null);
            }
            catch (Exception ex)
            {
                sw.WriteLine("Error during declaration dump: " + ex.Message);
            }

            // spit this line by line so it respects the indent.
            StringReader sr = new StringReader(sw.ToString());

            for (string ln = sr.ReadLine(); ln != null; ln = sr.ReadLine())
            {
                Debug.WriteLine(ln);
            }
        }
#endif

        /// returns true if the given type has a root designer.
        private bool HasRootDesignerAttribute(Type t)
        {
            AttributeCollection attributes = TypeDescriptor.GetAttributes(t);

            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i] is DesignerAttribute da)
                {
                    Type attributeBaseType = Type.GetType(da.DesignerBaseTypeName);

                    if (attributeBaseType != null && attributeBaseType == typeof(IRootDesigner))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///  This ensures that we can load the code dom elements into a
        ///  document.  It ensures that there is a code dom provider, that
        ///  the provider can parse the code, and that the returned 
        ///  code compile unit contains a class that can be deserialized
        ///  through a code dom serializer.  During all of this checking
        ///  it establishes state in our member variables so that 
        ///  calls after it can assume that the variables are set.  This
        ///  will throw a human readable exception if any part of the
        ///  process fails.
        /// </summary>
        private void EnsureDocument(IDesignerSerializationManager manager)
        {
            Debug.Assert(manager != null, "Should pass a serialization manager into EnsureDocument");

            // If we do not yet have the compile unit, ask for it.
            if (_documentCompileUnit == null)
            {
                Debug.Assert(_documentType == null && _documentNamespace == null, "We have no compile unit but we still have a type or namespace.  Our state is inconsistent.");
                _documentCompileUnit = Parse();

                if (_documentCompileUnit == null)
                {
                    Exception ex = new NotSupportedException(SR.CodeDomDesignerLoaderNoLanguageSupport);
                    ex.HelpLink = SR.CodeDomDesignerLoaderNoLanguageSupport;

                    throw ex;
                }
            }

            // Search namespaces and types to identify something that we can load.
            if (_documentType == null)
            {
                // We keep track of any failures here.  If we failed to find a type this
                // array list will contain a list of strings listing what we have tried.
                ArrayList failures = null;
                bool firstClass = true;

                if (_documentCompileUnit.UserData[typeof(InvalidOperationException)] != null)
                {
                    if (_documentCompileUnit.UserData[typeof(InvalidOperationException)] is InvalidOperationException invalidOp)
                    {
                        _documentCompileUnit = null; // not efficient but really a corner case...

                        throw invalidOp;
                    }
                }

                // Look in the compile unit for a class we can load.  The first one we find
                // that has an appropriate serializer attribute, we take.
                foreach (CodeNamespace ns in _documentCompileUnit.Namespaces)
                {
                    foreach (CodeTypeDeclaration typeDecl in ns.Types)
                    {
                        // Uncover the base type of this class.  In case we totally fail
                        // we document each time we were unable to load a particular class.
                        Type baseType = null;

                        foreach (CodeTypeReference typeRef in typeDecl.BaseTypes)
                        {
                            Type t = LoaderHost.GetType(CodeDomSerializerBase.GetTypeNameFromCodeTypeReference(manager, typeRef));

                            if (t != null && !(t.IsInterface))
                            {
                                baseType = t;
                                break;
                            }

                            if (t == null)
                            {
                                if (failures == null)
                                {
                                    failures = new ArrayList();
                                }

                                failures.Add(string.Format(SR.CodeDomDesignerLoaderDocumentFailureTypeNotFound, typeDecl.Name, typeRef.BaseType));
                            }
                        }

                        // We have a potential type.  The next step is to examine the type's attributes
                        // and see if there is a root designer serializer attribute that supports the
                        // code dom.
                        if (baseType != null)
                        {
                            bool foundAttribute = false;

                            // Backwards Compat:  RootDesignerSerializer is obsolete, but we need to still
                            // be compatible and read it.
                            // Walk the member attributes for this type, looking for an appropriate serializer attribute.
#pragma warning disable 0618
                            AttributeCollection attributes = TypeDescriptor.GetAttributes(baseType);

                            foreach (Attribute attr in attributes)
                            {
                                if (attr is RootDesignerSerializerAttribute)
                                {
                                    RootDesignerSerializerAttribute ra = (RootDesignerSerializerAttribute)attr;
                                    string typeName = ra.SerializerBaseTypeName;

                                    // This serializer must support a CodeDomSerializer or we're not interested.
                                    if (typeName != null && LoaderHost.GetType(typeName) == typeof(CodeDomSerializer))
                                    {
                                        Type serializerType = LoaderHost.GetType(ra.SerializerTypeName);

                                        if (serializerType != null)
                                        {
                                            foundAttribute = true;

                                            if (firstClass)
                                            {
                                                _rootSerializer = (CodeDomSerializer)Activator.CreateInstance(serializerType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);
                                                break;
                                            }
                                            else
                                            {
                                                throw new InvalidOperationException(string.Format(SR.CodeDomDesignerLoaderSerializerTypeNotFirstType, typeDecl.Name));
                                            }
                                        }
                                    }
                                }
                            }
#pragma warning restore 0618

                            //add a check for root designer -- this allows an extra level of checking so we can skip classes
                            //that cannot be designed.
                            if (_rootSerializer == null && HasRootDesignerAttribute(baseType))
                            {
                                _typeSerializer = manager.GetSerializer(baseType, typeof(TypeCodeDomSerializer)) as TypeCodeDomSerializer;

                                if (!firstClass && _typeSerializer != null)
                                {
                                    _typeSerializer = null;
                                    _documentCompileUnit = null;

                                    throw new InvalidOperationException(string.Format(SR.CodeDomDesignerLoaderSerializerTypeNotFirstType, typeDecl.Name));
                                }
                            }

                            // If we didn't find a serializer for this type, report it.
                            if (_rootSerializer == null && _typeSerializer == null)
                            {
                                if (failures == null)
                                {
                                    failures = new ArrayList();
                                }

                                if (foundAttribute)
                                {
                                    failures.Add(string.Format(SR.CodeDomDesignerLoaderDocumentFailureTypeDesignerNotInstalled, typeDecl.Name, baseType.FullName));
                                }
                                else
                                {
                                    failures.Add(string.Format(SR.CodeDomDesignerLoaderDocumentFailureTypeNotDesignable, typeDecl.Name, baseType.FullName));
                                }
                            }
                        }

                        // If we found a serializer, then we're done.  Save this type and namespace for later use.
                        if (_rootSerializer != null || _typeSerializer != null)
                        {
                            _documentNamespace = ns;
                            _documentType = typeDecl;
                            break;
                        }

                        firstClass = false;
                    }

                    if (_documentType != null)
                    {
                        break;
                    }
                }

                // If we did not get a document type, throw, because we're unable to continue.
                if (_documentType == null)
                {
                    // The entire compile unit needs to be thrown away so 
                    // we can later reparse.
                    _documentCompileUnit = null;

                    // Did we get any reasons why we can't load this document?  If so, synthesize a nice
                    // description to the user.
                    Exception ex;

                    if (failures != null)
                    {
                        StringBuilder builder = new StringBuilder();

                        foreach (string failure in failures)
                        {
                            builder.Append("\r\n");
                            builder.Append(failure);
                        }

                        ex = new InvalidOperationException(string.Format(SR.CodeDomDesignerLoaderNoRootSerializerWithFailures, builder.ToString()));
                        ex.HelpLink = SR.CodeDomDesignerLoaderNoRootSerializer;
                    }
                    else
                    {
                        ex = new InvalidOperationException(SR.CodeDomDesignerLoaderNoRootSerializer);
                        ex.HelpLink = SR.CodeDomDesignerLoaderNoRootSerializer;
                    }

                    throw ex;
                }
                else
                {
                    // We are successful.  At this point, we want to provide some of these
                    // code dom elements as services for outside parties to use.
                    LoaderHost.AddService(typeof(CodeTypeDeclaration), _documentType);
                }
            }

            s_codemarkers.CodeMarker((int)CodeMarkerEvent.perfFXGetDocumentType);
        }

        /// <summary>
        ///  Takes the given code element and integrates it into the existing CodeDom
        ///  tree stored in _documentCompileUnit.  This returns true if any changes
        ///  were made to the tree.
        /// </summary>
        private bool IntegrateSerializedTree(IDesignerSerializationManager manager, CodeTypeDeclaration newDecl)
        {
            EnsureDocument(manager);
            CodeTypeDeclaration docDecl = _documentType;
            bool caseInsensitive = false;
            bool codeDomDirty = false;
            CodeDomProvider provider = CodeDomProvider;

            if (provider != null)
            {
                caseInsensitive = ((provider.LanguageOptions & LanguageOptions.CaseInsensitive) != 0);
            }

            // Update the class name of the code type, in case it is different.
            if (!string.Equals(docDecl.Name, newDecl.Name, caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
            {
                docDecl.Name = newDecl.Name;
                codeDomDirty = true;
            }

            if (!docDecl.Attributes.Equals(newDecl.Attributes))
            {
                docDecl.Attributes = newDecl.Attributes;
                codeDomDirty = true;
            }

            // Now, hash up the member names in the document and use this
            // when determining what to add and what to replace.  In addition,
            // we also build up a set of indexes into approximate locations for
            // inserting fields and methods.
            int fieldInsertLocation = 0;
            bool lockField = false;
            int methodInsertLocation = 0;
            bool lockMethod = false;
            IDictionary docMemberHash = new HybridDictionary(docDecl.Members.Count, caseInsensitive);
            int memberCount = docDecl.Members.Count;

            for (int i = 0; i < memberCount; i++)
            {
                CodeTypeMember member = docDecl.Members[i];
                string memberName;

                if (member is CodeConstructor)
                {
                    memberName = ".ctor";
                }
                else if (member is CodeTypeConstructor)
                {
                    memberName = ".cctor";
                }
                else
                {
                    memberName = member.Name;
                }

                docMemberHash[memberName] = i;

                if (member is CodeMemberField)
                {
                    if (!lockField)
                    {
                        fieldInsertLocation = i;
                    }
                }
                else
                {
                    if (fieldInsertLocation > 0)
                    {
                        lockField = true;
                    }
                }

                if (member is CodeMemberMethod)
                {
                    if (!lockMethod)
                    {
                        methodInsertLocation = i;
                    }
                }
                else
                {
                    if (methodInsertLocation > 0)
                    {
                        lockMethod = true;
                    }
                }
            }

            // Now start looking through the new declaration and process it.
            // We are index driven, so if we need to add new values we put
            // them into an array list, and post process them.
            ArrayList newElements = new ArrayList();

            foreach (CodeTypeMember member in newDecl.Members)
            {
                string memberName;

                if (member is CodeConstructor)
                {
                    memberName = ".ctor";
                }
                else
                {
                    memberName = member.Name;
                }

                object existingSlot = docMemberHash[memberName];

                if (existingSlot != null)
                {
                    int slot = (int)existingSlot;
                    CodeTypeMember existingMember = docDecl.Members[slot];

                    if (existingMember == member)
                    {
                        continue;
                    }

                    if (member is CodeMemberField)
                    {
                        if (existingMember is CodeMemberField)
                        {
                            CodeMemberField docField = (CodeMemberField)existingMember;
                            CodeMemberField newField = (CodeMemberField)member;

                            // We will be case-sensitive always in working out whether to replace the field
                            if ((string.Equals(newField.Name, docField.Name)) && newField.Attributes == docField.Attributes && TypesEqual(newField.Type, docField.Type))
                            {
                                continue;
                            }
                            else
                            {
                                docDecl.Members[slot] = member;
                            }
                        }
                        else
                        {
                            // We adding a field with the same name as a method. This should cause a
                            // compile error, but we don't want to clobber the existing method.
                            newElements.Add(member);
                        }
                    }
                    else if (member is CodeMemberMethod)
                    {
                        if (existingMember is CodeMemberMethod)
                        {
                            // If there is an existing constructor, preserve it.
                            if (!(existingMember is CodeConstructor))
                            {
                                // For methods, we do not want to replace the method; rather, we
                                // just want to replace its contents.  This helps to preserve
                                // the layout of the file.
                                CodeMemberMethod existingMethod = (CodeMemberMethod)existingMember;
                                CodeMemberMethod newMethod = (CodeMemberMethod)member;

                                existingMethod.Statements.Clear();
                                existingMethod.Statements.AddRange(newMethod.Statements);
                            }
                        }
                    }
                    else
                    {
                        docDecl.Members[slot] = member;
                    }

                    codeDomDirty = true;
                }
                else
                {
                    newElements.Add(member);
                }
            }

            // Now, process all new elements.
            foreach (CodeTypeMember member in newElements)
            {
                if (member is CodeMemberField)
                {
                    if (fieldInsertLocation >= docDecl.Members.Count)
                    {
                        docDecl.Members.Add(member);
                    }
                    else
                    {
                        docDecl.Members.Insert(fieldInsertLocation, member);
                    }

                    fieldInsertLocation++;
                    methodInsertLocation++;
                    codeDomDirty = true;
                }
                else if (member is CodeMemberMethod)
                {
                    if (methodInsertLocation >= docDecl.Members.Count)
                    {
                        docDecl.Members.Add(member);
                    }
                    else
                    {
                        docDecl.Members.Insert(methodInsertLocation, member);
                    }

                    methodInsertLocation++;
                    codeDomDirty = true;
                }
                else
                {
                    // For rare members, just add them to the end.
                    docDecl.Members.Add(member);
                    codeDomDirty = true;
                }
            }

            return codeDomDirty;
        }

        /// <summary>
        ///  This method is called immediately after the first time
        ///  BeginLoad is invoked.  This is an appopriate place to
        ///  add custom services to the loader host.  Remember to
        ///  remove any custom services you add here by overriding
        ///  Dispose.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            ServiceCreatorCallback callback = new ServiceCreatorCallback(OnCreateService);

            LoaderHost.AddService(typeof(ComponentSerializationService), callback);
            LoaderHost.AddService(typeof(INameCreationService), this);
            LoaderHost.AddService(typeof(IDesignerSerializationService), this);

            // The code dom desinger loader requires a working ITypeResolutionService to
            // function.  See if someone added one already, and if not, provide
            // our own.
            if (GetService(typeof(ITypeResolutionService)) == null)
            {
                ITypeResolutionService trs = TypeResolutionService;

                if (trs == null)
                {
                    throw new InvalidOperationException(SR.CodeDomDesignerLoaderNoTypeResolution);
                }

                LoaderHost.AddService(typeof(ITypeResolutionService), trs);
                _state[s_stateOwnTypeResolution] = true;
            }

            _extenderProviderService = GetService(typeof(IExtenderProviderService)) as IExtenderProviderService;

            if (_extenderProviderService != null)
            {
                _extenderProviders = new IExtenderProvider[] {
                    new ModifiersExtenderProvider(),
                    new ModifiersInheritedExtenderProvider()
                };

                foreach (IExtenderProvider p in _extenderProviders)
                {
                    _extenderProviderService.AddExtenderProvider(p);
                }
            }
        }

        /// <summary>
        ///  Determines if the designer needs to be reloaded.  It does this
        ///  by examining the code dom tree for changes.  This does not check
        ///  for outside infleuences; the caller should already think a reload
        ///  is needed -- this is just a last optimization.
        /// </summary>
        protected override bool IsReloadNeeded()
        {
            if (!base.IsReloadNeeded())
            {
                return false;
            }

            // If we have no document, we definitely need a reload.
            if (_documentType == null)
            {
                return true;
            }

            // If we can't get to a code dom provider, or if that provider doesn't
            // implement ICodeDomDesignerReload, we can't optimize the reload, so we
            // just assume it is needed.
            if (!(CodeDomProvider is ICodeDomDesignerReload reloader))
            {
                return true;
            }

            bool reload = true;

            // Parse the file and see if we actually need to reload.
            string oldTypeName = _documentType.Name;

            // StartTimingMark();
            try
            {
                ClearDocument();
                EnsureDocument(GetService(typeof(IDesignerSerializationManager)) as IDesignerSerializationManager);
            }
            catch
            {
                // If the document is not in a state where we can get any information
                // from it, we will assume this is a reload.  The error will then
                // be displayed to the user when the designer does actually
                // reload.
            }

            // EndTimingMark("Reload Parse I");
            if (_documentCompileUnit != null)
            {
                reload = reloader.ShouldReloadDesigner(_documentCompileUnit);
                reload |= (_documentType == null || !_documentType.Name.Equals(oldTypeName));
            }

            return reload;
        }

        /// <summary>
        ///  This method should be called by the designer loader service
        ///  when the first dependent load has started.  This initializes
        ///  the state of the code dom loader and prepares it for loading.
        ///  By default, the designer loader provides
        ///  IDesignerLoaderService itself, so this is called automatically.
        ///  If you provide your own loader service, or if you choose not
        ///  to provide a loader service, you are responsible for calling
        ///  this method.  BeginLoad will automatically call this, either
        ///  indirectly by calling AddLoadDependency if IDesignerLoaderService
        ///  is available, or directly if it is not.
        /// </summary>
        protected override void OnBeginLoad()
        {
            // Make sure that we're removed any event sinks we added after we finished the load.
            IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            if (cs != null)
            {
                cs.ComponentRemoved -= new ComponentEventHandler(this.OnComponentRemoved);
                cs.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
            }

            base.OnBeginLoad();
        }

        /// <summary>
        ///  This method is called immediately before the document is unloaded.
        ///  The document may be unloaded in preparation for reload, or 
        ///  if the document failed the load.  If you added document-specific
        ///  services in OnBeginLoad or OnEndLoad, you should remove them
        ///  here.
        /// </summary>
        protected override void OnBeginUnload()
        {
            base.OnBeginUnload();
            ClearDocument();
        }

        /// <summary>
        ///  This is called whenever a component is removed from the design surface.
        /// </summary>
        private void OnComponentRemoved(object sender, ComponentEventArgs e)
        {
            string name = e.Component.Site.Name;
            RemoveDeclaration(name);
        }

        /// <summary>
        ///  Raised by the host when a component is renamed.  Here we dirty ourselves
        ///  and then whack the component declaration.  At the next code gen
        ///  cycle we will recreate the declaration.
        /// </summary>
        private void OnComponentRename(object sender, ComponentRenameEventArgs e)
        {
            OnComponentRename(e.Component, e.OldName, e.NewName);
        }

        /// <summary>
        ///  Callback to create our demand-created services.
        /// </summary>
        private object OnCreateService(IServiceContainer container, Type serviceType)
        {
            if (serviceType == typeof(ComponentSerializationService))
            {
                return new CodeDomComponentSerializationService(LoaderHost);
            }

            Debug.Fail("Called to create unknown service.");

            return null;
        }

        /// <summary>
        ///  This method should be called by the designer loader service
        ///  when all dependent loads have been completed.  This
        ///  "shuts down" the loading process that was initiated by
        ///  BeginLoad.  By default, the designer loader provides
        ///  IDesignerLoaderService itself, so this is called automatically.
        ///  If you provide your own loader service, or if you choose not
        ///  to provide a loader service, you are responsible for calling
        ///  this method.  BeginLoad will automatically call this, either
        ///  indirectly by calling DependentLoadComplete if IDesignerLoaderService
        ///  is available, or directly if it is not.
        /// </summary>
        protected override void OnEndLoad(bool successful, ICollection errors)
        {
            base.OnEndLoad(successful, errors);

            if (!successful)
            {
                return;
            }

            // After a successful load we will want to monitor a bunch of events so we know when
            // to make the loader dirty.
            IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            if (cs == null)
            {
                return;
            }

            cs.ComponentRemoved += new ComponentEventHandler(this.OnComponentRemoved);
            cs.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
        }

        /// <summary>
        ///  This abstract method is called when it is time to
        ///  parse the source code and create a CodeDom tree.
        /// </summary>
        protected abstract CodeCompileUnit Parse();

        /// <summary>
        ///  Overrides BasicDesignerLoader's PerformFlush method to actual
        ///  write out the code dom tree.
        /// </summary>
        protected override void PerformFlush(IDesignerSerializationManager manager)
        {
            CodeTypeDeclaration typeDecl = null;

            // Ask the serializer for the root component to serialize.  This should return
            // a CodeTypeDeclaration, which we will plug into our existing code DOM tree.
            Debug.Assert(_rootSerializer != null || _typeSerializer != null, "What are we saving right now?  Base component has no serializer: " + LoaderHost.RootComponent.GetType().FullName);

            if (_rootSerializer != null)
            {
                typeDecl = _rootSerializer.Serialize(manager, LoaderHost.RootComponent) as CodeTypeDeclaration;
                Debug.Assert(typeDecl != null, "Root CodeDom serializer must return a CodeTypeDeclaration");
            }
            else if (_typeSerializer != null)
            {
                typeDecl = _typeSerializer.Serialize(manager, LoaderHost.RootComponent, LoaderHost.Container.Components);
            }
#if DEBUG
            if (s_traceCDLoader.TraceVerbose)
            {
                Debug.WriteLine("****************** Pre-integrated tree **********************");
                DumpTypeDeclaration(typeDecl);
                EnsureDocument(manager);
                Debug.WriteLine("****************** Live tree **********************");
                DumpTypeDeclaration(_documentType);
            }
#endif
            s_codemarkers.CodeMarker((int)CodeMarkerEvent.perfFXGenerateCodeTreeEnd);

            // Now we must integrate the code DOM tree from the serializer with
            // our own tree.  If changes were made to the tree this will
            // return true.
            if (typeDecl != null && IntegrateSerializedTree(manager, typeDecl))
            {
                s_codemarkers.CodeMarker((int)CodeMarkerEvent.perfFXIntegrateSerializedTreeEnd);
#if DEBUG
                if (s_traceCDLoader.TraceVerbose)
                {
                    Debug.WriteLine("****************** Integrated tree **********************");
                    DumpTypeDeclaration(_documentType);
                }
#endif
                // EndTimingMark("Serialize tree");
                // StartTimingMark();
                Write(_documentCompileUnit);
                // EndTimingMark("Generate unit total");
            }
            else
            {
                Debug.WriteLineIf(s_traceCDLoader.TraceVerbose, "No need to integrate tree; no changes detected");
            }
        }

        /// <summary>
        ///  Overrides BasicDesignerLoader's PerformLoad method to deserialize the
        ///  classes from the code dom.
        /// </summary>
        protected override void PerformLoad(IDesignerSerializationManager manager)
        {
            // This ensures that all of the state for the document is available.  This
            // will throw if state we need to load cannot be obtained.
            EnsureDocument(manager);

            // Ok, now we have a document, and a root serializer and a namespace.  Or,
            // at least we better.
            Debug.Assert(_documentType != null, "EnsureDocument didn't create a document type");
            Debug.Assert(_documentNamespace != null, "EnsureDocument didn't create a document namespace");
            Debug.Assert(_rootSerializer != null || _typeSerializer != null, "EnsureDocument didn't create a root serializer");

            // StartTimingMark();
            s_codemarkers.CodeMarker((int)CodeMarkerEvent.perfFXDeserializeStart);

            if (_rootSerializer != null)
            {
                _rootSerializer.Deserialize(manager, _documentType);
            }
            else
            {
                _typeSerializer.Deserialize(manager, _documentType);
            }

            s_codemarkers.CodeMarker((int)CodeMarkerEvent.perfFXDeserializeEnd);

            // EndTimingMark("Deserialize document");
            string baseComp = string.Format(CultureInfo.CurrentCulture, "{0}.{1}", _documentNamespace.Name, _documentType.Name);
            SetBaseComponentClassName(baseComp);
        }

        /// <summary>
        ///  This virtual method gets override in the VsCodeDomDesignerLoader to call the RenameElement on the 
        ///  ChangeNotificationService to rename the component name through out the project scope.
        /// </summary>
        protected virtual void OnComponentRename(object component, string oldName, string newName)
        {
            if (LoaderHost.RootComponent == component)
            {
                if (_documentType != null)
                {
                    _documentType.Name = newName;
                }

                return;
            }

            if (_documentType == null)
            {
                return;
            }

            CodeTypeMemberCollection members = _documentType.Members;

            for (int i = 0; i < members.Count; i++)
            {
                if (members[i] is CodeMemberField && members[i].Name.Equals(oldName)
                                                  && ((CodeMemberField)members[i]).Type.BaseType.Equals(TypeDescriptor.GetClassName(component)))
                {
                    members[i].Name = newName;
                    break;
                }
            }
        }

        /// <summary>
        ///  This is called when a component is deleted or renamed.  We remove
        ///  the component's declaration here, if it exists.
        /// </summary>
        private void RemoveDeclaration(string name)
        {
            if (_documentType == null)
            {
                return;
            }

            CodeTypeMemberCollection members = _documentType.Members;

            for (int i = 0; i < members.Count; i++)
            {
                if (members[i] is CodeMemberField && members[i].Name.Equals(name))
                {
                    ((IList)members).RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        ///  Simple helper routine that will throw an exception if we need a service, but cannot get
        ///  to it.  You should only throw for missing services that are absolutely essential for
        ///  operation.  If there is a way to gracefully degrade, then you should do it.
        /// </summary>
        private void ThrowMissingService(Type serviceType)
        {
            Exception ex = new InvalidOperationException(string.Format(SR.BasicDesignerLoaderMissingService, serviceType.Name));
            ex.HelpLink = SR.BasicDesignerLoaderMissingService;

            throw ex;
        }

        /// <summary>
        ///  Determines of two type references are equal.
        /// </summary>
        private static bool TypesEqual(CodeTypeReference typeLeft, CodeTypeReference typeRight)
        {
            if (typeLeft.ArrayRank != typeRight.ArrayRank)
            {
                return false;
            }

            if (!typeLeft.BaseType.Equals(typeRight.BaseType))
            {
                return false;
            }

            if (typeLeft.TypeArguments != null && typeRight.TypeArguments == null)
            {
                return false;
            }

            if (typeLeft.TypeArguments == null && typeRight.TypeArguments != null)
            {
                return false;
            }

            if (typeLeft.TypeArguments != null && typeRight.TypeArguments != null)
            {
                if (typeLeft.TypeArguments.Count != typeRight.TypeArguments.Count)
                {
                    return false;
                }

                for (int i = 0; i < typeLeft.TypeArguments.Count; i++)
                {
                    if (!TypesEqual(typeLeft.TypeArguments[i], typeRight.TypeArguments[i]))
                    {
                        return false;
                    }
                }
            }

            if (typeLeft.ArrayRank > 0)
            {
                return TypesEqual(typeLeft.ArrayElementType, typeRight.ArrayElementType);
            }

            return true;
        }

        /// <summary>
        ///  This abstract method is called in response to a Flush
        ///  call when the designer loader is dirty.  It will pass
        ///  a new CodeCompileUnit that represents the source code
        ///  needed to recreate the current designer's component
        ///  graph.
        /// </summary>
        protected abstract void Write(CodeCompileUnit unit);

        /// <summary>
        ///  Deserializes the provided serialization data object and
        ///  returns a collection of objects contained within that
        ///  data.
        /// </summary>
        ICollection IDesignerSerializationService.Deserialize(object serializationData)
        {
            if (!(serializationData is SerializationStore))
            {
                Exception ex = new ArgumentException(SR.CodeDomDesignerLoaderBadSerializationObject);
                ex.HelpLink = SR.CodeDomDesignerLoaderBadSerializationObject;

                throw ex;
            }

            ComponentSerializationService css = GetService(typeof(ComponentSerializationService)) as ComponentSerializationService;

            if (css == null)
            {
                ThrowMissingService(typeof(ComponentSerializationService));
            }

            return css.Deserialize((SerializationStore)serializationData, LoaderHost.Container);
        }

        /// <summary>
        ///  Serializes the given collection of objects and 
        ///  stores them in an opaque serialization data object.
        ///  The returning object fully supports runtime serialization.
        /// </summary>
        object IDesignerSerializationService.Serialize(ICollection objects)
        {
            if (objects == null)
            {
                objects = new object[0];
            }

            ComponentSerializationService css = GetService(typeof(ComponentSerializationService)) as ComponentSerializationService;

            if (css == null)
            {
                ThrowMissingService(typeof(ComponentSerializationService));
            }

            SerializationStore store = css.CreateStore();

            using (store)
            {
                foreach (object o in objects)
                {
                    css.Serialize(store, o);
                }
            }

            return store;
        }

        /// <summary>
        ///  Creates a new name that is unique to all the components
        ///  in the given container.  The name will be used to create
        ///  an object of the given data type, so the service may
        ///  derive a name from the data type's name.
        /// </summary>
        string INameCreationService.CreateName(IContainer container, Type dataType)
        {
            if (dataType == null)
            {
                throw new ArgumentNullException(nameof(dataType));
            }

            string finalName;
            string baseName = dataType.Name;

            // Create a base member name that is a camel casing of the
            // data type name.
            StringBuilder b = new StringBuilder(baseName.Length);

            for (int i = 0; i < baseName.Length; i++)
            {
                if (char.IsUpper(baseName[i]) && (i == 0 || i == baseName.Length - 1 || char.IsUpper(baseName[i + 1])))
                {
                    b.Append(char.ToLower(baseName[i], CultureInfo.CurrentCulture));
                }
                else
                {
                    b.Append(baseName.Substring(i));
                    break;
                }
            }

            b.Replace('`', '_');
            baseName = b.ToString();

            // Now hash up all of the member variable names using a case insensitve hash.
            CodeTypeDeclaration type = _documentType;
            Hashtable memberHash = new Hashtable(StringComparer.CurrentCultureIgnoreCase);

            if (type != null)
            {
                foreach (CodeTypeMember member in type.Members)
                {
                    memberHash[member.Name] = member;
                }
            }

            // VSWhidbey 95065: Only attempt to build a unique name here if we have a valid container
            // against which to check the result. Otherwise, the name build here might be appended to
            // with yet another iterator elsewhere. FWIW, this check is identical to the check used in
            // BaseDesignerLoader's implementation of INameCreationService.CreateName().
            if (container != null)
            {
                int idx = 0;
                bool conflict;

                // Now loop until we find a name that hasn't been used.
                do
                {
                    idx++;
                    conflict = false;
                    finalName = string.Format(CultureInfo.CurrentCulture, "{0}{1}", baseName, idx.ToString(CultureInfo.InvariantCulture));

                    if (container != null && container.Components[finalName] != null)
                    {
                        conflict = true;
                    }

                    if (!conflict && memberHash[finalName] != null)
                    {
                        conflict = true;
                    }

                } while (conflict);
            }
            else
            {
                finalName = baseName;
            }

            // And validate the new name against the code dom's code
            // generator to ensure it's not a keyword.
            if (_codeGenerator == null)
            {
                CodeDomProvider provider = CodeDomProvider;

                if (provider != null)
                {
#pragma warning disable 618
                    _codeGenerator = provider.CreateGenerator();
#pragma warning restore 618
                }
            }

            if (_codeGenerator != null)
            {
                finalName = _codeGenerator.CreateValidIdentifier(finalName);
            }

            return finalName;
        }

        /// <summary>
        ///  Determines if the given name is valid.  A name 
        ///  creation service may have rules defining a valid
        ///  name, and this method allows the sevice to enforce
        ///  those rules.
        /// </summary>
        bool INameCreationService.IsValidName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.Length == 0)
            {
                return false;
            }

            if (_codeGenerator == null)
            {
                CodeDomProvider provider = CodeDomProvider;

                if (provider != null)
                {
#pragma warning disable 618
                    _codeGenerator = provider.CreateGenerator();
#pragma warning restore 618
                }
            }

            if (_codeGenerator != null)
            {
                if (!_codeGenerator.IsValidIdentifier(name))
                {
                    return false;
                }

                if (!_codeGenerator.IsValidIdentifier(name + "Handler"))
                {
                    return false;
                }
            }

            // We don't validate against the type members if we're loading,
            // because during load these members are being added by the 
            // parser, so of course there will be duplicates.
            if (!Loading)
            {
                CodeTypeDeclaration type = _documentType;

                if (type != null)
                {
                    foreach (CodeTypeMember member in type.Members)
                    {
                        if (string.Equals(member.Name, name, StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                    }
                }

                // If the designer has been modified there is a chance that
                // the document type does not have all the necessary
                // members in it yet.  So, we need to check the container
                // as well.
                if (Modified && LoaderHost.Container.Components[name] != null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///  Determines if the given name is valid.  A name 
        ///  creation service may have rules defining a valid
        ///  name, and this method allows the sevice to enforce
        ///  those rules.  It is similar to IsValidName, except
        ///  that this method will throw an exception if the
        ///  name is invalid.  This allows implementors to provide
        ///  rich information in the exception message.
        /// </summary>
        void INameCreationService.ValidateName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.Length == 0)
            {
                Exception ex = new ArgumentException(SR.CodeDomDesignerLoaderInvalidBlankIdentifier);
                ex.HelpLink = SR.CodeDomDesignerLoaderInvalidIdentifier;

                throw ex;
            }

            if (_codeGenerator == null)
            {
                CodeDomProvider provider = CodeDomProvider;

                if (provider != null)
                {
#pragma warning disable 618
                    _codeGenerator = provider.CreateGenerator();
#pragma warning restore 618
                }
            }

            if (_codeGenerator != null)
            {
                _codeGenerator.ValidateIdentifier(name);

                try
                {
                    // We add something arbitrary and try to validate that. This is because we want to make sure
                    // adding something is going to be fine, if not our event handler name generation will break in
                    // case this identifier is something like a VB escaped keyword. For example, [public] is fine
                    // but [public]_Click is not.
                    _codeGenerator.ValidateIdentifier(name + "_");
                }
                catch
                {
                    // we have to change the exception back to the original name
                    Exception ex = new ArgumentException(string.Format(SR.CodeDomDesignerLoaderInvalidIdentifier, name));
                    ex.HelpLink = SR.CodeDomDesignerLoaderInvalidIdentifier;

                    throw ex;
                }
            }

            if (Loading)
            {
                return;
            }

            // We don't validate against the type members if we're loading,
            // because during load these members are being added by the 
            // parser, so of course there will be duplicates.
            bool dup = false;
            CodeTypeDeclaration type = _documentType;

            if (type != null)
            {
                foreach (CodeTypeMember member in type.Members)
                {
                    if (string.Equals(member.Name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        dup = true;
                        break;
                    }
                }
            }

            // If the designer has been modified there is a chance that
            // the document type does not have all the necessary
            // members in it yet.  So, we need to check the container
            // as well.
            if (!dup && Modified && LoaderHost.Container.Components[name] != null)
            {
                dup = true;
            }

            if (dup)
            {
                Exception ex = new ArgumentException(string.Format(SR.CodeDomDesignerLoaderDupComponentName, name));
                ex.HelpLink = SR.CodeDomDesignerLoaderDupComponentName;

                throw ex;
            }
        }
    }
}
