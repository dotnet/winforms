using System.ComponentModel;

namespace CSharpControls;

// We are writing the fully-qualified name here to make sure, the Simplifier doesn't remove it,
// since this is nothing our code fix touches.
public class ScalableControl : System.Windows.Forms.Control
{
    private SizeF _scaleSize = new SizeF(3, 14);

    /// <Summary>
    ///  Sets or gets the scaled size of some foo bar thing.
    /// </Summary>
    [System.ComponentModel.Description("Sets or gets the scaled size of some foo bar thing.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SizeF ScaledSize
    {
        get => _scaleSize;
        set => _scaleSize = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float ScaleFactor { get; set; } = 1.0f;

    /// <Summary>
    ///  Sets or gets the scaled location of some foo bar thing.
    /// </Summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PointF ScaledLocation { get; set; }
}
