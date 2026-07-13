// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Silk.NET.OpenXR;

namespace Orama.VirtualReality.OpenXR.Bindings;

/// <summary>
/// Base class for all OpenXR managed bindings.
/// </summary>
internal class OpenXRBinding
{
	public XR OpenXR { get; }

	public OpenXRBinding(XR openXR)
	{
		OpenXR = openXR;
	}
}
