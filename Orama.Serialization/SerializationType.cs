// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Serialization;

/// <summary>
/// The serialization backend type.
/// </summary>
public enum SerializationType : byte
{
	YAML,
	JSON,
	Binary
}
