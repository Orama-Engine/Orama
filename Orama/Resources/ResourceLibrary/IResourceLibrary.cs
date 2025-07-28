namespace Orama.Resources.ResourceLibrary;


public interface IResourceLibrary
{
	/// <summary>
	/// Retrieves a resource.
	/// </summary>
	/// <typeparam name="T">The type of the resource.</typeparam>
	/// <param name="path">Path to the resource.</param>
	/// <returns>New Resource Type.</returns>
	T GetResource<T>(string path) where T : IResource<T>, new();

	/// <summary>
	/// Writes a resource.
	/// </summary>
	/// <typeparam name="T">The type of the resource.</typeparam>
	/// <param name="path">Path to save to.</param>
	void SaveResource<T>(string path, T resource) where T : IResource<T>, new();
}
