// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common;
using Orama.Rendering.Resources;
using Orama.Scenes.Components;

namespace Orama.Rendering.Components;

public class MeshRenderer : Component
{
	/// <summary> The <see cref="Resources.Mesh"/> to render. </summary>
	public Mesh? Mesh { get; set; }

	/// <inheritdoc/>
	public override void Update()
	{
		if (Mesh == null)
			return;

		ModuleManager.GetModule<RenderingModule>()?.QueueObject(Mesh, Entity.Transform.Matrix);
	}
}
