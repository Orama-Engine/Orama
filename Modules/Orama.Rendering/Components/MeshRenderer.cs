// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common;
using Orama.Common.Utility;
using Orama.Rendering.Resources;
using Orama.Scenes.Components;

namespace Orama.Rendering.Components;

public class MeshRenderer : Component
{
	/// <summary> The <see cref="Resources.Mesh"/> to render. </summary>
	public Mesh? Mesh { get; set; }

	public override void Start()
	{
		base.Start();

		if (Mesh == null)
			OramaConsole.Warning($"MeshRenderer '{Entity.Name}' has no mesh.");
	}

	/// <inheritdoc/>
	public override void Update()
	{
		base.Update();

		if (Mesh == null)
			return;

		ModuleManager.GetModule<RenderingModule>()?.QueueObject(Mesh, Entity.Transform.Matrix);
	}
}
