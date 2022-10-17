using static Interop;

namespace System.Windows.Forms
{
    public partial class TextBox
    {
        internal class TextBoxAccessibleObject : TextBoxBaseAccessibleObject
        {
            private readonly TextBox _owningTextBox;

            public TextBoxAccessibleObject(TextBox owner) : base(owner)
            {
                _owningTextBox = owner;
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.HelpTextPropertyId:
                        string? placeholderText = _owningTextBox?.PlaceholderText;
                        return string.IsNullOrEmpty(placeholderText) ? base.GetPropertyValue(propertyID) : placeholderText;

                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }
        }
    }
}
