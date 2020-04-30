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

namespace Stride.Rendering
{
    public static partial class ChromaKeyKeys
    {
        public static readonly ValueParameterKey<Color4> chromaKey = ParameterKeys.NewValue<Color4>(new Color4(0.05f,0.63f,0.14f,1.0f));
        public static readonly ValueParameterKey<Color4> BackgroundColor = ParameterKeys.NewValue<Color4>(new Color4(0.0f,0.0f,0.0f,0.0f));
        public static readonly ValueParameterKey<Vector2> maskRange = ParameterKeys.NewValue<Vector2>(new Vector2(0.005f,0.26f));
    }
}
