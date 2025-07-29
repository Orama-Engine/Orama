using Orama.Rendering.Shaders;
using System.Numerics;

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
	public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

	/// <summary>
	/// Sets a parameter on the <see cref="Shaders.Shader"/>
	/// </summary>
	/// <typeparam name="T">Type of the parameter.</typeparam>
	/// <param name="name">Name of the parameter.</param>
	/// <param name="value">Value of the parameter.</param>
	public void SetParameter<T>(string name, T value)
	{
		if (value is float || value is int ||
			value is Vector2 || value is Vector3 || value is Vector4 ||
			value is Matrix4x4 || value is bool)
		{
			Properties[name] = value!;
		}
		else
		{
			throw new NotSupportedException($"Unsupported parameter type {typeof(T).Name}");
		}
	}

}
