// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Rendering;

public sealed class GPUBufferPool : ObjectPool<GPUBufferPool, GPUBuffer>
{
	/// <inheritdoc/>
	protected override GPUBuffer CreateObject() => new();

	/// <inheritdoc/>
	protected override void ResetObject(GPUBuffer obj)
	{
		base.ResetObject(obj);

		obj.Reset();
	}
}
