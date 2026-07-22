// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Rendering.Resources.Caches;

namespace Orama.Rendering.Device.Resources;

/// <summary>
/// A GPU buffer.
/// </summary>
public interface IBuffer : IBindableResource
{
	/// <summary> The buffer size in bytes. </summary>
	uint SizeInBytes { get; }
}

/// <summary>
/// A GPU framebuffer.
/// </summary>
public interface IFramebuffer : IGPUResource { }

/// <summary>
/// A GPU shader.
/// </summary>
public interface IShader : IGPUResource { }

/// <summary>
/// A GPU texture.
/// </summary>
public interface ITexture : IBindableResource { }

/// <summary>
/// A view into a GPU texture.
/// </summary>
public interface ITextureView : IBindableResource { }

/// <summary>
/// A GPU sampler.
/// </summary>
public interface ISampler : IBindableResource { }

/// <summary>
/// A shader resource layout.
/// </summary>
public interface IResourceLayout : IGPUResource { }

/// <summary>
/// Bound resources matching an <see cref="IResourceLayout"/>.
/// </summary>
public interface IResourceSet : IGPUResource { }

/// <summary>
/// A graphics pipeline.
/// </summary>
public interface IPipeline : IGPUResource { }

/// <summary>
/// A GPU resource that can be bound to a shader.
/// </summary>
public interface IBindableResource : IGPUResource { }

/// <summary>
/// Supported GPU buffer usages.
/// </summary>
[Flags]
public enum BufferUsage : byte
{
	VertexBuffer = 1,
	IndexBuffer = 2,
	UniformBuffer = 4,
	Dynamic = 8
}

/// <summary>
/// The kind of shader resource.
/// </summary>
public enum ResourceKind : byte
{
	UniformBuffer
}

/// <summary>
/// All stages of a shader.
/// </summary>
[Flags]
public enum ShaderStages : byte
{
	Vertex = 1,
	Fragment = 2
}

public readonly ref struct BufferKey(uint size, BufferUsage usage) : IResourceKey
{
	public readonly uint Size = size;
	public readonly BufferUsage Usage = usage;

	/// <inheritdoc/>
	public int Hash => GetHashCode();

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(Size, Usage);
}

public readonly struct ResourceLayoutElementDescription(string name, ResourceKind kind, ShaderStages stages)
{
	public readonly string Name = name;
	public readonly ResourceKind Kind = kind;
	public readonly ShaderStages Stages = stages;
}

public readonly ref struct ResourceLayoutKey(ReadOnlySpan<ResourceLayoutElementDescription> elements) : IResourceKey
{
	public readonly ReadOnlySpan<ResourceLayoutElementDescription> Elements = elements;

	/// <inheritdoc/>
	public int Hash => GetHashCode();

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var hash = new HashCode();
		foreach (ref readonly var element in Elements)
		{
			hash.Add(element.Name);
			hash.Add(element.Kind);
			hash.Add(element.Stages);
		}
		return hash.ToHashCode();
	}
}

public readonly ref struct ResourceSetKey(IResourceLayout layout, ReadOnlySpan<IBindableResource> boundResources) : IResourceKey
{
	public readonly IResourceLayout Layout = layout;
	public readonly ReadOnlySpan<IBindableResource> BoundResources = boundResources;

	/// <inheritdoc/>
	public int Hash => GetHashCode();

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var hash = new HashCode();
		hash.Add(Layout);
		foreach (IBindableResource resource in BoundResources)
			hash.Add(resource);
		return hash.ToHashCode();
	}
}

public readonly ref struct ShaderKey(ReadOnlySpan<byte> bytecode, ShaderStages stage) : IResourceKey
{
	public readonly ReadOnlySpan<byte> Bytecode = bytecode;
	public readonly ShaderStages Stage = stage;

	/// <inheritdoc/>
	public int Hash => GetHashCode();

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;

			foreach (byte b in Bytecode)
				hash = hash * 31 + b;

			return hash;
		}
	}
}
