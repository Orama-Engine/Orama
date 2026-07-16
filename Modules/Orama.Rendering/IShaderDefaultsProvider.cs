// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Math;
using Orama.Rendering.Components;

namespace Orama.Rendering;

/// <summary>
/// Interface for providing values for default <see cref="Resources.Shader"/> parameter blocks.
/// </summary>
public interface IShaderDefaultsProvider
{
	/// <summary> Gets a <see cref="GPUBuffer"/> with data from <paramref name="camera"/>. </summary>
	GPUBuffer GetCameraBuffer(Camera camera);

	/// <summary> Gets a <see cref="GPUBuffer"/> with data from <paramref name="renderable"/>. </summary>
	GPUBuffer GetObjectBuffer(IClientRenderable renderable);
}

public class ShaderDefaultsProvider : IShaderDefaultsProvider
{
	/// <inheritdoc/>
	public GPUBuffer GetCameraBuffer(Camera camera)
	{
		GPUBuffer cameraBuffer = GPUBufferPool.Instance.Rent();
		cameraBuffer.AddMatrix4x4(camera.ViewMatrix);
		cameraBuffer.AddMatrix4x4(camera.ProjectionMatrix);

		return cameraBuffer;
	}

	/// <inheritdoc/>
	public GPUBuffer GetObjectBuffer(IClientRenderable renderable)
	{
		GPUBuffer objectBuffer = GPUBufferPool.Instance.Rent();
		objectBuffer.AddMatrix4x4(renderable.Transform);
		return objectBuffer;
	}
}
