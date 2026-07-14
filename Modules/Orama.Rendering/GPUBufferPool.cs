// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Rendering;

public sealed class GPUBufferPool : ObjectPool<GPUBufferPool, GPUBuffer>
{
    /// <inheritdoc/>
	public override GPUBuffer CreateObject() => new();
}
