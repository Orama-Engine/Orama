// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Rendering.Device.Resources;
using Orama.Rendering.Resources.Caches;

namespace Orama.Rendering.Device;

/// <summary>
/// Defines how an <see cref="IGraphicsDevice"/> creates GPU resources.
/// </summary>
public interface IResourceFactory
{
	/// <summary> Creates a new <see cref="ICommandBuffer"/>. </summary>
	ICommandBuffer CreateCommandBuffer();

	/// <summary> Creates a new <see cref="IShader"/> using the given <see cref="ShaderKey"/> information. </summary>
	IShader CreateShader(ShaderKey key);

	/// <summary> Creates a new <see cref="IBuffer"/>. </summary>
	IBuffer CreateBuffer(BufferKey key);

	/// <summary> Creates a new <see cref="ITexture"/>. </summary>
	ITexture CreateTexture(TextureKey key);

	/// <summary> Creates a new <see cref="ITextureView"/>. </summary>
	ITextureView CreateTextureView(TextureViewKey key);

	/// <summary> Creates a new <see cref="ISampler"/>. </summary>
	ISampler CreateSampler(SamplerKey key);

	/// <summary> Creates a new <see cref="IResourceLayout"/>. </summary>
	IResourceLayout CreateResourceLayout(ResourceLayoutKey key);

	/// <summary> Creates a new <see cref="IResourceSet"/>. </summary>
	IResourceSet CreateResourceSet(ResourceSetKey key);

	/// <summary> Creates a new <see cref="IPipeline"/>. </summary>
	IPipeline CreateGraphicsPipeline(PipelineKey key);
}
