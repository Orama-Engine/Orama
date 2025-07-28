using Orama.Rendering.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orama.Rendering.Materials;

public class Material
{
	public static Material Default { get; } = new Material()
	{
		Shader = Shader.Default
	};

	public Shader Shader { get; set; }
}
