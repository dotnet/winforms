// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Rendering.Animation;

internal partial class AnimationManager
{
    private class AnimationRendererItem
    {
        public long StopwatchTarget;

        public AnimationRendererItem(AnimatedControlRenderer renderer, int animationDuration, AnimationCycle animationCycle)
        {
            Renderer = renderer;
            AnimationDuration = animationDuration;
            AnimationCycle = animationCycle;
        }

        public AnimatedControlRenderer Renderer { get; }
        public int TriggerFrequency { get; }
        public int AnimationDuration { get; set; }
        public int FrameCount { get; set; }
        public AnimationCycle AnimationCycle { get; set; }
        public int FrameOffset { get; set; } = 1;
    }
}
