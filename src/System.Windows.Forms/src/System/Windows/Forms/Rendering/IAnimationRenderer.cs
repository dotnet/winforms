// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Rendering;

/// <summary>
///  Represents an animation renderer. Currently fixed set to around 40 frames per second.
/// </summary>
internal interface IAnimationRenderer : IDisposable
{
    /// <summary>
    ///  Triggers the animation for the specified number of frames.
    /// </summary>
    /// <param name="frameCount">The frame number of the animation.</param>
    void TriggerAnimation(int frameCount);

    /// <summary>
    ///  Starts the animation with the specified parameters.
    /// </summary>
    void StartAnimation();

    /// <summary>
    ///  Stops the animation.
    /// </summary>
    void StopAnimation();
}
