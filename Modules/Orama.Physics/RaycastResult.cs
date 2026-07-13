// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Math;

namespace Orama.Physics;

/// <summary>
/// Information about a completed raycast.
/// </summary>
public readonly struct RaycastResult
{
	/// <summary> The body that was hit. </summary>
	public IPhysicsBody Body { get; init; }

	/// <summary> The hit point of the raycast in world-space. </summary>
	public Vector3 HitPoint { get; init; }

	/// <summary> The surface normal at the point of impact. </summary>
	public Vector3 Normal { get; init; }

	/// <summary> The distance from the origin to the hit point. </summary>
	public float Distance { get; init; }
}
