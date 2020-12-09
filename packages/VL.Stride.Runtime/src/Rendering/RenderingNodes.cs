﻿using Stride.Core.Mathematics;
using Stride.Graphics.GeometricPrimitives;
using Stride.Rendering;
using Stride.Rendering.ProceduralModels;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using VL.Core;
using VL.Lang.Symbols;
using VL.Lib.Basics.Resources;
using VL.Lib.Mathematics;
using VL.Stride.Rendering.ComputeEffect;

namespace VL.Stride.Rendering
{
    using Model = global::Stride.Rendering.Model;

    static class RenderingNodes
    {
        public static IEnumerable<IVLNodeDescription> GetNodeDescriptions(IVLNodeDescriptionFactory factory)
        {
            var renderingCategory = "Stride.Rendering";
            var renderingAdvancedCategory = $"{renderingCategory}.Advanced";

            yield return NewInputRenderBaseNode<WithRenderTargetAndViewPort>(factory, category: renderingCategory)
                .AddCachedInput(nameof(WithRenderTargetAndViewPort.RenderTarget), x => x.RenderTarget, (x, v) => x.RenderTarget = v)
                .AddCachedInput(nameof(WithRenderTargetAndViewPort.DepthBuffer), x => x.DepthBuffer, (x, v) => x.DepthBuffer = v)
                ;

            yield return NewInputRenderBaseNode<WithinCommonSpace>(factory, category: renderingAdvancedCategory)
                .AddCachedInput(nameof(WithinCommonSpace.CommonScreenSpace), x => x.CommonScreenSpace, (x, v) => x.CommonScreenSpace = v, CommonSpace.DIPTopLeft)
                ;

            yield return NewInputRenderBaseNode<WithinPhysicalScreenSpace>(factory, category: renderingAdvancedCategory)
                .AddCachedInput(nameof(WithinPhysicalScreenSpace.Units), x => x.Units, (x, v) => x.Units = v, ScreenSpaceUnits.DIP)
                .AddCachedInput(nameof(WithinPhysicalScreenSpace.Anchor), x => x.Anchor, (x, v) => x.Anchor = v, Lib.Mathematics.RectangleAnchor.Center)
                .AddCachedInput(nameof(WithinPhysicalScreenSpace.Offset), x => x.Offset, (x, v) => x.Offset = v)
                .AddCachedInput(nameof(WithinPhysicalScreenSpace.Scale), x => x.Scale, (x, v) => x.Scale = v, 1f)
                .AddCachedInput(nameof(WithinPhysicalScreenSpace.IgnoreExistingView), x => x.IgnoreExistingView, (x, v) => x.IgnoreExistingView = v, true)
                .AddCachedInput(nameof(WithinPhysicalScreenSpace.IgnoreExistingProjection), x => x.IgnoreExistingProjection, (x, v) => x.IgnoreExistingProjection = v, true)
                ;


            yield return NewInputRenderBaseNode<WithinVirtualScreenSpace>(factory, category: renderingAdvancedCategory)
                .AddCachedInput(nameof(WithinVirtualScreenSpace.Bounds), x => x.Bounds, (x, v) => x.Bounds = v, new RectangleF(-0.5f, -0.5f, 1, 1))
                .AddCachedInput(nameof(WithinVirtualScreenSpace.AspectRatioCorrectionMode), x => x.AspectRatioCorrectionMode, (x, v) => x.AspectRatioCorrectionMode = v, SizeMode.FitOut)
                .AddCachedInput(nameof(WithinVirtualScreenSpace.Anchor), x => x.Anchor, (x, v) => x.Anchor = v, Lib.Mathematics.RectangleAnchor.Center)
                .AddCachedInput(nameof(WithinVirtualScreenSpace.IgnoreExistingView), x => x.IgnoreExistingView, (x, v) => x.IgnoreExistingView = v, true)
                .AddCachedInput(nameof(WithinVirtualScreenSpace.IgnoreExistingProjection), x => x.IgnoreExistingProjection, (x, v) => x.IgnoreExistingProjection = v, true)
                ;

            yield return NewInputRenderBaseNode<WithRenderView>(factory, category: renderingAdvancedCategory)
                .AddCachedInput(nameof(WithRenderView.RenderView), x => x.RenderView, (x, v) => x.RenderView = v)
                .AddCachedInput(nameof(WithRenderView.AspectRatioCorrectionMode), x => x.AspectRatioCorrectionMode, (x, v) => x.AspectRatioCorrectionMode = v)
                ;

            yield return NewInputRenderBaseNode<WithWindowInputSource>(factory, category: renderingAdvancedCategory)
                .AddCachedInput(nameof(WithWindowInputSource.InputSource), x => x.InputSource, (x, v) => x.InputSource = v)
                ;

            yield return factory.NewNode<GetWindowInputSource>(name: nameof(GetWindowInputSource), category: renderingAdvancedCategory, copyOnWrite: false, fragmented: true)
                .AddCachedInput(nameof(RendererBase.Input), x => x.Input, (x, v) => x.Input = v)
                .AddOutput(nameof(GetWindowInputSource.InputSource), x => x.InputSource)
            ;

            yield return factory.NewNode(c => new MipMapGenerator(c), name: "MipMap", category: renderingAdvancedCategory, copyOnWrite: false, fragmented: true, hasStateOutput: false)
                .AddInput("Input", x => x.InputTexture, (x, v) => x.InputTexture = v)
                .AddInput(nameof(MipMapGenerator.MaxMipMapCount), x => x.MaxMipMapCount, (x, v) => x.MaxMipMapCount = v)
                .AddOutput("Output", x => { x.ScheduleForRendering(); return x.OutputTexture; });

            // Compute effect dispatchers
            var dispatchersCategory = $"{renderingAdvancedCategory}.ComputeEffect";
            yield return factory.NewNode<DirectComputeEffectDispatcher>(name: "DirectDispatcher", category: renderingAdvancedCategory, copyOnWrite: false, fragmented: true, hasStateOutput: false)
                .AddCachedInput(nameof(DirectComputeEffectDispatcher.ThreadGroupCount), x => x.ThreadGroupCount, (x, v) => x.ThreadGroupCount = v, Int3.One)
                .AddOutput<IComputeEffectDispatcher>("Output", x => x);

            yield return factory.NewNode<IndirectComputeEffectDispatcher>(name: "IndirectDispatcher", category: renderingAdvancedCategory, copyOnWrite: false, fragmented: true, hasStateOutput: false)
                .AddCachedInput(nameof(IndirectComputeEffectDispatcher.ArgumentBuffer), x => x.ArgumentBuffer, (x, v) => x.ArgumentBuffer = v)
                .AddCachedInput(nameof(IndirectComputeEffectDispatcher.OffsetInBytes), x => x.OffsetInBytes, (x, v) => x.OffsetInBytes = v)
                .AddOutput<IComputeEffectDispatcher>("Output", x => x);

            yield return factory.NewNode<CustomComputeEffectDispatcher>(name: "CustomDispatcher", category: renderingAdvancedCategory, copyOnWrite: false, fragmented: true, hasStateOutput: false)
                .AddCachedInput(nameof(CustomComputeEffectDispatcher.ThreadGroupCountsSelector), x => x.ThreadGroupCountsSelector, (x, v) => x.ThreadGroupCountsSelector = v)
                .AddOutput<IComputeEffectDispatcher>("Output", x => x);

            //yield return factory.NewNode<RenderView>(name: "RenderView", category: renderingAdvancedCategory, copyOnWrite: false, fragmented: true)
            //    .AddInput(nameof(RenderView.View), x => x.View, (x, v) => x.View = v)
            //    .AddInput(nameof(RenderView.Projection), x => x.Projection, (x, v) => x.Projection = v)
            //    .AddInput(nameof(RenderView.NearClipPlane), x => x.NearClipPlane, (x, v) => x.NearClipPlane = v)
            //    .AddInput(nameof(RenderView.FarClipPlane), x => x.FarClipPlane, (x, v) => x.FarClipPlane = v)
            //    // TODO: add more
            //    ;

            // Meshes
            yield return factory.NewMeshNode((CapsuleProceduralModel x) => (x.Length, x.Radius, x.Tessellation))
                .AddCachedInput(nameof(CapsuleProceduralModel.Length), x => x.Length, (x, v) => x.Length = v, 0.5f)
                .AddCachedInput(nameof(CapsuleProceduralModel.Radius), x => x.Radius, (x, v) => x.Radius = v, 0.5f)
                .AddCachedInput(nameof(CapsuleProceduralModel.Tessellation), x => x.Tessellation, (x, v) => x.Tessellation = v, 16)
                .AddDefaultPins();

            yield return factory.NewMeshNode((ConeProceduralModel x) => (x.Height, x.Radius, x.Tessellation))
                .AddCachedInput(nameof(ConeProceduralModel.Height), x => x.Height, (x, v) => x.Height = v, 1f)
                .AddCachedInput(nameof(ConeProceduralModel.Radius), x => x.Radius, (x, v) => x.Radius = v, 0.5f)
                .AddCachedInput(nameof(ConeProceduralModel.Tessellation), x => x.Tessellation, (x, v) => x.Tessellation = v, 16)
                .AddDefaultPins();

            yield return factory.NewMeshNode((CubeProceduralModel x) => x.Size, name: "BoxMesh")
                .AddCachedInput(nameof(CubeProceduralModel.Size), x => x.Size, (x, v) => x.Size = v, Vector3.One)
                .AddDefaultPins();

            yield return factory.NewMeshNode((CylinderProceduralModel x) => (x.Height, x.Radius, x.Tessellation))
                .AddCachedInput(nameof(CylinderProceduralModel.Height), x => x.Height, (x, v) => x.Height = v, 1f)
                .AddCachedInput(nameof(CylinderProceduralModel.Radius), x => x.Radius, (x, v) => x.Radius = v, 0.5f)
                .AddCachedInput(nameof(CylinderProceduralModel.Tessellation), x => x.Tessellation, (x, v) => x.Tessellation = v, 16)
                .AddDefaultPins();

            yield return factory.NewMeshNode((GeoSphereProceduralModel x) => (x.Radius, x.Tessellation))
                .AddCachedInput(nameof(GeoSphereProceduralModel.Radius), x => x.Radius, (x, v) => x.Radius = v, 0.5f)
                .AddCachedInput(nameof(GeoSphereProceduralModel.Tessellation), x => x.Tessellation, (x, v) => x.Tessellation = VLMath.Clamp(v, 1, 5), 3)
                .AddDefaultPins();

            yield return factory.NewMeshNode((PlaneProceduralModel x) => (x.Size, x.Tessellation, x.Normal, x.GenerateBackFace))
                .AddCachedInput(nameof(PlaneProceduralModel.Size), x => x.Size, (x, v) => x.Size = v, Vector2.One)
                .AddCachedInput(nameof(PlaneProceduralModel.Tessellation), x => x.Tessellation, (x, v) => x.Tessellation = v, Int2.One)
                .AddCachedInput(nameof(PlaneProceduralModel.Normal), x => x.Normal, (x, v) => x.Normal = v, NormalDirection.UpZ)
                .AddCachedInput(nameof(PlaneProceduralModel.GenerateBackFace), x => x.GenerateBackFace, (x, v) => x.GenerateBackFace = v, true)
                .AddDefaultPins();

            yield return factory.NewMeshNode((SphereProceduralModel x) => (x.Radius, x.Tessellation))
                .AddCachedInput(nameof(SphereProceduralModel.Radius), x => x.Radius, (x, v) => x.Radius = v, 0.5f)
                .AddCachedInput(nameof(SphereProceduralModel.Tessellation), x => x.Tessellation, (x, v) => x.Tessellation = v, 16)
                .AddDefaultPins();

            yield return factory.NewMeshNode((TeapotProceduralModel x) => (x.Size, x.Tessellation))
                .AddCachedInput(nameof(TeapotProceduralModel.Size), x => x.Size, (x, v) => x.Size = v, 1f)
                .AddCachedInput(nameof(TeapotProceduralModel.Tessellation), x => x.Tessellation, (x, v) => x.Tessellation = v, 16)
                .AddDefaultPins();

            yield return factory.NewMeshNode((TorusProceduralModel x) => (x.Radius, x.Thickness, x.Tessellation))
                .AddCachedInput(nameof(TorusProceduralModel.Radius), x => x.Radius, (x, v) => x.Radius = v, 0.5f)
                .AddCachedInput(nameof(TorusProceduralModel.Thickness), x => x.Thickness, (x, v) => x.Thickness = v, 0.25f)
                .AddCachedInput(nameof(TorusProceduralModel.Tessellation), x => x.Tessellation, (x, v) => x.Tessellation = v, 16)
                .AddDefaultPins();

        }

