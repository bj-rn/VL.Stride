﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Rendering;
using SiliconStudio.Xenko.Rendering.Compositing;
using SiliconStudio.Xenko.Graphics;

namespace ComputeParticlesTest
{
    public interface ICanRender
    {
        void Initialize();

        void Collect(RenderContext context);

        void Draw(RenderContext renderContext, RenderDrawContext drawContext, CommandList commandList);
    }

    public class InterfaceRenderer : SceneRendererBase
    {
        public ICanRender Renderer { get; set; }

        protected override void InitializeCore()
        {
            base.InitializeCore();
            Renderer?.Initialize();
        }

        protected override void CollectCore(RenderContext context)
        {
            base.CollectCore(context);
            Renderer?.Collect(context);
        }

        protected override void DrawCore(RenderContext context, RenderDrawContext drawContext)
        {
            try
            {
                Renderer?.Draw(context, drawContext, drawContext.CommandList);
            }
            catch (Exception e)
            {
                var inner = e.InnerException;
                while (inner != null)
                {
                    e = inner;
                    inner = e.InnerException;
                }

                System.Diagnostics.Debug.WriteLine(e.Message);
               //
            }
        }

        
    }
}

