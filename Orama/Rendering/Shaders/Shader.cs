using System;

namespace Orama.Rendering.Shaders;

public class Shader
{
	public static Shader Default { get; } = new Shader(VertexSource, FragmentSource);

	internal byte[] VertexBytes { get; }
	internal byte[] FragmentBytes { get; }

	private const string VertexSource = @"
#version 450

layout(location = 0) in vec3 inPosition;

layout(set = 0, binding = 0) uniform Matrices
{
    mat4 Model;
    mat4 View;
    mat4 Projection;
};

void main()
{
    gl_Position = Projection * View * Model * vec4(inPosition, 1.0);
}";

	private const string FragmentSource = @"
#version 450

layout(location = 0) out vec4 fsoutColor;

void main()
{
    fsoutColor = vec4(1.0); // white
}";

	public Shader(string vertexSource, string fragmentSource)
	{
		if (vertexSource is null) throw new ArgumentNullException(nameof(vertexSource));
		if (fragmentSource is null) throw new ArgumentNullException(nameof(fragmentSource));

		(VertexBytes, FragmentBytes) = ShaderBaker.Bake(vertexSource, fragmentSource);
	}
}
