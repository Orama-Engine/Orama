
using Orama.Math;

namespace Orama.Rendering;

/// <summary>
/// Represents an object that can be rendered by the client.
/// </summary>
public interface IClientRenderable
{
	public void GetRenderingData(out IGeometryDrawData drawData, out Matrix4x4 model);
}
