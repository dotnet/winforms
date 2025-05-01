Create a ButtonDarkModeRenderer class similar to the existing ButtonRenderer in System.Windows.Forms, but specifically designed for dark mode rendering. The class should follow the same architectural patterns as the provided ButtonRenderer while implementing Windows 11 dark mode visual styling.

## Visual Styling Details

### Button Shapes and Corners
1. All buttons should have rounded corners with a radius of approximately 4-5 pixels
2. The corners must be properly anti-aliased
3. For the rounded corners, implement transparency at the very edges so the parent background shows through correctly - this is crucial as the button's rectangular client area includes the corners, but visually they should appear rounded
4. Use alpha blending at the rounded edges to create a smooth transition between the button and its background

### Color Schemes
1. Normal Button (non-default):
   - Background: Dark gray (#2B2B2B) when not hovered
   - Hover state: Slightly lighter gray (#3B3B3B) - this change should be clearly noticeable
   - Pressed state: Even lighter gray (#4B4B4B) with a subtle inward appearance
   - Disabled state: Very dark gray (#252525) with reduced opacity text (around 40% opacity)

2. Default Button:
   - Background: Accent color (purple tone similar to #6B2FBF when not hovered)
   - Hover state: Slightly lighter accent color (#7C3FD0)
   - Pressed state: Darker accent color (#5B1FAF) with subtle inward appearance
   - Disabled state: Desaturated accent color with reduced opacity text

3. Text Colors:
   - Normal state: Light gray (#E0E0E0) for standard buttons, white (#FFFFFF) for default buttons
   - Disabled state: Reduced opacity version of normal text color (around 40% opacity)

### Border Styles
1. None border style:
   - No visible border, matching Windows 11 dark mode
   - Only the fill color and rounded corners are visible

2. Single border style:
   - Hair-thin (1px) border with a color that provides sufficient contrast
   - For normal buttons: Medium gray (#555555)
   - For default buttons: Slightly darker version of the accent color

3. 3D border style:
   - Slightly thicker border (1-2px)
   - For normal buttons: Top/left slightly lighter (#555555), bottom/right slightly darker (#222222)
   - For default buttons: Similar effect but with accent color variations
   - On hover: Subtly enhance the contrast between the edges
   - On press: Invert the light/dark edges to create an inset appearance

### Focus Indication
1. When a button has focus, draw a 1px dotted outline 2px inside the button's edge
2. The focus indicator should follow the rounded corners
3. For normal buttons: Use a light gray (#AAAAAA) for the focus indicator
4. For default buttons: Use white (#FFFFFF) or very light purple for the focus indicator
5. Ensure the focus indicator doesn't interfere with text or image rendering

### State Transitions
1. All state changes (hover, press, etc.) should have a subtle fade/transition effect if possible
2. The hover and pressed state visual changes should be more pronounced than in the standard renderer
3. Implement a slight scale-down effect (approximately 0.5-1% reduction) when buttons are pressed

## Functional Requirements

1. Maintain complete feature parity with ButtonRenderer:
   - Support for text rendering with all the same formatting options
   - Support for image rendering with the same positioning options
   - Support for combined text and image rendering
   - Proper handling of all PushButtonState values

2. Use SystemColors appropriately:
   - Rely on the dark mode SystemColors when Application.IsDarkModeEnabled is true
   - If specific colors need adjustments beyond SystemColors, implement these within the renderer

3. Transparency and background handling:
   - Properly support the IsBackgroundPartiallyTransparent method
   - Correctly implement the DrawParentBackground method to handle transparent areas
   - Ensure transparent corners are properly rendered showing the parent background

4. Implement all public methods present in ButtonRenderer with identical signatures and parameters

5. Thread safety:
   - Maintain the same thread-safety approach as ButtonRenderer with ThreadStatic renderer instances

## Implementation Notes

1. Follow the same pattern as ButtonRenderer, creating a static class with identical method signatures
2. Do not add any additional features beyond what is specified
3. Maintain backward compatibility with existing button rendering behavior
4. Handle fallback for systems where visual styles are not enabled or supported
5. Provide appropriate XML documentation following the same style as the original ButtonRenderer
6. Prioritize rendering performance, especially for the alpha-blended rounded corners

The class should integrate seamlessly with the existing WinForms infrastructure while providing modern Windows 11 dark mode visuals.