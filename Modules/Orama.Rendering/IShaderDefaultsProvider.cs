// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Math;
using Orama.Rendering.Components;
using Orama.RHI;

namespace Orama.Rendering;

/// <summary>
/// Interface for providing values for default <see cref="Resources.Shader"/> parameter blocks.
/// </summary>
public interface IShaderDefaultsProvider
{
	/// <summary> Gets a <see cref="ReadOnlySpan{T}"/> with data from <paramref name="camera"/> formatted for uploading via <see cref="ICommandBuffer.SetConstantBuffer(string, ReadOnlySpan{byte})"/>. </summary>
	ReadOnlySpan<byte> GetCameraBuffer(Camera camera);

	/// <summary> Gets a <see cref="ReadOnlySpan{T}"/> with the given object data formatted for uploading via <see cref="ICommandBuffer.SetConstantBuffer(string, ReadOnlySpan{byte})"/>. </summary>
	ReadOnlySpan<byte> GetObjectBuffer(Matrix4x4 transform);
}

public class ShaderDefaultsProvider : IShaderDefaultsProvider
{
	/// <inheritdoc/>
	public ReadOnlySpan<byte> GetCameraBuffer(Camera camera)
	{
		using var cameraBuffer = GPUBufferPool.Shared.RentAuto();
		cameraBuffer.Object.AddMatrix4x4(camera.ViewMatrix);
		cameraBuffer.Object.AddMatrix4x4(camera.ProjectionMatrix);

		return cameraBuffer.Object.Data;
	}

	/// <inheritdoc/>
	public ReadOnlySpan<byte> GetObjectBuffer(Matrix4x4 transform)
	{
		using var objectBuffer = GPUBufferPool.Shared.RentAuto();
		objectBuffer.Object.AddMatrix4x4(transform);
		return objectBuffer.Object.Data;
	}
}
