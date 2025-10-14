
namespace Orama.Rendering.Backends;



/// <summary>
/// Backend-agnostic way of sending commands to the GPU.
/// </summary>
public interface ICommandBuffer
{
    /// <summary> Clears the screen. </summary>
    void Clear(float r, float g, float b, float a);
}