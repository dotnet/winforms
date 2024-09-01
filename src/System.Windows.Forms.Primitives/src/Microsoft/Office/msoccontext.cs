// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Office;

/// <summary>
///  <see cref="IMsoComponentManager.FCreateSubComponentManager" /> allows one to create a hierarchical
///  tree of component managers. This tree is used to maintain multiple contexts with regard to
///  <see cref="msocstate"/> states. These contexts are referred to as 'state contexts'
///
///  Each component manager in the tree defines a state context. The components registered with a
///  particular component manager or any of its descendents live within that component manager's
///  state context. Calls to <see cref="IMsoComponentManager.OnComponentEnterState"/>
///  and <see cref="IMsoComponentManager.FOnComponentExitState"/> can be used to affect all components,
///  only components within the component manager's state context, or only those components that are
///  outside of the component manager's state context. <see cref="IMsoComponentManager.FInState" />
///  is used to query the state of the component manager's state context at its root.
/// </summary>
internal enum msoccontext : uint
{
    All = 0,
    Mine = 1,
    Others = 2
}
