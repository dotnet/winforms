using System.ComponentModel;
using System.Threading.Tasks;

namespace System.Drawing;

public sealed unsafe partial class Graphics
{
    /// <summary>
    ///  Creates a Graphics object from a handle asynchronously.
    /// </summary>
    /// <param name="handle">The handle to the device context.</param>
    /// <returns>
    ///  A task that represents the asynchronous operation.
    ///  The task result contains the created Graphics object.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Task<Graphics> FromHwndAsync(IntPtr handle)
    {
        TaskCompletionSource<Graphics> tcs = new TaskCompletionSource<Graphics>();

        try
        {
            Graphics graphics = Graphics.FromHwnd(handle);
            tcs.TrySetResult(graphics);
        }
        catch (Exception ex)
        {
            tcs.TrySetException(ex);
        }

        return tcs.Task;
    }

    /// <summary>
    ///  Creates a Graphics object from a handle asynchronously, with a specified thread-confining bounds.
    /// </summary>
    /// <param name="handle">The handle to the device context.</param>
    /// <param name="threadConfiningBounds">The thread-confining bounds for the Graphics object.</param>
    /// <returns>
    ///  A task that represents the asynchronous operation.
    ///  The task result contains the created Graphics object.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Task<Graphics> FromHwndAsync(IntPtr handle, RectangleF threadConfiningBounds)
    {
        TaskCompletionSource<Graphics> tcs = new TaskCompletionSource<Graphics>();

        try
        {
            Graphics graphics = Graphics.FromHwnd(handle);
            graphics.TranslateTransform(threadConfiningBounds.X, threadConfiningBounds.Y);
            graphics.SetClip(new RectangleF(PointF.Empty, threadConfiningBounds.Size));
            tcs.TrySetResult(graphics);
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);
        }

        return tcs.Task;
    }

    /// <summary>
    ///  Creates a Graphics object from a handle asynchronously, with a specified thread-confining bounds.
    /// </summary>
    /// <param name="handle">The handle to the device context.</param>
    /// <param name="threadConfiningBounds">The thread-confining bounds for the Graphics object.</param>
    /// <returns>
    ///  A task that represents the asynchronous operation.
    ///  The task result contains the created Graphics object.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Task<Graphics> FromHwndAsync(IntPtr handle, Rectangle threadConfiningBounds)
    {
        TaskCompletionSource<Graphics> tcs = new TaskCompletionSource<Graphics>();

        try
        {
            Graphics graphics = Graphics.FromHwnd(handle);
            graphics.TranslateTransform(threadConfiningBounds.X, threadConfiningBounds.Y);
            graphics.SetClip(new RectangleF(PointF.Empty, threadConfiningBounds.Size));
            tcs.TrySetResult(graphics);
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);
        }

        return tcs.Task;
    }
}
