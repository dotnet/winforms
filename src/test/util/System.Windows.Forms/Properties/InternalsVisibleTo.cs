// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

// Awkward, but necessary to expose Interop based internals to other test libraries
[assembly: InternalsVisibleTo($"System.Windows.Forms.Tests, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.UI.IntegrationTests, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.Design.Tests, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"WinFormsControlsTest, PublicKey={PublicKeys.MicrosoftShared}")]
