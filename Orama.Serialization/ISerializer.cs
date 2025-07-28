
namespace Orama.Serialization;

public interface ISerializer
{
	public Stream Serialize<T>(T value);
	public T Deserialize<T>(Stream stream);
}
