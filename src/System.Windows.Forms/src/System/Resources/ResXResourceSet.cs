// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Resources
{

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using System;
    using System.Windows.Forms;
    using System.Reflection;
    using Microsoft.Win32;
    using System.Drawing;
    using System.IO;
    using System.ComponentModel;
    using System.Collections;
    using System.Resources;

    /// <summary>
    ///     ResX resource set.
    /// </summary>
    public class ResXResourceSet : ResourceSet
    {

        /// <summary>
        ///     Creates a resource set for the specified file.
        /// </summary>
        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // Shipped like this in Everett.
        ]
        public ResXResourceSet(string fileName) : base(new ResXResourceReader(fileName))
        {
            ReadResources();
        }

        /// <summary>
        ///     Creates a resource set for the specified stream.
        /// </summary>
        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // Shipped like this in Everett.
        ]
        public ResXResourceSet(Stream stream) : base(new ResXResourceReader(stream))
        {
            ReadResources();
        }

        /// <summary>
        ///     Gets the default reader type associated with this set.
        /// </summary>
        public override Type GetDefaultReader()
        {
            return typeof(ResXResourceReader);
        }

        /// <summary>
        ///     Gets the default writer type associated with this set.
        /// </summary>
        public override Type GetDefaultWriter()
        {
            return typeof(ResXResourceWriter);
        }
    }
}
