// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.Layout;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows.Forms
{
    internal abstract class ArrangedElement : Component, IArrangedElement
    {
        private Rectangle bounds = Rectangle.Empty;
        private IArrangedElement parent = null;
        private BitVector32 state = new BitVector32();
        private readonly PropertyStore propertyStore = new PropertyStore();  // Contains all properties that are not always set.
        private readonly int suspendCount = 0;

        private static readonly int stateVisible = BitVector32.CreateMask();
        private static readonly int stateDisposing = BitVector32.CreateMask(stateVisible);
        private static readonly int stateLocked = BitVector32.CreateMask(stateDisposing);

        private static readonly int PropControlsCollection = PropertyStore.CreateKey();
        private readonly Control spacer = new Control();

        internal ArrangedElement()
        {
            Padding = DefaultPadding;
            Margin = DefaultMargin;
            state[stateVisible] = true;
        }

        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
        }

        ArrangedElementCollection IArrangedElement.Children
        {
            get { return GetChildren(); }
        }

        IArrangedElement IArrangedElement.Container
        {
            get { return GetContainer(); }
        }

        protected virtual Padding DefaultMargin
        {
            get { return Padding.Empty; }
        }

        protected virtual Padding DefaultPadding
        {
            get { return Padding.Empty; }
        }

        public virtual Rectangle DisplayRectangle
        {
            get
            {
                Rectangle displayRectangle = Bounds;
                return displayRectangle;
            }
        }

        public abstract LayoutEngine LayoutEngine
        {
            get;
        }

        public Padding Margin
        {
            get { return CommonProperties.GetMargin(this); }
            set
            {

                Debug.Assert((value.Right >= 0 && value.Left >= 0 && value.Top >= 0 && value.Bottom >= 0), "who's setting margin negative?");
                value = LayoutUtils.ClampNegativePaddingToZero(value);
                if (Margin != value)
                { CommonProperties.SetMargin(this, value); }

            }
        }

        public virtual Padding Padding
        {
            get { return CommonProperties.GetPadding(this, DefaultPadding); }
            set
            {
                Debug.Assert((value.Right >= 0 && value.Left >= 0 && value.Top >= 0 && value.Bottom >= 0), "who's setting padding negative?");
                value = LayoutUtils.ClampNegativePaddingToZero(value);
                if (Padding != value)
                { CommonProperties.SetPadding(this, value); }
            }
        }

        public virtual IArrangedElement Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value as IArrangedElement;
            }
        }

        public virtual bool ParticipatesInLayout
        {
            get
            {
                return Visible;
            }
        }

        PropertyStore IArrangedElement.Properties
        {
            get
            {
                return Properties;
            }
        }

        private PropertyStore Properties
        {
            get
            {
                return propertyStore;
            }
        }

        public virtual bool Visible
        {
            get
            {
                return state[stateVisible];
            }
            set
            {
                if (state[stateVisible] != value)
                {
                    state[stateVisible] = value;
                    if (Parent != null)
                    {
                        LayoutTransaction.DoLayout(Parent, this, PropertyNames.Visible);
                    }
                }
            }
        }

        protected abstract IArrangedElement GetContainer();

        protected abstract ArrangedElementCollection GetChildren();

        public virtual Size GetPreferredSize(Size constrainingSize)
        {
            Size preferredSize = LayoutEngine.GetPreferredSize(this, constrainingSize - Padding.Size) + Padding.Size;

            return preferredSize;
        }

        public virtual void PerformLayout(IArrangedElement container, string propertyName)
        {
            if (suspendCount <= 0)
            {
                OnLayout(new LayoutEventArgs(container, propertyName));
            }
        }

        protected virtual void OnLayout(LayoutEventArgs e)
        {
            bool parentNeedsLayout = LayoutEngine.Layout(this, e);
        }

        protected virtual void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            ((IArrangedElement)this).PerformLayout((IArrangedElement)this, PropertyNames.Size);
        }

        public void SetBounds(Rectangle bounds, BoundsSpecified specified)
        {
            // in this case the parent is telling us to refresh our bounds - dont
            // call PerformLayout
            SetBoundsCore(bounds, specified);
        }

        protected virtual void SetBoundsCore(Rectangle bounds, BoundsSpecified specified)
        {
            if (bounds != this.bounds)
            {
                Rectangle oldBounds = this.bounds;

                this.bounds = bounds;
                OnBoundsChanged(oldBounds, bounds);
            }
        }

    }

}


