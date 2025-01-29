namespace CSharpControls;

public static class Program
{
    public static void Main()
    {
        var control = new ScalableControl();

        // We deliberately format this weirdly, to make sure we only format code our code fix touches.
        control.ScaleFactor = 1.5f;
        control.ScaledSize = new SizeF(100, 100);
        control.ScaledLocation = new PointF(10, 10);
    }
}

// We are writing the fully-qualified name here to make sure, the Simplifier doesn't remove it,
// since this is nothing our code fix touches.
public class ScalableControl : System.Windows.Forms.Control
{
    private SizeF _scaleSize = new SizeF(3, 14);

    /// <Summary>
    ///  Sets or gets the scaled size of some foo bar thing.
    /// </Summary>
    [System.ComponentModel.Description("Sets or gets the scaled size of some foo bar thing.")]
    public SizeF [|ScaledSize|]
            {
                get => _scaleSize;
                set => _scaleSize = value;
            }

public float [|ScaleFactor|] { get; set; } = 1.0f;

/// <Summary>
///  Sets or gets the scaled location of some foo bar thing.
/// </Summary>
public PointF [|ScaledLocation|]
{ get; set; }
        }
