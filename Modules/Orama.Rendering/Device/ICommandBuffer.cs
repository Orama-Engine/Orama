// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Math;
using Orama.Rendering.Resources;
using Veldrith;

namespace Orama.Rendering.Device;

/// <summary>
/// Buffer of GPU commands to be submitted.
/// </summary>
public interface ICommandBuffer : IDisposable
{
	#region Lifecycle
	/// <summary> Begins recording GPU commands. </summary>
	void Begin();

	/// <summary> Ends recording GPU commands. </summary>
	void End();
	#endregion

	#region Render Targets
	/// <summary> Sets the <see cref="Framebuffer"/> to render commands to. </summary>
	void SetFrameBuffer(Framebuffer frameBuffer);
	#endregion

	#region Drawing
	/// <summary> Draws a mesh. </summary>
	void Draw(ReadOnlySpan<Vector3> vertices, ReadOnlySpan<Vector3> normals, ReadOnlySpan<Vector2> uvs, ReadOnlySpan<uint> indices, Matrix4x4 transform, Material material);

	/// <summary> Draws a <see cref="Mesh"/> with the given transform <see cref="Matrix4x4"/> and <see cref="Material"/>. </summary>
	void Draw(Mesh mesh, Matrix4x4 transform, Material material);
	
	/// <summary> Draws an <see cref="IClientRenderable"/>. </summary>
	void Draw(IClientRenderable renderable);
	#endregion

	#region Binding
	/// <summary> Sets a constant buffer for the given slot. </summary>
	void SetConstantBuffer(int slot, ReadOnlySpan<byte> data);
	#endregion
}
