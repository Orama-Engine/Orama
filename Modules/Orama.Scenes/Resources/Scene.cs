// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common.Utility;
using Orama.Scenes.Entities;

namespace Orama.Scenes.Resources;

/// <summary>
/// A collection of Entities.
/// </summary>
public class Scene
{
	/// <summary> All Entities in the Scene. </summary>
	public List<Entity> Entities { get; set; } = new();

	public void StartAll()
	{
		foreach (var entity in Entities)
		{
			if (!entity.Enabled)
				continue;

			try
			{
				entity.Start();
			}
			catch (Exception ex)
			{
				EngineConsole.Exception(ex);
			}
		}
	}

	internal void UpdateAll()
	{
		foreach (var entity in Entities)
		{
			if (!entity.Enabled)
				continue;

			try
			{
				entity.Update();
			}
			catch (Exception ex)
			{
				EngineConsole.Exception(ex);
			}
		}
	}
}
