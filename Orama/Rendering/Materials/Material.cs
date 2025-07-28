using Orama.Rendering.Shaders;

namespace Orama.Rendering.Materials;

public class Material
{
	public static Material Default { get; } = new Material()
	{
		Shader = Shader.Default
	};

	public Shader Shader { get; set; }

	/// <summary>
	/// Custom properties.
	/// </summary>
	public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
}
