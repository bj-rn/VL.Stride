// <auto-generated>
// Do not edit this file yourself!
//
// This code was generated by Xenko Shader Mixin Code Generator.
// To generate it yourself, please install Stride.VisualStudio.Package .vsix
// and re-save the associated .xkfx.
// </auto-generated>

using System;
using Stride.Core;
using Stride.Rendering;
using Stride.Graphics;
using Stride.Shaders;
using Stride.Core.Mathematics;
using Buffer = Stride.Graphics.Buffer;

using Stride.Rendering.ComputeEffect;
namespace VL.Stride.Effects.ComputeFX
{
    internal static partial class ShaderMixins
    {
        internal partial class ComputeFXGraphEffect  : IShaderMixinBuilder
        {
            public void Generate(ShaderMixinSource mixin, ShaderMixinContext context)
            {
                mixin.AddMacro("ThreadNumberX", context.GetParam(ComputeEffectShaderKeys.ThreadNumbers).X);
                mixin.AddMacro("ThreadNumberY", context.GetParam(ComputeEffectShaderKeys.ThreadNumbers).Y);
                mixin.AddMacro("ThreadNumberZ", context.GetParam(ComputeEffectShaderKeys.ThreadNumbers).Z);
                context.Mixin(mixin, "ComputeFXGraph");
                context.Mixin(mixin, context.GetParam(ComputeFXKeys.ComputeFXRoot));
            }

            [ModuleInitializer]
            internal static void __Initialize__()

            {
                ShaderMixinManager.Register("ComputeFXGraphEffect", new ComputeFXGraphEffect());
            }
        }
    }
}
