﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Core.Mathematics;
using Xenko.Rendering;
using Xenko.Rendering.Compositing;
using XenkoMath = Xenko.Core.Mathematics;

namespace VL.Xenko.Rendering
{
    public static class GraphicsCompositorExtensions
    {
        public static GraphicsCompositor GetFirstForwardRenderer(this GraphicsCompositor compositor, out ForwardRenderer forwardRenderer)
        {
            var topChildRenderer = ((SceneCameraRenderer)compositor.Game).Child;
            forwardRenderer = (topChildRenderer as SceneRendererCollection)?.Children.OfType<ForwardRenderer>().FirstOrDefault() ?? (ForwardRenderer)topChildRenderer;
            return compositor;
        }

        public static ForwardRenderer SetClearOptions(this ForwardRenderer forwardRenderer, Color4 color, float depth = 1, byte stencilValue = 0, ClearRendererFlags clearFlags = ClearRendererFlags.ColorAndDepth, bool enabled = true)
        {
            forwardRenderer.Clear.Color = color;
            forwardRenderer.Clear.ClearFlags = clearFlags;
            forwardRenderer.Clear.Depth = depth;
            forwardRenderer.Clear.Stencil = stencilValue;
            forwardRenderer.Clear.Enabled = enabled;
            return forwardRenderer;
        }
    }
}
