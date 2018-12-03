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
    ///     <para>
    ///         Imports ActiveX controls and generates a wrapper that can be accessed by a
    ///         designer.
    ///     </para>
    /// </summary>
    public class AxImporter
    {
        public AxImporter(Options options)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
        public sealed class Options
        {
            /// The flag that controls when we sign the generated assemblies.
            public bool delaySign = false;

            /// Flag that controls whether we are to generate sources for the ActiveX control wrapper.
            public bool genSources = false;

            /// Regardless of which version of the library is registered, if at all, use the 
            /// path supplied on the command line for AxImp to generate the Windows Forms wrappers.
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
            //This class is used to communicate option values between components, it does not have any functionality which might modify these values.
            public bool ignoreRegisteredOcx;

            /// The file containing the strong name key container for the generated assemblies.
            public string keyContainer = null;

            /// The file containing the strong name key for the generated assemblies.
            public string keyFile = null;

            /// The strong name used for the generated assemblies.
            public StrongNameKeyPair keyPair = null;

            /// Flag that controls whether we should output errors in the MSBuild format.
            public bool msBuildErrors = false;

            /// Flag that controls whether we should show the logo.
            public bool noLogo = false;

            /// The output directory for all the generated assemblies.
            public string outputDirectory = null;

            /// The path-included filename of type library containing the definition of the ActiveX control.
            public string outputName = null;

            /// The flag that controls whether we try to overwrite existing generated assemblies.
            public bool overwriteRCW = false;
            /// The public key used to sign the generated assemblies.
            public byte[] publicKey = null;

            /// The object that allows us to resolve types and references needed to generate assemblies.
            public IReferenceResolver references = null;

            /// Flag that controls the output generated.
            public bool silentMode = false;

            /// Flag that controls the output generated.
            public bool verboseMode = false;
        }

        /// <summary>
        ///     The Reference Resolver service will try to look through the references it can obtain,
        ///     for a reference that matches the given criterion. For now, the only kind of references
        ///     it can look for are COM (RCW) references and ActiveX wrapper references.
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

namespace System.Runtime.InteropServices
{

    // UCOMITypeLib is not yet ported to interop on core.

    [Guid("00020402-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface UCOMITypeLib
    {
        [PreserveSig]
        int GetTypeInfoCount();

        //void GetTypeInfo(int index, out UCOMITypeInfo ppTI);
        void GetTypeInfoType(int index, out TYPEKIND pTKind);

        //void GetTypeInfoOfGuid(ref Guid guid, out UCOMITypeInfo ppTInfo);
        void GetLibAttr(out IntPtr ppTLibAttr);

        //void GetTypeComp(out UCOMITypeComp ppTComp);
        void GetDocumentation(int index, out string strName, out string strDocString, out int dwHelpContext,
            out string strHelpFile);

        [return: MarshalAs(UnmanagedType.Bool)]
        bool IsName([MarshalAs(UnmanagedType.LPWStr)] string szNameBuf, int lHashVal);

        //void FindName([MarshalAs(UnmanagedType.LPWStr)] String szNameBuf, int lHashVal, [MarshalAs(UnmanagedType.LPArray), Out] UCOMITypeInfo[] ppTInfo, [MarshalAs(UnmanagedType.LPArray), Out] int[] rgMemId, ref Int16 pcFound);
        [PreserveSig]
        void ReleaseTLibAttr(IntPtr pTLibAttr);
    }

}
