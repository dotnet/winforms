// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Imports ActiveX controls and generates a wrapper that can be accessed by a designer.
    /// </summary>
    public class AxImporter
    {
        public AxImporter(Options options)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public sealed class Options
        {
            /// <summary>
            ///  The flag that controls when we sign the generated assemblies.
            /// </summary>
            public bool delaySign = false;

            /// <summary>
            ///  Flag that controls whether we are to generate sources for the ActiveX control wrapper.
            /// </summary>
            public bool genSources = false;

            /// <summary>
            ///  Regardless of which version of the library is registered, if at all, use the
            ///  path supplied on the command line for AxImp to generate the Windows Forms wrappers.
            /// </summary>
            public bool ignoreRegisteredOcx;

            /// <summary>
            ///  The file containing the strong name key container for the generated assemblies.
            /// </summary>
            public string keyContainer = null;

            /// <summary>
            ///  The file containing the strong name key for the generated assemblies.
            /// </summary>
            public string keyFile = null;

            /// <summary>
            ///  The strong name used for the generated assemblies.
            /// </summary>
            public StrongNameKeyPair keyPair = null;

            /// <summary>
            ///  Flag that controls whether we should output errors in the MSBuild format.
            /// </summary>
            public bool msBuildErrors = false;

            /// <summary>
            ///  Flag that controls whether we should show the logo.
            /// </summary>
            public bool noLogo = false;

            /// <summary>
            ///  The output directory for all the generated assemblies.
            /// <summary>
            public string outputDirectory = null;

            /// <summary>
            ///  The path-included filename of type library containing the definition of the ActiveX control.
            /// </summary>
            public string outputName = null;

            /// <summary>
            ///  The flag that controls whether we try to overwrite existing generated assemblies.
            /// </summary>
            public bool overwriteRCW = false;

            /// <summary>
            ///  The public key used to sign the generated assemblies.
            /// </summary>
            public byte[] publicKey = null;

            /// <summary>
            ///  The object that allows us to resolve types and references needed to generate assemblies.
            /// </summary>
            public IReferenceResolver references = null;

            /// <summary>
            ///  Flag that controls the output generated.
            /// </summary>
            public bool silentMode = false;

            /// <summary>
            ///  Flag that controls the output generated.
            /// </summary>
            public bool verboseMode = false;
        }

        /// <summary>
        ///  The Reference Resolver service will try to look through the references it can obtain,
        ///  for a reference that matches the given criterion. For now, the only kind of references
        ///  it can look for are COM (RCW) references and ActiveX wrapper references.
        /// </summary>
        public interface IReferenceResolver
        {
            string ResolveManagedReference(string assemName);

            string ResolveComReference(UCOMITypeLib typeLib);

            string ResolveComReference(AssemblyName name);

            string ResolveActiveXReference(UCOMITypeLib typeLib);
        }
    }
}
