// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Rendering;

public enum CullingMode
{
	None,
	Front,
	Back
}

public struct RendererOptions
{
	/// <summary> The culling mode to use. </summary>
	public CullingMode Culling { get; set; }
}
