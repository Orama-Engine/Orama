using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orama.Resources;

public class Material
{
	public static Material Default { get; } = new Material()
	{
		Shader = Shader.Default
	};

	public Shader Shader { get; set; }
}
