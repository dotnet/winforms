// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Rendering.V10.CheckBox;

/// <summary>
///  Represents an abstract base class for animated control renderers.
/// </summary>
internal abstract class AnimatedControlRenderer
{
    private bool _disposedValue;
    private bool _isRunning;
    private readonly Control _control;

    /// <summary>
    ///  Initializes a new instance of the <see cref="AnimatedControlRenderer"/> class with the specified control.
    /// </summary>
    /// <param name="control">The control associated with the renderer.</param>
    protected AnimatedControlRenderer(Control control)
    {
        _control = control;
    }

    /// <summary>
    ///  Callback for the animation progress. This method is called by the animation manager roughly every
    ///  25ms which results in about 40 frames per second.
    /// </summary>
    /// <param name="animationProgress">A fraction between 0 and 1 representing the animation progress.</param>
    public abstract void AnimationProc(float animationProgress);

    /// <summary>
    ///  Called when the control needs to be painted.
    /// </summary>
    /// <param name="graphics">The <see cref="Graphics"/> to paint the control.</param>
    public abstract void RenderControl(Graphics graphics);

    /// <summary>
    ///  Invalidates the control, causing it to be redrawn, which in turns triggers
    ///  <see cref="RenderControl(PaintEventArgs)"/>.
    /// </summary>
    public void Invalidate()
    {
        _control.Invalidate();
    }

    /// <summary>
    ///  Starts the animation and gets the animation parameters.
    /// </summary>
    public void StartAnimation()
    {
        // Get the animation parameters
        (int animationDuration, AnimationCycle animationCycle) = OnStartAnimation();
        _isRunning = true;

        // Register the renderer with the animation manager
        AnimationManager.RegisterAnimationRenderer(
            this,
            animationDuration,
            animationCycle);
    }

    internal void RestartAnimation()
    {
        if (_isRunning)
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
    protected abstract (int animationDuration, AnimationCycle animationCycle) OnStartAnimation();

    /// <summary>
    ///  Stops the animation.
    /// </summary>
    public void StopAnimation()
    {
        OnStopAnimation();
        _isRunning = false;
    }

    /// <summary>
    ///  Called when the animation stops.
    /// </summary>
    protected abstract void OnStopAnimation();

    /// <summary>
    ///  Gets the DPI scale of the control.
    /// </summary>
    protected int DpiScale => (int)(_control.DeviceDpi / 96f);

    public bool IsRunning => _isRunning;

    /// <summary>
    ///  Gets the control associated with the renderer.
    /// </summary>
    protected Control Control => _control;

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
