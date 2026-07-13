// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Runtime.CompilerServices;

namespace Orama.Math;

/// <summary>
/// Provides additional float math utility.
/// </summary>
public static class MathFEx
{
	/// <summary> Clamps a value between min and max. </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Clamp(float value, float min, float max) => MathF.Min(MathF.Max(value, min), max);
}
