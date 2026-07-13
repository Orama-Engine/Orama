// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Rendering.Veldrid;

namespace Orama.Rendering.Device;

public static class CommandBufferPool
{
	private static readonly Queue<CommandBuffer> pool = new Queue<CommandBuffer>();

	/// <summary> Takes a <see cref="CommandBuffer"/> from the pool. </summary>
	public static CommandBuffer Rent()
	{
		if (pool.Count > 0)
			return pool.Dequeue();

		return new CommandBuffer(Renderer.Veldrid);
	}

	/// <summary> Returns a <see cref="CommandBuffer"/> to the pool. </summary>
	public static void Return(CommandBuffer buffer) => pool.Enqueue(buffer);
}
