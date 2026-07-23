// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Runtime.InteropServices;

using Orama.Common;
using Orama.Math;
using Orama.RHI;
using Orama.Rendering.Components;
using Orama.Rendering.Pipelines;
using Orama.Rendering.Pipelines.Forward;
using Orama.Rendering.Resources;

namespace Orama.Rendering;

/// <summary>
/// Module responsible for rendering meshes to the window.
/// </summary>
public class RenderingModule : BaseModule
{
	/// <summary> The renderable objects to render next frame. </summary>
	internal List<IClientRenderable> Renderables { get; set; } = new();

	/// <summary> The <see cref="RenderPipeline"/> in use. </summary>
	public RenderPipeline Pipeline { get; set; } = new ForwardRenderPipeline();

	/// <inheritdoc/>
	public override void Initialize()
	{
		Application.OnResize += (size) => OnResize((int)size.X, (int)size.Y);
		Application.OnRender += Render;

		RendererOptions options = new();
		options.Culling = CullingMode.Back;

		Renderer.Initialize(Application.Window.InternalWindow, RendererBackend.Vulkan, options);
		Application.Window.Title += $" - [{Renderer.Backend}]";
	}

	public void Render()
	{
		if (Camera.Main == null)
			new Camera();

		Matrix4x4 view = Camera.Main?.ViewMatrix ?? Matrix4x4.Identity;
		Matrix4x4 projection = Camera.Main?.ProjectionMatrix ?? Matrix4x4.Identity;

		ReadOnlySpan<byte> cameraBuffer = Shader.DefaultsProvider.GetCameraBuffer(Camera.Main!);

		var frame = new RenderFrame()
		{
			View = view,
			Projection = projection,
			CameraBuffer = cameraBuffer,
			Renderables = CollectionsMarshal.AsSpan(Renderables),
		};

		Pipeline.Render(in frame);
		Renderables.Clear();

		Renderer.Present();
	}

	/// <inheritdoc/>
	public override void Dispose()
	{
		base.Dispose();

		Renderer.Dispose();

		Application.OnResize -= (size) => OnResize((int)size.X, (int)size.Y);
		Application.OnRender -= Render;
	}

	/// <summary> Renders a client renderable to the window during the next frame. </summary>
	/// <param name="renderable">The object to render.</param>
	public void QueueObject(IClientRenderable renderable) => Renderables.Add(renderable);

	public void OnResize(int width, int height) => Renderer.Resize((uint)width, (uint)height);
}
