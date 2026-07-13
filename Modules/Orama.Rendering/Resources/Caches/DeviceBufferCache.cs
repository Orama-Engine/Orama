// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Drawing;

using NeoVeldrid;

namespace Orama.Rendering.Resources.Caches;

public sealed class DeviceBufferCache : ResourceCache<DeviceBufferCache, DeviceBufferKey, DeviceBuffer>
{
	/// <inheritdoc/>
	protected override DeviceBuffer Create(DeviceBufferKey key) => Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateBuffer(new BufferDescription(key.ByteSize, key.Usage));
}

public readonly record struct DeviceBufferKey(uint ByteSize, BufferUsage Usage);
