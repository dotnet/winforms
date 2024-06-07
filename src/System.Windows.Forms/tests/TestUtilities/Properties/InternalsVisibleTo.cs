// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

// Awkward, but necessary to expose Interop based internals to other test libraries
[assembly: InternalsVisibleTo("System.Windows.Forms.Tests, PublicKey=00000000000000000400000000000000")]
[assembly: InternalsVisibleTo("System.Windows.Forms.UI.IntegrationTests, PublicKey=00000000000000000400000000000000")]
[assembly: InternalsVisibleTo("System.Windows.Forms.Design.Tests, PublicKey=00000000000000000400000000000000")]
[assembly: InternalsVisibleTo("WinFormsControlsTest, PublicKey=00000000000000000400000000000000")]
