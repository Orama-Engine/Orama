// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common;
using Orama.Common.Utility;
using Orama.Physics.Components;
using Orama.Physics.Components.Colliders;
using Orama.Rendering.Components;
using Orama.Rendering.Resources;
using Orama.Scenes.Entities;

namespace Orama.Rendering.Entities;

/// <summary>
/// Internal entity useful for debugging systems.
/// </summary>
[Entity("debug_entity")]
public class DebugEntity : Entity
{
	[ImplicitComponentAttribute]
	public MeshRenderer Renderer { get; private set; } = null!;

	[ImplicitComponentAttribute]
	public RigidBody RigidBody { get; private set; } = null!;

	/// <inheritdoc/>
	public override void Start()
	{
		base.Start();

		var col = (BoxCollider)AddComponent(new BoxCollider(Transform.Scale));
		col.Start();

		RigidBody.IsStatic = true;

		var mesh = Application.ResourceProvider.GetResource<Mesh>("Assets/PrimitiveCube.fbx");

		Renderer.Mesh = mesh;
		Renderer.Mesh?.Material = Material.Default;

		OramaConsole.Log("Debug entity started.");
	}
}
