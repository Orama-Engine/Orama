using Veldrid;

namespace Orama.Modules.Rendering;

public class RenderableResources
{
	public DeviceBuffer VertexBuffer { get; }
	public DeviceBuffer IndexBuffer { get; }
	public Pipeline Pipeline { get; }
	public ResourceLayout ResourceLayout { get; }
	public Veldrid.Shader[] Shaders { get; }

	public RenderableResources(DeviceBuffer vb, DeviceBuffer ib, Pipeline pipeline,
							   ResourceLayout layout, Veldrid.Shader[] shaders)
	{
		VertexBuffer = vb;
		IndexBuffer = ib;
		Pipeline = pipeline;
		ResourceLayout = layout;
		Shaders = shaders;
	}
}

public class MaterialResources
{
	public byte[] VertexBytes { get; }
	public byte[] FragmentBytes { get; }

	internal DeviceBuffer? UniformBuffer { get; set; }
	internal ResourceSet? ResourceSet { get; set; }

	public MaterialResources(byte[] vertexBytes, byte[] fragmentBytes)
	{
		VertexBytes = vertexBytes ?? throw new ArgumentNullException(nameof(vertexBytes));
		FragmentBytes = fragmentBytes ?? throw new ArgumentNullException(nameof(fragmentBytes));
	}
}

