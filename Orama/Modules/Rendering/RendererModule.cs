using System.Numerics;
using Orama.Rendering.Materials;
using Veldrid;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

namespace Orama.Modules.Rendering;

public class RendererModule : Module
{
	public GraphicsBackend GraphicsBackend => _graphicsDevice.BackendType;

	private GraphicsDevice? _graphicsDevice;
	private CommandList? _commandList;

	private readonly Dictionary<IClientRenderable, RenderableResources> _renderableResources = new();
	private readonly Dictionary<Material, MaterialResources> _materialResources = new();
	private readonly List<IClientRenderable> _renderables = new();

	private Framebuffer? _mainFramebuffer;
	private Texture? _colorTarget;
	private Texture? _depthTarget;

	public override void Start()
	{
		var window = Window.InternalWindow ?? throw new Exception("Sdl2Window not initialized");

		window.Resized += () =>
		{
			_graphicsDevice?.ResizeMainWindow((uint)window.Width, (uint)window.Height);
			CreateRenderTarget((uint)window.Width, (uint)window.Height);
		};

		_graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, new GraphicsDeviceOptions(), GraphicsBackend.Vulkan);
		_commandList = _graphicsDevice.ResourceFactory.CreateCommandList();
		CreateRenderTarget((uint)window.Width, (uint)window.Height);
		RenderPipelineManager.Current.Initialize();
	}

	public void Clear(Vector4 clearColor) => _commandList?.ClearColorTarget(0, new(clearColor));

	public void ClearDepth(float clearDepth) => _commandList?.ClearDepthStencil(clearDepth);

	public void BeginFrame()
	{
		_commandList?.Begin();
		_commandList?.SetFramebuffer(_mainFramebuffer ?? _graphicsDevice!.SwapchainFramebuffer);
	}

	public void EndFrame()
	{
		if (_mainFramebuffer != null)
		{
			_commandList.SetFramebuffer(_graphicsDevice!.SwapchainFramebuffer);
			_commandList.CopyTexture(_colorTarget, _graphicsDevice.SwapchainFramebuffer.ColorTargets[0].Target);
		}

		_commandList?.End();
		_graphicsDevice!.SubmitCommands(_commandList);
		_graphicsDevice.SwapBuffers();
		_renderables.Clear();
	}

	public void AddRenderable(IClientRenderable renderable)
	{
		_renderables.Add(renderable);
	}

	public void Render(Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix)
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

	private RenderableResources? InitializeRenderableResources(IClientRenderable renderable)
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
			_materialResources[material] = new MaterialResources(material.Shader.VertexBytes!, material.Shader.FragmentBytes!);

		var matRes = _materialResources[material];

		if (matRes.UniformBuffer == null)
		{
			matRes.UniformBuffer = factory.CreateBuffer(
				new BufferDescription(256, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
		}

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
			new ResourceLayoutElementDescription("Matrices", ResourceKind.UniformBuffer, ShaderStages.Vertex),
			new ResourceLayoutElementDescription("MaterialData", ResourceKind.UniformBuffer, ShaderStages.Fragment)
		));

		GraphicsPipelineDescription pipelineDesc = new GraphicsPipelineDescription
		{
			BlendState = BlendStateDescription.SingleOverrideBlend,
			DepthStencilState = new DepthStencilStateDescription(true, true, ComparisonKind.LessEqual),
			RasterizerState = new RasterizerStateDescription(
				FaceCullMode.Back, PolygonFillMode.Solid, FrontFace.Clockwise, depthClipEnabled: true, scissorTestEnabled: false),
			PrimitiveTopology = PrimitiveTopology.TriangleList,
			ResourceLayouts = new[] { resourceLayout },
			ShaderSet = new ShaderSetDescription(new[] { vertexLayout }, shaders),
			Outputs = _mainFramebuffer?.OutputDescription ?? _graphicsDevice.SwapchainFramebuffer.OutputDescription
		};

		Pipeline pipeline = factory.CreateGraphicsPipeline(pipelineDesc);

		return new RenderableResources(vertexBuffer, indexBuffer, pipeline, resourceLayout, shaders);
	}

	private void Draw(IClientRenderable renderable, RenderableResources res, Matrix4x4 view, Matrix4x4 proj)
	{
		if (_graphicsDevice == null || _commandList == null)
			return;

		Matrix4x4 modelMatrix = renderable.ModelMatrix;

		// Camera matrices
		int matrixSize = sizeof(float) * 16;
		byte[] uboData = new byte[matrixSize * 3];
		Buffer.BlockCopy(GetMatrixBytes(modelMatrix), 0, uboData, 0, matrixSize);
		Buffer.BlockCopy(GetMatrixBytes(view), 0, uboData, matrixSize, matrixSize);
		Buffer.BlockCopy(GetMatrixBytes(proj), 0, uboData, matrixSize * 2, matrixSize);

		var factory = _graphicsDevice.ResourceFactory;

		// Camera uniform buffer
		DeviceBuffer cameraBuffer = factory.CreateBuffer(
			new BufferDescription((uint)uboData.Length, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
		_graphicsDevice.UpdateBuffer(cameraBuffer, 0, uboData);

		// Material parameters
		var matRes = _materialResources[renderable.Material];

		byte[] materialData = PackMaterialParameters(renderable.Material.Properties.Values);
		_graphicsDevice.UpdateBuffer(matRes.UniformBuffer!, 0, materialData);

		ResourceSet resourceSet = factory.CreateResourceSet(new ResourceSetDescription(
	res.ResourceLayout,
	cameraBuffer,            // slot 0: matrices
	matRes.UniformBuffer!    // slot 1: material params
));

		_commandList.SetPipeline(res.Pipeline);
		_commandList.SetVertexBuffer(0, res.VertexBuffer);
		_commandList.SetIndexBuffer(res.IndexBuffer, IndexFormat.UInt32);
		_commandList.SetGraphicsResourceSet(0, resourceSet);
		_commandList.DrawIndexed((uint)renderable.Indices.Length, 1, 0, 0, 0);

		// cameraBuffer.Dispose();
		// resourceSet.Dispose();
	}

	private byte[] GetMatrixBytes(Matrix4x4 matrix)
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

	private byte[] PackMaterialParameters(IEnumerable<object> parameters)
	{
		List<byte> buffer = new();
		int offset = 0;

		foreach (var parameter in parameters)
		{
			switch (parameter)
			{
				case float f:
					Align(ref buffer, ref offset, 4);
					buffer.AddRange(BitConverter.GetBytes(f));
					offset += 4;
					break;

				case int i:
					Align(ref buffer, ref offset, 4);
					buffer.AddRange(BitConverter.GetBytes(i));
					offset += 4;
					break;

				case bool b:
					Align(ref buffer, ref offset, 4);
					buffer.AddRange(BitConverter.GetBytes(b ? 1 : 0));
					offset += 4;
					break;

				case Vector2 v2:
					Align(ref buffer, ref offset, 8);
					buffer.AddRange(BitConverter.GetBytes(v2.X));
					buffer.AddRange(BitConverter.GetBytes(v2.Y));
					offset += 8;
					break;

				case Vector3 v3:
					Align(ref buffer, ref offset, 16);
					buffer.AddRange(BitConverter.GetBytes(v3.X));
					buffer.AddRange(BitConverter.GetBytes(v3.Y));
					buffer.AddRange(BitConverter.GetBytes(v3.Z));
					buffer.AddRange(new byte[4]); // pad to 16
					offset += 16;
					break;

				case Vector4 v4:
					Align(ref buffer, ref offset, 16);
					buffer.AddRange(BitConverter.GetBytes(v4.X));
					buffer.AddRange(BitConverter.GetBytes(v4.Y));
					buffer.AddRange(BitConverter.GetBytes(v4.Z));
					buffer.AddRange(BitConverter.GetBytes(v4.W));
					offset += 16;
					break;

				case Matrix4x4 m:
					Align(ref buffer, ref offset, 16);
					buffer.AddRange(BitConverter.GetBytes(m.M11));
					buffer.AddRange(BitConverter.GetBytes(m.M12));
					buffer.AddRange(BitConverter.GetBytes(m.M13));
					buffer.AddRange(BitConverter.GetBytes(m.M14));

					buffer.AddRange(BitConverter.GetBytes(m.M21));
					buffer.AddRange(BitConverter.GetBytes(m.M22));
					buffer.AddRange(BitConverter.GetBytes(m.M23));
					buffer.AddRange(BitConverter.GetBytes(m.M24));

					buffer.AddRange(BitConverter.GetBytes(m.M31));
					buffer.AddRange(BitConverter.GetBytes(m.M32));
					buffer.AddRange(BitConverter.GetBytes(m.M33));
					buffer.AddRange(BitConverter.GetBytes(m.M34));

					buffer.AddRange(BitConverter.GetBytes(m.M41));
					buffer.AddRange(BitConverter.GetBytes(m.M42));
					buffer.AddRange(BitConverter.GetBytes(m.M43));
					buffer.AddRange(BitConverter.GetBytes(m.M44));

					offset += 64;
					break;

				default:
					throw new NotSupportedException($"Unsupported uniform type: {parameter.GetType()}");
			}
		}

		return buffer.ToArray();
	}

	private void Align(ref List<byte> buffer, ref int offset, int alignment)
	{
		int padding = (alignment - (offset % alignment)) % alignment;
		if (padding > 0)
		{
			buffer.AddRange(new byte[padding]);
			offset += padding;
		}
	}

	private void CreateRenderTarget(uint width, uint height)
	{
		if (_graphicsDevice == null) 
			return;

		DisposeRenderTarget();

		var factory = _graphicsDevice.ResourceFactory;

		// Color texture
		_colorTarget = factory.CreateTexture(TextureDescription.Texture2D(
			width, height, mipLevels: 1, arrayLayers: 1,
			PixelFormat.B8_G8_R8_A8_UNorm, TextureUsage.RenderTarget | TextureUsage.Sampled));

		// Depth texture
		_depthTarget = factory.CreateTexture(TextureDescription.Texture2D(
			width, height, mipLevels: 1, arrayLayers: 1,
			PixelFormat.D32_Float_S8_UInt, TextureUsage.DepthStencil | TextureUsage.Sampled));

		// Framebuffer
		_mainFramebuffer = factory.CreateFramebuffer(new FramebufferDescription(
			_depthTarget, _colorTarget));
	}

	public void DisposeRenderable(IClientRenderable renderable)
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

	public void DisposeMaterial(Material material)
	{
		if (_materialResources.ContainsKey(material))
			_materialResources.Remove(material);
	}

	private void DisposeRenderTarget()
	{
		_mainFramebuffer?.Dispose();
		_mainFramebuffer = null;

		_colorTarget?.Dispose();
		_colorTarget = null;

		_depthTarget?.Dispose();
		_depthTarget = null;
	}
}