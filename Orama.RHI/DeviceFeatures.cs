// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.RHI;

/// <summary>
/// Flags that define different features supported by a <see cref="IGraphicsDevice"/> backend.
/// </summary>
[Flags]
public enum DeviceFeatures : ushort
{
	None = 0,

	ClipSpaceYInverted = 1 << 0
}
