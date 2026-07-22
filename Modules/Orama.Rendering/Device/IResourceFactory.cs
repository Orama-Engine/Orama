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

	/// <summary> Creates a new <see cref="IBuffer"/> using the given <see cref="BufferKey"/> information. </summary>
	IBuffer CreateBuffer(BufferKey key);

	/// <summary> Creates a new <see cref="ITexture"/> using the given <see cref="TextureKey"/> information. </summary>
	ITexture CreateTexture(TextureKey key);

	/// <summary> Creates a new <see cref="ITextureView"/> using the given <see cref="TextureViewKey"/> information. </summary>
	ITextureView CreateTextureView(TextureViewKey key);

	/// <summary> Creates a new <see cref="ISampler"/> using the given <see cref="SamplerKey"/> information. </summary>
	ISampler CreateSampler(SamplerKey key);

	/// <summary> Creates a new <see cref="IResourceLayout"/> using the given <see cref="ResourceLayoutKey"/> information. </summary>
	IResourceLayout CreateResourceLayout(ResourceLayoutKey key);

	/// <summary> Creates a new <see cref="IResourceSet"/> using the given <see cref="ResourceSetKey"/> information. </summary>
	IResourceSet CreateResourceSet(ResourceSetKey key);

	/// <summary> Creates a new <see cref="IPipeline"/> using the given <see cref="PipelineKey"/> information. </summary>
	IPipeline CreateGraphicsPipeline(PipelineKey key);
}
