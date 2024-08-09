// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Rendering.Animation;

/// <summary>
///  Represents an abstract base class for animated control renderers.
/// </summary>
/// <param name="control">The control associated with the renderer.</param>
internal abstract class AnimatedControlRenderer(Control control)
{
    private bool _disposedValue;
    protected float AnimationProgress = 1;

    /// <summary>
    ///  Callback for the animation progress. This method is called by the animation manager roughly every
    ///  25ms which results in about 40 frames per second.
    /// </summary>
    /// <param name="animationProgress">A fraction between 0 and 1 representing the animation progress.</param>
    public virtual void AnimationProc(float animationProgress)
    {
        AnimationProgress = animationProgress;
    }

    /// <summary>
    ///  Called when the control needs to be painted.
    /// </summary>
    /// <param name="graphics">The <see cref="Graphics"/> to paint the control.</param>
    public abstract void RenderControl(Graphics graphics);

    /// <summary>
    ///  Invalidates the control, causing it to be redrawn, which in turns triggers
    ///  <see cref="RenderControl(Graphics)"/>.
    /// </summary>
    public void Invalidate() => control.Invalidate();

    /// <summary>
    ///  Starts the animation and gets the animation parameters.
    /// </summary>
    public void StartAnimation()
    {
        if (IsRunning)
        {
            return;
        }

        // Get the animation parameters
        (int animationDuration, AnimationCycle animationCycle) = OnAnimationStarted();

        // Register the renderer with the animation manager
        AnimationManager.RegisterOrUpdateAnimationRenderer(
            this,
            animationDuration,
            animationCycle);

        IsRunning = true;
    }

    internal void StopAnimationInternal() => IsRunning = false;

    public void RestartAnimation()
    {
        if (IsRunning)
        {
            StopAnimation();
        }

        StartAnimation();
    }

    /// <summary>
    ///  Called in a derived class when the animation starts. The derived class returns the animation duration and cycle type.
    /// </summary>
    /// <returns>
    ///  Tuple containing the animation duration and cycle type.
    /// </returns>
    protected abstract (int animationDuration, AnimationCycle animationCycle) OnAnimationStarted();

    /// <summary>
    ///  Called by the animation manager when the animation ends.
    /// </summary>
    internal void EndAnimation()
    {
        OnAnimationEnded();
    }

    /// <summary>
    ///  Called in a derived class when the animation ends.
    ///  The derived class can perform any cleanup or state change operations.
    /// </summary>
    protected abstract void OnAnimationEnded();

    /// <summary>
    ///  Can be called by an implementing control, when the Animation needs to be stopped or restarted.
    /// </summary>
    public void StopAnimation()
    {
        AnimationManager.Suspend(this);
        OnAnimationStopped();
    }

    /// <summary>
    ///  Called in the derived class when the animation is stopped.
    ///  The derived class can perform any cleanup or state change operations.
    /// </summary>
    protected abstract void OnAnimationStopped();

    /// <summary>
    ///  Gets the DPI scale of the control.
    /// </summary>
    protected int DpiScale => (int)(control.DeviceDpi / 96f);

    /// <summary>
    ///  Gets a value indicating whether the animation is running.
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    ///  Gets the control associated with the renderer.
    /// </summary>
    protected Control Control => control;

    /// <summary>
    ///  Releases the unmanaged resources used by the <see cref="AnimatedControlRenderer"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // Remove the renderer from the animation manager
                AnimationManager.UnregisterAnimationRenderer(this);
            }

            _disposedValue = true;
        }
    }

    /// <summary>
    ///  Releases all resources used by the <see cref="AnimatedControlRenderer"/>.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
