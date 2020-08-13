// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

// Awkward, but necessary to expose Interop based internals to other test libaries
[assembly: InternalsVisibleTo("System.Windows.Forms.Primitives.Tests, PublicKey=00000000000000000400000000000000")]
[assembly: InternalsVisibleTo("System.Windows.Forms.Tests, PublicKey=00000000000000000400000000000000")]
[assembly: InternalsVisibleTo("System.Windows.Forms.TestUtilities, PublicKey=00000000000000000400000000000000")]
[assembly: InternalsVisibleTo("System.Windows.Forms.Design.Tests, PublicKey=00000000000000000400000000000000")]
[assembly: InternalsVisibleTo("WinformsControlsTest, PublicKey=00000000000000000400000000000000")]
[assembly: InternalsVisibleTo("MauiListViewTests, PublicKey=00000000000000000400000000000000")]
