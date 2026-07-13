// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Serialization;

/// <summary>
/// Data representation of a field.
/// </summary>
internal record struct FieldRepresentation(string Name, string Value);

/// <summary>
/// Data representation of an instance.
/// </summary>
/// <remarks>
/// We use this instead of always reflecting for caching + other reasons.
/// </remarks>
internal struct InstanceRepresentation
{
	/// <summary> All serialized fields of the instance. </summary>
	public FieldRepresentation[] Fields { get; set; }
}
