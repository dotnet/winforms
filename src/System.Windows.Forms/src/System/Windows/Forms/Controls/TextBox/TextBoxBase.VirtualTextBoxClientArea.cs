// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

public abstract partial class TextBoxBase
{
    private class VirtualClientArea : IArrangedElement
    {
        public event EventHandler? Disposed;

        private readonly PropertyStore _propertyStore = new PropertyStore();
        private static readonly int s_propertyPadding = PropertyStore.CreateKey();

        public VirtualClientArea()
        {
            _propertyStore = new PropertyStore();
            _propertyStore.SetPadding(s_propertyPadding, Padding.Empty);
        }

        public PropertyStore Properties => _propertyStore;

        private bool _disposedValue;

        public Padding Padding
        {
            get => (Padding)_propertyStore.GetObject(s_propertyPadding)!;
            set => _propertyStore.SetObject(s_propertyPadding, value);
        }

        public Rectangle Bounds => throw new NotImplementedException();

        public Rectangle DisplayRectangle => throw new NotImplementedException();

        public bool ParticipatesInLayout => false;

        public IArrangedElement? Container => null;

        public ArrangedElementCollection Children => throw new NotImplementedException();

        public ISite? Site
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public Size GetPreferredSize(Size proposedSize) => throw new NotImplementedException();
        public void PerformLayout(IArrangedElement affectedElement, string? propertyName) => throw new NotImplementedException();
        public void SetBounds(Rectangle bounds, BoundsSpecified specified) => new NotImplementedException();

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _disposedValue = true;
                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
