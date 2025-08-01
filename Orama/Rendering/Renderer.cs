using System.Numerics;
using Orama.Rendering.Materials;
using Orama.Resources;
using Veldrid;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

namespace Orama.Rendering;

public static class Renderer
{
	public static GraphicsBackend GraphicsBackend => _graphicsDevice.BackendType;

	private static GraphicsDevice? _graphicsDevice;
	private static CommandList? _commandList;

	private static readonly Dictionary<IClientRenderable, RenderableResources> _renderableResources = new();
	private static readonly Dictionary<Material, MaterialResources> _materialResources = new();
	private static readonly List<IClientRenderable> _renderables = new();

	public static void Initialize()
	{
		var window = Window.InternalWindow ?? throw new Exception("Sdl2Window not initialized");

		window.Resized += () =>
		{
			_graphicsDevice?.ResizeMainWindow((uint)window.Width, (uint)window.Height);
		};

		_graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, new GraphicsDeviceOptions(), GraphicsBackend.Vulkan);
		_commandList = _graphicsDevice.ResourceFactory.CreateCommandList();
	}

	public static void Clear(Vector4 clearColor)
	{
		_commandList?.ClearColorTarget(0, new(clearColor));
	}

	public static void BeginFrame()
	{
		_commandList?.Begin();
		_commandList?.SetFramebuffer(_graphicsDevice!.SwapchainFramebuffer);
	}

	public static void EndFrame()
	{
		_commandList?.End();
		_graphicsDevice!.SubmitCommands(_commandList);
		_graphicsDevice.SwapBuffers();
		_renderables.Clear();
	}

	public static void AddRenderable(IClientRenderable renderable)
	{
		_renderables.Add(renderable);
	}

	public static void Render(Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix)
	{
		foreach (var renderable in _renderables)
		{
			if (!_renderableResources.TryGetValue(renderable, out var res))
			{
				res = InitializeRenderableResources(renderable);
				if (res == null) continue;
				_renderableResources[renderable] = res;
			}

			Draw(renderable, res, viewMatrix, projectionMatrix);
		}
	}

	private static RenderableResources? InitializeRenderableResources(IClientRenderable renderable)
	{
		if (_graphicsDevice == null) return null;

		var factory = _graphicsDevice.ResourceFactory;

		// Prepare vertex data: interleaved position (vec3)
		var vertices = renderable.Vertices;
		int vertexCount = vertices.Length;
		int vertexSize = sizeof(float) * 3; // Position only
		byte[] vertexData = new byte[vertexCount * vertexSize];
		for (int i = 0; i < vertexCount; i++)
		{
			Buffer.BlockCopy(BitConverter.GetBytes(vertices[i].X), 0, vertexData, i * vertexSize + 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(vertices[i].Y), 0, vertexData, i * vertexSize + 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(vertices[i].Z), 0, vertexData, i * vertexSize + 8, 4);
		}

		DeviceBuffer vertexBuffer = factory.CreateBuffer(new BufferDescription((uint)vertexData.Length, BufferUsage.VertexBuffer));
		_graphicsDevice.UpdateBuffer(vertexBuffer, 0, vertexData);

		// Prepare index buffer
		var indices = renderable.Indices;
		DeviceBuffer indexBuffer = factory.CreateBuffer(new BufferDescription((uint)(indices.Length * sizeof(uint)), BufferUsage.IndexBuffer));
		_graphicsDevice.UpdateBuffer(indexBuffer, 0, indices);

		// Ensure material resources cached
		var material = renderable.Material;
		if (!_materialResources.ContainsKey(material))
		{
			_materialResources[material] = new MaterialResources(material.Shader.VertexBytes!, material.Shader.FragmentBytes!);
		}
		var matRes = _materialResources[material];

		// Create shaders
		ShaderDescription vertexShaderDesc = new ShaderDescription(ShaderStages.Vertex, matRes.VertexBytes, "main");
		ShaderDescription fragmentShaderDesc = new ShaderDescription(ShaderStages.Fragment, matRes.FragmentBytes, "main");
		Veldrid.Shader[] shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

		// Vertex layout: position only (vec3)
		VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
			stride: (uint)vertexSize,
			new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float3, offset: 0));

		// Resource layout (uniform buffer for matrices)
		ResourceLayout resourceLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
			new ResourceLayoutElementDescription("ubo", ResourceKind.UniformBuffer, ShaderStages.Vertex)
		));

		GraphicsPipelineDescription pipelineDesc = new GraphicsPipelineDescription
		{
			BlendState = BlendStateDescription.SingleOverrideBlend,
			DepthStencilState = DepthStencilStateDescription.Disabled,
			RasterizerState = new RasterizerStateDescription(
				FaceCullMode.Back, PolygonFillMode.Solid, FrontFace.Clockwise, depthClipEnabled: true, scissorTestEnabled: false),
			PrimitiveTopology = PrimitiveTopology.TriangleList,
			ResourceLayouts = new[] { resourceLayout },
			ShaderSet = new ShaderSetDescription(new[] { vertexLayout }, shaders),
			Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription
		};

		Pipeline pipeline = factory.CreateGraphicsPipeline(pipelineDesc);

		return new RenderableResources(vertexBuffer, indexBuffer, pipeline, resourceLayout, shaders);
	}

	private static void Draw(IClientRenderable renderable, RenderableResources res, Matrix4x4 view, Matrix4x4 proj)
	{
		if (_graphicsDevice == null || _commandList == null)
			return;

		Matrix4x4 modelMatrix = renderable.ModelMatrix;

		int matrixSize = sizeof(float) * 16;
		byte[] uboData = new byte[matrixSize * 3];
		Buffer.BlockCopy(GetMatrixBytes(modelMatrix), 0, uboData, 0, matrixSize);
		Buffer.BlockCopy(GetMatrixBytes(view), 0, uboData, matrixSize, matrixSize);
		Buffer.BlockCopy(GetMatrixBytes(proj), 0, uboData, matrixSize * 2, matrixSize);

		var factory = _graphicsDevice.ResourceFactory;
		DeviceBuffer uniformBuffer = factory.CreateBuffer(new BufferDescription((uint)uboData.Length, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
		_graphicsDevice.UpdateBuffer(uniformBuffer, 0, uboData);

		ResourceSet resourceSet = factory.CreateResourceSet(new ResourceSetDescription(
			res.ResourceLayout,
			uniformBuffer));

		_commandList.SetPipeline(res.Pipeline);
		_commandList.SetVertexBuffer(0, res.VertexBuffer);
		_commandList.SetIndexBuffer(res.IndexBuffer, IndexFormat.UInt32);
		_commandList.SetGraphicsResourceSet(0, resourceSet);
		_commandList.DrawIndexed((uint)renderable.Indices.Length, 1, 0, 0, 0);

		uniformBuffer.Dispose();
		resourceSet.Dispose();
	}

	private static byte[] GetMatrixBytes(Matrix4x4 matrix)
	{
		byte[] bytes = new byte[sizeof(float) * 16];
		Buffer.BlockCopy(BitConverter.GetBytes(matrix.M11), 0, bytes, 0 * 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(matrix.M12), 0, bytes, 1 * 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(matrix.M13), 0, bytes, 2 * 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(matrix.M14), 0, bytes, 3 * 4, 4);

		Buffer.BlockCopy(BitConverter.GetBytes(matrix.M21), 0, bytes, 4 * 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(matrix.M22), 0, bytes, 5 * 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(matrix.M23), 0, bytes, 6 * 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(matrix.M24), 0, bytes, 7 * 4, 4);

		Buffer.BlockCopy(BitConverter.GetBytes(matrix.M31), 0, bytes, 8 * 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(matrix.M32), 0, bytes, 9 * 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(matrix.M33), 0, bytes, 10 * 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(matrix.M34), 0, bytes, 11 * 4, 4);

		Buffer.BlockCopy(BitConverter.GetBytes(matrix.M41), 0, bytes, 12 * 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(matrix.M42), 0, bytes, 13 * 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(matrix.M43), 0, bytes, 14 * 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(matrix.M44), 0, bytes, 15 * 4, 4);

		return bytes;
	}

	public static void DisposeRenderable(IClientRenderable renderable)
	{
		if (_renderableResources.TryGetValue(renderable, out var res))
		{
			res.VertexBuffer.Dispose();
			res.IndexBuffer.Dispose();
			res.Pipeline.Dispose();
			res.ResourceLayout.Dispose();
			foreach (var shader in res.Shaders)
				shader.Dispose();

			_renderableResources.Remove(renderable);
		}
	}

	public static void DisposeMaterial(Material material)
	{
		if (_materialResources.ContainsKey(material))
			_materialResources.Remove(material);
	}
}