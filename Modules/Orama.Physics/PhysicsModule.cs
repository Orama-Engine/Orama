// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common;
using Orama.Physics.Engines.Jitter2;

namespace Orama.Physics;

/// <summary>
/// Module responsible for handling the physics simulation.
/// </summary>
public class PhysicsModule : BaseModule
{
	public IPhysicsWorld World { get; set; } = null!;

	private double accumulator = 0f;

	/// <inheritdoc/>
	public override void Initialize()
	{
		Application.OnUpdate += Update;

		World = new Jitter2World();
	}

	/// <inheritdoc/>
	public override void Dispose()
	{
		base.Dispose();

		Application.OnUpdate -= Update;
	}

	/// <inheritdoc/>
	public void Update()
	{
		accumulator += Time.PreciseDelta;
		while (accumulator > Time.FixedDelta)
		{
			accumulator -= Time.FixedDelta;
			World.Step(Time.FixedDelta);
		}
	}
}