        static CustomNodeDesc<TInputRenderBase> NewInputRenderBaseNode<TInputRenderBase>(IVLNodeDescriptionFactory factory, string category, string name = null)
            where TInputRenderBase : RendererBase, new()
        {
            return factory.NewNode<TInputRenderBase>(name: name, category: category, copyOnWrite: false, fragmented: true)
                .AddCachedInput(nameof(RendererBase.Input), x => x.Input, (x, v) => x.Input = v);
        }

        static CustomNodeDesc<TProceduralModel> NewMeshNode<TProceduralModel, TKey>(this IVLNodeDescriptionFactory factory, Func<TProceduralModel, TKey> getKey, string name = null)
           where TProceduralModel : PrimitiveProceduralModelBase, new()
        {
            return new CustomNodeDesc<TProceduralModel>(factory,
                name: name ?? typeof(TProceduralModel).Name.Replace("ProceduralModel", "Mesh"),
                category: "Stride.Models.Meshes",
                copyOnWrite: false,
                hasStateOutput: false,
                fragmented: true,
                ctor: nodeContext =>
                {
                    var generator = new TProceduralModel();
                    return (generator, default);
                })
                .AddCachedOutput("Output", nodeContext =>
                {
                    var disposable = new SerialDisposable();
                    Func<TProceduralModel, Mesh> getter = generator =>
                    {
                        var key = (typeof(TProceduralModel), generator.Scale, generator.UvScale, generator.LocalOffset, generator.NumberOfTextureCoordinates, getKey(generator));
                        var provider = ResourceProvider.NewPooledPerApp(nodeContext, key, _ =>
                        {
                            return nodeContext.GetGameProvider()
                                .Bind(game =>
                                {
                                    var model = new Model();
                                    generator.Generate(game.Services, model);
                                    return ResourceProvider.Return(model.Meshes[0], m =>
                                    {
                                        if (m.Draw != null)
                                        {
                                            m.Draw.IndexBuffer?.Buffer?.Dispose();
                                            foreach (var b in m.Draw.VertexBuffers)
                                                b.Buffer?.Dispose();
                                        }
                                    });
                                });
                        });
                        var meshHandle = provider.GetHandle();
                        disposable.Disposable = meshHandle;
                        return meshHandle.Resource;
                    };
                    return (getter, disposable);
                });
        }

        static CustomNodeDesc<TProceduralModel> AddDefaultPins<TProceduralModel>(this CustomNodeDesc<TProceduralModel> node)
            where TProceduralModel : PrimitiveProceduralModelBase, new()
        {
            return node
                .AddCachedInput(nameof(PrimitiveProceduralModelBase.Scale), x => x.Scale, (x, v) => x.Scale = v, Vector3.One)
                .AddCachedInput(nameof(PrimitiveProceduralModelBase.UvScale), x => x.UvScale, (x, v) => x.UvScale = v, Vector2.One)
                .AddCachedInput(nameof(PrimitiveProceduralModelBase.LocalOffset), x => x.LocalOffset, (x, v) => x.LocalOffset = v, Vector3.Zero)
                .AddCachedInput(nameof(PrimitiveProceduralModelBase.NumberOfTextureCoordinates), x => x.NumberOfTextureCoordinates, (x, v) => x.NumberOfTextureCoordinates = v, 1);
        }
    }
}
