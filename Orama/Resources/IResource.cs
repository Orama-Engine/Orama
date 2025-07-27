
namespace Orama.Resources;

/// <summary>
/// Represents a resource.
/// </summary>
public interface IResource<T> where T : IResource<T>
{
	/// <summary>
	/// Constructs a resource from a stream.
	/// </summary>
	/// <returns>Constructed resource.</returns>
	T Deserialize(Stream stream);

	/// <summary>
	/// Writes a resource to a stream.
	/// </summary>
	void Serialize(Stream stream);
}
