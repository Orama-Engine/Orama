// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Serialization.Attributes;

/// <summary>
/// Forces a property or field to be serialized when it typically wouldn't be.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class AlwaysSerializeAttribute : Attribute { }
