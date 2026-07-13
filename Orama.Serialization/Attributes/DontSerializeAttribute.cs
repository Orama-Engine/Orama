// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Serialization.Attributes;

/// <summary>
/// Forces a property to never serialize even if it would otherwise be serialized.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DontSerializeAttribute : Attribute { }
