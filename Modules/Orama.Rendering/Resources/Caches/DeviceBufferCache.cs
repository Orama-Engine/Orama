using System.Drawing;
using Veldrid;

namespace Orama.Rendering.Resources.Caches;

public sealed class DeviceBufferCache : ResourceCache<DeviceBufferCache, DeviceBufferKey, DeviceBuffer>
{
    /// <inheritdoc/>
    protected override DeviceBuffer Create(DeviceBufferKey key) => Renderer.Veldrid.GraphicsDevice.ResourceFactory.CreateBuffer(new BufferDescription(key.ByteSize, key.Usage));
}

public readonly record struct DeviceBufferKey(uint ByteSize, BufferUsage Usage);
