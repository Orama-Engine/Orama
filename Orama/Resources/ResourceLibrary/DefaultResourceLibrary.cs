namespace Orama.Resources.ResourceLibrary;

public class DefaultResourceLibrary : IResourceLibrary
{
	/// <inheritdoc/>
	public T GetResource<T>(string path) where T : IResource<T>, new()
	{
		if (!File.Exists(Path.Combine(AppContext.BaseDirectory, path)))
		{
			Console.WriteLine($"Resource {path} not found");
			return new();
		}

		using var stream = File.OpenRead(Path.Combine(AppContext.BaseDirectory, path));
		return new T().Deserialize(stream);
	}

	/// <inheritdoc/>
	public void WriteResource<T>(string path, T resource) where T : IResource<T>, new()
	{
		using var stream = File.OpenWrite(Path.Combine(AppContext.BaseDirectory, path));
		stream.SetLength(0);

		Stream output = resource.Serialize();
		output.CopyTo(stream);
	}
}
